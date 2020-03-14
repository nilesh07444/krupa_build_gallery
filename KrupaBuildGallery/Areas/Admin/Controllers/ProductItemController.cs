﻿using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ProductItemController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public ProductItemController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();

            try
            {
                lstProductItem = (from i in _db.tbl_ProductItems
                                  join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                  join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSubProduct
                                  from s in outerJoinSubProduct.DefaultIfEmpty()
                                  where !i.IsDelete && !c.IsDelete && !p.IsDelete
                                  select new ProductItemVM
                                  {
                                      ProductItemId = i.ProductItemId,
                                      CategoryId = c.CategoryId,
                                      ProductId = i.ProductId,
                                      SubProductId = i.SubProductId,
                                      ItemName = i.ItemName,
                                      CategoryName = c.CategoryName,
                                      ProductName = p.ProductName,
                                      SubProductName = s.SubProductName,
                                      MainImage = i.MainImage,
                                      MRPPrice = i.MRPPrice,
                                      CustomerPrice = i.CustomerPrice,
                                      DistributorPrice = i.DistributorPrice,
                                      IsActive = i.IsActive
                                  }).OrderByDescending(x => x.ProductItemId).ToList();
                if (lstProductItem != null && lstProductItem.Count() > 0)
                {
                    lstProductItem.ForEach(x => { x.Sold = SoldItems(x.ProductItemId); x.InStock = ItemStock(x.ProductItemId) - x.Sold;});
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstProductItem);

        }

        public ActionResult Add()
        {
            ProductItemVM objProductItem = new ProductItemVM();

            objProductItem.CategoryList = GetCategoryList();
            objProductItem.ProductList = new List<SelectListItem>();
            objProductItem.SubProductList = new List<SelectListItem>();

            return View(objProductItem);
        }

        [HttpPost]
        public ActionResult Add(ProductItemVM productItemVM, HttpPostedFileBase ItemMainImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath("~/Images/ProductItemMedia/");
                    if (ItemMainImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(ItemMainImageFile.FileName);
                        ItemMainImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = productItemVM.MainImage;
                    }

                    tbl_ProductItems objProductItem = new tbl_ProductItems();
                    objProductItem.CategoryId = productItemVM.CategoryId;
                    objProductItem.ProductId = productItemVM.ProductId;
                    objProductItem.SubProductId = productItemVM.SubProductId;
                    objProductItem.ItemName = productItemVM.ItemName;
                    objProductItem.ItemDescription = productItemVM.ItemDescription;
                    objProductItem.Sku = productItemVM.Sku;
                    objProductItem.MRPPrice = productItemVM.MRPPrice;
                    objProductItem.CustomerPrice = productItemVM.CustomerPrice;
                    objProductItem.DistributorPrice = productItemVM.DistributorPrice;
                    objProductItem.GST_Per = productItemVM.GST_Per;
                    objProductItem.IGST_Per = productItemVM.IGST_Per;
                    objProductItem.Notification = productItemVM.Notification;
                    objProductItem.MainImage = fileName;
                    objProductItem.IsPopularProduct = productItemVM.IsPopularProduct;

                    objProductItem.IsActive = true;
                    objProductItem.IsDelete = false;
                    objProductItem.CreatedBy = LoggedInUserId;
                    objProductItem.CreatedDate = DateTime.UtcNow;
                    objProductItem.UpdatedBy = LoggedInUserId;
                    objProductItem.UpdatedDate = DateTime.UtcNow;

                    _db.tbl_ProductItems.Add(objProductItem);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(productItemVM);
        }

        public ActionResult Edit(int Id)
        {
            ProductItemVM objProductItem = new ProductItemVM();

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
                                  IsActive = i.IsActive
                              }).FirstOrDefault();

            objProductItem.CategoryList = GetCategoryList();
            objProductItem.ProductList = GetProductListByCategoryId(objProductItem.CategoryId);
            objProductItem.SubProductList = GetSubProductListByProductId(objProductItem.ProductId);

            return View(objProductItem);
        }

        [HttpPost]
        public ActionResult Edit(ProductItemVM productItemVM, HttpPostedFileBase ItemMainImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    tbl_ProductItems objProductItem = _db.tbl_ProductItems.Where(x => x.ProductItemId == productItemVM.ProductItemId).FirstOrDefault();

                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath("~/Images/ProductItemMedia/");
                    if (ItemMainImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(ItemMainImageFile.FileName);
                        ItemMainImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objProductItem.MainImage;
                    }
                     
                    objProductItem.CategoryId = productItemVM.CategoryId;
                    objProductItem.ProductId = productItemVM.ProductId;
                    objProductItem.SubProductId = productItemVM.SubProductId;
                    objProductItem.ItemName = productItemVM.ItemName;
                    objProductItem.ItemDescription = productItemVM.ItemDescription;
                    objProductItem.Sku = productItemVM.Sku;
                    objProductItem.MRPPrice = productItemVM.MRPPrice;
                    objProductItem.CustomerPrice = productItemVM.CustomerPrice;
                    objProductItem.DistributorPrice = productItemVM.DistributorPrice;
                    objProductItem.GST_Per = productItemVM.GST_Per;
                    objProductItem.IGST_Per = productItemVM.IGST_Per;
                    objProductItem.Notification = productItemVM.Notification;
                    objProductItem.MainImage = fileName;
                    objProductItem.IsPopularProduct = productItemVM.IsPopularProduct;

                    objProductItem.UpdatedBy = LoggedInUserId;
                    objProductItem.UpdatedDate = DateTime.UtcNow; 
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(productItemVM);
        }
         
        [HttpPost]
        public string DeleteProductItem(long ProductItemId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_ProductItems objProductItem = _db.tbl_ProductItems.Where(x => x.ProductItemId == ProductItemId && x.IsActive && !x.IsDelete).FirstOrDefault();

                if (objProductItem == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objProductItem.IsDelete = true;
                    objProductItem.UpdatedBy = LoggedInUserId;
                    objProductItem.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

        public List<SelectListItem> GetProductListByCategoryId(long Id)
        {
            var ProductList = _db.tbl_Products.Where(x => x.IsActive && !x.IsDelete && x.CategoryId == Id)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.Product_Id).Trim(), Text = o.ProductName })
                         .OrderBy(x => x.Text).ToList();

            return ProductList;
        }

        public List<SelectListItem> GetSubProductListByProductId(long Id)
        {
            var ProductList = _db.tbl_SubProducts.Where(x => x.IsActive && !x.IsDelete && x.ProductId == Id)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.SubProductId).Trim(), Text = o.SubProductName })
                         .OrderBy(x => x.Text).ToList();

            return ProductList;
        }

        public int ItemStock(long ItemId)
        {
           long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?) o.Qty);
           return Convert.ToInt32(TotalStock);
        }
        public int SoldItems(long ItemId)
        {
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.Qty.Value);
            return Convert.ToInt32(TotalSold);
        }
    }
}