using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Entity_Framework;
using ElectionTool.Models;

namespace ElectionTool.ViewModelMap
{
    public class ViewModelMap
    {
        public static IEnumerable<ElectionViewModel> GetElectionViewModels(IEnumerable<Election> elections)
        {
            return elections.Select(GetElectionViewModel);
        } 

        public static ElectionViewModel GetElectionViewModel(Election election)
        {
            return new ElectionViewModel
            {
                Id = election.Id,
                Date = election.Date,
                SeatsBundestag = election.SeatsBundestag
            };
        }

        public static IEnumerable<PersonWithPartyViewModel> GetPersonWithPartyViewModels(int electionId, IEnumerable<Person> people, IEnumerable<Party> allParties)
        {
            return people.Select(p => GetPersonWithPartyViewModel(electionId, p, allParties));
        }

        public static PersonWithPartyViewModel GetPersonWithPartyViewModel(int electionId, Person person,
            IEnumerable<Party> allParties)
        {
            var partyId = person.PartyAffiliations.Single(a => a.Election_Id == electionId).Party_Id;

            return new PersonWithPartyViewModel
            {
                Person = new PersonViewModel
                {
                    Id = person.Id,
                    Title = person.Title,
                    Firstname = person.Firstname,
                    Lastname = person.Lastname
                },
                Party = new PartyViewModel
                {
                    Id = partyId,
                    Name = allParties.Single(pa => pa.Id == partyId).Name
                }
            };
        }

        public static IEnumerable<WahlkreisViewModel> GetWahlkreisViewModels(IEnumerable<Wahlkrei> wahlkreise)
        {
            return wahlkreise.Select(GetWahlkreisViewModel);
        } 

        public static WahlkreisViewModel GetWahlkreisViewModel(Wahlkrei wahlkreis)
        {
            return new WahlkreisViewModel
            {
                Id = wahlkreis.Id,
                Name = wahlkreis.Name,
                BundeslandId = wahlkreis.Bundesland_Id
            };
        }

        public static IEnumerable<SeatsBundestagViewModel> GetSeatsBundestagViewModels(
            IEnumerable<Sitzverteilung> seatsParty)
        {
            return seatsParty.Select(entry => new SeatsBundestagViewModel
            {
                ElectionId = entry.Election_Id,
                PartyId = entry.Party_Id ?? -1,
                Party = entry.PartyName,

                //TODO: add last election result to view
                Seats = new VoteViewModel
                {
                    Amount = entry.SeatsParty ?? -1,
                    Votes = entry.PercentParty ?? -1,
                    LastVotes = 0
                }
            });
        }

        public static IEnumerable<MemberOfBundestagViewModel> GetMemberOfBundestagViewModels(
            IEnumerable<ParliamentMember> members)
        {
            return members.Select(member => new MemberOfBundestagViewModel
            {
                ElectionId = member.Election_Id,

                Member = new PersonWithPartyViewModel
                {
                    Person = new PersonViewModel
                    {
                        Id = member.Person_Id,
                        Title = member.Title,
                        Firstname = member.Firstname,
                        Lastname = member.Lastname
                    },
                    Party = new PartyViewModel
                    {
                        Id = member.Party_Id,
                        Name = member.Party_Name
                    }
                },
                Bundesland = member.Bundesland_Name,
                Wahlkreis = member.Wahlkreis_Name
            });
        }

        public static IEnumerable<BundeslandWithWahlkreiseViewModel<WahlkreisViewModel>> GetBundeslandListViewModels(IEnumerable<Bundesland> bundeslands,
            IEnumerable<Wahlkrei> wahlkreise)
        {
            return bundeslands.Select(b => GetBundeslandListViewModel(b, wahlkreise.Where(w => w.Bundesland_Id == b.Id)));
        }

        public static BundeslandWithWahlkreiseViewModel<WahlkreisViewModel> GetBundeslandListViewModel(Bundesland bundesland,
            IEnumerable<Wahlkrei> wahlkreise)
        {
            return new BundeslandWithWahlkreiseViewModel<WahlkreisViewModel>
            {
                Bundesland = new BundeslandViewModel
                {
                    Id  = bundesland.Id,
                    Name = bundesland.Name
                },
                Wahlkreise = GetWahlkreisListViewModels(wahlkreise).ToList().OrderBy(r => r)
            };
        }

        public static IEnumerable<WahlkreisViewModel> GetWahlkreisListViewModels(IEnumerable<Wahlkrei> wahlkreis)
        {
            return wahlkreis.Select(w => new WahlkreisViewModel
            {
                Id = w.Id,
                Name = w.Name
            });
        }

        public static WahlkreisOverviewViewModel GetWahlkreisOverviewViewModel(int electionId,
            BasicWahlkreisOverview overview, IEnumerable<ErststimmeWahlkreisOverview> firstVotes,
            IEnumerable<ZweitstimmeWahlkreisOverview> secondVotes)
        {
            return new WahlkreisOverviewViewModel
            {
                ElectionId = electionId,
                Wahlkreis = new WahlkreisViewModel
                {
                    Id = overview.Wahlkreis_Id,
                    Name = overview.Wahlkreis_Name
                },
                Participation = overview.Participation ?? -1,
                Candidate = new PersonWithPartyViewModel
                {
                    Person = new PersonViewModel
                    {
                        Id = overview.Person_Id ?? -1,
                        Title = overview.Title,
                        Firstname = overview.Firstname,
                        Lastname = overview.Lastname
                    },
                    Party = new PartyViewModel
                    {
                        Id = overview.Party_Id,
                        Name = overview.Party_Name
                    }
                },
                FirstVotes = GetWahlkreisFirstVotesViewModels(electionId, overview.Wahlkreis_Id, firstVotes).ToList().OrderBy(r => r),
                SecondVotes = GetWahlkreisSecondVotesViewModels(electionId, overview.Wahlkreis_Id, secondVotes).ToList().OrderBy(r => r)
            };
        }

