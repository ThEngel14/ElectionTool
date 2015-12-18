using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
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

        protected T CallService<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (PublicException e)
            {
                AddException(e);
            }
            catch (Exception e)
            {
                AddException(e);
            }

            return default(T);
        }

        protected void CallService(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (PublicException e)
            {
                AddException(e);
            }
            catch (Exception e)
            {
                AddException(e);
            }
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

        private bool AllowedToSeeAllExceptions()
        {
            var seeAllExceptions = false;
            bool.TryParse(WebConfigurationManager.AppSettings["SeeAllExceptions"], out seeAllExceptions);

            return seeAllExceptions;
        }

        private bool AllowedToSeeStackTrace()
        {
            var seeStackTrace = false;
            bool.TryParse(WebConfigurationManager.AppSettings["SeeStackTrace"], out seeStackTrace);

            return seeStackTrace;
        }

        public void AddException(Exception e)
        {
            var messages = GetMessageBag();
            if (AllowedToSeeAllExceptions())
            {
                messages.Danger.Add(AllowedToSeeStackTrace() ? FlattenException(e) : e.Message);
            }
            else
            {
                messages.Danger.Add("Es ist ein unbekannter Fehler aufgetreten.");   
            }
        }

        public void AddException(PublicException e)
        {
            var messages = GetMessageBag();
            messages.Danger.Add(AllowedToSeeStackTrace() ? FlattenException(e) : e.Message);
        }

        private static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        protected bool IsCustomCaching()
        {
            var caching = false;
            bool.TryParse(WebConfigurationManager.AppSettings["CustomCaching"], out caching);

            return caching;
        }
    }
}