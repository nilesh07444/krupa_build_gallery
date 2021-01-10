using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public PurchaseController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Purchase
        public ActionResult Index()
        {
            List<PurchaseVM> lstPurchaseVM  = new List<PurchaseVM>();
            try
            {
                lstPurchaseVM =(from i in _db.tbl_Purchase                             
                              where i.IsDeleted == false
                              select new PurchaseVM
                              {
                                  PurchaseId = i.PurchaseId,
                                  BillNo = i.BillNo,
                                  DealerPartyCode = i.DealerPartyCode,
                                  DealerId = i.DealerId.Value,
                                  PurchaseDate = i.PurchaseDate,
                                  FinalBillAmount = i.FinalBillAmount.Value                                  
                              }).OrderByDescending(x => x.PurchaseDate).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstPurchaseVM);
        }

        public ActionResult Detail(long Id)
        {
            tbl_Purchase objPurchase = _db.tbl_Purchase.Where(o => o.PurchaseId == Id).FirstOrDefault();
            ViewData["objPurchase"] = objPurchase;

            List<PurchaseItemVM> lstPurchaseItemVM = (from cu in _db.tbl_PurchaseItems
                                                join i in _db.tbl_ProductItems on cu.ItemId equals i.ProductItemId
                                                join c in _db.tbl_Categories on cu.CategoryId equals c.CategoryId
                                                join p in _db.tbl_Products on cu.ProductId equals p.Product_Id
                                                join s in _db.tbl_SubProducts on cu.SubProductId equals s.SubProductId into outerJoinSubProduct
                                                from s in outerJoinSubProduct.DefaultIfEmpty()
                                                join v in _db.tbl_ItemVariant on cu.VariantId equals v.VariantItemId into outerJoinVartint
                                                from v in outerJoinVartint.DefaultIfEmpty()
                                                where cu.Fk_PurchaseId == Id
                                                select new PurchaseItemVM
                                                {
                                                    PurchaseItemId = cu.PurchaseItemId,
                                                    CategoryName = c.CategoryName,
                                                    ProductName = p.ProductName,
                                                    ItemName = i.ItemName,
                                                    SubProductName = s.SubProductName,
                                                    VariantName = v.UnitQty,
                                                    Qty = cu.Qty.Value,
                                                    Price = cu.Price.Value,
                                                    FinalPrice = cu.FinalPrice.Value,
                                                    LabourCharge = cu.LabourCharge.Value,
                                                    ExtraPlusMinus = cu.ExtraPlusMinus.Value,
                                                    Total = cu.Total.Value,
                                                    TradeDiscount = cu.TradeDiscount.Value,
                                                    CashDiscount = cu.CashDiscount.Value,
                                                    ExtraPlusMinus2 = cu.ExtraPlusMinus2.Value,
                                                    BillAmount = cu.BillAmount.Value,
                                                    IGST = cu.IGST.Value,
                                                    SGST = cu.SGST.Value,
                                                    CGST = cu.CGST.Value,
                                                    FinalAmount = cu.FinalAmount.Value
                                                }).OrderBy(x => x.PurchaseItemId).ToList();
            
            ViewData["lstPurchaseItems"] = lstPurchaseItemVM;           
            return View();
        }


        public ActionResult Add()
        {
            var DealerParty = _db.tbl_PurchaseDealers.OrderBy(x => x.FirmName).ThenBy(x => x.BussinessCode).ToList();
            ViewData["DealerParty"] = DealerParty;
            return View();
        }

        public ActionResult GetPurchaseItem(int Id)
        {
            ViewBag.Id = Id;
            ViewData["catrgories"] = GetCategoryList();
            return PartialView("~/Areas/Admin/Views/Purchase/_PurchaseItems.cshtml");
        }

        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

        public JsonResult GetVariantListByItemIdForPurchase(double Id)
        {
            List<VariantItemVM> lstVrntVM = GetVariantItmsForPurchase(Id);
            return Json(lstVrntVM, JsonRequestBehavior.AllowGet);
        }

        public List<VariantItemVM> GetVariantItmsForPurchase(double Id)
        {
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
            string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

            string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
            string[] sheetsqty = { "32", "28", "21", "24", "18" };
            var objProdItm = _db.tbl_ProductItems.Where(o => o.ProductItemId == Id).FirstOrDefault();
            var obj = _db.tbl_Units.Where(x => x.UnitId == objProdItm.UnitType).FirstOrDefault();
            ViewBag.UntTyp = obj.UnitName;
            var lstItmVarints = _db.tbl_ItemVariant.Where(o => o.ProductItemId == Id && (o.IsDeleted == null || o.IsDeleted == false)).ToList();
            List<VariantItemVM> lstVarintss = new List<VariantItemVM>();
            if (lstItmVarints != null && lstItmVarints.Count() > 0 && (obj.UnitName.ToLower().Contains("sheet") || obj.UnitName.ToLower().Contains("piece")))
            {
                int cnt = 1;
                foreach (var objvarint in lstItmVarints)
                {
                    VariantItemVM objVariant = new VariantItemVM();
                    objVariant.UnitQtyText = objvarint.UnitQty;
                    objVariant.PricePercentage = objvarint.PricePecentage.Value;
                    objVariant.VariantItemId = Convert.ToInt32(objvarint.VariantItemId);
                    objVariant.IsActive = objvarint.IsActive.Value;
                    objVariant.VariantImg = objvarint.VariantImage;
                    objVariant.MRPPrice = objvarint.MRPPrice.HasValue ? objvarint.MRPPrice.Value : 0;
                    objVariant.CustomerPrice = objvarint.CustomerPrice.HasValue ? objvarint.CustomerPrice.Value : 0;
                    objVariant.DistributorPrice = objvarint.DistributorPrice.HasValue ? objvarint.DistributorPrice.Value : 0;
                    objVariant.GST = Convert.ToString(objProdItm.GST_Per);
                    if (!obj.UnitName.ToLower().Contains("piece"))
                    {
                        if (Array.IndexOf(kgs, objVariant.UnitQtyText) >= 0)
                        {
                            int idxxx = Array.IndexOf(kgs, objVariant.UnitQtyText);
                            decimal qtt = Convert.ToDecimal(kgsQty[idxxx].ToString());
                            objVariant.UnitQtys = Convert.ToString(qtt);
                        }
                        else if (Array.IndexOf(ltrs, objVariant.UnitQtyText) >= 0)
                        {
                            int idxxx = Array.IndexOf(ltrs, objVariant.UnitQtyText);
                            decimal qtt = Convert.ToDecimal(ltrsQty[idxxx].ToString());
                            objVariant.UnitQtys = Convert.ToString(qtt);
                        }
                        else
                        {
                            objVariant.UnitQtys = "1";
                        }
                    }

                    lstVarintss.Add(objVariant);
                    cnt = cnt + 1;
                }
            }          
            else if(lstItmVarints != null && lstItmVarints.Count() > 0)
            {
                var objv = lstItmVarints.FirstOrDefault();
                VariantItemVM objVariant = new VariantItemVM();
                objVariant.VariantItemId = 0;
                objVariant.GST = Convert.ToString(objProdItm.GST_Per);
                if (obj.UnitName.ToLower().Contains("killo"))
                {
                    objVariant.UnitQtyText = "1 Kg";
                }
                else if(obj.UnitName.ToLower().Contains("litr"))
                {
                    objVariant.UnitQtyText = "1 Ltr";
                }
                else
                {
                    objVariant.UnitQtyText = objv.UnitQty;
                }
                lstVarintss.Add(objVariant);
            }
            return lstVarintss;
        }

        public string CheckBillNumber(string billnum,string dealerid,string date)
        {
            DateTime dt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(date))
            {
                 dt = DateTime.ParseExact(date, "dd/MM/yyyy", null);
            }
            
            int year = dt.Year;
            int toyear = year + 1;
            if (dt.Month <= 3)
            {
                year = year - 1;
                toyear = year;
            }
            string stryr = year + "-" + toyear;
            long delrid = Convert.ToInt64(dealerid);
            var objBill = _db.tbl_Purchase.Where(o => o.BillNo == billnum && o.DealerId == delrid && o.BillYear == stryr).FirstOrDefault();
            if(objBill != null)
            {
                return "BillNumberExist";
            }
            else
            {
                return "BillNumberNotExist";
            }
           
        }

        [ValidateInput(false)]
        [HttpPost]
        public string Add(FormCollection frm)
        {
            try
            {
                tbl_Purchase objPurch = new tbl_Purchase();
                string BillNum = frm["billnum"].ToString();
                string dealerid = frm["partydealer"].ToString();
                string purchasedate = frm["purchasedate"].ToString();
                DateTime dtPurchase = DateTime.ParseExact(purchasedate, "dd/MM/yyyy", null);
             
                int year = dtPurchase.Year;
                int toyear = year + 1;
                if (dtPurchase.Month <= 3)
                {
                    year = year - 1;
                    toyear = year;
                }
                DateTime dtfincialyear = new DateTime(year, 4, 1);
                DateTime dtendyear = new DateTime(toyear, 3, 31);
                var objPurchasetemp = _db.tbl_Purchase.Where(o => o.PurchaseDate >= dtfincialyear && o.PurchaseDate <= dtendyear).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                long Invno = 1;
                if (objPurchasetemp != null)
                {
                    if (objPurchasetemp.AutoBillId != null)
                    {
                        Invno = objPurchasetemp.AutoBillId.Value + 1;
                    }                   
                }
                string stryr = year + "-" + toyear;
                long DelerID = Convert.ToInt64(dealerid);
                var objPD = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == DelerID).FirstOrDefault();
                if(objPD != null)
                {
                    objPurch.DealerPartyCode = objPD.BussinessCode;
                }
                objPurch.DealerId = DelerID;                
                string TotlAmt = frm["txtTotalAmt"].ToString();
                objPurch.TotalBillAmt = Convert.ToDecimal(TotlAmt);
                
                string txtExtra1 = frm["txtExtra1"].ToString();
                if(!string.IsNullOrEmpty(txtExtra1))
                {
                    objPurch.PlusMinus1 = Convert.ToDecimal(txtExtra1);
                }
                else
                {
                    objPurch.PlusMinus1 = 0;
                }

                string txtInsu = frm["txtInsu"].ToString();
                objPurch.Insurance = 0;
                if (!string.IsNullOrEmpty(txtInsu))
                {
                    objPurch.Insurance = Convert.ToDecimal(txtInsu);
                }
               
                string txtTDS = frm["txtTDS"].ToString();
                if(!string.IsNullOrEmpty(txtTDS))
                {
                    objPurch.TDS = Convert.ToDecimal(txtTDS);
                }
                else
                {
                    objPurch.TDS = 0;
                }
                
                string txtExtra2 = frm["txtExtra2"].ToString();
                if (!string.IsNullOrEmpty(txtExtra2))
                {
                    objPurch.PlusMinus2 = Convert.ToDecimal(txtExtra2);
                }
                else
                {
                    objPurch.PlusMinus2 = 0;
                }
                string txtBillAmt = frm["txtBillAmt"].ToString();
                objPurch.FinalBillAmount = Convert.ToDecimal(txtBillAmt);
                if(!string.IsNullOrEmpty(BillNum))
                {
                    objPurch.BillNo = BillNum;
                }
                else
                {
                    objPurch.BillNo = "SSP/" + stryr + "/" + Invno;
                    objPurch.AutoBillId = Invno;
                }
                objPurch.BillYear = stryr;
                objPurch.PurchaseDate = dtPurchase;
                objPurch.CreatedDate = DateTime.UtcNow;
                objPurch.CreatedBy = clsAdminSession.UserID;
                objPurch.IsDeleted = false;
                objPurch.PaymentPaid = 0;
                objPurch.Remarks = frm["remarks"].ToString();
                _db.tbl_Purchase.Add(objPurch);
                _db.SaveChanges();
                if (frm["SubItemCategory"] != null)
                {
                    string[] arrySubItems = Request.Form.GetValues("SubItemProductItem");
                    string[] arrySubItemsCat = Request.Form.GetValues("SubItemCategory");
                    string[] arrySubItemsProd = Request.Form.GetValues("SubItemProduct");
                    string[] arrySubItemsSubProd = Request.Form.GetValues("SubItemSubProduct");
                    string[] arrySubItemsVariant = Request.Form.GetValues("SubItemVarints");
                    string[] arrySubItemsQty = Request.Form.GetValues("Qty");
                    string[] arryPrice = Request.Form.GetValues("Price");
                    string[] arryFP = Request.Form.GetValues("FP");
                    string[] arryLC = Request.Form.GetValues("LC");
                    string[] arryEXPM1 = Request.Form.GetValues("EXPM1");
                    string[] arryTotal = Request.Form.GetValues("Total");
                    string[] arryTD = Request.Form.GetValues("TD");
                    string[] arryCD = Request.Form.GetValues("CD");
                    string[] arryEXPM2 = Request.Form.GetValues("EXPM2");
                    string[] arryBA = Request.Form.GetValues("BA");
                    string[] arryIGST = Request.Form.GetValues("IGST");
                    string[] arrySGST = Request.Form.GetValues("SGST");
                    string[] arryCGST = Request.Form.GetValues("CGST");
                    string[] arryFamt = Request.Form.GetValues("Famt");
                    
                    for (int j = 0; j < arrySubItems.Length; j++)
                    {
                        tbl_PurchaseItems objItm = new tbl_PurchaseItems();
                        objItm.Fk_PurchaseId = objPurch.PurchaseId;
                        if (arrySubItems[j] != "" && arrySubItems[j] != "0" && arrySubItemsQty[j] != "" && arrySubItemsQty[j] != "0")
                        {
                            objItm.CategoryId = GetInt64Val(arrySubItemsCat[j]);
                            objItm.ProductId = GetInt64Val(arrySubItemsProd[j]);
                            objItm.SubProductId = GetInt64Val(arrySubItemsSubProd[j]);
                            objItm.VariantId = GetInt64Val(arrySubItemsVariant[j]);
                            objItm.ItemId = GetInt64Val(arrySubItems[j]);
                            objItm.Qty = GetInt64Val(arrySubItemsQty[j]);
                            objItm.Price = GetDecimlVal(arryPrice[j]);
                            objItm.FinalPrice = GetDecimlVal(arryFP[j]);
                            objItm.LabourCharge = GetDecimlVal(arryLC[j]);
                            objItm.ExtraPlusMinus = GetDecimlVal(arryEXPM1[j]);
                            objItm.Total = GetDecimlVal(arryTotal[j]);
                            objItm.TradeDiscount = GetDecimlVal(arryTD[j]);
                            objItm.CashDiscount = GetDecimlVal(arryCD[j]);
                            objItm.ExtraPlusMinus2 = GetDecimlVal(arryEXPM2[j]);
                            objItm.BillAmount = GetDecimlVal(arryBA[j]);
                            objItm.IGST = GetDecimlVal(arryIGST[j]);
                            objItm.SGST = GetDecimlVal(arrySGST[j]);
                            objItm.CGST = GetDecimlVal(arryCGST[j]);
                            objItm.FinalAmount = GetDecimlVal(arryFamt[j]);                            
                            _db.tbl_PurchaseItems.Add(objItm);
                            _db.SaveChanges();
                            tbl_ItemStocks objItemStock = new tbl_ItemStocks();
                            objItemStock.CategoryId = objItm.CategoryId.Value;
                            objItemStock.ProductId = objItm.ProductId.Value;
                            objItemStock.SubProductId = objItm.SubProductId;
                            objItemStock.VariantItemId = objItm.VariantId;
                            objItemStock.ProductItemId = objItm.ItemId.Value;
                            objItemStock.Qty = objItm.Qty.Value;
                            objItemStock.FakeStock = 0;
                            objItemStock.IsActive = true;
                            objItemStock.IsDelete = false;
                            objItemStock.CreatedBy = clsAdminSession.UserID;
                            objItemStock.CreatedDate = dtPurchase;
                            objItemStock.UpdatedBy = clsAdminSession.UserID;
                            objItemStock.UpdatedDate = dtPurchase;
                            objItemStock.PurchaseItemId = objItm.PurchaseItemId;
                            _db.tbl_ItemStocks.Add(objItemStock);

                            tbl_StockReport objstkreport = new tbl_StockReport();
                            objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                            objstkreport.StockDate = DateTime.UtcNow;
                            objstkreport.Qty = objItm.Qty.Value;
                            objstkreport.IsCredit = true;
                            objstkreport.VariantItemId = objItm.VariantId;
                            objstkreport.FakeStock = 0;
                            objstkreport.IsAdmin = true;
                            objstkreport.CreatedBy = clsAdminSession.UserID;
                            objstkreport.ItemId = objItm.ItemId;
                            objstkreport.PurchaseItemId = objItm.PurchaseItemId;
                            objstkreport.Remarks = "Stock Added";
                            _db.tbl_StockReport.Add(objstkreport);
                            _db.SaveChanges();
                            objItm.StockId = objItemStock.StockId;
                            objItm.StockReportId = objstkreport.Pk_StockReport_Id;
                            _db.SaveChanges();
                        }
                    }
                }
                return "Success";
            }
            catch (Exception e)
            {
                return "Fail " + e.Message.ToString();
            }

        }

        public long GetInt64Val(string vl)
        {
            long reurnvl = 0;
            if (!string.IsNullOrEmpty(vl))
            {
                reurnvl = Convert.ToInt64(vl);
            }
            return reurnvl;
        }

        public decimal GetDecimlVal(string vl)
        {
            decimal reurnvl = 0;
            if (!string.IsNullOrEmpty(vl))
            {
                reurnvl = Convert.ToDecimal(vl);
            }
            return reurnvl;
        }
    }

}