using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
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
    public class SubProductController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public SubProductController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {
            List<SubProductVM> lstSubProducts = new List<SubProductVM>();
            try
            {

                lstSubProducts = (from s in _db.tbl_SubProducts
                                  join c in _db.tbl_Categories on s.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on s.ProductId equals p.Product_Id
                                  where !s.IsDelete && !c.IsDelete && !p.IsDelete
                                  select new SubProductVM
                                  {
                                      SubProductId = s.SubProductId,
                                      SubProductName = s.SubProductName,
                                      ProductId = p.Product_Id,
                                      CategoryName = c.CategoryName,
                                      SubProductImage = s.SubProductImage,
                                      CategoryId = c.CategoryId,
                                      ProductName = p.ProductName,
                                      IsActive = c.IsActive
                                  }).OrderByDescending(x => x.ProductId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstSubProducts);
        }

        public ActionResult Add()
        {
            SubProductVM objSubProduct = new SubProductVM();

            objSubProduct.CategoryList = GetCategoryList();
            objSubProduct.ProductList = new List<SelectListItem>();

            return View(objSubProduct);
        }

        [HttpPost]
        public ActionResult Add(SubProductVM subcategoryVM, HttpPostedFileBase SubProductImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existCategory = _db.tbl_SubProducts.Where(x => x.SubProductName.ToLower() == subcategoryVM.SubProductName.ToLower()
                        && x.CategoryId == subcategoryVM.CategoryId && x.ProductId == subcategoryVM.ProductId
                        && !x.IsDelete).FirstOrDefault();

                    if (existCategory != null)
                    {
                        ModelState.AddModelError("SubProductName", ErrorMessage.SubProductNameExists);
                    }
                    else
                    {
                        string fileName = string.Empty;
                        string path = Server.MapPath("~/Images/SubProductMedia/");
                        if (SubProductImageFile != null)
                        {
                            fileName = Guid.NewGuid() + "-" + Path.GetFileName(SubProductImageFile.FileName);
                            SubProductImageFile.SaveAs(path + fileName);
                        }
                        else
                        {
                            fileName = subcategoryVM.SubProductImage;
                        }

                        tbl_SubProducts objSubCategory = new tbl_SubProducts();
                        objSubCategory.CategoryId = subcategoryVM.CategoryId;
                        objSubCategory.ProductId = subcategoryVM.ProductId;
                        objSubCategory.SubProductName = subcategoryVM.SubProductName;
                        objSubCategory.SubProductImage = fileName;

                        objSubCategory.IsActive = true;
                        objSubCategory.IsDelete = false;
                        objSubCategory.CreatedBy = LoggedInUserId;
                        objSubCategory.CreatedDate = DateTime.UtcNow;
                        objSubCategory.UpdatedBy = LoggedInUserId;
                        objSubCategory.UpdatedDate = DateTime.UtcNow;

                        _db.tbl_SubProducts.Add(objSubCategory);
                        _db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            // bind dropdown if get error
            subcategoryVM.CategoryList = GetCategoryList();
            subcategoryVM.ProductList = new List<SelectListItem>();

            return View(subcategoryVM);
        }

        public ActionResult Edit(int Id)
        {
            SubProductVM subcategoryVM = (from s in _db.tbl_SubProducts
                                          join c in _db.tbl_Categories on s.CategoryId equals c.CategoryId
                                          join p in _db.tbl_Products on s.ProductId equals p.Product_Id
                                          where s.SubProductId == Id
                                          select new SubProductVM
                                          {
                                              SubProductId = s.SubProductId,
                                              CategoryId = s.CategoryId,
                                              ProductId = s.ProductId,
                                              SubProductName = s.SubProductName,
                                              SubProductImage = s.SubProductImage
                                          }).FirstOrDefault();

            subcategoryVM.CategoryList = GetCategoryList();
            subcategoryVM.ProductList = _db.tbl_Products.Where(x => x.CategoryId == subcategoryVM.CategoryId && x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.Product_Id).Trim(), Text = o.ProductName })
                         .OrderBy(x => x.Text).ToList();

            return View(subcategoryVM);
        }

        [HttpPost]
        public ActionResult Edit(SubProductVM subcategoryVM, HttpPostedFileBase SubProductImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existCategory = _db.tbl_SubProducts.Where(x => x.SubProductId != subcategoryVM.SubProductId && x.SubProductName.ToLower() == subcategoryVM.SubProductName.ToLower()
                        && x.CategoryId == subcategoryVM.CategoryId && x.ProductId == subcategoryVM.ProductId
                        && !x.IsDelete).FirstOrDefault();

                    if (existCategory != null)
                    {
                        ModelState.AddModelError("SubProductName", ErrorMessage.SubProductNameExists);
                    }
                    else
                    {
                        tbl_SubProducts objSubCategory = _db.tbl_SubProducts.Where(x => x.SubProductId == subcategoryVM.SubProductId).FirstOrDefault();

                        string fileName = string.Empty;
                        string path = Server.MapPath("~/Images/SubProductMedia/");
                        if (SubProductImageFile != null)
                        {
                            fileName = Guid.NewGuid() + "-" + Path.GetFileName(SubProductImageFile.FileName);
                            SubProductImageFile.SaveAs(path + fileName);
                        }
                        else
                        {
                            fileName = objSubCategory.SubProductImage;
                        }

                        objSubCategory.CategoryId = subcategoryVM.CategoryId;
                        objSubCategory.ProductId = subcategoryVM.ProductId;
                        objSubCategory.SubProductName = subcategoryVM.SubProductName;
                        objSubCategory.SubProductImage = fileName;

                        objSubCategory.UpdatedBy = LoggedInUserId;
                        objSubCategory.UpdatedDate = DateTime.UtcNow;

                        _db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            // bind dropdown if get error
            subcategoryVM.CategoryList = GetCategoryList();
            subcategoryVM.ProductList = new List<SelectListItem>();

            return View(subcategoryVM);
        }

        [HttpPost]
        public string DeleteSubProduct(long SubProductId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_SubProducts objProducts = _db.tbl_SubProducts.Where(x => x.SubProductId == SubProductId && x.IsActive && !x.IsDelete).FirstOrDefault();

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
         
        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

        public JsonResult GetProductListByCategoryId(double Id)
        {
            var CategoryList = _db.tbl_Products.Where(x => x.CategoryId == Id && x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.Product_Id).Trim(), Text = o.ProductName })
                         .OrderBy(x => x.Text).ToList();

            return Json(CategoryList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubProductListByProductId(double Id)
        {
            var CategoryList = _db.tbl_SubProducts.Where(x => x.ProductId == Id && x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.SubProductId).Trim(), Text = o.SubProductName })
                         .OrderBy(x => x.Text).ToList();

            return Json(CategoryList, JsonRequestBehavior.AllowGet);
        }


    }
}