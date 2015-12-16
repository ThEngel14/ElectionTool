using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ElectionTool.Models;
using ElectionTool.Service;

namespace ElectionTool.Controllers
{
    public class VoteController : BaseController
    {
        // GET: Vote
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Elect(string txtToken)
        {
            var model = CallService(() => Service.ValidateToken(txtToken, Request.UserHostAddress));

            if (model != null)
            {
                return View(model);   
            }

            return RedirectToAction("Index");
        }

        public ActionResult PerformVote(ElectionVoteViewModel model)
        {
            var successful = CallService(() => Service.PerformVote(model, Request.UserHostAddress));

            if (successful)
            {
                GetMessageBag().Success.Add("Sie haben Ihre Stimme erfolgreich abgegeben.");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Elect", new {txtToken = model.TokenString});
        }

        public ActionResult GenerateToken()
        {
            var model = CallService(() => Service.GetGenerateTokenModel());

            model.SelectedElectionId = TempData["ElectionId"] as int? ?? model.SelectedElectionId;
            model.SelectedWahlkreisId = TempData["WahlkreisId"] as int? ?? model.SelectedWahlkreisId;
            model.Amount = TempData["Amount"] as int? ?? model.Amount;

            return View(model);
        }

        public ActionResult GenerateTokenFor(GenerateTokenViewModel model)
        {
            CallService(() =>
            {
                TempData["ElectionId"] = model.SelectedElectionId;
                TempData["WahlkreisId"] = model.SelectedWahlkreisId;
                TempData["Amount"] = model.Amount;

                if (!model.Password.Equals("ichdarfdasschon"))
                {
                    throw new Exception("Sie dürfen mit diesem Passwort keine Token generieren.");
                }

                var election = Service.GetElection(model.SelectedElectionId);
                var wahlkreis = Service.GetWahlkreis(model.SelectedWahlkreisId);

                var infoToken = string.Format("Bundestagswahl: {0}\nWahlkreis: {1}\n\nErstellte Token:",
                    election.Date.ToShortDateString(), wahlkreis.Name);

                for (var i = 0; i < model.Amount; i++)
                {
                    infoToken += string.Format("\n    {0}",
                        TokenValidation.GenerateTokenString(model.SelectedElectionId, model.SelectedWahlkreisId));
                }

                GetMessageBag().Info.Add(infoToken);
            });

            return RedirectToAction("GenerateToken");
        }
    }
}