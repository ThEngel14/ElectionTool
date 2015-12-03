using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectionTool.Models;

namespace ElectionTool.Controllers
{
    public class MemberBundestagController : BaseController
    {
        // GET: MemberBundestag
        public ActionResult Index(int electionId)
        {
            // Work around for displaying a simple example
            //var model = new AllMemberOfBundestagViewModel
            //{
            //    ElectionId = electionId,
            //    Members = new List<MemberOfBundestagViewModel> { new MemberOfBundestagViewModel
            //        {
            //            ElectionId = electionId,
            //            Wahlkreis = "München I",
            //            Bundesland = "Bayern",
            //            Member = new PersonWithPartyViewModel
            //            {
            //                Person = new PersonViewModel
            //                {
            //                    Id = 0,
            //                    Title = "Prof. Dr.",
            //                    Firstname = "Thomas",
            //                    Lastname = "Engel"
            //                },
            //                Party = new PartyViewModel
            //                {
            //                    Id = 0,
            //                    Name = electionId == 1 ? "SPD" : "CSU"
            //                }
            //            }
            //        }
            //    }
            //};

            var model = Service.GetAllMemberOfBundestag(electionId);

            return View(model);
        }
    }
}