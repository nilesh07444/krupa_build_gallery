using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class AccessDeniedController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }
    }
}