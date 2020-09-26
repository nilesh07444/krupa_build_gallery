using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class WishListController : ApiController
    {
        krupagallarydbEntities _db;
        public WishListController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("AddRemoveToWishList"), HttpPost]
        public ResponseDataModel<OtpVM> AddRemoveToWishList(GeneralVM objGen)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long ItemIds = Convert.ToInt64(objGen.ItemId);
                tbl_WishList objWish = _db.tbl_WishList.Where(o => o.ItemId == ItemIds && o.ClientUserId == UserId).FirstOrDefault();
                if (objWish != null)
                {
                    _db.tbl_WishList.Remove(objWish);
                    _db.SaveChanges();
                }
                else
                {
                    objWish = new tbl_WishList();
                    objWish.ClientUserId = UserId;
                    objWish.ItemId = ItemIds;
                    objWish.CreatedDate = DateTime.UtcNow;
                    _db.tbl_WishList.Add(objWish);
                    _db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("RemoveWishlistitem"), HttpPost]
        public ResponseDataModel<OtpVM> RemoveWishlistitem(GeneralVM objGen)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                long wishlistid = Convert.ToInt64(objGen.WishListId);
                var objWishList = _db.tbl_WishList.Where(o => o.PK_WishListId == wishlistid).FirstOrDefault();
                _db.tbl_WishList.Remove(objWishList);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetWishlist"), HttpPost]
        public ResponseDataModel<List<WishListVM>> GetWishlist(GeneralVM objGen)
        {
            ResponseDataModel<List<WishListVM>> response = new ResponseDataModel<List<WishListVM>>();
            List<WishListVM> lstWishItems = new List<WishListVM>();
            try
            {
                long ClientUserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
                lstWishItems = (from wshl in _db.tbl_WishList
                                                 join i in _db.tbl_ProductItems on wshl.ItemId equals i.ProductItemId
                                                 where wshl.ClientUserId == ClientUserId
                                                 select new WishListVM
                                                 {
                                                     WishListId = wshl.PK_WishListId,
                                                     ItemName = i.ItemName,
                                                     ItemId = i.ProductItemId,
                                                     Price = RoleId == 1 ? i.CustomerPrice : i.DistributorPrice,
                                                     ItemImage = i.MainImage
                                                 }).OrderByDescending(x => x.WishListId).ToList();
                lstWishItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price, RoleId); });
                response.Data = lstWishItems;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        public decimal GetPriceGenral(long Itemid, decimal price,long RoleId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                if (RoleId == 1)
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