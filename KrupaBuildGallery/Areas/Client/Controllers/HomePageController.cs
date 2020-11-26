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

            //string randomReferralCode = CommonMethod.GetRandomReferralCode(8);

            //EmailMessageVM emailModel = clsCommon.GetSampleEmailTemplate();
            //clsCommon.SendEmail2(emailModel);
            //clsCommon.SendEmail("prajapati.nileshbhai@gmail.com", "admin@shopping-saving.com", "Test Email", emailModel.Body);
            if (clsClientSession.UserID == 0)
            {
                if (Request.Cookies["sessionkeyval"] != null)
                {
                    string strcokki = Request.Cookies["sessionkeyval"].Value;
                    if (strcokki.Contains("cust"))
                    {
                        Response.Cookies["sessionkeyval"].Value = Guid.NewGuid().ToString();
                        Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                    }
                }
            }

            List<AdvertiseImageVM> lstAdvertiseImages = new List<AdvertiseImageVM>();
            WebsiteStatisticsVM objStatistics = new WebsiteStatisticsVM();
            List<HappyCustomerVM> lstHappyCustomers = new List<HappyCustomerVM>();
            List<ProductItemVM> lstPopularProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstUnpackProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstOfferItems = new List<ProductItemVM>();
            List<ComboOfferVM> lstComboOffers = new List<ComboOfferVM>();
            List<long> wishlistitemsId = new List<long>();
            List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
            if (clsClientSession.UserID != 0)
            {
                long UserId = clsClientSession.UserID;
                wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
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
                                         CreatedDate = i.CreatedDate,
                                         IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                                     }).OrderByDescending(x => x.CreatedDate).ToList().Take(8).ToList();

            if (clsClientSession.UserID != 0)
            {
                lstPopularProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
            }
            else
            {
                lstPopularProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
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
                                        CreatedDate = i.CreatedDate,
                                        IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                                    }).OrderByDescending(x => x.CreatedDate).ToList().Take(8).ToList();

            if (clsClientSession.UserID != 0)
            {
                lstUnpackProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
            }
            else
            {
                lstUnpackProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
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
                                 CreatedDate = i.CreatedDate,
                                 IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                             }).OrderByDescending(x => x.CreatedDate).ToList().Take(8).ToList();

            if (clsClientSession.UserID != 0)
            {
                lstOfferItems.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
            }
            else
            {
                lstOfferItems.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
            }

            List<HomeImageVM> lstImages = GetHomeImages();
            List<CategoryVM> lstCategory = (from c in _db.tbl_Categories
                                            where !c.IsDelete && c.IsActive
                                            select new CategoryVM
                                            {
                                                CategoryId = c.CategoryId,
                                                CategoryName = c.CategoryName,
                                                CategoryImage = c.CategoryImage
                                            }).OrderBy(x => x.CategoryName).ToList().Take(9).ToList();

            lstHappyCustomers = (from c in _db.tbl_HappyCustomers
                                 where c.IsActive
                                 select new HappyCustomerVM
                                 {
                                     HappyCustomerId = c.HappyCustomerId,
                                     FinanceYear = c.FinanceYear,
                                     CustomerName = c.CustomerName,
                                     CustomerImage = c.CustomerImage,
                                     IsActive = c.IsActive
                                 }).OrderByDescending(x => x.HappyCustomerId).ToList();

            // Get website statistics data
            var ClientUserData = _db.tbl_ClientUsers.Where(x => !x.IsDelete && x.IsActive).ToList();
            objStatistics.TotalCustomers = ClientUserData.Where(x => x.ClientRoleId == (int)ClientRoles.Customer).ToList().Count;
            objStatistics.TotalDistributers = ClientUserData.Where(x => x.ClientRoleId == (int)ClientRoles.Distributor).ToList().Count;
            objStatistics.TotalHappyCustomers = _db.tbl_HappyCustomers.Where(x => !x.IsDelete && x.IsActive).ToList().Count;
            objStatistics.TotalItems = _db.tbl_ProductItems.Where(x => !x.IsDelete && x.IsActive).ToList().Count;
            objStatistics.TotalSiteVisitors = 500;

            // Get Advertise Images
            lstAdvertiseImages = (from c in _db.tbl_AdvertiseImages
                                  where !c.IsDeleted && c.IsActive
                                  select new AdvertiseImageVM
                                  {
                                      AdvertiseImageId = c.AdvertiseImageId,
                                      SliderType = c.SliderType.HasValue? c.SliderType.Value : 3,
                                      ImageUrl = c.AdvertiseImage,
                                      ImageFor = c.ImageFor
                                  }).Where(x => x.ImageFor == 1).OrderByDescending(x => x.AdvertiseImageId).ToList();

            lstComboOffers = (from i in _db.tbl_ComboOfferMaster
                              where i.IsActive == true && DateTime.UtcNow >= i.OfferStartDate && DateTime.UtcNow <= i.OfferEndDate && i.IsDeleted == false
                              select new ComboOfferVM
                              {
                                  OfferTitle = i.OfferTitle,
                                  ComboOfferId = i.ComboOfferId,
                                  ComboOfferPrice = i.OfferPrice,
                                  OfferImage = i.OfferImage,
                                  TotlOriginalOfferPrice = i.TotalActualPrice.Value
                              }).OrderBy(x => x.OfferTitle).ToList().Take(8).ToList();


            ViewData["lstPopularProductItem"] = lstPopularProductItem;
            ViewData["lstOfferItems"] = lstOfferItems;
            ViewData["lstImages"] = lstImages;
            ViewData["lstCategory"] = lstCategory;
            ViewData["lstUnpackProductItem"] = lstUnpackProductItem;
            ViewData["lstHappyCustomers"] = lstHappyCustomers;
            ViewData["objStatistics"] = objStatistics;
            ViewData["lstAdvertiseImages2"] = lstAdvertiseImages.Where(x => x.SliderType == 2).ToList();
            ViewData["lstAdvertiseImages3"] = lstAdvertiseImages.Where(x => x.SliderType == 3).ToList();
            ViewData["lstComboOffers"] = lstComboOffers;

            return View();
        }

        public List<HomeImageVM> GetHomeImages()
        {
            List<HomeImageVM> lstImages = new List<HomeImageVM>();

            try
            {
                List<tbl_HomeImages> lst = _db.tbl_HomeImages.Where(x => x.IsActive && x.HomeImageFor == 1).ToList();
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

        public decimal GetRatingOfItem(long ItemId, List<tbl_ReviewRating> lstreviewratings)
        {
            decimal rating = 0;
            var lstt = lstreviewratings.Where(o => o.ProductItemId == ItemId).ToList();
            if (lstt != null && lstt.Count() > 0)
            {
                rating = lstt.Select(o => o.Rating.Value).Average();
            }
            return rating;
        }
    }
}