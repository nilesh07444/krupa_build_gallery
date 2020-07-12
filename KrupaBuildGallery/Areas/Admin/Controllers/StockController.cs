using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class StockController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public StockController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Stock
        public ActionResult Index()
        {
            List<ItemStockVM> lstItemStockVM = new List<ItemStockVM>();

            try
            {
                lstItemStockVM = (from stk in _db.tbl_ItemStocks
                                  join i in _db.tbl_ProductItems on stk.ProductItemId equals i.ProductItemId
                                  join c in _db.tbl_Categories on stk.CategoryId equals c.CategoryId
                                  join p in _db.tbl_Products on stk.ProductId equals p.Product_Id
                                  join s in _db.tbl_SubProducts on stk.SubProductId equals s.SubProductId into outerJoinSubProduct
                                  from s in outerJoinSubProduct.DefaultIfEmpty()
                                  where !stk.IsDelete
                                  select new ItemStockVM
                                  {
                                      StockId = stk.StockId,
                                      ProductItemId = stk.ProductItemId,
                                      CategoryId = stk.CategoryId,
                                      ProductId = stk.ProductId,
                                      SubProductId = stk.SubProductId,
                                      ProductItemName = i.ItemName,
                                      CategoryName = c.CategoryName,
                                      ProductName = p.ProductName,
                                      SubProductName = s.SubProductName,
                                      Quantity = stk.Qty,
                                      IsActive = i.IsActive
                                  }).OrderByDescending(x => x.StockId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstItemStockVM);
        }

        public ActionResult Add()
        {
            ItemStockVM objItemStock = new ItemStockVM();

            objItemStock.CategoryList = GetCategoryList();
            objItemStock.ProductList = new List<SelectListItem>();
            objItemStock.SubProductList = new List<SelectListItem>();
            objItemStock.ProductItemList = new List<SelectListItem>();
            return View(objItemStock);
        }
        public ActionResult Edit(int id)
        {
            ItemStockVM objProductItemStock = new ItemStockVM();

            objProductItemStock = (from i in _db.tbl_ItemStocks
                                   join utm in _db.tbl_ProductItems on i.ProductItemId equals utm.ProductItemId
                                   where i.StockId == id
                                   select new ItemStockVM
                                   {
                                       StockId = i.StockId,
                                       ProductItemId = i.ProductItemId,
                                       CategoryId = i.CategoryId,
                                       ProductId = i.ProductId,
                                       SubProductId = i.SubProductId,
                                       Quantity = i.Qty,
                                       ItemType = utm.ItemType.HasValue ? utm.ItemType.Value : 1,
                                       IsActive = i.IsActive
                                   }).FirstOrDefault();

            objProductItemStock.CategoryList = GetCategoryList();
            objProductItemStock.ProductList = GetProductListByCategoryId(objProductItemStock.CategoryId);
            objProductItemStock.SubProductList = GetSubProductListByProductId(objProductItemStock.ProductId);
            objProductItemStock.ProductItemList = GetProductItems(objProductItemStock.ProductId, objProductItemStock.SubProductId);

            return View(objProductItemStock);
        }
        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

        public JsonResult GetItemList(long ProductId, long? SubProductId, int ItemType = 1)
        {
            var ProductItemList = _db.tbl_ProductItems.Where(x => x.ProductId == ProductId && (SubProductId == 0 || x.SubProductId == SubProductId) && x.IsActive && !x.IsDelete && (ItemType == -1 || ((ItemType == 1 && x.ItemType == null) || x.ItemType == ItemType)))
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.ProductItemId).Trim(), Text = o.ItemName })
                         .OrderBy(x => x.Text).ToList();

            return Json(ProductItemList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(ItemStockVM itemStockVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    if (itemStockVM.Quantity <= 0)
                    {
                        ModelState.AddModelError("Quantity", ErrorMessage.QtyGreater);
                        return View(itemStockVM);
                    }
                    else
                    {

                        long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                        tbl_ItemStocks objItemStock = new tbl_ItemStocks();
                        objItemStock.CategoryId = itemStockVM.CategoryId;
                        objItemStock.ProductId = itemStockVM.ProductId;
                        objItemStock.SubProductId = itemStockVM.SubProductId;
                        objItemStock.ProductItemId = itemStockVM.ProductItemId;
                        objItemStock.Qty = Convert.ToInt64(itemStockVM.Quantity);

                        objItemStock.IsActive = true;
                        objItemStock.IsDelete = false;
                        objItemStock.CreatedBy = LoggedInUserId;
                        objItemStock.CreatedDate = DateTime.UtcNow;
                        objItemStock.UpdatedBy = LoggedInUserId;
                        objItemStock.UpdatedDate = DateTime.UtcNow;
                        _db.tbl_ItemStocks.Add(objItemStock);

                        tbl_StockReport objstkreport = new tbl_StockReport();
                        objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                        objstkreport.StockDate = DateTime.UtcNow;
                        objstkreport.Qty = Convert.ToInt64(itemStockVM.Quantity);
                        objstkreport.IsCredit = true;
                        objstkreport.IsAdmin = true;
                        objstkreport.CreatedBy = LoggedInUserId;
                        objstkreport.ItemId = itemStockVM.ProductItemId;
                        objstkreport.Remarks = "Stock Added";
                        _db.tbl_StockReport.Add(objstkreport);
                        _db.SaveChanges();

                        return RedirectToAction("Index");

                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(itemStockVM);
        }

        [HttpPost]
        public ActionResult Edit(ItemStockVM itemStockVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    tbl_ItemStocks objItemStocks = _db.tbl_ItemStocks.Where(x => x.StockId == itemStockVM.StockId).FirstOrDefault();

                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objItemStocks.CategoryId = itemStockVM.CategoryId;
                    objItemStocks.ProductId = itemStockVM.ProductId;
                    objItemStocks.SubProductId = itemStockVM.SubProductId;
                    objItemStocks.ProductItemId = itemStockVM.ProductItemId;
                    objItemStocks.Qty = itemStockVM.Quantity;
                    objItemStocks.UpdatedBy = LoggedInUserId;
                    objItemStocks.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(itemStockVM);
        }


        [HttpPost]
        public string DeleteStock(long StockId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_ItemStocks objtbl_ItemStocks = _db.tbl_ItemStocks.Where(x => x.StockId == StockId && x.IsActive && !x.IsDelete).FirstOrDefault();

                if (objtbl_ItemStocks == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long Itmid = objtbl_ItemStocks.ProductItemId;
                    int soldQty = SoldItems(Itmid);
                    int stockQty = ItemStock(Itmid);
                    int remaining = stockQty - soldQty;
                    if (remaining < objtbl_ItemStocks.Qty)
                    {
                        ReturnMessage = "qtyless";
                    }
                    else
                    {
                        long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                        objtbl_ItemStocks.IsDelete = true;
                        objtbl_ItemStocks.UpdatedBy = LoggedInUserId;
                        objtbl_ItemStocks.UpdatedDate = DateTime.UtcNow;

                        _db.SaveChanges();
                        tbl_StockReport objstkreport = new tbl_StockReport();
                        objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                        objstkreport.StockDate = DateTime.UtcNow;
                        objstkreport.Qty = Convert.ToInt64(objtbl_ItemStocks.Qty);
                        objstkreport.IsCredit = false;
                        objstkreport.IsAdmin = true;
                        objstkreport.CreatedBy = LoggedInUserId;
                        objstkreport.ItemId = objtbl_ItemStocks.ProductItemId;
                        objstkreport.Remarks = "Stock Deleted";
                        _db.tbl_StockReport.Add(objstkreport);
                        _db.SaveChanges();
                        ReturnMessage = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
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

        public List<SelectListItem> GetProductItems(long ProductId, long? SubProductId)
        {
            var ProductItemList = _db.tbl_ProductItems.Where(x => x.ProductId == ProductId && (SubProductId == 0 || x.SubProductId == SubProductId) && x.IsActive && !x.IsDelete)
                        .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.ProductItemId).Trim(), Text = o.ItemName })
                        .OrderBy(x => x.Text).ToList();

            return ProductItemList;
        }

        public int ItemStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            if (TotalStock != null)
            {
                TotalStock = 0;
            }
            return Convert.ToInt32(TotalStock);
        }
        public int SoldItems(long ItemId)
        {
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.QtyUsed.Value);
            if (TotalSold != null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
        }

        public ActionResult View(int id)
        {
            ItemStockVM objProductItemStock = new ItemStockVM();

            objProductItemStock = (from i in _db.tbl_ItemStocks
                                   join c in _db.tbl_Categories on i.CategoryId equals c.CategoryId
                                   join p in _db.tbl_Products on i.ProductId equals p.Product_Id
                                   join s in _db.tbl_SubProducts on i.SubProductId equals s.SubProductId into outerJoinSub
                                   from s in outerJoinSub.DefaultIfEmpty()

                                   join itm in _db.tbl_ProductItems on i.ProductItemId equals itm.ProductItemId

                                   join uC in _db.tbl_AdminUsers on i.CreatedBy equals uC.AdminUserId into outerCreated
                                   from uC in outerCreated.DefaultIfEmpty()
                                   join uM in _db.tbl_AdminUsers on i.UpdatedBy equals uM.AdminUserId into outerModified
                                   from uM in outerModified.DefaultIfEmpty()

                                   where i.StockId == id
                                   select new ItemStockVM
                                   {
                                       StockId = i.StockId,
                                       ProductItemId = i.ProductItemId,
                                       CategoryId = i.CategoryId,
                                       ProductId = i.ProductId,
                                       SubProductId = i.SubProductId,
                                       Quantity = i.Qty,
                                       IsActive = i.IsActive,

                                       CategoryName = c.CategoryName,
                                       ProductName = p.ProductName,
                                       SubProductName = s.SubProductName,
                                       ProductItemName = itm.ItemName,

                                       CreatedDate = i.CreatedDate,
                                       UpdatedDate = i.UpdatedDate,
                                       strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                       strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                                   }).FirstOrDefault();

            return View(objProductItemStock);
        }

        public ActionResult StockReport()
        {
            ViewData["CategoryList"] = GetCategoryList();
            ViewData["ProductList"] = new List<SelectListItem>();
            ViewData["SubProductList"] = new List<SelectListItem>();
            return View();
        }

        public void ExportStockReport1(long ItemId, string StartDate, string EndDate)
        {
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            var objPrdItm = _db.tbl_ProductItems.Where(o => o.ProductItemId == ItemId).FirstOrDefault();
            List<tbl_StockReport> creditstock = _db.tbl_StockReport.Where(o => o.ItemId == ItemId && o.StockDate < dtStart && o.IsCredit == true).ToList();
            List<tbl_StockReport> debitstock = _db.tbl_StockReport.Where(o => o.ItemId == ItemId && o.StockDate < dtStart && o.IsCredit == false).ToList();
            decimal TotalCredit = 0;
            decimal TotalDebit = 0;
            TotalCredit = creditstock.Sum(x => x.Qty.HasValue ? x.Qty.Value : 0);
            TotalDebit = debitstock.Sum(x => x.Qty.HasValue ? x.Qty.Value : 0);
            decimal TotalOpening = TotalCredit - TotalDebit;
            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing" };
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td colspan='5'> Stock Report -" + objPrdItm.ItemName + " - " + StartDate + "to " + EndDate + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            //Looping through the column names
            foreach (var col in arrycolmns)
                sb.Append("<td>" + col + "</td>");
            sb.Append("</tr>");

            List<tbl_StockReport> lstStockss = _db.tbl_StockReport.Where(o => o.ItemId == ItemId && o.StockDate >= dtStart && o.StockDate <= dtEnd).OrderBy(x => x.StockDate).ToList();
            decimal OpStock = TotalOpening;
            if (lstStockss != null && lstStockss.Count() > 0)
            {
                foreach (var objj in lstStockss)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + objj.StockDate.Value.ToString("dd-MM-yyyy") + "</td>");
                    sb.Append("<td>" + OpStock + "</td>");
                    if (objj.IsCredit == true)
                    {
                        sb.Append("<td>" + objj.Qty.Value + "</td>");
                        OpStock = OpStock + objj.Qty.Value;
                    }
                    else
                    {
                        sb.Append("<td></td>");
                    }
                    if (objj.IsCredit == false)
                    {
                        sb.Append("<td>" + objj.Qty.Value + "</td>");
                        OpStock = OpStock - objj.Qty.Value;
                    }
                    else
                    {
                        sb.Append("<td></td>");
                    }
                    sb.Append("<td>" + OpStock + "</td>");
                    sb.Append("</tr>");
                }
            }
            //Looping through the records

            sb.Append("</table>");
            string reportname = "Stock Report -" + objPrdItm.ItemName + ".xls";
            //Writing StringBuilder content to an excel file.
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=" + reportname);
            Response.Write(sb.ToString());
            Response.Flush();
            Response.Close();
        }

        public void ExportStockReport(long ItemId, string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            var objPrdItm = _db.tbl_ProductItems.Where(o => o.ProductItemId == ItemId).FirstOrDefault();
            List<tbl_StockReport> creditstock = _db.tbl_StockReport.Where(o => o.ItemId == ItemId && o.StockDate < dtStart && o.IsCredit == true).ToList();
            List<tbl_StockReport> debitstock = _db.tbl_StockReport.Where(o => o.ItemId == ItemId && o.StockDate < dtStart && o.IsCredit == false).ToList();
            decimal TotalCredit = 0;
            decimal TotalDebit = 0;
            TotalCredit = creditstock.Sum(x => x.Qty.HasValue ? x.Qty.Value : 0);
            TotalDebit = debitstock.Sum(x => x.Qty.HasValue ? x.Qty.Value : 0);
            decimal TotalOpening = TotalCredit - TotalDebit;
            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing" };
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Stock Report -" + objPrdItm.ItemName + " - " + StartDate + " to " + EndDate;
            for (var col = 1; col < arrycolmns.Length + 1; col++)
            {
                workSheet.Cells[2, col].Style.Font.Bold = true;
                workSheet.Cells[2, col].Style.Font.Size = 12;
                workSheet.Cells[2, col].Value = arrycolmns[col - 1];
                workSheet.Cells[2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[2, col].AutoFitColumns(30, 70);
                workSheet.Cells[2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.WrapText = true;
            }
            //Looping through the column names
            foreach (var col in arrycolmns)
                sb.Append("<td>" + col + "</td>");
            sb.Append("</tr>");

            List<tbl_StockReport> lstStockss = _db.tbl_StockReport.Where(o => o.ItemId == ItemId && o.StockDate >= dtStart && o.StockDate <= dtEnd).OrderBy(x => x.StockDate).ToList();
            decimal OpStock = TotalOpening;
            int row1 = 1;
            if (lstStockss != null && lstStockss.Count() > 0)
            {
                foreach (var objj in lstStockss)
                {
                    workSheet.Cells[row1 + 2, 1].Style.Font.Bold = false;
                    workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                    workSheet.Cells[row1 + 2, 1].Value = objj.StockDate.Value.ToString("dd-MM-yyyy");
                    workSheet.Cells[row1 + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 1].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 1].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 2].Style.Font.Bold = false;
                    workSheet.Cells[row1 + 2, 2].Style.Font.Size = 12;
                    workSheet.Cells[row1 + 2, 2].Value = OpStock;
                    workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 3].Style.Font.Bold = false;
                    workSheet.Cells[row1 + 2, 3].Style.Font.Size = 12;
                    if (objj.IsCredit == true)
                    {
                        workSheet.Cells[row1 + 2, 3].Value = objj.Qty.Value;
                        OpStock = OpStock + objj.Qty.Value;
                    }
                    else
                    {
                        workSheet.Cells[row1 + 2, 3].Value = "";
                    }
                    workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 4].Style.Font.Bold = false;
                    workSheet.Cells[row1 + 2, 4].Style.Font.Size = 12;
                    if (objj.IsCredit == false)
                    {
                        workSheet.Cells[row1 + 2, 4].Value = objj.Qty.Value;
                        OpStock = OpStock - objj.Qty.Value;
                    }
                    else
                    {
                        workSheet.Cells[row1 + 2, 4].Value = "";
                    }
                    workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 5].Style.Font.Bold = false;
                    workSheet.Cells[row1 + 2, 5].Style.Font.Size = 12;
                    workSheet.Cells[row1 + 2, 5].Value = OpStock;
                    workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);
                    row1 = row1 + 1;
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Stock Report-" + objPrdItm.ItemName + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public void AddOldStock()
        {
           List<tbl_ItemStocks> lststks = _db.tbl_ItemStocks.Where(o => o.StockId <= 8).ToList();
            foreach(var objstkk in lststks)
            {
                tbl_StockReport objstkreport = new tbl_StockReport();
                objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                objstkreport.StockDate = objstkk.CreatedDate;
                objstkreport.Qty = Convert.ToInt64(objstkk.Qty);
                objstkreport.IsCredit = true;
                objstkreport.IsAdmin = true;
                objstkreport.CreatedBy = objstkk.CreatedBy;
                objstkreport.ItemId = objstkk.ProductItemId;
                objstkreport.Remarks = "Stock Added";
                _db.tbl_StockReport.Add(objstkreport);
                _db.SaveChanges();
            }
        }

        public void StockOrdered()
        {
            List<tbl_OrderItemDetails> lsttbl_OrderItemDetails = _db.tbl_OrderItemDetails.Where(o => o.IsDelete == false).ToList();
            foreach (var objstkk in lsttbl_OrderItemDetails)
            {
                tbl_StockReport objstkreport = new tbl_StockReport();
                objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                objstkreport.StockDate = objstkk.CreatedDate;
                objstkreport.Qty = Convert.ToInt64(objstkk.QtyUsed);
                objstkreport.IsCredit = false;
                objstkreport.IsAdmin = false;
                objstkreport.CreatedBy = objstkk.CreatedBy;
                objstkreport.ItemId = objstkk.ProductItemId;
                objstkreport.Remarks = "Item Ordered";
                _db.tbl_StockReport.Add(objstkreport);
                _db.SaveChanges();
            }
        }
    }
}