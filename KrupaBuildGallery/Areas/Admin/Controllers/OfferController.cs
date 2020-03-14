using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class OfferController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public OfferController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Offer
        public ActionResult Index()
        {
            List<OfferVM> lstOfferVm = new List<OfferVM>();

            try
            {
                lstOfferVm = (from stk in _db.tbl_Offers
                                  join i in _db.tbl_ProductItems on stk.ProductItemId equals i.ProductItemId
                                  join c in _db.tbl_Categories on stk.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on stk.ProductId equals p.Product_Id
                                  join s in _db.tbl_SubProducts on stk.SubproductId equals s.SubProductId into outerJoinSubProduct
                                  from s in outerJoinSubProduct.DefaultIfEmpty()
                                  where !stk.IsDelete
                                  select new OfferVM
                                  {
                                      OfferId = stk.OfferId,
                                      ProductItemId = stk.ProductItemId,
                                      CategoryId = stk.CategoryId,
                                      ProductId = stk.ProductId,
                                      SubProductId = stk.SubproductId,
                                      ProductItemName = i.ItemName,
                                      CategoryName = c.CategoryName,
                                      ProductName = p.ProductName,
                                      SubProductName = s.SubProductName,
                                      OfferTitle = stk.OfferName,
                                      OfferStartDate = stk.StartDate,
                                      OfferEndDate = stk.EndDate,
                                      CustomerOfferPrice = stk.OfferPrice,
                                      DistributorOfferPrice = stk.OfferPriceforDistributor.Value,
                                      IsActive = stk.IsActive
                                  }).OrderByDescending(x => x.OfferId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstOfferVm);
        }


        public ActionResult Add()
        {
            OfferVM objOffer = new OfferVM();

            objOffer.CategoryList = GetCategoryList();
            objOffer.ProductList = new List<SelectListItem>();
            objOffer.SubProductList = new List<SelectListItem>();
            objOffer.ProductItemList = new List<SelectListItem>();
            return View(objOffer);
        }
        public ActionResult Edit(int id)
        {
            OfferVM objOffer = new OfferVM();

            objOffer = (from i in _db.tbl_Offers
                                   where i.OfferId == id
                                   select new OfferVM
                                   {
                                       OfferId = i.OfferId,
                                       ProductItemId = i.ProductItemId,
                                       CategoryId = i.CategoryId,
                                       ProductId = i.ProductId,
                                       SubProductId = i.SubproductId,
                                       CustomerOfferPrice = i.OfferPrice,
                                       DistributorOfferPrice = i.OfferPriceforDistributor.Value,
                                       OfferTitle = i.OfferName,
                                       OfferStartDate = i.StartDate,
                                       OfferEndDate = i.EndDate,
                                       IsActive = i.IsActive
                                   }).FirstOrDefault();

            objOffer.CategoryList = GetCategoryList();
            objOffer.ProductList = GetProductListByCategoryId(objOffer.CategoryId);
            objOffer.SubProductList = GetSubProductListByProductId(objOffer.ProductId);
            objOffer.ProductItemList = GetProductItems(objOffer.ProductId, objOffer.SubProductId);

            return View(objOffer);
        }
        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

        public JsonResult GetItemList(long ProductId, long? SubProductId)
        {
            var ProductItemList = _db.tbl_ProductItems.Where(x => x.ProductId == ProductId && (SubProductId == 0 || x.SubProductId == SubProductId) && x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.ProductItemId).Trim(), Text = o.ItemName })
                         .OrderBy(x => x.Text).ToList();

            return Json(ProductItemList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(OfferVM objOffer)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    //if (itemStockVM.Quantity <= 0)
                    //{
                    //    ModelState.AddModelError("Quantity", ErrorMessage.QtyGreater);
                    //    return View(itemStockVM);
                    //}
                    //else
                    //{


                    //}
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    tbl_Offers objtbl_Offers = new tbl_Offers();
                    objtbl_Offers.CategoryId = objOffer.CategoryId;
                    objtbl_Offers.ProductId = objOffer.ProductId;
                    objtbl_Offers.SubproductId = objOffer.SubProductId;
                    objtbl_Offers.ProductItemId = objOffer.ProductItemId;
                    objtbl_Offers.OfferPrice = objOffer.CustomerOfferPrice;
                    objtbl_Offers.OfferPriceforDistributor = objOffer.DistributorOfferPrice;
                    objtbl_Offers.StartDate = objOffer.OfferStartDate;
                    objtbl_Offers.EndDate = objOffer.OfferEndDate;
                    objtbl_Offers.OfferName = objOffer.OfferTitle;

                    objtbl_Offers.IsActive = true;
                    objtbl_Offers.IsDelete = false;
                    objtbl_Offers.CreatedBy = LoggedInUserId;
                    objtbl_Offers.CreatedDate = DateTime.UtcNow;
                    objtbl_Offers.UpdatedBy = LoggedInUserId;
                    objtbl_Offers.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_Offers.Add(objtbl_Offers);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objOffer);
        }

        [HttpPost]
        public ActionResult Edit(OfferVM objOffer)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    tbl_Offers objtbl_Offers = _db.tbl_Offers.Where(x => x.OfferId == objOffer.OfferId).FirstOrDefault();

                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objtbl_Offers.CategoryId = objOffer.CategoryId;
                    objtbl_Offers.ProductId = objOffer.ProductId;
                    objtbl_Offers.SubproductId = objOffer.SubProductId;
                    objtbl_Offers.ProductItemId = objOffer.ProductItemId;
                    objtbl_Offers.OfferPrice = objOffer.CustomerOfferPrice;
                    objtbl_Offers.OfferPriceforDistributor = objOffer.DistributorOfferPrice;
                    objtbl_Offers.StartDate = objOffer.OfferStartDate;
                    objtbl_Offers.OfferName = objOffer.OfferTitle;
                    objtbl_Offers.EndDate = objOffer.OfferEndDate;
                    objtbl_Offers.UpdatedBy = LoggedInUserId;
                    objtbl_Offers.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objOffer);
        }


        [HttpPost]
        public string DeleteOffer(long offerid)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Offers objtbloffer = _db.tbl_Offers.Where(x => x.OfferId == offerid && x.IsActive && !x.IsDelete).FirstOrDefault();

                if (objtbloffer == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objtbloffer.IsDelete = true;
                    objtbloffer.UpdatedBy = LoggedInUserId;
                    objtbloffer.UpdatedDate = DateTime.UtcNow;

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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Offers objtbl_Offers = _db.tbl_Offers.Where(x => x.OfferId == Id).FirstOrDefault();

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

                    objtbl_Offers.UpdatedBy = LoggedInUserId;
                    objtbl_Offers.UpdatedDate = DateTime.UtcNow;

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