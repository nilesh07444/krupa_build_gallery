using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: WebAPI/Checkout
        public ActionResult Index()
        {
            return View();
        }
    }
}