        public static IEnumerable<WahlkreisFirstVotesViewModel> GetWahlkreisFirstVotesViewModels(int electionId, int wahlkreisId,
            IEnumerable<ErststimmeWahlkreisOverview> firstVotes)
        {
            return firstVotes.Select(v => new WahlkreisFirstVotesViewModel
            {
                ElectionId = electionId,
                WahlkreisId = wahlkreisId,
                Candidate = new PersonWithPartyViewModel
                {
                    Person = new PersonViewModel
                    {
                        Id = v.person_id ?? -1,
                        Title = v.title,
                        Firstname = v.firstname,
                        Lastname = v.lastname
                    },
                    Party = new PartyViewModel
                    {
                        Id = v.Party_Id ?? -1,
                        Name = v.Party_Name
                    }
                },
                Vote = new VoteViewModel
                {
                    Amount = v.Votes,
                    Votes = v.PercentVotes ?? -1,
                    LastVotes = v.Previous
                }
            });
        }

        public static IEnumerable<WahlkreisSecondVotesViewModel> GetWahlkreisSecondVotesViewModels(int electionId, int wahlkreisId,
            IEnumerable<ZweitstimmeWahlkreisOverview> secondVotes)
        {
            return secondVotes.Select(v => new WahlkreisSecondVotesViewModel
            {
                ElectionId = electionId,
                WahlkreisId = wahlkreisId,
                Party = new PartyViewModel
                {
                    Id = v.Party_Id ?? -1,
                    Name = v.Party_Name
                },
                Vote = new VoteViewModel
                {
                    Amount = v.Votes,
                    Votes = v.PercentVotes ?? -1,
                    LastVotes = v.Previous
                }
            });
        }

        public static IEnumerable<PartyViewModel> GetPartyViewModels(IEnumerable<Party> parties)
        {
            return parties.Select(p => new PartyViewModel
            {
                Id = p.Id,
                Name = p.Name
            });
        }

        public static ClosestWinnerForPartyViewModel GetClosestWinnerForPartyViewModel(int electionId, Party party,
            IEnumerable<ClosestErststimmeResult> winner, IEnumerable<ClosestErststimmeResult> loser)
        {
            return new ClosestWinnerForPartyViewModel
            {
                ElectionId = electionId,
                Party = new PartyViewModel
                {
                    Id = party.Id,
                    Name = party.Name
                },
                ClosestWinner = GetClosestWinnerEntryViewModels(winner.Any() ? winner : loser).ToList().OrderBy(r => r)
            };
        }

        public static IEnumerable<ClosestWinnerEntryViewModel> GetClosestWinnerEntryViewModels(
            IEnumerable<ClosestErststimmeResult> entries)
        {
            return entries.Select(e => new ClosestWinnerEntryViewModel
            {
                Person = new PersonViewModel
                {
                    Id = e.Person_Id ?? -1,
                    Title = e.Title,
                    Firstname = e.Firstname,
                    Lastname = e.Lastname
                },
                Difference = e.Diff ?? 0,
                Wahlkreis = e.Wahlkreis_Name
            });
        }

        public static IEnumerable<UeberhangmandatEntryViewModel> GetUeberhangmandatEntryViewModels(
            IEnumerable<Ueberhangmandate> entries)
        {
            return entries.Select(e => new UeberhangmandatEntryViewModel
            {
                ElectionId = e.Election_Id,
                Bundesland = new BundeslandViewModel
                {
                    Id = e.Bundesland_Id,
                    Name = e.Bundesland_Name
                },
                Party = new PartyViewModel
                {
                    Id = e.Party_Id ?? -1,
                    Name = e.Party_Name
                },
                Amount = e.Number ?? 0
            });
        }

        public static IEnumerable<BundeslandWithWahlkreiseViewModel<WahlkreisWithWinnerViewModel>>
            GetWinnerWahlkreiseViewModel(IEnumerable<Bundesland> bundeslands, IEnumerable<WinnerFirstAndSecondVote> entries)
        {
            return bundeslands.Select(b => new BundeslandWithWahlkreiseViewModel<WahlkreisWithWinnerViewModel>
            {
                Bundesland = new BundeslandViewModel
                {
                    Id = b.Id,
                    Name = b.Name
                },
                Wahlkreise = GetWahlkreisWithWinnerViewModels(entries.Where(e => e.Bundesland_Id == b.Id)).ToList().OrderBy(r => r)
            });
        }

        public static IEnumerable<WahlkreisWithWinnerViewModel> GetWahlkreisWithWinnerViewModels(
            IEnumerable<WinnerFirstAndSecondVote> entries)
        {
            return entries.Select(e => new WahlkreisWithWinnerViewModel
            {
                Id = e.Wahlkreis_Id,
                Name = e.Wahlkreis_Name,
                FirstVotes = new PersonWithPartyViewModel
                {
                    Person = new PersonViewModel
                    {
                        Id = e.Person_Id ?? -1,
                        Title = e.Title,
                        Firstname = e.Firstname,
                        Lastname = e.Lastname
                    },
                    Party = new PartyViewModel
                    {
                        Id = e.FirstVote_Party_Id,
                        Name = e.FirstVote_Party_Name
                    }
                },
                SecondVotes = new PartyViewModel
                {
                    Id = e.SecondVote_Party_Id ?? -1,
                    Name = e.SecondVote_Party_Name
                }
            });
        } 
    }
}