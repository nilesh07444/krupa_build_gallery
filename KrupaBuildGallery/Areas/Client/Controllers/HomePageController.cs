using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;

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

            //clsCommon.SendEmail("prajapati.nileshbhai@gmail.com", "" , "Test email by Nilesh", "Test");

            List<ProductItemVM> lstNewProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstPopularProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstUnpackProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstOfferItems = new List<ProductItemVM>();
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
                lstNewProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
            }
            else
            {
                lstNewProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
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
                lstPopularProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
            }
            else
            {
                lstPopularProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
            }

            lstUnpackProductItem = (from i in _db.tbl_ProductItems
                                     where !i.IsDelete && i.IsActive == true && i.ItemType == (int)ItemTypes.UnPackedItem
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
                lstUnpackProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
            }
            else
            {
                lstUnpackProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
            }

            List<long> lstOfferItemsId = new List<long>();
            List<tbl_Offers> lstOfferss = _db.tbl_Offers.Where(o => DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate && o.IsActive == true && o.IsDelete == false).ToList();
            if (lstOfferss != null && lstOfferss.Count() > 0)
            {
                lstOfferItemsId = lstOfferss.Select(o => o.ProductItemId).Distinct().ToList();
            }

            lstOfferItems = (from i in _db.tbl_ProductItems
                             where !i.IsDelete && i.IsActive == true && lstOfferItemsId.Contains(i.ProductItemId)
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
                lstOfferItems.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
            }
            else
            {
                lstOfferItems.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
            }

            List<HomeImageVM> lstImages = GetHomeImages();
            List<CategoryVM> lstCategory = (from c in _db.tbl_Categories
                                              where !c.IsDelete && c.IsActive
                                              select new CategoryVM
                                              {
                                                  CategoryId = c.CategoryId,
                                                  CategoryName = c.CategoryName,
                                                  CategoryImage = c.CategoryImage 
                                              }).OrderByDescending(x => x.CategoryId).ToList();

            ViewData["lstPopularProductItem"] = lstPopularProductItem;
            ViewData["lstNewProductItem"] = lstNewProductItem;
            ViewData["lstOfferItems"] = lstOfferItems;
            ViewData["lstImages"] = lstImages;
            ViewData["lstCategory"] = lstCategory;
            ViewData["lstUnpackProductItem"] = lstUnpackProductItem;
            
            return View();
        }

        public List<HomeImageVM> GetHomeImages()
        {
            List<HomeImageVM> lstImages = new List<HomeImageVM>();

            try
            {
                List<tbl_HomeImages> lst = _db.tbl_HomeImages.Where(x => x.IsActive).ToList();
                if (lst.Count > 0)
                {
                    lst.ForEach(obj =>
                    {
                        if (!string.IsNullOrEmpty(obj.HomeImageName))
                        {
                            if (System.IO.File.Exists(Server.MapPath(ErrorMessage.HomeDirectoryPath + obj.HomeImageName)))
                            {
                                lstImages.Add(new HomeImageVM
                                {
                                    ImageUrl = ErrorMessage.HomeDirectoryPath + "/" + obj.HomeImageName,
                                    HeadingText1 = obj.HeadingText1,
                                    HeadingText2 = obj.HeadingText2
                                });
                            }
                        }

                    });
                }
            }
            catch (Exception ex)
            {
            }

            return lstImages;
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
    }
}