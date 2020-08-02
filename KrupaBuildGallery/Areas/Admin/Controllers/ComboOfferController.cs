using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel.Admin;
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
                lstOfferVm = (from i in _db.tbl_ComboOfferMaster
                              join p in _db.tbl_ProductItems on i.MainItemId equals p.ProductItemId                              
                              where i.IsDeleted == false
                             select new ComboOfferVM
                             {
                                ComboOfferId = i.ComboOfferId,
                                Main_CategoryId = i.MainItemCatId,
                                Main_ProductId = i.MainItemProductId.Value,
                                Main_SubProductId = i.MainItemsSubProductId,
                                Main_ProductItemId = i.MainItemId,
                                Main_Qty = i.MainItemQty,                               
                                Main_ProductItemName = p.ItemName,                               
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
        public ActionResult Add(ComboOfferVM objOffer, HttpPostedFileBase OfferImageFile,FormCollection frm)
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

                    tbl_ComboOfferMaster objComoffer = new tbl_ComboOfferMaster();
                    objComoffer.MainItemCatId = objOffer.Main_CategoryId;
                    objComoffer.MainItemId = objOffer.Main_ProductItemId;
                    objComoffer.MainItemProductId = objOffer.Main_ProductId;
                    objComoffer.MainItemsSubProductId = objOffer.Main_SubProductId;
                    objComoffer.MainItemQty = Convert.ToInt64(objOffer.Main_Qty);
                    objComoffer.MainItemVarintId = Convert.ToInt64(frm["MainVariantItem"].ToString());
                    objComoffer.OfferTitle = objOffer.OfferTitle;
                    objComoffer.Description = objOffer.OfferDescription;
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
                    objComoffer.IsCashOnDelivery = objOffer.IsCashonDelieveryuse;
                    objComoffer.TotalActualPrice = Convert.ToDecimal(frm["hdnTotalActualOfferPrice"].ToString());
                    objComoffer.MainItemActualPrice = Convert.ToDecimal(frm["hdnTotalMain"].ToString());
                    _db.tbl_ComboOfferMaster.Add(objComoffer);
                    _db.SaveChanges();
                    if(frm["SubItemProductItem"] != null)
                    {
                        string[] arrySubItems = Request.Form.GetValues("SubItemProductItem");
                        string[] arrySubItemsCat = Request.Form.GetValues("SubItemCategory");
                        string[] arrySubItemsProd = Request.Form.GetValues("SubItemProduct");
                        string[] arrySubItemsSubProd = Request.Form.GetValues("SubItemSubProduct");
                        string[] arrySubItemsVariant = Request.Form.GetValues("SubItemVarints");
                        string[] arrySubItemsQty = Request.Form.GetValues("SubItemQty");
                        string[] arrySubItemsTtl = Request.Form.GetValues("hdntotl");
                        for (int j=0; j< arrySubItems.Length;j++)
                        {
                            tbl_ComboOfferSubItems objSub = new tbl_ComboOfferSubItems();
                            objSub.ComboOfferId = objComoffer.ComboOfferId;
                            if(arrySubItems[j] != "" && arrySubItems[j] != "0")
                            {
                                objSub.CategoryId = GetInt64Val(arrySubItemsCat[j]);
                                objSub.ProductId = GetInt64Val(arrySubItemsProd[j]);
                                objSub.SubProductId = GetInt64Val(arrySubItemsSubProd[j]);
                                objSub.VariantItemId = GetInt64Val(arrySubItemsVariant[j]);
                                objSub.ProductItemId = GetInt64Val(arrySubItems[j]);
                                objSub.Qty = GetInt64Val(arrySubItemsQty[j]);
                                objSub.OfferPrice = 0;
                                objSub.ActualPrice = Convert.ToDecimal(arrySubItemsTtl[j]);
                                objSub.IsDeleted = false;
                                objSub.CreatedBy = LoggedInUserId;
                                objSub.CreatedDate = DateTime.UtcNow;
                                objSub.ModifiedBy = LoggedInUserId;
                                objSub.ModifiedDate = DateTime.UtcNow;
                                _db.tbl_ComboOfferSubItems.Add(objSub);
                                _db.SaveChanges();
                            }
                        }
                    }
                   

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
                objOffer = (from i in _db.tbl_ComboOfferMaster
                            where i.ComboOfferId == Id
                                  select new ComboOfferVM
                                  {
                                      ComboOfferId = i.ComboOfferId,
                                      Main_CategoryId = i.MainItemCatId,
                                      Main_ProductId = i.MainItemProductId.Value,
                                      Main_SubProductId = i.MainItemsSubProductId,
                                      Main_ProductItemId = i.MainItemId,
                                      Main_Qty = i.MainItemQty,
                                      OfferTitle = i.OfferTitle,
                                      OfferImage = i.OfferImage,
                                      IsCashonDelieveryuse = i.IsCashOnDelivery.HasValue ? i.IsCashOnDelivery.Value : false,
                                      dtOfferEndDate = i.OfferEndDate,
                                      dtOfferStartDate = i.OfferStartDate,
                                      OfferDescription = i.Description,
                                      ComboOfferPrice = i.OfferPrice,
                                      MainVariantId = i.MainItemVarintId
                                  }).FirstOrDefault();

                objOffer.OfferStartDate = Convert.ToDateTime(objOffer.dtOfferStartDate).ToString("dd/MM/yyyy");
                objOffer.OfferEndDate = Convert.ToDateTime(objOffer.dtOfferEndDate).ToString("dd/MM/yyyy");
                objOffer.Main_CategoryList = GetCategoryList();
                objOffer.Main_ProductList = GetProductListByCategoryId(objOffer.Main_CategoryId);
                objOffer.Main_SubProductList = GetSubProductListByProductId(objOffer.Main_ProductId);
                objOffer.Main_ProductItemList = GetProductItems(objOffer.Main_ProductId, objOffer.Main_SubProductId);
                ViewData["MainVariantList"] = _db.tbl_ItemVariant.Where(x => (Id == -1 || x.ProductItemId.Value == objOffer.Main_ProductItemId) && x.IsActive == true).OrderBy(x => x.UnitQty).ToList();
                              

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
                    tbl_ComboOfferMaster objComoffer = _db.tbl_ComboOfferMaster.Where(o => o.ComboOfferId == objOffer.ComboOfferId).FirstOrDefault();
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
                    objComoffer.MainItemQty = Convert.ToInt64(objOffer.Main_Qty);
                    objComoffer.Description = objOffer.OfferDescription;
                    //objComoffer.SubItemCatId = objOffer.Sub_CategoryId;
                    //objComoffer.SubItemId = objOffer.Sub_ProductItemId;
                    //objComoffer.SubItemProductId = objOffer.Sub_ProductId;
                    //objComoffer.SubItemSubProductId = objOffer.Sub_SubProductId;
                    //objComoffer.SubItemQty = objOffer.Sub_Qty;
                    objComoffer.OfferTitle = objOffer.OfferTitle;
                    DateTime dtStart = DateTime.ParseExact(objOffer.OfferStartDate, "dd/MM/yyyy", null);
                    objComoffer.OfferStartDate = dtStart;
                    objComoffer.IsCashOnDelivery = objOffer.IsCashonDelieveryuse;
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
                tbl_ComboOfferMaster objtbloffer = _db.tbl_ComboOfferMaster.Where(x => x.ComboOfferId == ComboOfferId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();

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
                tbl_ComboOfferMaster objtbl_Offers = _db.tbl_ComboOfferMaster.Where(x => x.ComboOfferId == Id).FirstOrDefault();

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

        public JsonResult GetVariantListByItemId(double Id)
        {
            var tbl_ItemVariantList = _db.tbl_ItemVariant.Where(x => (Id == -1 || x.ProductItemId.Value == Id) && x.IsActive == true).OrderBy(x => x.UnitQty).ToList();

            return Json(tbl_ItemVariantList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubItemsOfCombo(int Id)
        {
            ViewBag.Id = Id;
            ViewData["catrgories"] = GetCategoryList();
            return PartialView("~/Areas/Admin/Views/ComboOffer/_SubItems.cshtml");
        }

        public ActionResult GetSubItemsOfComboEdit(long ComboId)
        {
            List<tbl_ComboOfferSubItems> lstComboSub = _db.tbl_ComboOfferSubItems.Where(o => o.ComboOfferId == ComboId).ToList();
            List<ComboSubItemVM> lstSubItemss = new List<ComboSubItemVM>();
            if (lstComboSub != null && lstComboSub.Count() > 0)
            {
                foreach (var objsub in lstComboSub)
                {
                    if (objsub != null)
                    {
                        ComboSubItemVM objj = new ComboSubItemVM();
                        objj.CategoryId = objsub.CategoryId;
                        objj.ProductId = objsub.ProductId;
                        objj.SubProductId = objsub.SubProductId.Value;
                        objj.Qty = objsub.Qty;
                        objj.ProductItemId = objsub.ProductItemId;
                        objj.VarintId = objsub.VariantItemId;
                        objj.ActualPrice = objsub.ActualPrice.Value;
                        objj.Sub_CategoryList = GetCategoryList();
                        objj.Sub_ProductList = GetProductListByCategoryId(objj.CategoryId);
                        objj.Sub_SubProductList = GetSubProductListByProductId(objj.ProductId);
                        objj.Sub_ProductItemList = GetProductItems(objj.ProductId, objj.SubProductId);
                        objj.Sub_ProductVariantList = _db.tbl_ItemVariant.Where(x => (ComboId == -1 || x.ProductItemId.Value == objj.ProductItemId) && x.IsActive == true).OrderBy(x => x.UnitQty).ToList();
                        lstSubItemss.Add(objj);
                    }
                }
            }
            //objOffer.Sub_CategoryList = GetCategoryList();
            //objOffer.Sub_ProductList = GetProductListByCategoryId(objOffer.Sub_CategoryId);
            //objOffer.Sub_SubProductList = GetSubProductListByProductId(objOffer.Sub_ProductId);
            //objOffer.Sub_ProductItemList = GetProductItems(objOffer.Sub_ProductId, objOffer.Sub_SubProductId);
            ViewData["lstSubItemss"] = lstSubItemss;
            return PartialView("~/Areas/Admin/Views/ComboOffer/_SubItemsEdit.cshtml");
        }

        public long GetInt64Val(string vl)
        {
            long reurnvl = 0;
            if(!string.IsNullOrEmpty(vl))
            {
                reurnvl = Convert.ToInt64(vl);
            }
            return reurnvl;
        }
    }
}