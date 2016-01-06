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

            GetMessageBag().Info.Add(@"Mit <strong>Erststimme=Partei</strong> bzw. <strong>Zweiststimme=Partei</strong> kann auch nach der Siegerpartei nach Erststimmen bzw. Zweitstimmen gefiltert werden.");

            return View(model);
        }
    }
}