﻿using KrupaBuildGallery.Model;
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
                    lstProductItem.ForEach(x => x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId));                    
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
            ViewBag.FromPage = "client/products/bycategory?Id=" + CategoryId;

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
                    lstProductItem.ForEach(x => x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId));
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
            ViewBag.FromPage = "client/products/Bysubproduct?Id=" + SubproductId;

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
                                  where !i.IsDelete && i.IsActive == true && i.SubProductId == productid
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
                    lstProductItem.ForEach(x => x.IsWishListItem = IsInWhishList(x.ProductItemId, wishlistitemsId));
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
            ViewBag.FromPage = "client/products/Byproduct?Id=" + productid;

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

    }
}