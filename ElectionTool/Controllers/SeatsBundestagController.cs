using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectionTool.Models;

namespace ElectionTool.Controllers
{
    public class SeatsBundestagController : BaseController
    {
        // GET: SeatsBundestag
        public ActionResult Index(int electionId)
        {
            // Work around for displaying a simple example
            //var model = new AllSeatsBundestagViewModel
            //{
            //    ElectionId = electionId,
            //    SeatsDistribution = new List<SeatsBundestagViewModel>
            //    {
            //        new SeatsBundestagViewModel
            //        {
            //            ElectionId = electionId,
            //            PartyId = 1,
            //            Party = "Partei1",
            //            Seats = new VoteViewModel
            //            {
            //                Amount = 100*electionId,
            //                Votes = (decimal) 0.38
            //            }
            //        },
            //        new SeatsBundestagViewModel
            //        {
            //            ElectionId = electionId,
            //            PartyId = 2,
            //            Party = "Partei2",
            //            Seats = new VoteViewModel
            //            {
            //                Amount = 100*electionId,
            //                Votes = (decimal) 0.62,
            //                LastVotes = (decimal) 0.55
            //            }
            //        }
            //    }
            //};

            var model = Service.GetAllSeatsBundestag(electionId);

            return View(model);
        }
    }
}