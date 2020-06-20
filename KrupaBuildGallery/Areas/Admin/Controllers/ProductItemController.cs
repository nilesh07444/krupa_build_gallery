using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel.Admin;
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

        public ActionResult Index(int CategoryId = -1, int ProductId = -1, int SubProductId = -1, int Active = -1)
        {
            List<ProductItemVM> lstProductItem = new List<ProductItemVM>();

            try
            {
                bool IsActv = false;
                if (Active != -1)
                {
                    if (Active == 1)
                    {
                        IsActv = true;
                    }
                }
                lstProductItem = (from i in _db.tbl_ProductItems
                                  join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                  join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSubProduct
                                  from s in outerJoinSubProduct.DefaultIfEmpty()
                                  where !i.IsDelete && !c.IsDelete && !p.IsDelete && (CategoryId == -1 || i.CategoryId == CategoryId) && (ProductId == -1 || i.ProductId == ProductId) && (SubProductId == -1 || i.SubProductId == SubProductId) && (Active == -1 || i.IsActive == IsActv)
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
                    lstProductItem.ForEach(x => { x.Sold = SoldItems(x.ProductItemId); x.InStock = ItemStock(x.ProductItemId) - x.Sold; });
                }

                ViewData["CategoryList"] = GetCategoryList();
                ViewData["ProductList"] = GetProductListByCategoryId(CategoryId);
                ViewData["SubProductList"] = GetSubProductListByProductId(ProductId);
                ViewBag.CatId = CategoryId;
                ViewBag.ProductId = ProductId;
                ViewBag.SubProductId = SubProductId;
                ViewBag.Active = Active;

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
            if (Id > 0)
            {
                var objProductItm = _db.tbl_ProductItems.Where(o => o.ProductItemId == Id).FirstOrDefault();
                objProductItem.CategoryId = objProductItm.CategoryId;
                objProductItem.ProductId = objProductItm.ProductId;
                objProductItem.SubProductId = objProductItm.SubProductId;
                objProductItem.CategoryList = GetCategoryList();
                objProductItem.ProductList = GetProductListByCategoryId(objProductItm.CategoryId);
                objProductItem.SubProductList = GetSubProductListByProductId(objProductItm.ProductId);
                objProductItem.GST = GetGST();
                objProductItem.UnitList = GetUnitItems();
                objProductItem.GST_Per = objProductItm.GST_Per;
                objProductItem.GodownList = GetGodownList();
                objProductItem.GodownId = objProductItem.GodownId;
            }
            else
            {
                objProductItem.CategoryList = GetCategoryList();
                objProductItem.ProductList = new List<SelectListItem>();
                objProductItem.SubProductList = new List<SelectListItem>();
                objProductItem.GodownList = GetGodownList();
                objProductItem.GST = GetGST();
                objProductItem.UnitList = GetUnitItems();
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
        public ActionResult Add(ProductItemVM productItemVM, HttpPostedFileBase ItemMainImageFile, HttpPostedFileBase[] ItemGalleryImageFile,FormCollection frm)
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
                        objProductItem.HSNCode = productItemVM.HSNCode;
                        objProductItem.UpdatedDate = DateTime.UtcNow;
                        objProductItem.GodownId = productItemVM.GodownId;
                        objProductItem.IsReturnable = productItemVM.IsReturnableItem;
                        objProductItem.PayAdvancePer = productItemVM.PayAdvancePer;
                        objProductItem.ItemType = productItemVM.ItemType;
                        objProductItem.IsCashonDeliveryUse = productItemVM.IsCashonDelieveryuse;
                        objProductItem.MinimumStock = productItemVM.MinimumQty;
                        objProductItem.UnitType = productItemVM.UnitType;
                        objProductItem.Tags = productItemVM.Tags;
                        _db.tbl_ProductItems.Add(objProductItem);
                        _db.SaveChanges();
                        string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
                        string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
                        string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
                        string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

                        string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
                        string[] sheetsqty = { "32", "28", "21", "24", "18" };
                        var objUnt =_db.tbl_Units.Where(o => o.UnitId == objProductItem.UnitType).FirstOrDefault();
                        if(objUnt != null)
                        {
                            if(objUnt.UnitName.ToLower().Contains("killo") || objUnt.UnitName.ToLower().Contains("litr"))
                            {
                                for(int kk = 1; kk <= kgs.Length; kk++)
                                {
                                    tbl_ItemVariant objtbl_ItemVariant = new tbl_ItemVariant();
                                    objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                    objtbl_ItemVariant.IsActive = false;
                                    if (Request.Form["chkvariant_"+kk] != null)
                                    {
                                        objtbl_ItemVariant.IsActive = true;
                                    }

                                    if(Request.Form["variantper_" + kk] != null)
                                    {
                                       decimal perc = Convert.ToDecimal(frm["variantper_" + kk].ToString());
                                        int k = kk - 1;
                                        objtbl_ItemVariant.PricePecentage = perc;
                                        if (objUnt.UnitName.ToLower().Contains("killo"))
                                        {                                            
                                            objtbl_ItemVariant.UnitQty = kgs[k];
                                            decimal qtt = Convert.ToDecimal(kgsQty[k].ToString());
                                            if ( qtt >= 1)
                                            {
                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * perc)/100,2);
                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * perc) / 100, 2);
                                            }
                                            else
                                            {
                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * perc)/100,2);
                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * perc) / 100, 2);
                                            }
                                        }
                                        else
                                        {
                                            objtbl_ItemVariant.UnitQty = ltrs[k];
                                            decimal qtt = Convert.ToDecimal(ltrsQty[k].ToString());                                           
                                            if (qtt >= 1)
                                            {
                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * perc) / 100, 2);
                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * perc) / 100, 2);
                                            }
                                            else
                                            {
                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * perc) / 100, 2);
                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * perc) / 100, 2);
                                            }
                                        }
                                    }
                                    objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                    _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                }
                                _db.SaveChanges();
                            }
                            else if (objUnt.UnitName.ToLower().Contains("sheet"))
                            {
                                for (int kk = 1; kk <= sheets.Length; kk++)
                                {
                                    int k = kk - 1;
                                    tbl_ItemVariant objtbl_ItemVariant = new tbl_ItemVariant();
                                    objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                    objtbl_ItemVariant.IsActive = false;
                                    if (Request.Form["chkvariant_" + kk] != null)
                                    {
                                        objtbl_ItemVariant.IsActive = true;
                                    }
                                    decimal sqft = Convert.ToDecimal(sheetsqty[k]);
                                    objtbl_ItemVariant.UnitQty = sheets[k];
                                    objtbl_ItemVariant.CustomerPrice = Math.Round(sqft * objProductItem.CustomerPrice, 2);
                                    objtbl_ItemVariant.DistributorPrice = Math.Round(sqft * objProductItem.DistributorPrice, 2);
                                    objtbl_ItemVariant.PricePecentage = 100;
                                    objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                    _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                }
                                _db.SaveChanges();
                            }
                            else
                            {
                                tbl_ItemVariant objtbl_ItemVariant = new tbl_ItemVariant();
                                objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                objtbl_ItemVariant.IsActive = true;                                                             
                                objtbl_ItemVariant.UnitQty = objUnt.UnitName;
                                objtbl_ItemVariant.CustomerPrice = Math.Round(objProductItem.CustomerPrice, 2);
                                objtbl_ItemVariant.DistributorPrice = Math.Round(objProductItem.DistributorPrice, 2);
                                objtbl_ItemVariant.PricePecentage = 100;
                                objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                _db.SaveChanges();
                            }
                        }

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
                        return RedirectToAction("Add", new { Id = objProductItem.ProductItemId });
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
            productItemVM.UnitList = GetUnitItems();
            productItemVM.GodownList = GetGodownList();
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
                                  HSNCode = i.HSNCode,
                                  IsReturnableItem = i.IsReturnable.HasValue ? i.IsReturnable.Value : false,
                                  ItemType = i.ItemType.HasValue ? i.ItemType.Value : 1,
                                  PayAdvancePer = i.PayAdvancePer.HasValue ? i.PayAdvancePer.Value : 0,
                                  ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                  UnitType = i.UnitType.HasValue ? i.UnitType.Value : 0,
                                  MinimumQty = i.MinimumStock.HasValue ? i.MinimumStock.Value : 0,
                                  IsCashonDelieveryuse = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false,
                                  GodownId = i.GodownId.HasValue ? i.GodownId.Value : 0
                              }).FirstOrDefault();

            objProductItem.CategoryList = GetCategoryList();
            objProductItem.ProductList = GetProductListByCategoryId(objProductItem.CategoryId);
            objProductItem.SubProductList = GetSubProductListByProductId(objProductItem.ProductId);
            objProductItem.GST = GetGST();
            objProductItem.UnitList = GetUnitItems();
            objProductItem.GodownList = GetGodownList();
            return View(objProductItem);
        }

        [HttpPost]
        public ActionResult Edit(ProductItemVM productItemVM, HttpPostedFileBase ItemMainImageFile, HttpPostedFileBase[] ItemGalleryImageFile,FormCollection frm)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    var existProductItem = _db.tbl_ProductItems.Where(x => x.ProductItemId != productItemVM.ProductItemId && x.ItemName.ToLower() == productItemVM.ItemName.ToLower()
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
                        objProductItem.GodownId = productItemVM.GodownId;
                        objProductItem.HSNCode = productItemVM.HSNCode;
                        objProductItem.UpdatedDate = DateTime.UtcNow;
                        objProductItem.IsReturnable = productItemVM.IsReturnableItem;
                        objProductItem.PayAdvancePer = productItemVM.PayAdvancePer;
                        objProductItem.MinimumStock = productItemVM.MinimumQty;
                        objProductItem.UnitType = productItemVM.UnitType;
                        objProductItem.IsCashonDeliveryUse = productItemVM.IsCashonDelieveryuse;
                        objProductItem.Tags = productItemVM.Tags;
                        objProductItem.ItemType = productItemVM.ItemType;
                        _db.SaveChanges();

                        string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
                        string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
                        string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
                        string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

                        string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
                        string[] sheetsqty = { "32", "28", "21", "24", "18" };
                        var objUnt = _db.tbl_Units.Where(o => o.UnitId == objProductItem.UnitType).FirstOrDefault();
                        if (objUnt != null)
                        {
                            if (objUnt.UnitName.ToLower().Contains("killo") || objUnt.UnitName.ToLower().Contains("litr"))
                            {
                                List<tbl_ItemVariant> lstItmvrnt = _db.tbl_ItemVariant.Where(o => o.ProductItemId == objProductItem.ProductItemId).ToList();
                                for (int kk = 1; kk <= kgs.Length; kk++)
                                {
                                    if (lstItmvrnt != null && lstItmvrnt.Count() > 0)
                                    {
                                        tbl_ItemVariant objtbl_ItemVariant = new tbl_ItemVariant();
                                        objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                        objtbl_ItemVariant.IsActive = false;
                                        if (Request.Form["chkvariant_" + kk] != null)
                                        {
                                            objtbl_ItemVariant.IsActive = true;
                                        }

                                        if (Request.Form["variantper_" + kk] != null)
                                        {
                                            decimal perc = Convert.ToDecimal(frm["variantper_" + kk].ToString());
                                            int k = kk - 1;
                                            objtbl_ItemVariant.PricePecentage = perc;
                                            if (objUnt.UnitName.ToLower().Contains("killo"))
                                            {
                                                objtbl_ItemVariant.UnitQty = kgs[k];
                                                decimal qtt = Convert.ToDecimal(kgsQty[k].ToString());
                                                if (qtt >= 1)
                                                {
                                                    objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * perc) / 100, 2);
                                                    objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * perc) / 100, 2);
                                                }
                                                else
                                                {
                                                    objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * perc) / 100, 2);
                                                    objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * perc) / 100, 2);
                                                }
                                            }
                                            else
                                            {
                                                objtbl_ItemVariant.UnitQty = ltrs[k];
                                                decimal qtt = Convert.ToDecimal(ltrsQty[k].ToString());
                                                if (qtt >= 1)
                                                {
                                                    objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * perc) / 100, 2);
                                                    objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * perc) / 100, 2);
                                                }
                                                else
                                                {
                                                    objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * perc) / 100, 2);
                                                    objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * perc) / 100, 2);
                                                }
                                            }
                                        }
                                        objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                        tbl_ItemVariant objtbl_ItemVariant1 = lstItmvrnt.Where(o => o.UnitQty == objtbl_ItemVariant.UnitQty).FirstOrDefault();
                                        if(objtbl_ItemVariant1 != null)
                                        {
                                            objtbl_ItemVariant1.PricePecentage = objtbl_ItemVariant.PricePecentage;
                                            objtbl_ItemVariant1.CustomerPrice = objtbl_ItemVariant.CustomerPrice;
                                            objtbl_ItemVariant1.DistributorPrice = objtbl_ItemVariant.DistributorPrice;
                                            objtbl_ItemVariant1.IsActive = objtbl_ItemVariant.IsActive;
                                            _db.SaveChanges();
                                        }
                                        else
                                        {
                                            _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                            _db.SaveChanges();
                                        }                                       
                                       
                                    }
                                  
                                }
                                _db.SaveChanges();
                            }
                            else if (objUnt.UnitName.ToLower().Contains("sheet"))
                            {
                                List<tbl_ItemVariant> lstItmvrnt = _db.tbl_ItemVariant.Where(o => o.ProductItemId == objProductItem.ProductItemId).ToList();
                                for (int kk = 1; kk <= sheets.Length; kk++)
                                {
                                    if(lstItmvrnt != null && lstItmvrnt.Count() > 0)
                                    {
                                        int k = kk - 1;
                                        tbl_ItemVariant objtbl_ItemVariant = lstItmvrnt.Where(o => o.UnitQty == sheets[k]).FirstOrDefault();
                                        if(objtbl_ItemVariant != null)
                                        {                                           
                                            objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                            objtbl_ItemVariant.IsActive = false;
                                            if (Request.Form["chkvariant_" + kk] != null)
                                            {
                                                objtbl_ItemVariant.IsActive = true;
                                            }
                                            decimal sqft = Convert.ToDecimal(sheetsqty[k]);
                                            objtbl_ItemVariant.UnitQty = sheets[k];
                                            objtbl_ItemVariant.CustomerPrice = Math.Round(sqft * objProductItem.CustomerPrice, 2);
                                            objtbl_ItemVariant.DistributorPrice = Math.Round(sqft * objProductItem.DistributorPrice, 2);
                                            objtbl_ItemVariant.PricePecentage = 100;
                                            objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;                                          
                                        }
                                    }                                    
                                }
                                _db.SaveChanges();
                            }
                            else
                            {
                               var objtbl_ItemVariant = _db.tbl_ItemVariant.Where(o => o.ProductItemId == objProductItem.ProductItemId).FirstOrDefault();
                               if(objtbl_ItemVariant != null)
                               {
                                    objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                    objtbl_ItemVariant.IsActive = true;
                                    objtbl_ItemVariant.UnitQty = objUnt.UnitName;
                                    objtbl_ItemVariant.CustomerPrice = Math.Round(objProductItem.CustomerPrice, 2);
                                    objtbl_ItemVariant.DistributorPrice = Math.Round(objProductItem.DistributorPrice, 2);
                                    objtbl_ItemVariant.PricePecentage = 100;
                                    objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;                                  
                                    _db.SaveChanges();
                               }
                               else
                                {
                                    objtbl_ItemVariant = new tbl_ItemVariant();
                                    objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                    objtbl_ItemVariant.IsActive = true;
                                    objtbl_ItemVariant.UnitQty = objUnt.UnitName;
                                    objtbl_ItemVariant.CustomerPrice = Math.Round(objProductItem.CustomerPrice, 2);
                                    objtbl_ItemVariant.DistributorPrice = Math.Round(objProductItem.DistributorPrice, 2);
                                    objtbl_ItemVariant.PricePecentage = 100;
                                    objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                    _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                    _db.SaveChanges();
                                }
                            }
                        }

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
            productItemVM.GodownList = GetGodownList();
            productItemVM.UnitList = GetUnitItems();
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
            var lstGST = _db.tbl_GSTMaster.OrderBy(x => x.GSTPer).ToList();
            List<SelectListItem> lstselc = new List<SelectListItem>();
            if (lstGST != null && lstGST.Count() > 0)
            {
                foreach (var objj in lstGST)
                {
                    SelectListItem obb = new SelectListItem();
                    obb.Value = objj.GSTPer.ToString();
                    obb.Text = objj.GSTText;
                    lstselc.Add(obb);
                }
            }

            return lstselc;
        }

        public List<SelectListItem> GetUnitItems()
        {
            var lstUnts = _db.tbl_Units.OrderBy(x => x.UnitName).ToList();
            List<SelectListItem> lstselc = new List<SelectListItem>();
            if (lstUnts != null && lstUnts.Count() > 0)
            {
                foreach (var objj in lstUnts)
                {
                    SelectListItem obb = new SelectListItem();
                    obb.Value = objj.UnitId.ToString();
                    obb.Text = objj.UnitName;
                    lstselc.Add(obb);
                }
            }

            return lstselc;
        }

        public List<SelectListItem> GetProductListByCategoryId(long Id)
        {
            var ProductList = _db.tbl_Products.Where(x => x.IsActive && !x.IsDelete && (Id == -1 || x.CategoryId == Id))
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.Product_Id).Trim(), Text = o.ProductName })
                         .OrderBy(x => x.Text).ToList();

            return ProductList;
        }

        public List<SelectListItem> GetSubProductListByProductId(long Id)
        {
            var ProductList = _db.tbl_SubProducts.Where(x => x.IsActive && !x.IsDelete && (Id == -1 || x.ProductId == Id))
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.SubProductId).Trim(), Text = o.SubProductName })
                         .OrderBy(x => x.Text).ToList();

            return ProductList;
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

        public List<SelectListItem> GetGodownList()
        {
            var GodownList = _db.tbl_Godown.Where(x => x.IsActive && !x.IsDeleted)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.GodownId).Trim(), Text = o.GodownName })
                         .OrderBy(x => x.Text).ToList();

            return GodownList;
        }

        public ActionResult View(int Id)
        {
            ProductItemVM objProductItem = new ProductItemVM();

            try
            {

                objProductItem = (from i in _db.tbl_ProductItems
                                  join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on i.ProductId equals p.Product_Id

                                  join sub in _db.tbl_SubProducts on i.SubProductId equals sub.SubProductId into outerSub
                                  from sub in outerSub.DefaultIfEmpty()

                                  join uG in _db.tbl_Godown on i.GodownId equals uG.GodownId into outerGodown
                                  from uG in outerGodown.DefaultIfEmpty()

                                  join uC in _db.tbl_AdminUsers on i.CreatedBy equals uC.AdminUserId into outerCreated
                                  from uC in outerCreated.DefaultIfEmpty()

                                  join uM in _db.tbl_AdminUsers on i.UpdatedBy equals uM.AdminUserId into outerModified
                                  from uM in outerModified.DefaultIfEmpty()

                                  where i.ProductItemId == Id
                                  select new ProductItemVM
                                  {
                                      ProductItemId = i.ProductItemId,
                                      CategoryId = i.CategoryId,
                                      CategoryName = c.CategoryName,
                                      ProductId = i.ProductId,
                                      ProductName = p.ProductName,
                                      SubProductId = i.SubProductId,
                                      SubProductName = (sub != null ? sub.SubProductName : ""),
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
                                      HSNCode = i.HSNCode,
                                      ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                      GodownId = i.GodownId.HasValue ? i.GodownId.Value : 0,
                                      IsReturnableItem = i.IsReturnable.HasValue ? i.IsReturnable.Value : false,
                                      ItemType = i.ItemType.HasValue ? i.ItemType.Value : 1,
                                      PayAdvancePer = i.PayAdvancePer.HasValue ? i.PayAdvancePer.Value : 0,
                                      CreatedDate = i.CreatedDate,
                                      UpdatedDate = i.UpdatedDate,
                                      strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                      strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : ""),
                                      GodownName = (uG != null ? uG.GodownName : ""),

                                      //GalleryImagesList = _db.tbl_ProductItemImages.Where(x => x.ProductItemId == i.ProductItemId)
                                      //                          .Select(galry => new SelectListItem { Value = galry.ItemImage }).ToList()

                                  }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objProductItem);
        }

        public ActionResult GetUnitsData(int UnitTypeId,int ProductItemId = 0)
        {
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
            string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

            string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
            string[] sheetsqty = { "32", "28", "21", "24", "18" };
            var obj = _db.tbl_Units.Where(x => x.UnitId == UnitTypeId).FirstOrDefault();
            ViewBag.UntTyp = obj.UnitName;
            if (ProductItemId == 0)
            {                
                List<VariantItemVM> lstVarintss = new List<VariantItemVM>();
                if (obj != null)
                {
                    if (obj.UnitName.ToLower().Contains("killo"))
                    {
                        for (int j = 0; j < kgs.Length; j++)
                        {
                            VariantItemVM objVariant = new VariantItemVM();
                            objVariant.UnitQtyText = kgs[j];
                            objVariant.UnitQtys = kgsQty[j];
                            objVariant.PricePercentage = 0;
                            objVariant.VariantItemId = j + 1;
                            objVariant.IsActive = false;
                            lstVarintss.Add(objVariant);
                        }
                    }
                    else if (obj.UnitName.ToLower().Contains("litr"))
                    {
                        for (int j = 0; j < ltrs.Length; j++)
                        {
                            VariantItemVM objVariant = new VariantItemVM();
                            objVariant.UnitQtyText = ltrs[j];
                            objVariant.UnitQtys = ltrsQty[j];
                            objVariant.PricePercentage = 0;
                            objVariant.IsActive = false;
                            objVariant.VariantItemId = j + 1;
                            lstVarintss.Add(objVariant);
                        }
                    }
                    else if (obj.UnitName.ToLower().Contains("sheet"))
                    {
                        for (int j = 0; j < sheets.Length; j++)
                        {
                            VariantItemVM objVariant = new VariantItemVM();
                            objVariant.UnitQtyText = sheets[j];
                            objVariant.UnitQtys = sheetsqty[j];
                            objVariant.PricePercentage = 0;
                            objVariant.VariantItemId = j + 1;
                            objVariant.IsActive = false;
                            lstVarintss.Add(objVariant);
                        }
                    }
                }
                ViewData["lstVarintss"] = lstVarintss;
            }
            else
            {
                var lstItmVarints = _db.tbl_ItemVariant.Where(o => o.ProductItemId == ProductItemId).ToList();
                List<VariantItemVM> lstVarintss = new List<VariantItemVM>();
                if (lstItmVarints != null && lstItmVarints.Count() > 0 && (obj.UnitName.ToLower().Contains("killo") || obj.UnitName.ToLower().Contains("litr") || obj.UnitName.ToLower().Contains("sheet")))
                {
                    int cnt = 1;
                    foreach(var objvarint in lstItmVarints)
                    {
                        VariantItemVM objVariant = new VariantItemVM();
                        objVariant.UnitQtyText = objvarint.UnitQty;                        
                        objVariant.PricePercentage = objvarint.PricePecentage.Value;
                        objVariant.VariantItemId = cnt;
                        objVariant.IsActive = objvarint.IsActive.Value;
                        lstVarintss.Add(objVariant);
                        cnt = cnt + 1;
                    }
                 
                    ViewData["lstVarintss"] = lstVarintss;
                }
            }
          
            return PartialView("~/Areas/Admin/Views/ProductItem/_UnitPriceSet.cshtml");
        }

        public ActionResult RatingReviews(decimal Rating = -1)
        {
            List<RatingReviewVM> lstRatings = new List<RatingReviewVM>();
            lstRatings = (from p in _db.tbl_ReviewRating
                        join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                        join pr in _db.tbl_ProductItems on p.ProductItemId equals pr.ProductItemId  
                        join orddet in _db.tbl_OrderItemDetails on p.OrderDetailId equals orddet.OrderDetailId
                        where Rating == -1 || p.Rating == Rating
                        select new RatingReviewVM
                        {
                            OrderId = orddet.OrderId.Value,
                            ClientName = c.FirstName + " " + c.LastName,
                            MobileNo = c.MobileNo,
                            Ratings = p.Rating.Value,
                            Review = p.Review,
                            ItemName = pr.ItemName,
                            RatingReviewId = p.ReviewRatingId,
                            RatingDate = p.CreatedDate.Value                            
                        }).OrderByDescending(x => x.RatingDate).ToList();
            ViewBag.Ratings = Rating;
            return View(lstRatings);
        }
    }
}