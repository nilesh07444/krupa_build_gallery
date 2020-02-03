using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
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
                               where !c.IsDelete
                               select new CategoryVM
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   CategoryImage = c.CategoryName,
                                   IsActive = c.IsActive
                               }).OrderByDescending(x => x.CategoryId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            
            return View(lstCategory);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(CategoryVM categoryVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existCategory = _db.tbl_Categories.Where(x => x.CategoryName.ToLower() == categoryVM.CategoryName.ToLower() && !x.IsDelete).FirstOrDefault();
                    if (existCategory != null)
                    {
                        ModelState.AddModelError("CategoryName", ErrorMessage.CategoryNameExists);
                        return View(categoryVM);
                    }

                    tbl_Categories objCategory = new tbl_Categories();
                    objCategory.CategoryName = categoryVM.CategoryName;

                    objCategory.IsActive = true;
                    objCategory.IsDelete = false;
                    objCategory.CreatedBy = LoggedInUserId;
                    objCategory.CreatedDate = DateTime.UtcNow;
                    objCategory.UpdatedBy = LoggedInUserId;
                    objCategory.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_Categories.Add(objCategory);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(categoryVM);
        }

        public ActionResult Edit(int Id)
        {
            CategoryVM objCategory = new CategoryVM();

            try
            {
                objCategory = (from c in _db.tbl_Categories
                               where c.CategoryId == Id
                               select new CategoryVM
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   CategoryImage = c.CategoryName,
                                   IsActive = c.IsActive
                               }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objCategory);
        }

        [HttpPost]
        public ActionResult Edit(CategoryVM categoryVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existCategory = _db.tbl_Categories.Where(x => x.CategoryId != categoryVM.CategoryId && x.CategoryName.ToLower() == categoryVM.CategoryName.ToLower() && !x.IsDelete).FirstOrDefault();
                    if (existCategory != null)
                    {
                        ModelState.AddModelError("CategoryName", ErrorMessage.CategoryNameExists);
                        return View(categoryVM);
                    }

                    tbl_Categories objCategory = _db.tbl_Categories.Where(x => x.CategoryId == categoryVM.CategoryId).FirstOrDefault();
                    objCategory.CategoryName = categoryVM.CategoryName;

                    objCategory.UpdatedBy = LoggedInUserId;
                    objCategory.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(categoryVM);
        }

        [HttpPost]
        public string DeleteCategory(long CategoryId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Categories objCategory = _db.tbl_Categories.Where(x => x.CategoryId == CategoryId && x.IsActive && !x.IsDelete).FirstOrDefault();

                if (objCategory == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objCategory.IsDelete = true;
                    objCategory.UpdatedBy = LoggedInUserId;
                    objCategory.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

    }
}