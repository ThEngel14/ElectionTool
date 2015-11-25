using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectionTool.Controllers
{
    public class WinnerFirstVotesController : BaseController
    {
        // GET: WinnerFirstVotes
        public ActionResult Index()
        {
            var model = Service.GetAllWinnerFirstVotes();

            return View(model);
        }
    }
}