using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class WishlistController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public WishlistController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Wishlist
        public ActionResult Index()
        {
            long ClientUserId = Convert.ToInt64(clsClientSession.UserID);
          List<WishListVM>  lstWishItems = (from wshl in _db.tbl_WishList
                            join i in _db.tbl_ProductItems on wshl.ItemId equals i.ProductItemId
                            where wshl.ClientUserId == ClientUserId
                            select new WishListVM
                            {
                                WishListId = wshl.PK_WishListId,
                                ItemName = i.ItemName,
                                ItemId = i.ProductItemId,
                                Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                ItemImage = i.MainImage,
                                IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                            }).OrderByDescending(x => x.WishListId).ToList();
            lstWishItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price); });
            return View(lstWishItems);
        }

        [HttpPost]
        public string RemoveWishlistitem(long WishlistId)
        {
            string ReturnMessage = "";

            try
            {
                var objWishList = _db.tbl_WishList.Where(o => o.PK_WishListId == WishlistId).FirstOrDefault();
                _db.tbl_WishList.Remove(objWishList);
                _db.SaveChanges();
                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public decimal GetPriceGenral(long Itemid, decimal price)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                if (clsClientSession.RoleID == 1)
                {
                    return objItem.OfferPrice;
                }
                else
                {
                    return objItem.OfferPriceforDistributor.Value;
                }

            }

            return price;
        }
    }
}