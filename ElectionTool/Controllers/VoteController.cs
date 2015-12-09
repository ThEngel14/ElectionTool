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
            ElectionVoteViewModel model = null;
            try
            {
                model = Service.ValidateToken(tokenString, Request.UserHostAddress);
            }
            catch (Exception e)
            {
                AddException(e);
            }

            if (model != null)
            {
                return View(model);   
            }

            return RedirectToAction("Index");
        }

        public ActionResult PerformVote(ElectionVoteViewModel model)
        {
            var successful = false;
            try
            {
                successful = Service.PerformVote(model, Request.UserHostAddress);
            }
            catch (Exception e)
            {
                AddException(e);
            }

            if (successful)
            {
                GetMessageBag().Success.Add("Sie haben Ihre Stimme erfolgreich abgegeben.");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Elect", model.TokenString);
        }
    }
}