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
                                Price = i.CustomerPrice,
                                ItemImage = i.MainImage                                
                            }).OrderByDescending(x => x.WishListId).ToList();
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
    }
}