using KrupaBuildGallery.Model;
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
        public ActionResult ByCategory(int Id,int sortby = 3)
        {
            long CategoryId = Convert.ToInt64(Id);
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();
            try
            {
                ViewBag.Name = "";
               var objCat = _db.tbl_Categories.Where(o => o.CategoryId == CategoryId).FirstOrDefault();
               if(objCat != null)
                {
                    ViewBag.Name = objCat.CategoryName;
                }
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
                                      IsActive = i.IsActive
                                  }).OrderBy(x => x.ItemName).ToList();
                
                if(clsClientSession.UserID != 0)
                {
                    long UserId = clsClientSession.UserID;
                    List<long> wishlistitemsId = _db.tbl_WishList.Where(o => o.ClientUserId == UserId).Select(o => o.ItemId.Value).ToList();
                    lstProductItem.ForEach(x => { x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId);x.CustomerPrice = GetOfferPrice(x.ProductItemId,x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });                    
                }
                else
                {
                    lstProductItem.ForEach(x => { x.CustomerPrice = GetOfferPrice(x.ProductItemId, x.CustomerPrice); x.DistributorPrice = GetDistributorOfferPrice(x.ProductItemId, x.DistributorPrice); });
                }
                
                if(sortby == 1)
                {
                    if(clsClientSession.UserID == 0 || clsClientSession.RoleID == 1)
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
                else if(sortby == 4)
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
        
        public bool IsInWhishList(long ItemId,List<long> ItemList)
        {
            if(ItemList.Contains(ItemId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public string AddRemoveWishList(int ItemId)
        {
            string ReturnMessage = "";

            try
            {
                if(clsClientSession.UserID > 0)
                {
                    long UserId = clsClientSession.UserID;
                    tbl_WishList objWish =_db.tbl_WishList.Where(o => o.ItemId == ItemId && o.ClientUserId == UserId).FirstOrDefault();
                    if(objWish != null)
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
        public ActionResult Detail (int Id)
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
                                  IsActive = i.IsActive                                  
                              }).FirstOrDefault();
            if (clsClientSession.UserID != 0)
            {                
               var objWishlist =  _db.tbl_WishList.Where(o => o.ClientUserId == UserId && o.ItemId == ProdItem).FirstOrDefault();
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
            return View(objProductItem);
        }
        public decimal GetOfferPrice(long Itemid,decimal price)
        {
           var objItem =_db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
           if(objItem != null)
           {
                return objItem.OfferPrice;
           }

            return price;  
        }

        public decimal GetDistributorOfferPrice(long Itemid,decimal price)
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
            if(TotalStock == null)
            {
                TotalStock = 0;
            }
            return Convert.ToInt32(TotalStock);
        }
        public int SoldItems(long ItemId)
        {
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.Qty.Value);
            if(TotalSold == null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
        }
    }
}