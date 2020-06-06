using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class SearchController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public SearchController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Search
        public ActionResult Index(string q)
        {
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                ViewBag.Name = "";              
                lstProductItem = (from i in _db.tbl_ProductItems
                                  join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                  join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSubProduct
                                  from s in outerJoinSubProduct.DefaultIfEmpty()
                                      //where !i.IsDelete && !c.IsDelete && !p.IsDelete
                                  where !i.IsDelete && i.IsActive == true && !c.IsDelete && !p.IsDelete && (q.ToLower().Contains(i.ItemName.ToLower()) || (i.Tags != "" && i.Tags.ToLower().Contains(q.ToLower())) || i.ItemName.ToLower().Contains(q.ToLower()) || q.ToLower().Contains(c.CategoryName.ToLower()) || c.CategoryName.ToLower().Contains(q.ToLower()) || q.ToLower().Contains(p.ProductName.ToLower()) || p.ProductName.ToLower().Contains(q.ToLower()) || q.ToLower().Contains(s.SubProductName.ToLower()) || s.SubProductName.ToLower().Contains(q.ToLower()))
                                  select new ProductItemVM
                                  {
                                      ProductItemId = i.ProductItemId,
                                      ProductId = i.ProductId,
                                      SubProductId = i.SubProductId,
                                      ItemName = i.ItemName,
                                      MainImage = i.MainImage,
                                      MRPPrice = i.MRPPrice,
                                      CustomerPrice = i.CustomerPrice,
                                      DistributorPrice = i.DistributorPrice,
                                      IsActive = i.IsActive
                                  }).OrderBy(x => x.ItemName).ToList();

                if (clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }

                //if (sortby == 1)
                //{
                //    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                //    {
                //        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                //    }
                //    else
                //    {
                //        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                //    }
                //}
                //else if (sortby == 2)
                //{
                //    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                //    {
                //        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                //    }
                //    else
                //    {
                //        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                //    }
                //}
                //else if (sortby == 3)
                //{
                //    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                //}
                //else if (sortby == 4)
                //{
                //    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                //}
                ViewBag.Search = q;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            
            return View(lstProductItem);
        }

        public bool IsInWhishList(long ItemId, List<long> ItemList)
        {
            if (ItemList.Contains(ItemId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public decimal GetOfferPrice(long Itemid, decimal price)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return objItem.OfferPrice;
            }

            return price;
        }

        public decimal GetDistributorOfferPrice(long Itemid, decimal price)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return objItem.OfferPriceforDistributor.Value;
            }

            return price;
        }

        public int ItemStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            if (TotalStock == null)
            {
                TotalStock = 0;
            }
            return Convert.ToInt32(TotalStock);
        }
        public int SoldItems(long ItemId)
        {
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.QtyUsed.Value);
            if (TotalSold == null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
        }
    }
}