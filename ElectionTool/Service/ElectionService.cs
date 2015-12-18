using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ElectionTool.Entity_Framework;
using ElectionTool.Models;

namespace ElectionTool.Service
{
    public class ElectionService
    {
        private readonly TokenHandler _tokenHandler = new TokenHandler();

        public ElectionViewModel GetElection(int electionId)
        {
            using (var context = new ElectionDBEntities())
            {
                var election = context.Elections.SingleOrDefault(e => e.Id == electionId);

                if (election == null)
                {
                    throw new PublicException(string.Format("Es gibt keine Bundestagswahl mit der Id {0}.", electionId));
                }

                return ViewModelMap.ViewModelMap.GetElectionViewModel(election);
            }
        }

        public WahlkreisViewModel GetWahlkreis(int wahlkreisId)
        {
            using (var context = new ElectionDBEntities())
            {
                var wahlkreis = context.Wahlkreis.SingleOrDefault(w => w.Id == wahlkreisId);

                if (wahlkreis == null)
                {
                    throw new PublicException(string.Format("Es gibt keinen Wahlkreis mit der Id {0}.", wahlkreisId));
                }

                return ViewModelMap.ViewModelMap.GetWahlkreisViewModel(wahlkreis);
            }
        }

        public GenerateTokenViewModel GetGenerateTokenModel()
        {
            var model = new GenerateTokenViewModel();

            using (var context = new ElectionDBEntities())
            {
                var elections = context.Elections.Where(e => e.Id < 3);
                var electionVms = ViewModelMap.ViewModelMap.GetElectionViewModels(elections).ToList();

                var wahlkreise = context.Wahlkreis;
                var wahlkreiseVms = ViewModelMap.ViewModelMap.GetWahlkreisViewModels(wahlkreise).ToList();

                model.Election = electionVms.OrderBy(r => r.Date);
                model.SelectedElectionId = elections.Any() ? elections.OrderByDescending(r => r.Date).First().Id : 0;
                model.Wahlkreise = wahlkreiseVms.OrderBy(w => w.Name);
                model.Amount = 1;
            }

            return model;
        }

        public ElectionVoteViewModel ValidateToken(string tokenString, string ip)
        {
            var token = _tokenHandler.BuildToken(tokenString, ip);

            var model = new ElectionVoteViewModel
            {
                TokenString = tokenString
            };

            var electionId = token.GetElectionId();
            var wahlkreisId = token.GetWahlkreisId();

            using (var context = new ElectionDBEntities())
            {
                var election = context.Elections.Single(e => e.Id == electionId);

                var wahlkreis = context.Wahlkreis.Single(w => w.Id == wahlkreisId);

                var allParties = context.Parties;

                var parties = GetElectableParties(context, electionId, wahlkreis.Bundesland_Id);

                var people = GetElectablePeople(context, electionId, wahlkreisId);

                var partyVm = ViewModelMap.ViewModelMap.GetPartyViewModels(parties).ToList();
                var peopleVm = ViewModelMap.ViewModelMap.GetPersonWithPartyViewModels(electionId, people, allParties).ToList();

                model.Election = ViewModelMap.ViewModelMap.GetElectionViewModel(election);
                model.Wahlkreis = ViewModelMap.ViewModelMap.GetWahlkreisViewModel(wahlkreis);
                model.Parties = partyVm.OrderBy(r => r.Name);
                model.People = peopleVm.OrderBy(r => r.Party.Name);
            }

            return model;
        }

        private static IEnumerable<Person> GetElectablePeople(ElectionDBEntities context, int electionId, int wahlkreisId)
        {
            var people =
                context.People
                    .Include("IsElectableCandidates")
                    .Include("PartyAffiliations")
                    .Where(
                        p =>
                            p.IsElectableCandidates.Any(
                                e => e.Election_Id == electionId && e.Wahlkreis_Id == wahlkreisId));

            return people;
        }

