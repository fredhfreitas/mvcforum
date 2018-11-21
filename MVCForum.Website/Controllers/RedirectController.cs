using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcForum.Web.Controllers
{
    public class RedirectController : Controller
    {
        // GET: Redirect
        public ActionResult Index(string redirectToController)
        {
            return this.RedirectPermanent(redirectToController);
        }
    }
}