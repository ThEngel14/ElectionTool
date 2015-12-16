using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectionTool.Controllers
{
    public class ClosestWinnerController : BaseController
    {
        // GET: ClosestWinner
        public ActionResult Index(int electionId)
        {
            var model = Service.GetAllPartiesForClosestWinner(electionId);

            return View(model);
        }

        // GET: ClosestWinner/Result
        public ActionResult Result(int electionId, int partyId)
        {
            var model = Service.GetClosestWinnerForParty(electionId, partyId);

            return View(model);
        }
    }
}