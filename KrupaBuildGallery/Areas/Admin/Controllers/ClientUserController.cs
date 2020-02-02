using ConstructionDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ClientUserController : Controller
    {
        // GET: Admin/ClientUser
        public ActionResult Index()
        {
            return View();
        }
    }
}