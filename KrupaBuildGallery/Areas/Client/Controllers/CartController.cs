﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class CartController : Controller
    {
        // GET: Client/Cart
        public ActionResult Index()
        {
            return View();
        }
    }
}