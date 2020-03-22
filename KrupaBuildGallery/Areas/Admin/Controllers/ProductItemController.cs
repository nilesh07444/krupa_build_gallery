using ConstructionDiary.Models;
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

        public ActionResult Add(long Id = 0)
        {
            ProductItemVM objProductItem = new ProductItemVM();
            if(Id > 0)
            {
                var objProductItm =  _db.tbl_ProductItems.Where(o => o.ProductItemId == Id).FirstOrDefault();
                objProductItem.CategoryId = objProductItm.CategoryId;
                objProductItem.ProductId = objProductItm.ProductId;
                objProductItem.SubProductId = objProductItm.SubProductId;
                objProductItem.CategoryList = GetCategoryList();
                objProductItem.ProductList = GetProductListByCategoryId(objProductItm.CategoryId);
                objProductItem.SubProductList = GetSubProductListByProductId(objProductItm.ProductId);
                objProductItem.GST = GetGST();
                objProductItem.GST_Per = objProductItm.GST_Per;
            }
            else
            {
                objProductItem.CategoryList = GetCategoryList();
                objProductItem.ProductList = new List<SelectListItem>();
                objProductItem.SubProductList = new List<SelectListItem>();
                objProductItem.GST = GetGST();
            }
            //if (productItemVM != null && productItemVM.CategoryId > 0)
            //{
            //    //objProductItem = productItemVM;
            //    productItemVM.CategoryId = productItemVM.CategoryId;
            //    productItemVM.ProductId = productItemVM.ProductId;
            //    productItemVM.SubProductId = productItemVM.SubProductId;
            //    productItemVM.ProductList = GetProductListByCategoryId(objProductItem.CategoryId);
            //    productItemVM.SubProductList = GetSubProductListByProductId(objProductItem.ProductId);
            //    productItemVM.GST = GetGST();
            //    productItemVM.GST_Per = productItemVM.GST_Per;
            //}
            //else
            //{
            
           // }
           
            return View(objProductItem);
        }

        [HttpPost]
        public ActionResult Add(ProductItemVM productItemVM, HttpPostedFileBase ItemMainImageFile, HttpPostedFileBase[] ItemGalleryImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    // Check for exist
                    var existProductItem = _db.tbl_ProductItems.Where(x => x.ItemName.ToLower() == productItemVM.ItemName.ToLower()
                        && x.CategoryId == productItemVM.CategoryId && x.ProductId == productItemVM.ProductId
                        && !x.IsDelete).FirstOrDefault();

                    if (existProductItem != null)
                    {
                        ModelState.AddModelError("ItemName", ErrorMessage.ItemNameExists);
                    }                  
                    else
                    {
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
                        objProductItem.ShippingCharge = productItemVM.ShippingCharge;
                        objProductItem.IsActive = true;
                        objProductItem.IsDelete = false;
                        objProductItem.CreatedBy = LoggedInUserId;
                        objProductItem.CreatedDate = DateTime.UtcNow;
                        objProductItem.UpdatedBy = LoggedInUserId;
                        objProductItem.UpdatedDate = DateTime.UtcNow;

                        _db.tbl_ProductItems.Add(objProductItem);
                        _db.SaveChanges();

                        //iterating through multiple file collection   
                        if (ItemGalleryImageFile != null && ItemGalleryImageFile.Count() > 0)
                        {
                            foreach (HttpPostedFileBase file in ItemGalleryImageFile)
                            {
                                //Checking file is available to save.  
                                if (file != null)
                                {
                                    string fileName1 = Guid.NewGuid() + "-" + Path.GetFileName(file.FileName);
                                    string path1 = Server.MapPath("~/Images/ProductItemMedia/");
                                    file.SaveAs(path1 + fileName1);

                                    tbl_ProductItemImages objGalleryImage = new tbl_ProductItemImages();
                                    objGalleryImage.ProductItemId = objProductItem.ProductItemId;
                                    objGalleryImage.ItemImage = fileName1;
                                    objGalleryImage.IsActive = true;
                                    objGalleryImage.IsDelete = false;
                                    objGalleryImage.CreatedBy = LoggedInUserId;
                                    objGalleryImage.CreatedDate = DateTime.UtcNow;
                                    objGalleryImage.UpdatedBy = LoggedInUserId;
                                    objGalleryImage.UpdatedDate = DateTime.UtcNow;
                                    _db.tbl_ProductItemImages.Add(objGalleryImage);
                                    _db.SaveChanges();

                                }

                            }
                        }


                        tbl_ItemStocks objItemStock = new tbl_ItemStocks();
                        objItemStock.CategoryId = productItemVM.CategoryId;
                        objItemStock.ProductId = productItemVM.ProductId;
                        objItemStock.SubProductId = productItemVM.SubProductId;
                        objItemStock.ProductItemId = objProductItem.ProductItemId;
                        objItemStock.Qty = Convert.ToInt64(productItemVM.InitialQty);

                        objItemStock.IsActive = true;
                        objItemStock.IsDelete = false;
                        objItemStock.CreatedBy = LoggedInUserId;
                        objItemStock.CreatedDate = DateTime.UtcNow;
                        objItemStock.UpdatedBy = LoggedInUserId;
                        objItemStock.UpdatedDate = DateTime.UtcNow;
                        _db.tbl_ItemStocks.Add(objItemStock);
                        _db.SaveChanges();

                        //productItemVM = new ProductItemVM();
                        //productItemVM.ItemName = "";
                        //productItemVM.ItemDescription = "";
                        //productItemVM.MRPPrice = 0;
                        //productItemVM.CustomerPrice = 0;
                        //productItemVM.DistributorPrice = 0;
                        //productItemVM.InitialQty = 0;
                        //productItemVM.Notification = "";
                        //productItemVM.ProductItemId = 0;
                        //productItemVM.Sku = "";                        
                        //productItemVM.CategoryId = objProductItem.CategoryId;
                        //productItemVM.ProductId = objProductItem.ProductId;
                        //productItemVM.SubProductId = objProductItem.SubProductId;
                        //productItemVM.CategoryList = GetCategoryList();
                        //productItemVM.GST_Per = objProductItem.GST_Per;
                        //productItemVM.ProductList = GetProductListByCategoryId(objProductItem.CategoryId);
                        //productItemVM.SubProductList = GetSubProductListByProductId(objProductItem.ProductId);
                        //productItemVM.GST = GetGST();
                        //return RedirectToAction("Add", productItemVM);
                        //return View(productItemVM);
                        return RedirectToAction("Add",new {Id= objProductItem .ProductItemId});
                    }                  

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            productItemVM.CategoryList = GetCategoryList();
            productItemVM.ProductList = new List<SelectListItem>();
            productItemVM.SubProductList = new List<SelectListItem>();
            productItemVM.GST = GetGST();
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
                                  IsActive = i.IsActive,
                                  InitialQty = 1,
                                  ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value:0
                              }).FirstOrDefault();

            objProductItem.CategoryList = GetCategoryList();
            objProductItem.ProductList = GetProductListByCategoryId(objProductItem.CategoryId);
            objProductItem.SubProductList = GetSubProductListByProductId(objProductItem.ProductId);
            objProductItem.GST = GetGST();
            return View(objProductItem);
        }

        [HttpPost]
        public ActionResult Edit(ProductItemVM productItemVM, HttpPostedFileBase ItemMainImageFile, HttpPostedFileBase[] ItemGalleryImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    var existProductItem = _db.tbl_ProductItems.Where(x => x.ProductItemId != productItemVM.ProductItemId &&  x.ItemName.ToLower() == productItemVM.ItemName.ToLower()
                      && x.CategoryId == productItemVM.CategoryId && x.ProductId == productItemVM.ProductId
                      && !x.IsDelete).FirstOrDefault();

                    if (existProductItem != null)
                    {
                        ModelState.AddModelError("ItemName", ErrorMessage.ItemNameExists);
                    }
                    else
                    {
                        tbl_ProductItems objProductItem = _db.tbl_ProductItems.Where(x => x.ProductItemId == productItemVM.ProductItemId).FirstOrDefault();
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
                        objProductItem.ShippingCharge = productItemVM.ShippingCharge;
                        objProductItem.UpdatedBy = LoggedInUserId;
                        objProductItem.UpdatedDate = DateTime.UtcNow;
                        _db.SaveChanges();

                        //iterating through multiple file collection   
                        if (ItemGalleryImageFile != null && ItemGalleryImageFile.Count() > 0)
                        {
                            foreach (HttpPostedFileBase file in ItemGalleryImageFile)
                            {
                                //Checking file is available to save.  
                                if (file != null)
                                {
                                    string fileName1 = Guid.NewGuid() + "-" + Path.GetFileName(file.FileName);
                                    string path1 = Server.MapPath("~/Images/ProductItemMedia/");
                                    file.SaveAs(path1 + fileName1);

                                    tbl_ProductItemImages objGalleryImage = new tbl_ProductItemImages();
                                    objGalleryImage.ProductItemId = objProductItem.ProductItemId;
                                    objGalleryImage.ItemImage = fileName1;
                                    objGalleryImage.IsActive = true;
                                    objGalleryImage.IsDelete = false;
                                    objGalleryImage.CreatedBy = LoggedInUserId;
                                    objGalleryImage.CreatedDate = DateTime.UtcNow;
                                    objGalleryImage.UpdatedBy = LoggedInUserId;
                                    objGalleryImage.UpdatedDate = DateTime.UtcNow;
                                    _db.tbl_ProductItemImages.Add(objGalleryImage);
                                    _db.SaveChanges();

                                }

                            }
                        }

                        return RedirectToAction("Index");

                    }

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            productItemVM.CategoryList = GetCategoryList();
            productItemVM.ProductList = GetProductListByCategoryId(productItemVM.CategoryId);
            productItemVM.SubProductList = GetSubProductListByProductId(productItemVM.ProductId);
            productItemVM.GST = GetGST();
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

        public List<SelectListItem> GetGST()
        {
           var lstGST =  _db.tbl_GSTMaster.OrderBy(x => x.GSTPer).ToList();
            List<SelectListItem> lstselc = new List<SelectListItem>();
           if (lstGST != null && lstGST.Count() > 0)
            {
                foreach(var objj in lstGST)
                {
                    SelectListItem obb = new SelectListItem();
                    obb.Value = objj.GSTPer.ToString();
                    obb.Text = objj.GSTText;
                    lstselc.Add(obb);
                }
            }
        
            return lstselc;
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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_ProductItems objtbl_ProductItems = _db.tbl_ProductItems.Where(x => x.ProductItemId == Id).FirstOrDefault();

                if (objtbl_ProductItems != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_ProductItems.IsActive = true;
                    }
                    else
                    {
                        objtbl_ProductItems.IsActive = false;
                    }

                    objtbl_ProductItems.UpdatedBy = LoggedInUserId;
                    objtbl_ProductItems.UpdatedDate = DateTime.UtcNow;

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

        [HttpPost]
        public JsonResult GetItemText(string prefix)
        {           
            var itmtext = (from txt in _db.tbl_Itemtext_master
                             where txt.ItemText.ToLower().Contains(prefix.ToLower())
                             select new
                             {
                                 label = txt.ItemText,
                                 val = txt.ItemText
                             }).ToList();

            return Json(itmtext);
        }

    }
}