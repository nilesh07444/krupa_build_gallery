using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class StockController : Controller
    {
        // GET: Admin/Stock
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View("~/Areas/Admin/Views/Stock/AddStock.cshtml");
        }
        public ActionResult Edit(int id)
        {
            return View("~/Areas/Admin/Views/Stock/AddStock.cshtml");
        }
    }
}