using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class CategoryController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public CategoryController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<CategoryVM> lstCategory = new List<CategoryVM>();
            try
            {
                lstCategory = (from c in _db.tbl_Categories
                               where !c.IsDelete && c.IsActive
                               select new CategoryVM
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   CategoryImage = c.CategoryImage,
                                   IsActive = c.IsActive
                               }).OrderBy(x => x.CategoryName).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstCategory);
        }
    }
}