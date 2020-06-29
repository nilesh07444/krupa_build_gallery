using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class ProductsController : ApiController
    {
        krupagallarydbEntities _db;
        public ProductsController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("GetItemsByCategoryId"), HttpPost]
        public ResponseDataModel<List<ProductItemVM>> GetItemsByCategoryId(GeneralVM objGen)
        {
            ResponseDataModel<List<ProductItemVM>> response = new ResponseDataModel<List<ProductItemVM>>();
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
                long CategoryId = Convert.ToInt64(objGen.CategoryId);
                var objCat = _db.tbl_Categories.Where(o => o.CategoryId == CategoryId).FirstOrDefault();
                lstProductItem = (from i in _db.tbl_ProductItems
                                  where !i.IsDelete && i.IsActive == true && i.CategoryId == CategoryId
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

                if (UserId != 0)
                {                   
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }

                if (objGen.SortBy == "1")
                {
                    if (UserId == 0 || RoleId == 1)
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                    }
                }
                else if (objGen.SortBy == "2")
                {
                    if (UserId == 0 || RoleId == 1)
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                    }
                }
                else if (objGen.SortBy == "3")
                {
                    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                }
                else if (objGen.SortBy == "4")
                {
                    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                }
                response.Data = lstProductItem;             

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetItemsByProductId"), HttpPost]
        public ResponseDataModel<List<ProductItemVM>> GetItemsByProductId(GeneralVM objGen)
        {
            ResponseDataModel<List<ProductItemVM>> response = new ResponseDataModel<List<ProductItemVM>>();
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
                long productid = Convert.ToInt64(objGen.ProductId);
                lstProductItem = (from i in _db.tbl_ProductItems
                                  where !i.IsDelete && i.IsActive == true && i.ProductId == productid
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

                if (UserId != 0)
                {
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }

                if (objGen.SortBy == "1")
                {
                    if (UserId == 0 || RoleId == 1)
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                    }
                }
                else if (objGen.SortBy == "2")
                {
                    if (UserId == 0 || RoleId == 1)
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                    }
                }
                else if (objGen.SortBy == "3")
                {
                    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                }
                else if (objGen.SortBy == "4")
                {
                    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                }
                response.Data = lstProductItem;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetItemsBySubProductId"), HttpPost]
        public ResponseDataModel<List<ProductItemVM>> GetItemsBySubProductId(GeneralVM objGen)
        {
            ResponseDataModel<List<ProductItemVM>> response = new ResponseDataModel<List<ProductItemVM>>();
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
                long subproductid = Convert.ToInt64(objGen.SubProductId);
                lstProductItem = (from i in _db.tbl_ProductItems
                                  where !i.IsDelete && i.IsActive == true && i.SubProductId == subproductid
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

                if (UserId != 0)
                {
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }

                if (objGen.SortBy == "1")
                {
                    if (UserId == 0 || RoleId == 1)
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                    }
                }
                else if (objGen.SortBy == "2")
                {
                    if (UserId == 0 || RoleId == 1)
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                    }
                }
                else if (objGen.SortBy == "3")
                {
                    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                }
                else if (objGen.SortBy == "4")
                {
                    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                }
                response.Data = lstProductItem;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetItemDetails"), HttpPost]
        public ResponseDataModel<ProductItemVM> GetItemDetails(GeneralVM objGen)
        {
            ResponseDataModel<ProductItemVM> response = new ResponseDataModel<ProductItemVM>();
            ProductItemVM objProductItem = new ProductItemVM();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);            
                long ProdItem = Convert.ToInt64(objGen.ItemId);
                List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
                List<string> lstimages = _db.tbl_ProductItemImages.Where(o => o.ProductItemId == ProdItem).Select(o => o.ItemImage).ToList();
                objProductItem = (from i in _db.tbl_ProductItems
                                  where i.ProductItemId == ProdItem
                                  select new ProductItemVM
                                  {
                                      ProductItemId = i.ProductItemId,
                                      CategoryId = i.CategoryId,
                                      ProductId = i.ProductId,
                                      SubProductId = i.SubProductId,
                                      ItemName = i.ItemName,
                                      ItemDescription = i.ItemDescription,
                                      MainImage = i.MainImage,
                                      MRPPrice = i.MRPPrice,
                                      CustomerPrice = i.CustomerPrice,
                                      DistributorPrice = i.DistributorPrice,
                                      GST_Per = i.GST_Per,
                                      IGST_Per = i.IGST_Per,
                                      Notification = i.Notification,
                                      IsPopularProduct = i.IsPopularProduct,
                                      Sku = i.Sku,
                                      OtherImages = lstimages,
                                      IsActive = i.IsActive,
                                      UnitType = i.UnitType.HasValue ? i.UnitType.Value : 0,
                                      IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                                  }).FirstOrDefault();
                if (UserId != 0)
                {
                    var objWishlist = _db.tbl_WishList.Where(o => o.ClientUserId == UserId && o.ItemId == ProdItem).FirstOrDefault();
                    objProductItem.IsWishListItem = false;
                    if (objWishlist != null)
                    {
                        objProductItem.IsWishListItem = true;
                    }
                }
                objProductItem.Ratings = GetRatingOfItem(objProductItem.ProductItemId, lstRatings);
                objProductItem.CustomerPrice = GetOfferPrice(objProductItem.ProductItemId, objProductItem.CustomerPrice);
                objProductItem.DistributorPrice = GetDistributorOfferPrice(objProductItem.ProductItemId, objProductItem.DistributorPrice);
                int TotalStk = ItemStock(objProductItem.ProductItemId);
                int TotalSold = SoldItems(objProductItem.ProductItemId);
                objProductItem.InStock = TotalStk - TotalSold;
                var objUnt = _db.tbl_Units.Where(o => o.UnitId == objProductItem.UnitType).FirstOrDefault();
                List<tbl_ItemVariant> lstVarint = new List<tbl_ItemVariant>();
                List<VariantItemVM> lstVrntVM = new List<VariantItemVM>();
                string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
                string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
                string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
                string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

                string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
                string[] sheetsqty = { "32", "28", "21", "24", "18" };
                if (objUnt != null)
                {
                    lstVarint = _db.tbl_ItemVariant.Where(o => o.ProductItemId == objProductItem.ProductItemId && o.IsActive == true).ToList();
                    if (lstVarint != null && lstVarint.Count() > 0)
                    {
                        foreach (tbl_ItemVariant objvv in lstVarint)
                        {
                            VariantItemVM objVM = new VariantItemVM();
                            objVM.VariantItemId = Convert.ToInt32(objvv.VariantItemId);
                            objVM.UnitQtys = objvv.UnitQty;
                            if (objUnt.UnitName.ToLower().Contains("killo"))
                            {
                                int idxxx = Array.IndexOf(kgs, objvv.UnitQty);
                                decimal qtt = Convert.ToDecimal(kgsQty[idxxx].ToString());
                                if (qtt >= 1)
                                {
                                    objvv.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * objvv.PricePecentage.Value) / 100, 2);
                                    objvv.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * objvv.PricePecentage.Value) / 100, 2);
                                    objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                    objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                    objVM.MRPPrice = Math.Round((objProductItem.MRPPrice * qtt * objvv.PricePecentage.Value) / 100, 2);
                                }
                                else
                                {
                                    objvv.CustomerPrice = Math.Round((objProductItem.CustomerPrice * objvv.PricePecentage.Value) / 100, 2);
                                    objvv.DistributorPrice = Math.Round((objProductItem.DistributorPrice * objvv.PricePecentage.Value) / 100, 2);
                                    objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                    objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                    objVM.MRPPrice = Math.Round((objProductItem.MRPPrice * objvv.PricePecentage.Value) / 100, 2);
                                }
                            }
                            else if (objUnt.UnitName.ToLower().Contains("litr"))
                            {
                                int idxxx = Array.IndexOf(ltrs, objvv.UnitQty);
                                decimal qtt = Convert.ToDecimal(ltrsQty[idxxx].ToString());
                                if (qtt >= 1)
                                {
                                    objvv.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * objvv.PricePecentage.Value) / 100, 2);
                                    objvv.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * objvv.PricePecentage.Value) / 100, 2);
                                    objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                    objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                    objVM.MRPPrice = Math.Round((objProductItem.MRPPrice * qtt * objvv.PricePecentage.Value) / 100, 2);
                                }
                                else
                                {
                                    objvv.CustomerPrice = Math.Round((objProductItem.CustomerPrice * objvv.PricePecentage.Value) / 100, 2);
                                    objvv.DistributorPrice = Math.Round((objProductItem.DistributorPrice * objvv.PricePecentage.Value) / 100, 2);
                                    objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                    objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                    objVM.MRPPrice = Math.Round((objProductItem.MRPPrice * objvv.PricePecentage.Value) / 100, 2);
                                }
                            }
                            else if (objUnt.UnitName.ToLower().Contains("sheet"))
                            {
                                int idxxx = Array.IndexOf(sheets, objvv.UnitQty);
                                decimal sqft = Convert.ToDecimal(sheetsqty[idxxx]);
                                objvv.CustomerPrice = Math.Round(sqft * objProductItem.CustomerPrice, 2);
                                objvv.DistributorPrice = Math.Round(sqft * objProductItem.DistributorPrice, 2);
                                objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                objVM.MRPPrice = Math.Round(sqft * objProductItem.MRPPrice, 2);
                            }
                            else
                            {
                                objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                objVM.MRPPrice = Math.Round(objProductItem.MRPPrice, 2);
                            }
                            lstVrntVM.Add(objVM);
                        }
                    }
                }
                objProductItem.lstVariants = lstVrntVM;
                objProductItem.UnitTyp = objUnt.UnitName;
                response.Data = objProductItem;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetHomePageProductLists"), HttpPost]
        public ResponseDataModel<HomePageVM> GetHomePageProductLists(GeneralVM objGen)
        {
            ResponseDataModel<HomePageVM> response = new ResponseDataModel<HomePageVM>();
            HomePageVM objHome = new HomePageVM();
            List<ProductItemVM> lstNewProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstPopularProductItem = new List<ProductItemVM>();
            List<ProductItemVM> lstOfferItems = new List<ProductItemVM>();
            List<long> wishlistitemsId = new List<long>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
                long subproductid = Convert.ToInt64(objGen.SubProductId);
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

                if (UserId != 0)
                {                   
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

                if (UserId != 0)
                {
                    lstPopularProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                else
                {
                    lstPopularProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
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

                if (UserId != 0)
                {
                    lstOfferItems.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                else
                {
                    lstOfferItems.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                objHome.PopularProducts = lstPopularProductItem;
                objHome.NewArrivalProducts = lstNewProductItem;
                objHome.OfferProducts = lstOfferItems;
                objHome.HomePageSlider = GetHomeImages();
                response.Data = objHome;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

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
                            lstImages.Add(new HomeImageVM
                            {
                                HomeImageName = obj.HomeImageName,
                                HeadingText1 = obj.HeadingText1,
                                HeadingText2 = obj.HeadingText2
                            });
                        }

                    });
                }
            }
            catch (Exception ex)
            {
            }

            return lstImages;
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