﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class ProductItemController : Controller
    {
        // GET: Admin/ProductItem
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View("~/Areas/Admin/Views/ProductItem/AddProductItem.cshtml");
        }
        public ActionResult Edit(int id)
        {
            return View("~/Areas/Admin/Views/ProductItem/AddProductItem.cshtml");
        }
    }
}