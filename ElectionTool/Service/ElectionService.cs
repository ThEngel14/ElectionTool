using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Entity_Framework;
using ElectionTool.Models;

namespace ElectionTool.Service
{
    public class ElectionService
    {
        public AllSeatsBundestagViewModel GetAllSeatsBundestag(int electionId)
        {
            var model = new AllSeatsBundestagViewModel
            {
                ElectionId = electionId
            };

            using (var context = new ElectionDBEntities())
            {
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
                        v => v.election_id == electionId && v.wahlkreis_id == wahlkreisId);
                var secondVotes =
                    context.ZweitstimmeWahlkreisOverviews.Where(
                        v => v.Election_Id == electionId && v.Wahlkreis_Id == wahlkreisId);

                model = ViewModelMap.ViewModelMap.GetWahlkreisOverviewViewModel(electionId, basicOverview, firstVotes,
                    secondVotes);
            }

            return model;
        }
    }
}