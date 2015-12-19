using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectionTool.Controllers
{
    public class WahlkreisController : BaseController
    {
        // GET: Wahlkreis
        public ActionResult Index(int electionId)
        {
            var model = CallService(() => Service.GetWahlkreisSelection(electionId));

            return View(model);
        }

        // GET: Wahlkreis/Overview
        public ActionResult Overview(int electionId, int wahlkreisId)
        {
            var model = CallService(() => Service.GetWahlkreisOverview(electionId, wahlkreisId));

            return View(model);
        }
    }
}