        private static IEnumerable<Party> GetElectableParties(ElectionDBEntities context, int electionId, int bundeslandId)
        {
            var allParties = context.Parties
                .Include("PartyAffiliations")
                .Include("PartyAffiliations.Person")
                .Include("PartyAffiliations.Person.CandidateLists");

            var parties =
                allParties.Where(
                    p =>
                        p.PartyAffiliations.Any(
                            a =>
                                a.Person.CandidateLists.Any(
                                    c => c.Election_Id == electionId && c.Bundesland_Id == bundeslandId)));

            return parties;
        } 

        public bool PerformVote(ElectionVoteViewModel model, string ip)
        {
            var token = _tokenHandler.BuildToken(model.TokenString, ip);

            var wahlkreis = GetWahlkreis(token.GetWahlkreisId());

            using (var context = new ElectionDBEntities())
            {
                //Validate voted person and party
                var electablePeople = GetElectablePeople(context, token.GetElectionId(), token.GetWahlkreisId());
                var electableParties = GetElectableParties(context, token.GetElectionId(), wahlkreis.BundeslandId);

                if (model.VotedPersonId > 0 && !electablePeople.Select(p => p.Id).Contains(model.VotedPersonId ?? -1))
                {
                    throw new PublicException(
                        string.Format(
                            "Die Person mit Id {0} darf in diesem Wahlkreis bei dieser Wahl nicht gewählt werden.",
                            model.VotedPersonId));
                }

                if (model.VotedPartyId > 0 && !electableParties.Select(p => p.Id).Contains(model.VotedPartyId ?? -1))
                {
                    throw new PublicException(
                        string.Format(
                            "Die Partei mit Id {0} darf in diesem Wahlkreis bei dieser Wahl nicht gewählt werden.",
                            model.VotedPartyId));
                }

                // Election_Id is always set to 3 so that the generated votes for past elections are not changed
                context.Erststimmes.Add(new Erststimme
                {
                    Election_Id = 3, //token.GetElectionId(),
                    Wahlkreis_Id = token.GetWahlkreisId(),
                    Person_Id = model.VotedPersonId > 0 ? model.VotedPersonId : null,
                });

                context.Zweitstimmes.Add(new Zweitstimme
                {
                    Election_Id = 3, //token.GetElectionId(),
                    Wahlkreis_Id = token.GetWahlkreisId(),
                    Party_Id = model.VotedPartyId > 0 ? model.VotedPartyId : null
                });

                context.SaveChanges();

                _tokenHandler.FinishedToken(token);
            }

            return true;
        }

        public AllSeatsBundestagViewModel GetAllSeatsBundestag(int electionId)
        {
            var model = new AllSeatsBundestagViewModel
            {
                ElectionId = electionId
            };

            using (var context = new ElectionDBEntities())
            {
                context.Database.CommandTimeout = 5000000;
                var seatsParty = context.Sitzverteilungs.Where(s => s.Election_Id == electionId);

                var seatsDistribution = ViewModelMap.ViewModelMap.GetSeatsBundestagViewModels(seatsParty).ToList();
                model.SeatsDistribution = seatsDistribution.OrderBy(r => r);
            }

            return model;
        }

        public AllMemberOfBundestagViewModel GetAllMemberOfBundestag(int electionId)
        {
            var model = new AllMemberOfBundestagViewModel
            {
                ElectionId = electionId
            };

            using (var context = new ElectionDBEntities())
            {
                context.Database.CommandTimeout = 5000000;

                var members = context.ParliamentMembers.Where(m => m.Election_Id == electionId);

                var allMembers = ViewModelMap.ViewModelMap.GetMemberOfBundestagViewModels(members).ToList();
                model.Members = allMembers.OrderBy(r => r);
            }

            return model;
        }

        public WahlkreisSelectionViewModel GetWahlkreisSelection(int electionId)
        {
            var model = new WahlkreisSelectionViewModel
            {
                ElectionId = electionId
            };

            using (var context = new ElectionDBEntities())
            {
                var bundeslands = context.Bundeslands;
                var wahlkreise = context.Wahlkreis;

                var bModels = ViewModelMap.ViewModelMap.GetBundeslandListViewModels(bundeslands, wahlkreise).ToList();
                model.Bundeslands = bModels.OrderBy(r => r);
            }

            return model;
        }

