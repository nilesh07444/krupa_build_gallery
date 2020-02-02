using ConstructionDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ProductController : Controller
    {
        // GET: Admin/Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View("~/Areas/Admin/Views/Product/AddProduct.cshtml");
        }
        public ActionResult Edit(int id)
        {
            return View("~/Areas/Admin/Views/Product/AddProduct.cshtml");
        }
    }
}