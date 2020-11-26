using ConstructionDiary.Models;
using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.IO;
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

        [AdminPermission(RolePermissionEnum.View)]
        public ActionResult Index(int CategoryId = -1, int Active = -1)
        {
            List<ProductVM> lstProducts = new List<ProductVM>();
            try
            {
                bool IsActv = false;
                if (Active != -1)
                {
                    if (Active == 1)
                    {
                        IsActv = true;
                    }
                }
                lstProducts = (from p in _db.tbl_Products
                               join c in _db.tbl_Categories on p.CategoryId equals c.CategoryId
                               where !p.IsDelete && (CategoryId == -1 || p.CategoryId == CategoryId) && (Active == -1 || p.IsActive == IsActv)
                               select new ProductVM
                               {
                                   ProductId = p.Product_Id,
                                   CategoryName = c.CategoryName,
                                   ProductImage = p.ProductImage,
                                   CategoryId = c.CategoryId,
                                   ProductName = p.ProductName,
                                   IsActive = p.IsActive
                               }).OrderByDescending(x => x.ProductId).ToList();
                ViewData["CategoryList"] = GetCategoryList();
                ViewBag.CatId = CategoryId;
                ViewBag.Active = Active;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return View(lstProducts);
        }

        [AdminPermission(RolePermissionEnum.Add)]
        public ActionResult Add()
        {
            ProductVM objProduct = new ProductVM();

            objProduct.CategoryList = GetCategoryList();

            return View(objProduct);
        }

        [AdminPermission(RolePermissionEnum.Edit)]
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
        public ActionResult Add(ProductVM productVM, HttpPostedFileBase ProductImageFile)
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
                       // return View(productVM);
                    }
                    else
                    {
                        string fileName = string.Empty;
                        string path = Server.MapPath("~/Images/ProductMedia/");
                        if (ProductImageFile != null)
                        {
                            fileName = Guid.NewGuid() + "-" + Path.GetFileName(ProductImageFile.FileName);
                            ProductImageFile.SaveAs(path + fileName);
                        }
                        else
                        {
                            fileName = productVM.ProductImage;
                        }


                        tbl_Products objProducts = new tbl_Products();
                        objProducts.ProductName = productVM.ProductName;
                        objProducts.ProductImage = fileName;
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
        public ActionResult Edit(ProductVM productVM, HttpPostedFileBase ProductImageFile)
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

                    string fileName = string.Empty;
                    string path = Server.MapPath("~/Images/ProductMedia/");
                    if (ProductImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(ProductImageFile.FileName);
                        ProductImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objProducts.ProductImage;
                    }

                    objProducts.ProductName = productVM.ProductName;
                    objProducts.ProductImage = fileName;
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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Products objtbl_Products = _db.tbl_Products.Where(x => x.Product_Id == Id).FirstOrDefault();

                if (objtbl_Products != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_Products.IsActive = true;
                    }
                    else
                    {
                        objtbl_Products.IsActive = false;
                    }

                    objtbl_Products.UpdatedBy = LoggedInUserId;
                    objtbl_Products.UpdatedDate = DateTime.UtcNow;

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

        [AdminPermission(RolePermissionEnum.View)]
        public ActionResult View(int Id)
        {
            ProductVM objProduct = new ProductVM();

            try
            {
                objProduct = (from p in _db.tbl_Products
                              join c in _db.tbl_Categories on p.CategoryId equals c.CategoryId

                              join uC in _db.tbl_AdminUsers on p.CreatedBy equals uC.AdminUserId into outerCreated
                              from uC in outerCreated.DefaultIfEmpty()

                              join uM in _db.tbl_AdminUsers on p.UpdatedBy equals uM.AdminUserId into outerModified
                              from uM in outerModified.DefaultIfEmpty()

                              where p.Product_Id == Id
                              select new ProductVM
                              {
                                  CategoryId = p.CategoryId,
                                  ProductId = p.Product_Id,
                                  ProductName = p.ProductName,
                                  ProductImage = p.ProductImage,
                                  CategoryName = c.CategoryName,
                                  IsActive = p.IsActive,

                                  CreatedDate = p.CreatedDate,
                                  UpdatedDate = p.UpdatedDate,
                                  strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                  strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")
                                   
                              }).FirstOrDefault();
                 
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objProduct);
        }

        [HttpPost]
        public JsonResult GetProductNameText(string prefix,long CatId)
        {
            var itmtext = (from txt in _db.tbl_Products
                           where txt.ProductName.ToLower().Contains(prefix.ToLower()) && (CatId == 0 || txt.CategoryId == CatId)
                           select new
                           {
                               label = txt.ProductName,
                               val = txt.ProductName
                           }).ToList();

            return Json(itmtext);
        }

    }
}