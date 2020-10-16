using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class ProductsController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public ProductsController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Products
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ByCategory(int Id, int sortby = 3)
        {
            long CategoryId = Convert.ToInt64(Id);
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                ViewBag.Name = "";
                var objCat = _db.tbl_Categories.Where(o => o.CategoryId == CategoryId).FirstOrDefault();
                if (objCat != null)
                {
                    ViewBag.Name = objCat.CategoryName;
                }
                List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
                lstProductItem = (from i in _db.tbl_ProductItems
                                      //join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                      //join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                      //join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSubProduct
                                      //from s in outerJoinSubProduct.DefaultIfEmpty()
                                      //where !i.IsDelete && !c.IsDelete && !p.IsDelete
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
                                      IsActive = i.IsActive,
                                      IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                                  }).OrderBy(x => x.ItemName).ToList();

                if (clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }

                if (sortby == 1)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 2)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 3)
                {
                    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                }
                else if (sortby == 4)
                {
                    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            ViewBag.SortBy = sortby;
            ViewBag.FromPage = "/products/bycategory?Id=" + CategoryId;

            // _db.tbl_ProductItems.Where(o => o.CategoryId == CategoryId).L();
            return View("/Areas/Client/Views/Products/ProductItemList.cshtml", lstProductItem);
        }

        public ActionResult Bysubproduct(int Id, int sortby = 3)
        {
            long SubproductId = Convert.ToInt64(Id);
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                ViewBag.Name = "";
                var objCat = _db.tbl_SubProducts.Where(o => o.SubProductId == SubproductId).FirstOrDefault();
                if (objCat != null)
                {
                    ViewBag.Name = objCat.SubProductName;
                }
                List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
                lstProductItem = (from i in _db.tbl_ProductItems
                                      //join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                      //join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                      //join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSubProduct
                                      //from s in outerJoinSubProduct.DefaultIfEmpty()
                                      //where !i.IsDelete && !c.IsDelete && !p.IsDelete
                                  where !i.IsDelete && i.IsActive == true && i.SubProductId == SubproductId
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
                                      IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                                  }).OrderBy(x => x.ItemName).ToList();

                if (clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }

                if (sortby == 1)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 2)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 3)
                {
                    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                }
                else if (sortby == 4)
                {
                    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            ViewBag.SortBy = sortby;
            ViewBag.FromPage = "products/Bysubproduct?Id=" + SubproductId;

            // _db.tbl_ProductItems.Where(o => o.CategoryId == CategoryId).L();
            return View("/Areas/Client/Views/Products/ProductItemList.cshtml", lstProductItem);
        }

        public ActionResult Byproduct(int Id, int sortby = 3)
        {
            long productid = Convert.ToInt64(Id);
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
                ViewBag.Name = "";
                var objCat = _db.tbl_Products.Where(o => o.Product_Id == productid).FirstOrDefault();
                if (objCat != null)
                {
                    ViewBag.Name = objCat.ProductName;
                }
                lstProductItem = (from i in _db.tbl_ProductItems
                                      //join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                      //join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                      //join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSubProduct
                                      //from s in outerJoinSubProduct.DefaultIfEmpty()
                                      //where !i.IsDelete && !c.IsDelete && !p.IsDelete
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
                                      IsActive = i.IsActive,
                                      IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                                  }).OrderBy(x => x.ItemName).ToList();

                if (clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }

                if (sortby == 1)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 2)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 3)
                {
                    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                }
                else if (sortby == 4)
                {
                    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            ViewBag.SortBy = sortby;
            ViewBag.FromPage = "products/Byproduct?Id=" + productid;

            // _db.tbl_ProductItems.Where(o => o.CategoryId == CategoryId).L();
            return View("/Areas/Client/Views/Products/ProductItemList.cshtml", lstProductItem);
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

        public ActionResult ItemList(int CatId, int sortby = 3, int ProductId = -1,int SubProductId = -1)
        {
            long CategoryId = Convert.ToInt64(CatId);
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                ViewBag.Name = "";
                var objCat = _db.tbl_Categories.Where(o => o.CategoryId == CategoryId).FirstOrDefault();
                if (objCat != null)
                {
                    ViewBag.Name = objCat.CategoryName;
                }
                List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
                lstProductItem = (from i in _db.tbl_ProductItems
                                      //join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                      //join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                      //join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSubProduct
                                      //from s in outerJoinSubProduct.DefaultIfEmpty()
                                      //where !i.IsDelete && !c.IsDelete && !p.IsDelete
                                  where !i.IsDelete && i.IsActive == true && i.CategoryId == CategoryId && (ProductId == -1 || i.ProductId == ProductId) && (SubProductId == -1 || i.SubProductId == SubProductId)
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
                                      IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                                  }).OrderBy(x => x.ItemName).ToList();

                if (clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }

                if (sortby == 1)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderBy(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 2)
                {
                    if (clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.CustomerPrice).ToList();
                    }
                    else
                    {
                        lstProductItem = lstProductItem.OrderByDescending(o => o.DistributorPrice).ToList();
                    }
                }
                else if (sortby == 3)
                {
                    lstProductItem = lstProductItem.OrderBy(o => o.ItemName).ToList();
                }
                else if (sortby == 4)
                {
                    lstProductItem = lstProductItem.OrderByDescending(o => o.ItemName).ToList();
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            ViewBag.SortBy = sortby;
            ViewBag.FromPage = "/products/bycategory?Id=" + CategoryId;
            ViewBag.ProductId = ProductId;
            ViewBag.SubProductId = SubProductId;
            ViewBag.CatId = CategoryId;
            List<tbl_Products> lstProducts = _db.tbl_Products.Where(o => o.IsDelete == false && o.IsActive == true && o.CategoryId == CategoryId).ToList();
            List<tbl_SubProducts> lstSubProd = new List<tbl_SubProducts>();
            if(ProductId != -1)
            {
                lstSubProd = _db.tbl_SubProducts.Where(o => o.ProductId == ProductId && o.IsActive == true && o.IsDelete == false).ToList();
            }

            ViewData["lstProduc"] = lstProducts;
            ViewData["lstSubProduc"] = lstSubProd;
            // _db.tbl_ProductItems.Where(o => o.CategoryId == CategoryId).L();
            return View("/Areas/Client/Views/Products/ProductItemList.cshtml", lstProductItem);
        }

        [HttpPost]
        public string AddRemoveWishList(int ItemId)
        {
            string ReturnMessage = "";

            try
            {
                if (clsClientSession.UserID > 0)
                {
                    long UserId = clsClientSession.UserID;
                    tbl_WishList objWish = _db.tbl_WishList.Where(o => o.ItemId == ItemId && o.ClientUserId == UserId).FirstOrDefault();
                    if (objWish != null)
                    {
                        _db.tbl_WishList.Remove(objWish);
                        _db.SaveChanges();
                    }
                    else
                    {
                        objWish = new tbl_WishList();
                        objWish.ClientUserId = UserId;
                        objWish.ItemId = ItemId;
                        objWish.CreatedDate = DateTime.UtcNow;
                        _db.tbl_WishList.Add(objWish);
                        _db.SaveChanges();
                    }
                }

                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }
        public ActionResult Detail(int Id)
        {
            long UserId = clsClientSession.UserID;
            long ProdItem = Convert.ToInt64(Id);
            ProductItemVM objProductItem = new ProductItemVM();
            List<string> lstimages = _db.tbl_ProductItemImages.Where(o => o.ProductItemId == ProdItem).Select(o => o.ItemImage).ToList();
            objProductItem = (from i in _db.tbl_ProductItems
                              where i.ProductItemId == Id
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
                                  IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false,
                                  IsAssured = i.IsAssured.HasValue ? i.IsAssured.Value : false
                              }).FirstOrDefault();
            if (clsClientSession.UserID != 0)
            {
                var objWishlist = _db.tbl_WishList.Where(o => o.ClientUserId == UserId && o.ItemId == ProdItem).FirstOrDefault();
                objProductItem.IsWishListItem = false;
                if (objWishlist != null)
                {
                    objProductItem.IsWishListItem = true;
                }
            }
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
                lstVarint = _db.tbl_ItemVariant.Where(o => o.ProductItemId == objProductItem.ProductItemId && o.IsActive == true && (o.IsDeleted == null || o.IsDeleted == false)).ToList();
                if(lstVarint != null && lstVarint.Count() > 0)
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
                                objVM.VariantImg = objvv.VariantImage;
                            }
                            else
                            {
                                objvv.CustomerPrice = Math.Round((objProductItem.CustomerPrice * objvv.PricePecentage.Value) / 100, 2);
                                objvv.DistributorPrice = Math.Round((objProductItem.DistributorPrice * objvv.PricePecentage.Value) / 100, 2);
                                objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                objVM.MRPPrice = Math.Round((objProductItem.MRPPrice * objvv.PricePecentage.Value) / 100, 2);
                                objVM.VariantImg = objvv.VariantImage;
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
                                objVM.VariantImg = objvv.VariantImage;
                            }
                            else
                            {
                                objvv.CustomerPrice = Math.Round((objProductItem.CustomerPrice * objvv.PricePecentage.Value) / 100, 2);
                                objvv.DistributorPrice = Math.Round((objProductItem.DistributorPrice * objvv.PricePecentage.Value) / 100, 2);
                                objVM.CustomerPrice = objvv.CustomerPrice.Value;
                                objVM.DistributorPrice = objvv.DistributorPrice.Value;
                                objVM.MRPPrice = Math.Round((objProductItem.MRPPrice * objvv.PricePecentage.Value) / 100, 2);
                                objVM.VariantImg = objvv.VariantImage;
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
                            objVM.VariantImg = objvv.VariantImage;
                        }
                        else if (objUnt.UnitName.ToLower().Contains("piece"))
                        {  
                            objVM.CustomerPrice = Math.Round(objvv.CustomerPrice.Value,2);
                            objVM.DistributorPrice = Math.Round(objvv.DistributorPrice.Value,2);
                            objVM.MRPPrice = Math.Round(objvv.MRPPrice.Value, 2);
                            objVM.VariantImg = objvv.VariantImage;
                        }
                        else
                        {
                            objVM.CustomerPrice = objvv.CustomerPrice.Value;
                            objVM.DistributorPrice = objvv.DistributorPrice.Value;
                            objVM.MRPPrice = Math.Round(objProductItem.MRPPrice, 2);
                            objVM.VariantImg = objvv.VariantImage;
                        }
                        lstVrntVM.Add(objVM);
                    }
                }
            }
            ViewData["lstVarint"] = lstVrntVM;
            ViewBag.UnitTyp = objUnt.UnitName;
            List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
            List<long> wishlistitemsId = new List<long>();
            if (clsClientSession.UserID != 0)
            {               
                wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
            }

            List<ProductItemVM> lstRelatedItems = new List<ProductItemVM>();
            lstRelatedItems = (from i in _db.tbl_ProductItems                           
                              join p in _db.tbl_Products on i.ProductId equals p.Product_Id                                                            
                                  //where !i.IsDelete && !c.IsDelete && !p.IsDelete
                              where !i.IsDelete && i.IsActive == true && !p.IsDelete && i.ProductItemId != objProductItem.ProductItemId && i.ProductId == objProductItem.ProductId
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
                              }).OrderBy(x => Guid.NewGuid()).ToList().Take(8).ToList();
            if (clsClientSession.UserID != 0)
            {
                lstRelatedItems.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
            }
            else
            {
                lstRelatedItems.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
            }
            ViewData["lstRelatedItems"] = lstRelatedItems;
            return View(objProductItem);
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
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.Qty.Value);
            if (TotalSold == null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
        }

        public ActionResult Popular()
        {
            List<ProductItemVM> lstPopularProductItem = new List<ProductItemVM>();
            List<long> wishlistitemsId = new List<long>();

            List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
            try
            {
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
                                         }).OrderBy(x => x.ItemName).ToList();
                 
                if (clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();

                    lstPopularProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
                else
                {
                    lstPopularProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }

            }
            catch (Exception ex)
            {
            }

            return View(lstPopularProductItem);
        }

        public ActionResult UnPacked()
        {
            List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
            List<ProductItemVM> lstUnpackProductItem = new List<ProductItemVM>();
            List<long> wishlistitemsId = new List<long>();

            try
            {
                lstUnpackProductItem = (from i in _db.tbl_ProductItems
                                        where !i.IsDelete && i.IsActive && i.ItemType == (int)ItemTypes.UnPackedItem
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
                                        }).OrderBy(x => x.ItemName).ToList();

                if (clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();

                    lstUnpackProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
                else
                {
                    lstUnpackProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
            }
            catch (Exception ex)
            {
            }

            return View(lstUnpackProductItem);
        }

        public ActionResult Offer()
        {
            List<ProductItemVM> lstOfferItems = new List<ProductItemVM>();         

            try
            {
                List<tbl_ReviewRating> lstRatings = _db.tbl_ReviewRating.ToList();
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
                                 }).OrderBy(x => x.ItemName).ToList();

                if (clsClientSession.UserID != 0)
                {
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == clsClientSession.UserID).Select(o => o.ItemId.Value).ToList();
                    lstOfferItems.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId); x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }
                else
                {
                    lstOfferItems.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); x.Ratings = GetRatingOfItem(x.ProductItemId, lstRatings); });
                }

            }
            catch (Exception ex)
            {
            }

            return View(lstOfferItems);
        }

        public ActionResult ComboOffers()
        {
            List<ComboOfferVM> lstCombo = new List<ComboOfferVM>();       
         
            try
            {
                lstCombo = (from i in _db.tbl_ComboOfferMaster
                                  where i.IsActive == true && DateTime.UtcNow >= i.OfferStartDate && DateTime.UtcNow <= i.OfferEndDate && i.IsDeleted == false
                                  select new ComboOfferVM
                                  {
                                      OfferTitle = i.OfferTitle,
                                      ComboOfferId = i.ComboOfferId,
                                      ComboOfferPrice = i.OfferPrice,
                                      OfferImage = i.OfferImage,
                                      TotlOriginalOfferPrice = i.TotalActualPrice.Value
                                  }).OrderBy(x => x.OfferTitle).ToList();

            }
            catch (Exception ex)
            {
            }

            return View("~/Areas/Client/Views/Products/ComboOffers.cshtml", lstCombo);
        }

        public ActionResult OfferDetail(int Id)
        {
            long UserId = clsClientSession.UserID;
            long OfferId = Convert.ToInt64(Id);
            ProductItemVM objProductItem = new ProductItemVM();
            tbl_ComboOfferMaster objCommbo = _db.tbl_ComboOfferMaster.Where(o => o.ComboOfferId == OfferId).FirstOrDefault();           
            objProductItem = (from i in _db.tbl_ProductItems
                              join v in _db.tbl_ItemVariant on i.ProductItemId equals v.ProductItemId
                              where i.ProductItemId == objCommbo.MainItemId && v.VariantItemId == objCommbo.MainItemVarintId
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
                                  IsActive = i.IsActive,
                                  UnitType = i.UnitType.HasValue ? i.UnitType.Value : 0,
                                  UnitTyp = v.UnitQty,
                                  IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false
                              }).FirstOrDefault();

            List<ComboSubItemVM> lstSubItemss = new List<ComboSubItemVM>();
            lstSubItemss = (  from c in _db.tbl_ComboOfferSubItems
                              join i in _db.tbl_ProductItems on c.ProductItemId equals i.ProductItemId
                              join v in _db.tbl_ItemVariant on c.VariantItemId equals v.VariantItemId
                              where c.ComboOfferId == OfferId
                              select new ComboSubItemVM
                              {
                                  ProductItemId = i.ProductItemId,
                                  CategoryId = i.CategoryId,
                                  ProductId = i.ProductId,
                                  Sub_ProductItemName = i.ItemName,                                                                   
                                  ActualPrice = c.ActualPrice.Value,
                                  VarintId = c.VariantItemId,
                                  Qty = c.Qty,
                                  VarintNm = v.UnitQty                              
                              }).ToList();

            ViewData["MainItem"] = objProductItem;
            ViewData["lstSubItemss"] = lstSubItemss;
            return View("~/Areas/Client/Views/Products/OfferDetail.cshtml", objCommbo);
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

        public string CheckItemAvailablePincode(string ItmId, string Pincode)
        {   
            string msg = "";
            try
            {
                long ItemId = Convert.ToInt64(ItmId);
                int Pincd = Convert.ToInt32(Pincode);
                var objPinc = _db.tbl_AvailablePincode.Where(o => o.AvailablePincode == Pincode).FirstOrDefault();
                if (objPinc != null)
                {
                    List<tbl_ItemAvailablePincode> lstAvil = _db.tbl_ItemAvailablePincode.Where(o => o.ProductItemId == ItemId).ToList();
                    if (lstAvil != null && lstAvil.Count() > 0)
                    {
                        var objPnAvail = lstAvil.Where(o => o.Pincode == Pincd).FirstOrDefault();
                        if (objPnAvail == null)
                        {
                            msg = "Item not available for this pincode";
                        }
                        else
                        {
                            msg = "Item available for this pincode";
                        }

                    }
                    else
                    {
                        msg = "Item available for this pincode";
                    }
                }
                else
                {
                    msg = "Item not available for this pincode";
                }


            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

            return msg;
        }
    }
}