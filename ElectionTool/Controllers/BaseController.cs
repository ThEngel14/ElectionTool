using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectionTool.Models;
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

        public MessageBag GetMessageBag()
        {
            var messages = TempData["Messages"] as MessageBag;
            if (messages != null)
            {
                return messages;
            }

            messages = new MessageBag();
            TempData["Messages"] = messages;

            return messages;
        }

        public void AddException(Exception e)
        {
            var messages = GetMessageBag();
            messages.Danger.Add(e.Message);
        }
    }
}