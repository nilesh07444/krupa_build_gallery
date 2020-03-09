using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class HomePageController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public HomePageController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<ProductItemVM> lstNewProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstPopularProductItem = new List<ProductItemVM>();
            List<long> wishlistitemsId = new List<long>();
            lstNewProductItem = (from i in _db.tbl_ProductItems                                
                              where !i.IsDelete && i.IsActive == true && i.IsPopularProduct == false
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
                                  IsActive = i.IsActive,
                                  CreatedDate = i.CreatedDate
                              }).OrderByDescending(x => x.CreatedDate).ToList().Take(10).ToList();

            if (clsClientSession.UserID != 0)
            {
                long UserId = clsClientSession.UserID;
                wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                lstNewProductItem.ForEach(x => x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId));
            }

            lstPopularProductItem = (from i in _db.tbl_ProductItems
                                 where !i.IsDelete && i.IsActive == true && i.IsPopularProduct == true
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
                                     IsActive = i.IsActive,
                                     CreatedDate = i.CreatedDate
                                 }).OrderByDescending(x => x.CreatedDate).ToList().Take(10).ToList();

            if (clsClientSession.UserID != 0)
            {               
                lstPopularProductItem.ForEach(x => x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId));
            }

            ViewData["lstPopularProductItem"] = lstPopularProductItem;
            ViewData["lstNewProductItem"] = lstNewProductItem;
            return View();
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
    }
}