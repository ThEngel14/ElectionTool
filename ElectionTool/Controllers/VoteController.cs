using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ElectionTool.Models;

namespace ElectionTool.Controllers
{
    public class VoteController : BaseController
    {
        // GET: Vote
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Elect(string tokenString)
        {
            var model = Service.ValidateToken(tokenString, Request.UserHostAddress);

            return View(model);
        }

        public ActionResult PerformVote(ElectionVoteViewModel model)
        {
            var successful = Service.PerformVote(model, Request.UserHostAddress);

            if (successful)
            {
                return View();
            }

            return RedirectToAction("Elect", model.TokenString);
        }
    }
}