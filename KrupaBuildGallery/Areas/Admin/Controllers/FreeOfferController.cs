using ConstructionDiary.Models;
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
    public class FreeOfferController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public FreeOfferController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/FreeOffer
        public ActionResult Index()
        {
            List<FreeOfferVM> lstOfferVm = new List<FreeOfferVM>();
            try
            {
                lstOfferVm = (from i in _db.tbl_FreeOffers                              
                              where i.IsDeleted == false
                              select new FreeOfferVM
                              {
                                  FreeOfferId = i.FreeOfferId,
                                  OrderAmountFrom = i.OrderAmountFrom.Value,
                                  OrderAmountTo = i.OrderAmountTo.Value,
                                  OfferStartDate = i.OfferStartDate.Value,
                                  OfferEndDate = i.OfferEndDate.Value,
                                  IsActive = i.IsActive.HasValue ? i.IsActive.Value : true
                              }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstOfferVm);
        }

        public ActionResult Add()
        {
            FreeOfferVM objOffer = new FreeOfferVM();

            return View(objOffer);
        }

        [HttpPost]
        public ActionResult Add(FreeOfferVM objOffer, FormCollection frm)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                   
                    tbl_FreeOffers objFreeoffer = new tbl_FreeOffers();
                    DateTime dtStart = DateTime.ParseExact(objOffer.OfferStartDateStr, "dd/MM/yyyy", null);
                    objFreeoffer.OfferStartDate = dtStart;

                    DateTime dtEnd = DateTime.ParseExact(objOffer.OfferEndDateStr, "dd/MM/yyyy", null);
                    objFreeoffer.OfferEndDate = dtEnd;
                    objFreeoffer.IsDeleted = false;
                    objFreeoffer.CreatedBy = LoggedInUserId;
                    objFreeoffer.OrderAmountFrom = objOffer.OrderAmountFrom;
                    objFreeoffer.OrderAmountTo = objOffer.OrderAmountTo;
                    objFreeoffer.CreatedDate = DateTime.UtcNow;
                    objFreeoffer.CreatedBy = clsAdminSession.UserID;
                    _db.tbl_FreeOffers.Add(objFreeoffer);
                    _db.SaveChanges();
                    if (frm["SubItemProductItem"] != null)
                    {
                        string[] arrySubItems = Request.Form.GetValues("SubItemProductItem");
                        string[] arrySubItemsCat = Request.Form.GetValues("SubItemCategory");
                        string[] arrySubItemsProd = Request.Form.GetValues("SubItemProduct");
                        string[] arrySubItemsSubProd = Request.Form.GetValues("SubItemSubProduct");
                        string[] arrySubItemsVariant = Request.Form.GetValues("SubItemVarints");
                        string[] arrySubItemsQty = Request.Form.GetValues("SubItemQty");
                        for (int j = 0; j < arrySubItems.Length; j++)
                        {
                            tbl_FreeOfferItems objSub = new tbl_FreeOfferItems();
                            objSub.FreeOfferId = objFreeoffer.FreeOfferId;
                            if (arrySubItems[j] != "" && arrySubItems[j] != "0")
                            {
                                objSub.CategoryId = GetInt64Val(arrySubItemsCat[j]);
                                objSub.ProductId = GetInt64Val(arrySubItemsProd[j]);
                                objSub.SubProductId = GetInt64Val(arrySubItemsSubProd[j]);
                                objSub.VariantItemId = GetInt64Val(arrySubItemsVariant[j]);
                                objSub.ProductItemId = GetInt64Val(arrySubItems[j]);                                
                                objSub.Qty = GetInt64Val(arrySubItemsQty[j]);
                                _db.tbl_FreeOfferItems.Add(objSub);
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
         
            return View(objOffer);
        }

        public ActionResult Edit(int Id)
        {
            FreeOfferVM objOffer = new FreeOfferVM();
            try
            {
                objOffer = (from i in _db.tbl_FreeOffers
                            where i.FreeOfferId == Id
                            select new FreeOfferVM
                            {
                                FreeOfferId = i.FreeOfferId,
                                OfferEndDate = i.OfferEndDate.Value,
                                OfferStartDate = i.OfferStartDate.Value,
                                OrderAmountFrom = i.OrderAmountFrom.Value,
                                OrderAmountTo = i.OrderAmountTo.Value                                
                            }).FirstOrDefault();

                objOffer.OfferStartDateStr = Convert.ToDateTime(objOffer.OfferStartDate).ToString("dd/MM/yyyy");
                objOffer.OfferEndDateStr = Convert.ToDateTime(objOffer.OfferEndDate).ToString("dd/MM/yyyy");                                                

            }
            catch (Exception ex)
            {
            }

            return View(objOffer);
        }

        [HttpPost]
        public ActionResult Edit(FreeOfferVM objOffer, FormCollection frm)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());                    
                    tbl_FreeOffers objFreeOffer = _db.tbl_FreeOffers.Where(o => o.FreeOfferId == objOffer.FreeOfferId).FirstOrDefault();
                    
                    DateTime dtStart = DateTime.ParseExact(objOffer.OfferStartDateStr, "dd/MM/yyyy", null);
                    objFreeOffer.OfferStartDate = dtStart;
                    DateTime dtEnd = DateTime.ParseExact(objOffer.OfferEndDateStr, "dd/MM/yyyy", null);
                    objFreeOffer.OfferEndDate = dtEnd;
                    objFreeOffer.OrderAmountFrom = objOffer.OrderAmountFrom;
                    objFreeOffer.OrderAmountTo = objOffer.OrderAmountTo;
                    _db.SaveChanges();
                    if (frm["SubItemProductItem"] != null)
                    {
                        long FreeOfferIds = objFreeOffer.FreeOfferId;
                        List<tbl_FreeOfferItems> lstFreeOfferIems = _db.tbl_FreeOfferItems.Where(o => o.FreeOfferId == FreeOfferIds).ToList(); 
                        if(lstFreeOfferIems != null && lstFreeOfferIems.Count() > 0)
                        {
                            foreach(var objfreeitms in lstFreeOfferIems)
                            {
                                _db.tbl_FreeOfferItems.Remove(objfreeitms);
                            }
                            _db.SaveChanges();
                        }
                        string[] arrySubItems = Request.Form.GetValues("SubItemProductItem");
                        string[] arrySubItemsCat = Request.Form.GetValues("SubItemCategory");
                        string[] arrySubItemsProd = Request.Form.GetValues("SubItemProduct");
                        string[] arrySubItemsSubProd = Request.Form.GetValues("SubItemSubProduct");
                        string[] arrySubItemsVariant = Request.Form.GetValues("SubItemVarints");
                        string[] arrySubItemsQty = Request.Form.GetValues("SubItemQty");
                        for (int j = 0; j < arrySubItems.Length; j++)
                        {
                            tbl_FreeOfferItems objSub = new tbl_FreeOfferItems();
                            objSub.FreeOfferId = objFreeOffer.FreeOfferId;
                            if (arrySubItems[j] != "" && arrySubItems[j] != "0")
                            {
                                objSub.CategoryId = GetInt64Val(arrySubItemsCat[j]);
                                objSub.ProductId = GetInt64Val(arrySubItemsProd[j]);
                                objSub.SubProductId = GetInt64Val(arrySubItemsSubProd[j]);
                                objSub.VariantItemId = GetInt64Val(arrySubItemsVariant[j]);
                                objSub.ProductItemId = GetInt64Val(arrySubItems[j]);
                                objSub.Qty = GetInt64Val(arrySubItemsQty[j]);
                                _db.tbl_FreeOfferItems.Add(objSub);
                                _db.SaveChanges();
                            }
                        }
                    }
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

        [HttpPost]
        public string DeleteOffer(long FreeOfferId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_FreeOffers objtbloffer = _db.tbl_FreeOffers.Where(x => x.FreeOfferId == FreeOfferId && x.IsDeleted == false).FirstOrDefault();

                if (objtbloffer == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objtbloffer.IsDeleted = true;
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

        public ActionResult GetSubItemsOfFree(int Id)
        {
            ViewBag.Id = Id;
            ViewData["catrgories"] = GetCategoryList();
            return PartialView("~/Areas/Admin/Views/FreeOffer/_SubItems.cshtml");
        }

        public ActionResult GetSubItemsOfFreeEdit(long FreeOfferId)
        {
            List<tbl_FreeOfferItems> lstComboSub = _db.tbl_FreeOfferItems.Where(o => o.FreeOfferId == FreeOfferId).ToList();
            List<FreeOfferSubItems> lstSubItemss = new List<FreeOfferSubItems>();
            if (lstComboSub != null && lstComboSub.Count() > 0)
            {
                foreach (var objsub in lstComboSub)
                {
                    if (objsub != null)
                    {
                        FreeOfferSubItems objj = new FreeOfferSubItems();
                        objj.CategoryId = objsub.CategoryId.Value;
                        objj.ProductId = objsub.ProductId.Value;
                        objj.SubProductId = objsub.SubProductId.Value;
                        objj.Qty = objsub.Qty;
                        objj.ProductItemId = objsub.ProductItemId.Value;
                        objj.VarintId = objsub.VariantItemId.Value;                        
                        objj.Sub_CategoryList = GetCategoryList();
                        objj.Sub_ProductList = GetProductListByCategoryId(objj.CategoryId);
                        objj.Sub_SubProductList = GetSubProductListByProductId(objj.ProductId);
                        objj.Sub_ProductItemList = GetProductItems(objj.ProductId, objj.SubProductId);
                        objj.Sub_ProductVariantList = _db.tbl_ItemVariant.Where(x => (FreeOfferId == -1 || x.ProductItemId.Value == objj.ProductItemId) && x.IsActive == true).OrderBy(x => x.UnitQty).ToList();
                        lstSubItemss.Add(objj);
                    }
                }
            }
            //objOffer.Sub_CategoryList = GetCategoryList();
            //objOffer.Sub_ProductList = GetProductListByCategoryId(objOffer.Sub_CategoryId);
            //objOffer.Sub_SubProductList = GetSubProductListByProductId(objOffer.Sub_ProductId);
            //objOffer.Sub_ProductItemList = GetProductItems(objOffer.Sub_ProductId, objOffer.Sub_SubProductId);
            ViewData["lstSubItemss"] = lstSubItemss;
            return PartialView("~/Areas/Admin/Views/FreeOffer/_SubItemsEdit.cshtml");
        }

        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
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
            var ProductItemList = _db.tbl_ProductItems.Where(x => x.ProductId == ProductId && (SubProductId == null || SubProductId == 0 || x.SubProductId == SubProductId) && x.IsActive && !x.IsDelete)
                        .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.ProductItemId).Trim(), Text = o.ItemName })
                        .OrderBy(x => x.Text).ToList();

            return ProductItemList;
        }

        public JsonResult GetVariantListByItemId(double Id)
        {
            var tbl_ItemVariantList = _db.tbl_ItemVariant.Where(x => (Id == -1 || x.ProductItemId.Value == Id) && x.IsActive == true).OrderBy(x => x.UnitQty).ToList();

            return Json(tbl_ItemVariantList, JsonRequestBehavior.AllowGet);
        }

        public long GetInt64Val(string vl)
        {
            long reurnvl = 0;
            if (!string.IsNullOrEmpty(vl))
            {
                reurnvl = Convert.ToInt64(vl);
            }
            return reurnvl;
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_FreeOffers objtbl_Offers = _db.tbl_FreeOffers.Where(x => x.FreeOfferId == Id).FirstOrDefault();

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