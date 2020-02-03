using ConstructionDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class DistributorController : Controller
    {
        // GET: Admin/Distributor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RequestList()
        {
            return View();
        }
    }
}