using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectionTool.Service;

namespace ElectionTool.Controllers
{
    public class BaseController : Controller
    {
        protected ElectionService Service;

        public BaseController()
        {
            Service = new ElectionService();
        }
    }
}