using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectionTool.Models;

namespace ElectionTool.Controllers
{
    public class UeberhangmandateController : BaseController
    {
        private static readonly IDictionary<int, UeberhangmandatViewModel> Models = new Dictionary<int, UeberhangmandatViewModel>();

        // GET: Ueberhangmandate
        public ActionResult Index(int electionId)
        {
            if(!IsCustomCaching() || !Models.ContainsKey(electionId))
                Models[electionId] = Service.GetUeberhangmandate(electionId);

            return View(Models[electionId]);
        }
    }
}