using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectionTool.Controllers
{
    public class UeberhangmandateController : BaseController
    {
        // GET: Ueberhangmandate
        public ActionResult Index(int electionId)
        {
            var model = Service.GetUeberhangmandate(electionId);

            return View(model);
        }
    }
}