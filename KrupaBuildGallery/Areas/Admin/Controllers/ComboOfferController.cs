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
    public class ComboOfferController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public ComboOfferController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<ComboOfferVM> lstOfferVm = new List<ComboOfferVM>();
            try
            {
                lstOfferVm = (from i in _db.tbl_ComboOffer
                              join p in _db.tbl_ProductItems on i.MainItemId equals p.ProductItemId
                              join s in _db.tbl_ProductItems on i.SubItemProductId equals s.ProductItemId
                              where i.IsDeleted == false
                             select new ComboOfferVM
                             {
                                ComboOfferId = i.ComboOfferId,
                                Main_CategoryId = i.MainItemCatId,
                                Main_ProductId = i.MainItemProductId.Value,
                                Main_SubProductId = i.MainItemsSubProductId,
                                Main_ProductItemId = i.MainItemId,
                                Main_Qty = i.MainItemQty,
                                Sub_CategoryId = i.SubItemCatId.Value,
                                Sub_ProductItemId = i.SubItemId.Value,
                                Sub_ProductId = i.SubItemProductId.Value,
                                Sub_SubProductId = i.SubItemSubProductId.Value,
                                Sub_Qty = i.SubItemQty.Value,
                                Main_ProductItemName = p.ItemName,
                                Sub_ProductItemName = s.ItemName,
                                OfferTitle = i.OfferTitle,
                                OfferImage = i.OfferImage,
                                dtOfferEndDate = i.OfferEndDate,
                                dtOfferStartDate = i.OfferStartDate,
                                ComboOfferPrice = i.OfferPrice,
                                IsActive = i.IsActive.Value
                            }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstOfferVm);
        }

        public ActionResult Add()
        {
            ComboOfferVM objOffer = new ComboOfferVM();

            objOffer.Main_CategoryList = GetCategoryList();
            objOffer.Main_ProductList = new List<SelectListItem>();
            objOffer.Main_SubProductList = new List<SelectListItem>();
            objOffer.Main_ProductItemList = new List<SelectListItem>();

            objOffer.Sub_CategoryList = GetCategoryList();
            objOffer.Sub_ProductList = new List<SelectListItem>();
            objOffer.Sub_SubProductList = new List<SelectListItem>();
            objOffer.Sub_ProductItemList = new List<SelectListItem>();

            return View(objOffer);
        }

        [HttpPost]
        public ActionResult Add(ComboOfferVM objOffer, HttpPostedFileBase OfferImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    // Check for exist
                    string fileName = string.Empty;
                    string path = Server.MapPath("~/Images/ComboOffer/");
                    if (OfferImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(OfferImageFile.FileName);
                        OfferImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objOffer.OfferImage;
                    }

                    tbl_ComboOffer objComoffer = new tbl_ComboOffer();
                    objComoffer.MainItemCatId = objOffer.Main_CategoryId;
                    objComoffer.MainItemId = objOffer.Main_ProductItemId;
                    objComoffer.MainItemProductId = objOffer.Main_ProductId;
                    objComoffer.MainItemsSubProductId = objOffer.Main_SubProductId;
                    objComoffer.MainItemQty = objOffer.Main_Qty;

                    objComoffer.SubItemCatId = objOffer.Sub_CategoryId;
                    objComoffer.SubItemId = objOffer.Sub_ProductItemId;
                    objComoffer.SubItemProductId = objOffer.Sub_ProductId;
                    objComoffer.SubItemSubProductId = objOffer.Sub_SubProductId;
                    objComoffer.SubItemQty = objOffer.Sub_Qty;
                    objComoffer.OfferTitle = objOffer.OfferTitle;
                    DateTime dtStart = DateTime.ParseExact(objOffer.OfferStartDate, "dd/MM/yyyy", null);
                    objComoffer.OfferStartDate = dtStart;

                    DateTime dtEnd = DateTime.ParseExact(objOffer.OfferEndDate, "dd/MM/yyyy", null);
                    objComoffer.OfferEndDate = dtEnd;
                    objComoffer.OfferImage = fileName;
                    objComoffer.OfferPrice = objOffer.ComboOfferPrice;
                    objComoffer.IsActive = true;
                    objComoffer.IsDeleted = false;
                    objComoffer.CreatedBy = LoggedInUserId;
                    objComoffer.CreatedDate = DateTime.UtcNow;
                    objComoffer.ModifiedBy = LoggedInUserId;
                    objComoffer.ModifiedDate = DateTime.UtcNow;

                    _db.tbl_ComboOffer.Add(objComoffer);
                    _db.SaveChanges();
                   
                    return RedirectToAction("Add");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            objOffer.Main_CategoryList = GetCategoryList();
            objOffer.Main_ProductList = new List<SelectListItem>();
            objOffer.Main_SubProductList = new List<SelectListItem>();
            objOffer.Main_ProductItemList = new List<SelectListItem>();

            objOffer.Sub_CategoryList = GetCategoryList();
            objOffer.Sub_ProductList = new List<SelectListItem>();
            objOffer.Sub_SubProductList = new List<SelectListItem>();
            objOffer.Sub_ProductItemList = new List<SelectListItem>();

            return View(objOffer);
        }

        public ActionResult Edit(int Id)
        {
            ComboOfferVM objOffer = new ComboOfferVM();

            try
            {

                objOffer = (from i in _db.tbl_ComboOffer
                                  where i.ComboOfferId == Id
                                  select new ComboOfferVM
                                  {
                                      ComboOfferId = i.ComboOfferId,
                                      Main_CategoryId = i.MainItemCatId,
                                      Main_ProductId = i.MainItemProductId.Value,
                                      Main_SubProductId = i.MainItemsSubProductId,
                                      Main_ProductItemId = i.MainItemId,
                                      Main_Qty = i.MainItemQty,
                                      Sub_CategoryId = i.SubItemCatId.Value,
                                      Sub_ProductItemId = i.SubItemId.Value,
                                      Sub_ProductId = i.SubItemProductId.Value,
                                      Sub_SubProductId = i.SubItemSubProductId.Value,
                                      Sub_Qty = i.SubItemQty.Value,
                                      OfferTitle = i.OfferTitle,
                                      OfferImage = i.OfferImage,
                                      dtOfferEndDate = i.OfferEndDate,
                                      dtOfferStartDate = i.OfferStartDate,
                                      ComboOfferPrice = i.OfferPrice                                      
                                  }).FirstOrDefault();

                objOffer.OfferStartDate = Convert.ToDateTime(objOffer.dtOfferStartDate).ToString("dd/MM/yyyy");
                objOffer.OfferEndDate = Convert.ToDateTime(objOffer.dtOfferEndDate).ToString("dd/MM/yyyy");
                objOffer.Main_CategoryList = GetCategoryList();
                objOffer.Main_ProductList = GetProductListByCategoryId(objOffer.Main_CategoryId);
                objOffer.Main_SubProductList = GetSubProductListByProductId(objOffer.Main_ProductId);
                objOffer.Main_ProductItemList = GetProductItems(objOffer.Main_ProductId, objOffer.Main_SubProductId);

                objOffer.Sub_CategoryList = GetCategoryList();
                objOffer.Sub_ProductList = GetProductListByCategoryId(objOffer.Sub_CategoryId);
                objOffer.Sub_SubProductList = GetSubProductListByProductId(objOffer.Sub_ProductId);
                objOffer.Sub_ProductItemList = GetProductItems(objOffer.Sub_ProductId, objOffer.Sub_SubProductId);

                
            }
            catch (Exception ex)
            {
            }

            return View(objOffer);
        }

        [HttpPost]
        public ActionResult Edit(ComboOfferVM objOffer,HttpPostedFileBase OfferImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    // Check for exist
                    tbl_ComboOffer objComoffer = _db.tbl_ComboOffer.Where(o => o.ComboOfferId == objOffer.ComboOfferId).FirstOrDefault();
                    string fileName = string.Empty;
                    string path = Server.MapPath("~/Images/ComboOffer/");
                    if (OfferImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(OfferImageFile.FileName);
                        OfferImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objOffer.OfferImage;
                    }
                  
                    objComoffer.MainItemCatId = objOffer.Main_CategoryId;
                    objComoffer.MainItemId = objOffer.Main_ProductItemId;
                    objComoffer.MainItemProductId = objOffer.Main_ProductId;
                    objComoffer.MainItemsSubProductId = objOffer.Main_SubProductId;
                    objComoffer.MainItemQty = objOffer.Main_Qty;

                    objComoffer.SubItemCatId = objOffer.Sub_CategoryId;
                    objComoffer.SubItemId = objOffer.Sub_ProductItemId;
                    objComoffer.SubItemProductId = objOffer.Sub_ProductId;
                    objComoffer.SubItemSubProductId = objOffer.Sub_SubProductId;
                    objComoffer.SubItemQty = objOffer.Sub_Qty;
                    objComoffer.OfferTitle = objOffer.OfferTitle;
                    DateTime dtStart = DateTime.ParseExact(objOffer.OfferStartDate, "dd/MM/yyyy", null);
                    objComoffer.OfferStartDate = dtStart;

                    DateTime dtEnd = DateTime.ParseExact(objOffer.OfferEndDate, "dd/MM/yyyy", null);
                    objComoffer.OfferEndDate = dtEnd;
                    objComoffer.OfferImage = fileName;
                    objComoffer.OfferPrice = objOffer.ComboOfferPrice;
                    objComoffer.ModifiedBy = LoggedInUserId;
                    objComoffer.ModifiedDate = DateTime.UtcNow;

                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            //objOffer.Main_CategoryList = GetCategoryList();
            //objOffer.Main_ProductList = new List<SelectListItem>();
            //objOffer.Main_SubProductList = new List<SelectListItem>();
            //objOffer.Main_ProductItemList = new List<SelectListItem>();

            //objOffer.Sub_CategoryList = GetCategoryList();
            //objOffer.Sub_ProductList = new List<SelectListItem>();
            //objOffer.Sub_SubProductList = new List<SelectListItem>();
            //objOffer.Sub_ProductItemList = new List<SelectListItem>();

            return View(objOffer);
        }

        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

        [HttpPost]
        public string DeleteOffer(long ComboOfferId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_ComboOffer objtbloffer = _db.tbl_ComboOffer.Where(x => x.ComboOfferId == ComboOfferId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();

                if (objtbloffer == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objtbloffer.IsDeleted = true;
                    objtbloffer.ModifiedBy = LoggedInUserId;
                    objtbloffer.ModifiedDate = DateTime.UtcNow;

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
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_ComboOffer objtbl_Offers = _db.tbl_ComboOffer.Where(x => x.ComboOfferId == Id).FirstOrDefault();

                if (objtbl_Offers != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_Offers.IsActive = true;
                    }
                    else
                    {
                        objtbl_Offers.IsActive = false;
                    }

                    objtbl_Offers.ModifiedBy = LoggedInUserId;
                    objtbl_Offers.ModifiedDate = DateTime.UtcNow;

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

        public List<SelectListItem> GetProductListByCategoryId(long Id)
        {
            var ProductList = _db.tbl_Products.Where(x => x.IsActive && !x.IsDelete && x.CategoryId == Id)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.Product_Id).Trim(), Text = o.ProductName })
                         .OrderBy(x => x.Text).ToList();

            return ProductList;
        }

        public List<SelectListItem> GetSubProductListByProductId(long Id)
        {
            var ProductList = _db.tbl_SubProducts.Where(x => x.IsActive && !x.IsDelete && x.ProductId == Id)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.SubProductId).Trim(), Text = o.SubProductName })
                         .OrderBy(x => x.Text).ToList();

            return ProductList;
        }

        public List<SelectListItem> GetProductItems(long ProductId, long? SubProductId)
        {
            var ProductItemList = _db.tbl_ProductItems.Where(x => x.ProductId == ProductId && (SubProductId == 0 || x.SubProductId == SubProductId) && x.IsActive && !x.IsDelete)
                        .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.ProductItemId).Trim(), Text = o.ItemName })
                        .OrderBy(x => x.Text).ToList();

            return ProductItemList;
        }

    }
}