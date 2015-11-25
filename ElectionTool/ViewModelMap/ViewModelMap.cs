using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Entity_Framework;
using ElectionTool.Models;

namespace ElectionTool.ViewModelMap
{
    public class ViewModelMap
    {
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
                         //TODO: add person_id and title to view
                        Id = 0,
                        Title = null,

                        Firstname = member.FirstName,
                        Lastname = member.LastName
                    },
                    Party = new PartyViewModel
                    {
                        // TODO: add party id to view
                        Id = 0,

                        Name = member.Party
                    }
                },
                Bundesland = member.State,

                //TODO: add wahlkreis to view
                Wahlkreis = member.HowCome,
            });
        }

        public static IEnumerable<BundeslandListViewModel> GetBundeslandListViewModels(IEnumerable<Bundesland> bundeslands,
            IEnumerable<Wahlkrei> wahlkreise)
        {
            return bundeslands.Select(b => GetBundeslandListViewModel(b, wahlkreise.Where(w => w.Bundesland_Id == b.Id)));
        }

        public static BundeslandListViewModel GetBundeslandListViewModel(Bundesland bundesland,
            IEnumerable<Wahlkrei> wahlkreise)
        {
            return new BundeslandListViewModel
            {
                BundeslandId = bundesland.Id,
                BundeslandName = bundesland.Name,
                Wahlkreise = GetWahlkreisListViewModels(wahlkreise).ToList().OrderBy(r => r)
            };
        }

        public static IEnumerable<WahlkreisListViewModel> GetWahlkreisListViewModels(IEnumerable<Wahlkrei> wahlkreis)
        {
            return wahlkreis.Select(w => new WahlkreisListViewModel
            {
                WahlkreisId = w.Id,
                WahlkreisName = w.Name
            });
        }

        public static WahlkreisOverviewViewModel GetWahlkreisOverviewViewModel(int electionId,
            BasicWahlkreisOverview overview, IEnumerable<ErststimmeWahlkreisOverview> firstVotes,
            IEnumerable<ZweitstimmeWahlkreisOverview> secondVotes)
        {
            return new WahlkreisOverviewViewModel
            {
                ElectionId = electionId,
                WahlkreisId = overview.Wahlkreis_Id,
                WahlkreisName = overview.Wahlkreis_Name,
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
    }
}