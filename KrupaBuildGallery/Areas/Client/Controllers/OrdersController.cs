using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Client/Orders
        public ActionResult Index()
        {
            return View();
        }
    }
}