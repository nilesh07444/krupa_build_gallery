using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ProductController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public ProductController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {
            List<ProductVM> lstProducts = new List<ProductVM>();
            try
            {

                lstProducts = (from p in _db.tbl_Products
                               join c in _db.tbl_Categories on p.CategoryId equals c.CategoryId
                               where !p.IsDelete
                               select new ProductVM
                               {
                                   ProductId = p.Product_Id,
                                   CategoryName = c.CategoryName,
                                   ProductImage = p.ProductImage,
                                   CategoryId = c.CategoryId,
                                   ProductName = p.ProductName,
                                   IsActive = c.IsActive
                               }).OrderByDescending(x => x.ProductId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstProducts);
        }

        public ActionResult Add()
        {
            ProductVM objProduct = new ProductVM();

            objProduct.CategoryList = GetCategoryList();

            return View(objProduct);
        }

        public ActionResult Edit(int Id)
        {
            ProductVM objProduct = new ProductVM();

            try
            {
                objProduct = (from c in _db.tbl_Products
                              where c.Product_Id == Id
                              select new ProductVM
                              {
                                  CategoryId = c.CategoryId,
                                  ProductId = c.Product_Id,
                                  ProductName = c.ProductName,
                                  ProductImage = c.ProductImage,
                                  IsActive = c.IsActive
                              }).FirstOrDefault();

                objProduct.CategoryList = GetCategoryList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objProduct);
        }

        [HttpPost]
        public ActionResult Add(ProductVM productVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existProduct = _db.tbl_Products.Where(x => x.ProductName.ToLower() == productVM.ProductName.ToLower() && !x.IsDelete).FirstOrDefault();
                    if (existProduct != null)
                    {
                        ModelState.AddModelError("ProductName", ErrorMessage.ProductNameExists);
                        return View(productVM);
                    }

                    tbl_Products objProducts = new tbl_Products();
                    objProducts.ProductName = productVM.ProductName;
                    objProducts.CategoryId = Convert.ToInt64(productVM.CategoryId);
                    objProducts.IsActive = true;
                    objProducts.IsDelete = false;
                    objProducts.CreatedBy = LoggedInUserId;
                    objProducts.CreatedDate = DateTime.UtcNow;
                    objProducts.UpdatedBy = LoggedInUserId;
                    objProducts.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_Products.Add(objProducts);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            productVM.CategoryList = GetCategoryList();

            return View(productVM);
        }

        [HttpPost]
        public string DeleteProduct(long ProductId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Products objProducts = _db.tbl_Products.Where(x => x.Product_Id == ProductId && x.IsActive && !x.IsDelete).FirstOrDefault();

                if (objProducts == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objProducts.IsDelete = true;
                    objProducts.UpdatedBy = LoggedInUserId;
                    objProducts.UpdatedDate = DateTime.UtcNow;

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

        [HttpPost]
        public ActionResult Edit(ProductVM productVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existProduct = _db.tbl_Products.Where(x => x.Product_Id != productVM.ProductId && x.ProductName.ToLower() == productVM.ProductName.ToLower() && !x.IsDelete).FirstOrDefault();
                    if (existProduct != null)
                    {
                        ModelState.AddModelError("ProductName", ErrorMessage.ProductNameExists);
                        return View(productVM);
                    }

                    tbl_Products objProducts = _db.tbl_Products.Where(x => x.Product_Id == productVM.ProductId).FirstOrDefault();
                    objProducts.ProductName = productVM.ProductName;
                    objProducts.CategoryId = Convert.ToInt64(productVM.CategoryId);
                    objProducts.UpdatedBy = LoggedInUserId;
                    objProducts.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            productVM.CategoryList = GetCategoryList();

            return View(productVM);
        }

        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive == true && x.IsDelete == false)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

    }
}