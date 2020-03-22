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
    public class StockController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public StockController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Stock
        public ActionResult Index()
        {
            List<ItemStockVM> lstItemStockVM = new List<ItemStockVM>();

            try
            {
                lstItemStockVM = (from stk in _db.tbl_ItemStocks
                                  join i in _db.tbl_ProductItems on stk.ProductItemId equals i.ProductItemId
                                  join c in _db.tbl_Categories on stk.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on stk.ProductId equals p.Product_Id
                                  join s in _db.tbl_SubProducts on stk.SubProductId equals s.SubProductId into outerJoinSubProduct
                                  from s in outerJoinSubProduct.DefaultIfEmpty()
                                  where !stk.IsDelete
                                  select new ItemStockVM
                                  {
                                      StockId = stk.StockId,
                                      ProductItemId = stk.ProductItemId,
                                      CategoryId = stk.CategoryId,
                                      ProductId = stk.ProductId,
                                      SubProductId = stk.SubProductId,
                                      ProductItemName = i.ItemName,
                                      CategoryName = c.CategoryName,
                                      ProductName = p.ProductName,
                                      SubProductName = s.SubProductName,
                                      Quantity = stk.Qty,
                                      IsActive = i.IsActive
                                  }).OrderByDescending(x => x.StockId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstItemStockVM);
        }

        public ActionResult Add()
        {
            ItemStockVM objItemStock = new ItemStockVM();

            objItemStock.CategoryList = GetCategoryList();
            objItemStock.ProductList = new List<SelectListItem>();
            objItemStock.SubProductList = new List<SelectListItem>();
            objItemStock.ProductItemList = new List<SelectListItem>();
            return View(objItemStock);
        }
        public ActionResult Edit(int id)
        {
            ItemStockVM objProductItemStock = new ItemStockVM();

            objProductItemStock = (from i in _db.tbl_ItemStocks
                                   where i.StockId == id
                                   select new ItemStockVM
                                   {
                                       StockId = i.StockId,
                                       ProductItemId = i.ProductItemId,
                                       CategoryId = i.CategoryId,                                       
                                       ProductId = i.ProductId,
                                       SubProductId = i.SubProductId,
                                       Quantity = i.Qty,
                                       IsActive = i.IsActive
                                   }).FirstOrDefault();

            objProductItemStock.CategoryList = GetCategoryList();
            objProductItemStock.ProductList = GetProductListByCategoryId(objProductItemStock.CategoryId);
            objProductItemStock.SubProductList = GetSubProductListByProductId(objProductItemStock.ProductId);
            objProductItemStock.ProductItemList = GetProductItems(objProductItemStock.ProductId, objProductItemStock.SubProductId);

            return View(objProductItemStock);
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
        public ActionResult Add(ItemStockVM itemStockVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    if (itemStockVM.Quantity <= 0)
                    {
                        ModelState.AddModelError("Quantity", ErrorMessage.QtyGreater);
                        return View(itemStockVM);
                    }
                    else
                    {

                        long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                        tbl_ItemStocks objItemStock = new tbl_ItemStocks();
                        objItemStock.CategoryId = itemStockVM.CategoryId;
                        objItemStock.ProductId = itemStockVM.ProductId;
                        objItemStock.SubProductId = itemStockVM.SubProductId;
                        objItemStock.ProductItemId = itemStockVM.ProductItemId;
                        objItemStock.Qty = Convert.ToInt64(itemStockVM.Quantity);

                        objItemStock.IsActive = true;
                        objItemStock.IsDelete = false;
                        objItemStock.CreatedBy = LoggedInUserId;
                        objItemStock.CreatedDate = DateTime.UtcNow;
                        objItemStock.UpdatedBy = LoggedInUserId;
                        objItemStock.UpdatedDate = DateTime.UtcNow;
                        _db.tbl_ItemStocks.Add(objItemStock);
                        _db.SaveChanges();

                        return RedirectToAction("Index");

                    } 
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(itemStockVM);
        }

        [HttpPost]
        public ActionResult Edit(ItemStockVM itemStockVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    tbl_ItemStocks objItemStocks = _db.tbl_ItemStocks.Where(x => x.StockId == itemStockVM.StockId).FirstOrDefault();

                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objItemStocks.CategoryId = itemStockVM.CategoryId;
                    objItemStocks.ProductId = itemStockVM.ProductId;
                    objItemStocks.SubProductId = itemStockVM.SubProductId;
                    objItemStocks.ProductItemId = itemStockVM.ProductItemId;
                    objItemStocks.Qty = itemStockVM.Quantity;
                    objItemStocks.UpdatedBy = LoggedInUserId;
                    objItemStocks.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(itemStockVM);
        }


        [HttpPost]
        public string DeleteStock(long StockId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_ItemStocks objtbl_ItemStocks = _db.tbl_ItemStocks.Where(x => x.StockId == StockId && x.IsActive && !x.IsDelete).FirstOrDefault();

                if (objtbl_ItemStocks == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long Itmid = objtbl_ItemStocks.ProductItemId;
                    int soldQty = SoldItems(Itmid);
                    int stockQty = ItemStock(Itmid);
                    int remaining = stockQty - soldQty;
                    if(remaining < objtbl_ItemStocks.Qty)
                    {
                        ReturnMessage = "qtyless";
                    }
                    else
                    {
                        long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                        objtbl_ItemStocks.IsDelete = true;
                        objtbl_ItemStocks.UpdatedBy = LoggedInUserId;
                        objtbl_ItemStocks.UpdatedDate = DateTime.UtcNow;

                        _db.SaveChanges();
                        ReturnMessage = "success";
                    }
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

        public int ItemStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            if(TotalStock != null)
            {
                TotalStock = 0;
            }
            return Convert.ToInt32(TotalStock);
        }
        public int SoldItems(long ItemId)
        {
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.Qty.Value);
            if(TotalSold != null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
        }
    }
}