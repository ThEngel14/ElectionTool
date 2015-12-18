using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectionTool.Models;

namespace ElectionTool.Controllers
{
    public class MemberBundestagController : BaseController
    {
        private static readonly Dictionary<int, AllMemberOfBundestagViewModel> MemberModels = new Dictionary<int, AllMemberOfBundestagViewModel>(); 

        // GET: MemberBundestag
        public ActionResult Index(int electionId)
        {
            if(!IsCustomCaching() || !MemberModels.ContainsKey(electionId))
                MemberModels[electionId] = Service.GetAllMemberOfBundestag(electionId);

            return View(MemberModels[electionId]);
        }
    }
}