        public WahlkreisOverviewViewModel GetWahlkreisOverview(int electionId, int wahlkreisId)
        {
            WahlkreisOverviewViewModel model;

            using (var context = new ElectionDBEntities())
            {
                var basicOverview =
                    context.BasicWahlkreisOverviews.Single(
                        w => w.Election_Id == electionId && w.Wahlkreis_Id == wahlkreisId);
                var firstVotes =
                    context.ErststimmeWahlkreisOverviews.Where(
                        v => v.election_id == electionId && v.wahlkreis_id == wahlkreisId && v.person_id != null);
                var secondVotes =
                    context.ZweitstimmeWahlkreisOverviews.Where(
                        v => v.Election_Id == electionId && v.Wahlkreis_Id == wahlkreisId && v.Party_Id != null);

                model = ViewModelMap.ViewModelMap.GetWahlkreisOverviewViewModel(electionId, basicOverview, firstVotes,
                    secondVotes);
            }

            return model;
        }

        public PartySelectionViewModel GetAllPartiesForClosestWinner(int electionId)
        {
            var model = new PartySelectionViewModel
            {
                ElectionId = electionId
            };

            using (var context = new ElectionDBEntities())
            {
                var partyIds =
                    context.ClosestErststimmeResults.Where(r => r.Election_Id == electionId)
                        .Select(r => r.Party_Id)
                        .Distinct();

                var parties = context.Parties.Where(p => partyIds.Contains(p.Id));

                var pModel = ViewModelMap.ViewModelMap.GetPartyViewModels(parties).ToList();
                model.Parties = pModel.OrderBy(r => r);
            }

            return model;
        }

        public ClosestWinnerForPartyViewModel GetClosestWinnerForParty(int electionId, int partyId)
        {
            ClosestWinnerForPartyViewModel model;
            using (var context = new ElectionDBEntities())
            {
                var allForParty =
                    context.ClosestErststimmeResults.Where(e => e.Election_Id == electionId && e.Party_Id == partyId);

                var winner = allForParty.Where(e => e.Diff > 0).OrderBy(e => e.AbsDiff).Take(10);

                var loser = allForParty.Where(e => e.Diff < 0).OrderBy(e => e.AbsDiff).Take(10);

                var party = context.Parties.Single(p => p.Id == partyId);

                model = ViewModelMap.ViewModelMap.GetClosestWinnerForPartyViewModel(electionId, party, winner, loser);
            }

            return model;
        }

        public UeberhangmandatViewModel GetUeberhangmandate(int electionId)
        {
            var model = new UeberhangmandatViewModel
            {
                ElectionId = electionId
            };

            using (var context = new ElectionDBEntities())
            {
                context.Database.CommandTimeout = 500000;
                var entries = context.Ueberhangmandates.Where(u => u.Election_Id == electionId && u.Number > 0);

                var mandate = ViewModelMap.ViewModelMap.GetUeberhangmandatEntryViewModels(entries).ToList();
                model.Mandate = mandate.OrderBy(r => r);
            }

            return model;
        }

        public WinnerWahlkreiseViewModel GetWinnerWahlkreise(int electionId)
        {
            var model = new WinnerWahlkreiseViewModel
            {
                ElectionId = electionId
            };

            using (var context = new ElectionDBEntities())
            {
                var bundeslands = context.Bundeslands;

                var winnerEntries = context.WinnerFirstAndSecondVotes.Where(w => w.Election_Id == electionId);

                var bLands = ViewModelMap.ViewModelMap.GetWinnerWahlkreiseViewModel(bundeslands,
                    winnerEntries).ToList();

                model.Bundeslands = bLands.OrderBy(r => r);
            }

            return model;
        }
    }
}