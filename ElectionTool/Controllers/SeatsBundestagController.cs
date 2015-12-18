using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectionTool.Models;

namespace ElectionTool.Controllers
{
    public class SeatsBundestagController : BaseController
    {
        private static readonly Dictionary<int, AllSeatsBundestagViewModel> SeatsModels = new Dictionary<int, AllSeatsBundestagViewModel>(); 

        // GET: SeatsBundestag
        public ActionResult Index(int electionId)
        {
            if(!IsCustomCaching() || !SeatsModels.ContainsKey(electionId))
                SeatsModels[electionId] = Service.GetAllSeatsBundestag(electionId);

            return View(SeatsModels[electionId]);
        }
    }
}