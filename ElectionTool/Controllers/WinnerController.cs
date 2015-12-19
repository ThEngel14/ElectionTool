using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectionTool.Controllers
{
    public class WinnerController : BaseController
    {
        // GET: Winner
        public ActionResult Index(int electionId)
        {
            var model = CallService(() => Service.GetWinnerWahlkreise(electionId));

            return View(model);
        }
    }
}