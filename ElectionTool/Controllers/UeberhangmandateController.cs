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
        private static readonly IDictionary<int, UeberhangmandatViewModel> UeberhangModels = new Dictionary<int, UeberhangmandatViewModel>();

        // GET: Ueberhangmandate
        public ActionResult Index(int electionId)
        {
            if(!IsCustomCaching() || !UeberhangModels.ContainsKey(electionId) || UeberhangModels[electionId] == null)
                UeberhangModels[electionId] = CallService(() => Service.GetUeberhangmandate(electionId));

            //Needed because of caching
            AddStackTraceInfoToViewBag();

            return View(UeberhangModels[electionId]);
        }
    }
}