using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class MyProfileController : Controller
    {
        // GET: Admin/MyProfile
        public ActionResult Index()
        {
            return View();
        }
    }
}