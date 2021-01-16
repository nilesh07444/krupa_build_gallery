using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
                    toyear = year;
                    year = year - 1;                    
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

                tbl_PurchasePayment objPurcPay = new tbl_PurchasePayment();
                objPurcPay.PurchaseId = objPurch.PurchaseId;
                objPurcPay.BillNumber = objPurch.BillNo;
                objPurcPay.PaymentDate = objPurch.PurchaseDate;
                objPurcPay.PaymentBy = "";
                objPurcPay.ChequeNo = "";
                objPurcPay.DealerId = Convert.ToInt64(dealerid);
                objPurcPay.ChequeBankName = "";              
                objPurcPay.Amount = objPurch.TotalBillAmt;
                decimal vatav = 0;
                objPurcPay.Vatav = vatav;
                objPurcPay.CreatedDate = DateTime.UtcNow;
                objPurcPay.CreatedBy = clsAdminSession.UserID;
                objPurcPay.IsDeleted = false;
                objPurcPay.IsDebit = false;                
                _db.tbl_PurchasePayment.Add(objPurcPay);
                _db.SaveChanges();
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


        public ActionResult Payments()
        {
            List<PurchasePaymentVM> lstPayVM = new List<PurchasePaymentVM>();
            try
            {
                lstPayVM = (from i in _db.tbl_PurchasePayment
                             join d in _db.tbl_PurchaseDealers on i.DealerId.Value equals d.Pk_Dealer_Id
                             join s in _db.tbl_Purchase on i.PurchaseId equals s.PurchaseId into outerJoinPay
                                 from s in outerJoinPay.DefaultIfEmpty()
                                 where i.IsDeleted == false && i.IsDebit == true                                  
                                 select new PurchasePaymentVM
                                 {
                                     PurchasePaymentId = i.PurchasePaymentId,
                                     PurchaseId = i.PurchaseId.Value,
                                     BillNumber = i.BillNumber,
                                     DealerId = i.DealerId.Value,
                                     PaymentDate = i.PaymentDate.Value,
                                     Amount = i.Amount.Value,
                                     PaymentBy = i.PaymentBy,
                                     DealerCode = d.BussinessCode
                                 }).OrderByDescending(x => x.PaymentDate).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstPayVM);
        }

        public ActionResult AddPayment()
        {
            var DealerParty = _db.tbl_PurchaseDealers.OrderBy(x => x.FirmName).ThenBy(x => x.BussinessCode).ToList();
            ViewData["DealerParty"] = DealerParty;

            return View();
        }


        [ValidateInput(false)]
        [HttpPost]
        public string AddPayment(FormCollection frm)
        {
            try
            {
                string BillNum = frm["purchasebills"].ToString();
                string dealerid = frm["partydealer"].ToString();
                string paymentdate = frm["paymentdate"].ToString();
                string paymentby = frm["paymentby"].ToString();
                DateTime dtpaymentdate = DateTime.ParseExact(paymentdate, "dd/MM/yyyy", null);
                long PurchaseId = 0;
                string BillNumber = "";
                if(!string.IsNullOrEmpty(BillNum))
                {
                    PurchaseId = Convert.ToInt64(BillNum);
                }
                tbl_Purchase objPurchasetemp = _db.tbl_Purchase.Where(o => o.PurchaseId == PurchaseId).FirstOrDefault();
                decimal Totlpaymentpaid = 0;
                decimal Totlvatav = 0;
                if (objPurchasetemp != null)
                {
                    BillNumber = objPurchasetemp.BillNo;
                    List<tbl_PurchasePayment> lstPym = _db.tbl_PurchasePayment.Where(o => o.PurchaseId == PurchaseId && o.IsDeleted == false && o.IsDebit == true).ToList();
                    if(lstPym != null && lstPym.Count() > 0)
                    {
                        Totlpaymentpaid = lstPym.Select(x => x.Amount.Value).Sum();
                        Totlvatav = lstPym.Select(x => x.Vatav.Value).Sum();
                    }
                }

                tbl_PurchasePayment objPurcPay = new tbl_PurchasePayment();
                objPurcPay.PurchaseId = PurchaseId;
                objPurcPay.BillNumber = BillNumber;
                objPurcPay.PaymentDate = dtpaymentdate;
                objPurcPay.PaymentBy = paymentby;
                objPurcPay.ChequeNo = "";
                objPurcPay.DealerId = Convert.ToInt64(dealerid);
                objPurcPay.ChequeBankName = "";
                if (paymentby == "Cheque")
                {
                    objPurcPay.ChequeNo = frm["cheqnum"].ToString();
                    string cheqdt = frm["chqdate"].ToString();
                    DateTime dtchkdate = DateTime.ParseExact(cheqdt, "dd/MM/yyyy", null);
                    objPurcPay.ChequeDate = dtchkdate;
                    objPurcPay.ChequeBankName = frm["cheqbank"].ToString();
                }
                else if(paymentby == "Bank Transfer")
                {
                    objPurcPay.AccountNumber = frm["accnum"].ToString();
                }
                objPurcPay.Amount = Convert.ToDecimal(frm["amount"].ToString());
                decimal vatav = 0;
                if(!string.IsNullOrEmpty(frm["vatav"].ToString()))
                {
                    vatav = Convert.ToDecimal(frm["vatav"].ToString());
                }
                objPurcPay.Vatav = vatav;
                objPurcPay.CreatedDate = DateTime.UtcNow;
                objPurcPay.CreatedBy = clsAdminSession.UserID;
                objPurcPay.IsDeleted = false;
                objPurcPay.IsDebit = true;
                objPurcPay.Remarks = frm["remarks"].ToString();              
                _db.tbl_PurchasePayment.Add(objPurcPay);
                _db.SaveChanges();
               if(objPurchasetemp != null)
                {
                    objPurchasetemp.PaymentPaid = Totlpaymentpaid + objPurcPay.Amount;
                    objPurchasetemp.TotalVatav = objPurcPay.Vatav + Totlvatav;
                    objPurchasetemp.TotalAmtPayment = Totlpaymentpaid + objPurcPay.Amount + objPurcPay.Vatav + Totlvatav;
                    _db.SaveChanges();
                }
                return "Success";
            }
            catch (Exception e)
            {
                return "Fail " + e.Message.ToString();
            }

        }

        public ActionResult PaymentDetail(long Id)
        {
            tbl_PurchasePayment objPurchasePayment = _db.tbl_PurchasePayment.Where(o => o.PurchasePaymentId == Id).FirstOrDefault();
            tbl_PurchaseDealers objDelr = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == objPurchasePayment.DealerId).FirstOrDefault();
            ViewData["objPurchasePayment"] = objPurchasePayment;
            ViewData["objDealer"] = objDelr;
            return View();
        }

        public JsonResult GetPurchaseBills(double DealerId)
        {
            decimal TotalAmtPaidWithoutBill = 0;
            List<tbl_Purchase> lstPurchases = _db.tbl_Purchase.Where(o => o.DealerId == DealerId).ToList();
            List<PurchaseVM> lstPrch = new List<PurchaseVM>();
            if(lstPurchases != null && lstPurchases.Count() > 0)
            {
                List<tbl_PurchasePayment> lstPurchasePayment = _db.tbl_PurchasePayment.Where(o => o.DealerId == DealerId && o.IsDebit == true).ToList();
                if(lstPurchasePayment != null && lstPurchasePayment.Count() > 0)
                {
                    TotalAmtPaidWithoutBill = lstPurchasePayment.Where(o => o.BillNumber == "").Select(x => x.Amount.Value).Sum();                    
                }
                foreach (var objPr in lstPurchases)
                {
                    decimal amtpaid = objPr.TotalAmtPayment.HasValue ? objPr.TotalAmtPayment.Value : 0;
                    decimal amtBill = objPr.FinalBillAmount.Value;
                    decimal AmtRemain = amtBill - amtpaid;
                    if (AmtRemain > 0)
                    {
                        PurchaseVM objP = new PurchaseVM();
                        objP.OutStandingAmt = AmtRemain;
                        objP.BillNo = objPr.BillNo;
                        objP.BillYear = objPr.BillYear;
                        objP.PurchaseId = objPr.PurchaseId;
                        objP.TotalAmtPaidWithoutBill = TotalAmtPaidWithoutBill;
                        lstPrch.Add(objP);
                    }
                }
            }
           
            return Json(lstPrch, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccNum(double DealerId)
        {
            List<string> accnum = new List<string>();
            var objdl = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == DealerId).FirstOrDefault();
            if(objdl != null)
            {
                if(!string.IsNullOrEmpty(objdl.BankAcNumber))
                {
                    accnum.Add(objdl.BankAcNumber);
                }
                if (!string.IsNullOrEmpty(objdl.BankAcNumber2))
                {
                    accnum.Add(objdl.BankAcNumber2);
                }
            }
            return Json(accnum, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string DeletePayment(long PaymentId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_PurchasePayment objPay = _db.tbl_PurchasePayment.Where(x => x.PurchasePaymentId == PaymentId).FirstOrDefault();
                if(objPay != null && objPay.PurchaseId > 0)
                {
                    tbl_Purchase objPurchasetemp = _db.tbl_Purchase.Where(o => o.PurchaseId == objPay.PurchaseId).FirstOrDefault();                
                    if (objPurchasetemp != null)
                    {
                        objPurchasetemp.PaymentPaid = objPurchasetemp.PaymentPaid - objPay.Amount;
                        objPurchasetemp.TotalVatav = objPurchasetemp.TotalVatav - objPay.Vatav;
                        objPurchasetemp.TotalAmtPayment = objPurchasetemp.PaymentPaid + objPurchasetemp.TotalVatav;
                    }
                }
                objPay.IsDeleted = true;
                objPay.ModfiedBy = clsAdminSession.UserID;
                objPay.ModifiedDate = DateTime.UtcNow;
                _db.SaveChanges();
                ReturnMessage = "success";

            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public JsonResult GetPurchaseBillsForReport(double DealerId,int billtype = -1)
        {
            decimal TotalAmtPaidWithoutBill = 0;
            List<tbl_Purchase> lstPurchases = _db.tbl_Purchase.Where(o => o.DealerId == DealerId).ToList();
            List<PurchaseVM> lstPrch = new List<PurchaseVM>();
            if (lstPurchases != null && lstPurchases.Count() > 0)
            {
                foreach (var objPr in lstPurchases)
                {
                    decimal amtpaid = objPr.TotalAmtPayment.HasValue ? objPr.TotalAmtPayment.Value : 0;
                    decimal amtBill = objPr.FinalBillAmount.Value;
                    decimal AmtRemain = amtBill - amtpaid;
                    if (billtype == -1 || (billtype == 0 && AmtRemain > 0))
                    {
                        PurchaseVM objP = new PurchaseVM();
                        objP.OutStandingAmt = AmtRemain;
                        objP.BillNo = objPr.BillNo;
                        objP.BillYear = objPr.BillYear;
                        objP.PurchaseId = objPr.PurchaseId;
                        objP.TotalAmtPaidWithoutBill = TotalAmtPaidWithoutBill;
                        lstPrch.Add(objP);
                    }
                    else if(billtype == -1 || (billtype == 1 && AmtRemain <= 0))
                    {
                        PurchaseVM objP = new PurchaseVM();
                        objP.OutStandingAmt = AmtRemain;
                        objP.BillNo = objPr.BillNo;
                        objP.BillYear = objPr.BillYear;
                        objP.PurchaseId = objPr.PurchaseId;
                        objP.TotalAmtPaidWithoutBill = TotalAmtPaidWithoutBill;
                        lstPrch.Add(objP);
                    }
                }
            }
            return Json(lstPrch, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PaymentReportByBill()
        {
            var DealerParty = _db.tbl_PurchaseDealers.OrderBy(x => x.FirmName).ThenBy(x => x.BussinessCode).ToList();
            ViewData["DealerParty"] = DealerParty;
            return View("~/Areas/Admin/Views/Purchase/PaymentReportBillwise.cshtml");
        }

        public void ExportPaymentReportBillWise(long DealerId, long PurchaseId, string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);

            List<tbl_PurchaseDealers> lstDealers = new List<tbl_PurchaseDealers>();
            List<PaymentReportBillVM> lstReports = new List<PaymentReportBillVM>();
            if (DealerId == -1)
            {
                lstDealers = _db.tbl_PurchaseDealers.OrderBy(x => x.BussinessCode).ToList();
            }
            else
            {
                lstDealers = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == DealerId).OrderBy(x => x.BussinessCode).ToList();
            }

            foreach (tbl_PurchaseDealers objDel in lstDealers)
            {
                PaymentReportBillVM objBil = new PaymentReportBillVM();
                objBil.DealerId = objDel.Pk_Dealer_Id;
                objBil.SupplierCode = objDel.BussinessCode;
                List<PurchaseVM> lstPurchases = new List<PurchaseVM>();
                List<long> purchseids = _db.tbl_PurchasePayment.Where(o => ((PurchaseId == 0 && o.PurchaseId > 0) || (o.PurchaseId == PurchaseId && o.PurchaseId > 0)) && o.PaymentDate >= dtStart && o.PaymentDate <= dtEnd && o.DealerId == objDel.Pk_Dealer_Id && o.IsDeleted == false && o.IsDebit == true).Select(o => o.PurchaseId.Value).Distinct().ToList();
                if (purchseids != null && purchseids.Count() > 0)
                {
                    foreach (long pid in purchseids)
                    {
                        PurchaseVM objPur = new PurchaseVM();
                        var objPurchse = _db.tbl_Purchase.Where(o => o.PurchaseId == pid).FirstOrDefault();
                        objPur.PurchaseId = pid;
                        objPur.BillNo = objPurchse.BillNo;
                        objPur.BillYear = objPurchse.BillYear;
                        objPur.lstPurchasePayments = _db.tbl_PurchasePayment.Where(o => o.PaymentDate >= dtStart && o.PurchaseId == pid && o.PaymentDate <= dtEnd && o.IsDeleted == false && o.IsDebit == true).OrderBy(x => x.PaymentDate.Value).ToList();
                        lstPurchases.Add(objPur);
                    }
                }
                if (lstPurchases.Count() > 0)
                {
                    objBil.lstPurchases = lstPurchases;
                    lstReports.Add(objBil);
                }
            }
            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "Supplier Code", "Bill Number (Year)", "Payment Date", "Amount", "Vatav", "Payment By", "Cheque Number", "Cheque Date", "Bank Name", "Account Number" };
            var workSheet = excel.Workbook.Worksheets.Add("Purchase Payment Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Purchase Payment Report: " + StartDate + " to " + EndDate;
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
            int row1 = 1;
            foreach (var obj in lstReports)
            {
                decimal TotalDateWise = 0;
                decimal TotalDateWiseQty = 0;
                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = true;
                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                workSheet.Cells[row1 + 2, 1].Value = obj.SupplierCode;
                workSheet.Cells[row1 + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Cells[row1 + 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[row1 + 2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.WrapText = true;
                workSheet.Cells[row1 + 2, 1].AutoFitColumns(30, 70);
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Merge = true;

                row1 = row1 + 1;
                if (obj.lstPurchases != null && obj.lstPurchases.Count() > 0)
                {
                    foreach (var ordrr in obj.lstPurchases)
                    {
                        string Bilno = ordrr.BillNo + " (" + ordrr.BillYear +")";
                        workSheet.Cells[row1 + 2, 2].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 2].Value = Bilno;
                        workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 2, row1 + 2, arrycolmns.Length].Merge = true;
                        workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                        row1 = row1 + 1;
                        if (ordrr.lstPurchasePayments != null && ordrr.lstPurchasePayments.Count() > 0)
                        {
                            foreach (var objItem in ordrr.lstPurchasePayments)
                            {

                                for (var col = 3; col < arrycolmns.Length + 1; col++)
                                {
                                    if (arrycolmns[col - 1] == "Payment Date")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.PaymentDate.Value.ToString("dd/MM/yyyy");
                                    }
                                    else if (arrycolmns[col - 1] == "Amount")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Amount;
                                    }
                                    else if (arrycolmns[col - 1] == "Vatav")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Vatav;
                                    }
                                    else if (arrycolmns[col - 1] == "Payment By")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.PaymentBy;
                                    }
                                    else if (arrycolmns[col - 1] == "Cheque Number")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.ChequeNo;
                                    }
                                    else if (arrycolmns[col - 1] == "Cheque Date")
                                    {
                                        if(objItem.PaymentBy == "Cheque")
                                        {
                                            workSheet.Cells[row1 + 2, col].Value = objItem.ChequeDate.Value.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            workSheet.Cells[row1 + 2, col].Value = "";
                                        }
                                      
                                    }
                                    else if (arrycolmns[col - 1] == "Bank Name")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.ChequeBankName;
                                    }
                                    else if (arrycolmns[col - 1] == "Account Number")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.AccountNumber;
                                    }
                                    workSheet.Cells[row1 + 2, col].Style.Font.Bold = false;
                                    workSheet.Cells[row1 + 2, col].Style.Font.Size = 12;
                                    workSheet.Cells[row1 + 2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    workSheet.Cells[row1 + 2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.WrapText = true;
                                    workSheet.Cells[row1 + 2, col].AutoFitColumns(30, 70);
                                }
                                row1 = row1 + 1;
                            }
                        }

                        row1 = row1 + 1;
                    }
                    row1 = row1 + 1;
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Purchase Payment Report Billwise.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult GetPaymentReportBillmWise(long DealerId,long PurchaseId,string StartDate, string EndDate)
        {
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);

            List<tbl_PurchaseDealers> lstDealers = new List<tbl_PurchaseDealers>();
            List<PaymentReportBillVM> lstReports = new List<PaymentReportBillVM>();
            if (DealerId == -1)
            {
                lstDealers = _db.tbl_PurchaseDealers.OrderBy(x => x.BussinessCode).ToList();
            }
            else
            {
                lstDealers = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == DealerId).OrderBy(x => x.BussinessCode).ToList();
            }

            foreach (tbl_PurchaseDealers objDel in lstDealers)
            {
                PaymentReportBillVM objBil = new PaymentReportBillVM();
                objBil.DealerId = objDel.Pk_Dealer_Id;
                objBil.SupplierCode = objDel.BussinessCode;
                List<PurchaseVM> lstPurchases = new List<PurchaseVM>();
                List<long> purchseids = _db.tbl_PurchasePayment.Where(o => ((PurchaseId == 0 && o.PurchaseId > 0) || (o.PurchaseId == PurchaseId && o.PurchaseId > 0)) && o.PaymentDate >= dtStart && o.PaymentDate <= dtEnd && o.DealerId == objDel.Pk_Dealer_Id && o.IsDeleted == false && o.IsDebit == true).Select(o => o.PurchaseId.Value).Distinct().ToList();
                if (purchseids != null && purchseids.Count() > 0)
                {
                    foreach(long pid in purchseids)
                    {
                        PurchaseVM objPur = new PurchaseVM();
                        var objPurchse = _db.tbl_Purchase.Where(o => o.PurchaseId == pid).FirstOrDefault();
                        objPur.PurchaseId = pid;
                        objPur.BillNo = objPurchse.BillNo;
                        objPur.BillYear = objPurchse.BillYear;
                        objPur.lstPurchasePayments = _db.tbl_PurchasePayment.Where(o => o.PaymentDate >= dtStart && o.PurchaseId == pid && o.PaymentDate <= dtEnd && o.IsDeleted == false && o.IsDebit == true).OrderBy(x => x.PaymentDate.Value).ToList();
                        lstPurchases.Add(objPur);
                    }
                }
                if(lstPurchases.Count() > 0)
                {
                    objBil.lstPurchases = lstPurchases;
                    lstReports.Add(objBil);
                }
            }
            return PartialView("~/Areas/Admin/Views/Purchase/_PaymentReportBillwise.cshtml", lstReports);
        }

        public ActionResult PurchaseReport()
        {
            var DealerParty = _db.tbl_PurchaseDealers.OrderBy(x => x.FirmName).ThenBy(x => x.BussinessCode).ToList();
            ViewData["DealerParty"] = DealerParty;
            return View("~/Areas/Admin/Views/Purchase/PurchaseReport.cshtml");
        }

        public void ExportPurchaseReport(long DealerId, long BillDisplay, string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);            

            List<tbl_PurchaseDealers> lstDealers = new List<tbl_PurchaseDealers>();
            List<PurchaseReportVM> lstReports = new List<PurchaseReportVM>();
            if (DealerId == -1)
            {
                lstDealers = _db.tbl_PurchaseDealers.OrderBy(x => x.BussinessCode).ToList();
            }
            else
            {
                lstDealers = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == DealerId).OrderBy(x => x.BussinessCode).ToList();
            }

            foreach (tbl_PurchaseDealers objDel in lstDealers)
            {
                PurchaseReportVM objPRort = new PurchaseReportVM();
                objPRort.DealerId = objDel.Pk_Dealer_Id;
                objPRort.SupplierCode = objDel.BussinessCode;
                List<tbl_Purchase> lstPurchases = _db.tbl_Purchase.Where(o => o.DealerId == objDel.Pk_Dealer_Id && o.PurchaseDate >= dtStart && o.PurchaseDate <= dtEnd && o.IsDeleted == false).OrderBy(x => x.PurchaseDate.Value).ToList();
                List<PurchaseVM> lstPrch = new List<PurchaseVM>();
                if (lstPurchases != null && lstPurchases.Count() > 0)
                {
                    foreach (var objPr in lstPurchases)
                    {
                        decimal amtpaid = objPr.TotalAmtPayment.HasValue ? objPr.TotalAmtPayment.Value : 0;
                        decimal amtBill = objPr.FinalBillAmount.Value;
                        decimal AmtRemain = amtBill - amtpaid;
                        if (BillDisplay == -1 || (BillDisplay == 0 && AmtRemain > 0))
                        {
                            PurchaseVM objP = new PurchaseVM();
                            objP.OutStandingAmt = AmtRemain;
                            objP.PurchaseDate = objPr.PurchaseDate.Value;
                            objP.BillNo = objPr.BillNo;
                            objP.BillYear = objPr.BillYear;
                            objP.FinalBillAmount = objPr.FinalBillAmount.Value;
                            objP.PurchaseId = objPr.PurchaseId;
                           
                            List<PurchaseItemVM> lstPurchaseItemVM = (from cu in _db.tbl_PurchaseItems
                                                                      join i in _db.tbl_ProductItems on cu.ItemId equals i.ProductItemId
                                                                      join c in _db.tbl_Categories on cu.CategoryId equals c.CategoryId
                                                                      join p in _db.tbl_Products on cu.ProductId equals p.Product_Id
                                                                      join s in _db.tbl_SubProducts on cu.SubProductId equals s.SubProductId into outerJoinSubProduct
                                                                      from s in outerJoinSubProduct.DefaultIfEmpty()
                                                                      join v in _db.tbl_ItemVariant on cu.VariantId equals v.VariantItemId into outerJoinVartint
                                                                      from v in outerJoinVartint.DefaultIfEmpty()
                                                                      where cu.Fk_PurchaseId == objPr.PurchaseId
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
                            objP.lstPurchaseItems = lstPurchaseItemVM;
                            lstPrch.Add(objP);
                        }
                        else if (BillDisplay == -1 || (BillDisplay == 1 && AmtRemain <= 0))
                        {
                            PurchaseVM objP = new PurchaseVM();
                            objP.OutStandingAmt = AmtRemain;
                            objP.PurchaseDate = objPr.PurchaseDate.Value;
                            objP.BillNo = objPr.BillNo;
                            objP.BillYear = objPr.BillYear;
                            objP.FinalBillAmount = objPr.FinalBillAmount.Value;
                            objP.PurchaseId = objPr.PurchaseId;
                            List<PurchaseItemVM> lstPurchaseItemVM = (from cu in _db.tbl_PurchaseItems
                                                                      join i in _db.tbl_ProductItems on cu.ItemId equals i.ProductItemId
                                                                      join c in _db.tbl_Categories on cu.CategoryId equals c.CategoryId
                                                                      join p in _db.tbl_Products on cu.ProductId equals p.Product_Id
                                                                      join s in _db.tbl_SubProducts on cu.SubProductId equals s.SubProductId into outerJoinSubProduct
                                                                      from s in outerJoinSubProduct.DefaultIfEmpty()
                                                                      join v in _db.tbl_ItemVariant on cu.VariantId equals v.VariantItemId into outerJoinVartint
                                                                      from v in outerJoinVartint.DefaultIfEmpty()
                                                                      where cu.Fk_PurchaseId == objPr.PurchaseId
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
                            objP.lstPurchaseItems = lstPurchaseItemVM;
                            lstPrch.Add(objP);
                        }
                    }
                }
                if (lstPrch.Count() > 0)
                {
                    objPRort.lstPurchases = lstPrch;
                    lstReports.Add(objPRort);
                }
            }
            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "Supplier Code", "Bill Number (Year)", "Purchase Date", "FinalBillAmount", "OutStandingAmt", "CategoryName", "ProductName", "ItemName", "VariantName", "Qty","Price","FinalPrice","LabourCharge", "ExtraPlusMinus","Total","TradeDiscount", "CashDiscount", "ExtraPlusMinus2", "BillAmount", "IGST","SGST", "CGST","FinalAmount"};
            var workSheet = excel.Workbook.Worksheets.Add("Purchase Payment Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Purchase Report: " + StartDate + " to " + EndDate;
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
            int row1 = 1;
            foreach (var obj in lstReports)
            {
                decimal TotalDateWise = 0;
                decimal TotalDateWiseQty = 0;
                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = true;
                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                workSheet.Cells[row1 + 2, 1].Value = obj.SupplierCode;
                workSheet.Cells[row1 + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Cells[row1 + 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[row1 + 2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.WrapText = true;
                workSheet.Cells[row1 + 2, 1].AutoFitColumns(30, 70);
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Merge = true;

                row1 = row1 + 1;
                if (obj.lstPurchases != null && obj.lstPurchases.Count() > 0)
                {
                    foreach (var ordrr in obj.lstPurchases)
                    {
                        string Bilno = ordrr.BillNo + " (" + ordrr.BillYear + ")";
                        workSheet.Cells[row1 + 2, 2].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 2].Value = Bilno;
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
                        workSheet.Cells[row1 + 2, 3].Value = ordrr.PurchaseDate.Value.ToString("dd/MM/yyyy");
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
                        workSheet.Cells[row1 + 2, 4].Value = ordrr.FinalBillAmount;
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
                        workSheet.Cells[row1 + 2, 5].Value = ordrr.OutStandingAmt;
                        workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 5, row1 + 2, arrycolmns.Length].Merge = true;
                        workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);

                        row1 = row1 + 1;
                        if (ordrr.lstPurchaseItems != null && ordrr.lstPurchaseItems.Count() > 0)
                        {
                            foreach (var objItem in ordrr.lstPurchaseItems)
                            {

                                for (var col = 6; col < arrycolmns.Length + 1; col++)
                                {
                                    if (arrycolmns[col - 1] == "CategoryName")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.CategoryName;
                                    }
                                    else if (arrycolmns[col - 1] == "ProductName")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.ProductName;
                                    }
                                    else if (arrycolmns[col - 1] == "ItemName")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.ItemName;
                                    }
                                    else if (arrycolmns[col - 1] == "VariantName")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.VariantName;
                                    }
                                    else if (arrycolmns[col - 1] == "Qty")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Qty;
                                    }
                                    else if (arrycolmns[col - 1] == "Price")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Price;
                                    }
                                    else if (arrycolmns[col - 1] == "FinalPrice")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.FinalPrice;
                                    }
                                    else if (arrycolmns[col - 1] == "LabourCharge")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.LabourCharge;
                                    }
                                    else if (arrycolmns[col - 1] == "ExtraPlusMinus")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.ExtraPlusMinus;
                                    }
                                    else if (arrycolmns[col - 1] == "Total")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Total;
                                    }
                                    else if (arrycolmns[col - 1] == "TradeDiscount")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.TradeDiscount +"%";
                                    }
                                    else if (arrycolmns[col - 1] == "CashDiscount")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.CashDiscount + "%";
                                    }
                                    else if (arrycolmns[col - 1] == "ExtraPlusMinus2")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.ExtraPlusMinus2;
                                    }
                                    else if (arrycolmns[col - 1] == "BillAmount")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.BillAmount;
                                    }
                                    else if (arrycolmns[col - 1] == "IGST")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.IGST;
                                    }
                                    else if (arrycolmns[col - 1] == "SGST")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.SGST;
                                    }
                                    else if (arrycolmns[col - 1] == "CGST")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.CGST;
                                    }
                                    else if (arrycolmns[col - 1] == "FinalAmount")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.FinalAmount;
                                    }

                                    workSheet.Cells[row1 + 2, col].Style.Font.Bold = false;
                                    workSheet.Cells[row1 + 2, col].Style.Font.Size = 12;
                                    workSheet.Cells[row1 + 2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    workSheet.Cells[row1 + 2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.WrapText = true;
                                    workSheet.Cells[row1 + 2, col].AutoFitColumns(30, 70);
                                }
                                row1 = row1 + 1;
                            }
                        }

                        row1 = row1 + 1;
                    }
                    row1 = row1 + 1;
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Purchase Payment Report Billwise.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult GeneralPurchasePaymentReport()
        {
            var DealerParty = _db.tbl_PurchaseDealers.OrderBy(x => x.FirmName).ThenBy(x => x.BussinessCode).ToList();
            ViewData["DealerParty"] = DealerParty;
            return View("~/Areas/Admin/Views/Purchase/GeneralPurchasePaymentReport.cshtml");
        }

        public ActionResult GetGeneralPaymentReport(long DealerId,string StartDate, string EndDate)
        {
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);
            List<ReportVM> lstReportVm = new List<ReportVM>();
      
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            var lstordrss = _db.tbl_Orders.ToList();
            string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing", "PaymentMethod", "Remarks", "BillNo." };
            if (DealerId != -1)
            {
                List<tbl_PurchasePayment> lstCrdt = _db.tbl_PurchasePayment.Where(o => o.DealerId == DealerId && o.PaymentDate < dtStart && o.IsDebit == false).ToList();
                List<tbl_PurchasePayment> lstDebt = _db.tbl_PurchasePayment.Where(o => o.DealerId == DealerId && o.PaymentDate < dtStart && o.IsDebit == true).ToList();
                decimal TotalCredit = 0;
                decimal TotalDebit = 0;
                TotalCredit = lstCrdt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                TotalDebit = lstDebt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                decimal TotalVatavCredit = lstCrdt.Sum(x => x.Vatav.HasValue ? x.Vatav.Value : 0);
                decimal TotalVatavDebit = lstDebt.Sum(x => x.Vatav.HasValue ? x.Vatav.Value : 0);
                TotalCredit = TotalCredit + TotalVatavCredit;
                TotalDebit = TotalDebit + TotalVatavDebit;
                decimal TotalOpening = TotalCredit - TotalDebit;
                List<tbl_PurchasePayment> lstAllTransaction = _db.tbl_PurchasePayment.Where(o => o.DealerId == DealerId && o.PaymentDate >= dtStart && o.PaymentDate <= dtEnd).OrderBy(x => x.PaymentDate.Value).ToList();
                int row1 = 1;
                if (lstAllTransaction != null && lstAllTransaction.Count() > 0)
                {
                    foreach (var objTrn in lstAllTransaction)
                    {
                        //double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(objTrn.Amount));
                        //objTrn.Amount = Convert.ToDecimal(Convert.ToDouble(objTrn.Amount));
                        ReportVM objrp = new ReportVM();
                        objrp.Date = objTrn.PaymentDate.Value.ToString("dd-MM-yyyy");
                        objrp.Opening = TotalOpening.ToString();
                        if (objTrn.IsDebit == false)
                        {
                            objrp.Credit = objTrn.Amount.Value.ToString();
                            TotalOpening = TotalOpening + objTrn.Amount.Value;
                        }
                        else
                        {
                            objrp.Credit = "";
                        }


                        if (objTrn.IsDebit == true)
                        {
                            objrp.Debit = objTrn.Amount.Value + objTrn.Vatav.Value + "";
                            TotalOpening = TotalOpening - Convert.ToDecimal(objrp.Debit);
                        }
                        else
                        {
                            objrp.Debit = "";
                        }

                        objrp.Closing = TotalOpening.ToString();
                        objrp.PaymentMethod = objTrn.PaymentBy;
                        objrp.Remarks = objTrn.Remarks;
                        objrp.InvoiceNo = objTrn.BillNumber;
                        lstReportVm.Add(objrp);
                        row1 = row1 + 1;
                    }
                }
            }
            else
            {

                List<tbl_PurchasePayment> lstCrdt = _db.tbl_PurchasePayment.Where(o => o.PaymentDate < dtStart && o.IsDebit == false).ToList();
                List<tbl_PurchasePayment> lstDebt = _db.tbl_PurchasePayment.Where(o => o.PaymentDate < dtStart && o.IsDebit == true).ToList();
                decimal TotalCredit = 0;
                decimal TotalDebit = 0;
                TotalCredit = lstCrdt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                TotalDebit = lstDebt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                decimal TotalVatavCredit = lstCrdt.Sum(x => x.Vatav.HasValue ? x.Vatav.Value : 0);
                decimal TotalVatavDebit = lstDebt.Sum(x => x.Vatav.HasValue ? x.Vatav.Value : 0);
                TotalCredit = TotalCredit + TotalVatavCredit;
                TotalDebit = TotalDebit + TotalVatavDebit;
                decimal TotalOpening = TotalCredit - TotalDebit;
                List<tbl_PurchasePayment> lstAllTransaction = _db.tbl_PurchasePayment.Where(o => o.PaymentDate >= dtStart && o.PaymentDate <= dtEnd).OrderBy(x => x.PaymentDate.Value).ToList();
                int row1 = 1;
                if (lstAllTransaction != null && lstAllTransaction.Count() > 0)
                {
                    foreach (var objTrn in lstAllTransaction)
                    {
                        //double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(objTrn.Amount));
                        //objTrn.Amount = Convert.ToDecimal(Convert.ToDouble(objTrn.Amount));
                        ReportVM objrp = new ReportVM();
                        objrp.Date = objTrn.PaymentDate.Value.ToString("dd-MM-yyyy");
                        objrp.Opening = TotalOpening.ToString();
                        if (objTrn.IsDebit == false)
                        {
                            objrp.Credit = objTrn.Amount.Value.ToString();
                            TotalOpening = TotalOpening + objTrn.Amount.Value;
                        }
                        else
                        {
                            objrp.Credit = "";
                        }


                        if (objTrn.IsDebit == true)
                        {
                            objrp.Debit = objTrn.Amount.Value + objTrn.Vatav.Value+"";
                            TotalOpening = TotalOpening - Convert.ToDecimal(objrp.Debit);
                        }
                        else
                        {
                            objrp.Debit = "";
                        }

                        objrp.Closing = TotalOpening.ToString();
                        objrp.PaymentMethod = objTrn.PaymentBy;
                        objrp.Remarks = objTrn.Remarks;                      
                        objrp.InvoiceNo = objTrn.BillNumber;
                        lstReportVm.Add(objrp);
                        row1 = row1 + 1;
                    }
                }
            }
            return PartialView("~/Areas/Admin/Views/Purchase/_GeneralPurchasePaymentReport.cshtml", lstReportVm);
        }

        public ActionResult GetAndAssignBillForPayment(long DealerId)
        {        
            decimal TotalAmtPaidWithoutBill = 0;
            List<tbl_Purchase> lstPurchases = _db.tbl_Purchase.Where(o => o.DealerId == DealerId).ToList();
            List<PurchaseVM> lstPrch = new List<PurchaseVM>();
            if (lstPurchases != null && lstPurchases.Count() > 0)
            {
                List<tbl_PurchasePayment> lstPurchasePayment = _db.tbl_PurchasePayment.Where(o => o.DealerId == DealerId && o.IsDebit == true).ToList();
                if (lstPurchasePayment != null && lstPurchasePayment.Count() > 0)
                {
                    TotalAmtPaidWithoutBill = lstPurchasePayment.Where(o => o.BillNumber == "").Select(x => x.Amount.Value).Sum();
                }
                foreach (var objPr in lstPurchases)
                {
                    decimal amtpaid = objPr.TotalAmtPayment.HasValue ? objPr.TotalAmtPayment.Value : 0;
                    decimal amtBill = objPr.FinalBillAmount.Value;
                    decimal AmtRemain = amtBill - amtpaid;
                    if (AmtRemain > 0)
                    {
                        PurchaseVM objP = new PurchaseVM();
                        objP.OutStandingAmt = AmtRemain;
                        objP.BillNo = objPr.BillNo;
                        objP.BillYear = objPr.BillYear;
                        objP.PurchaseId = objPr.PurchaseId;
                        objP.TotalAmtPaidWithoutBill = TotalAmtPaidWithoutBill;
                        lstPrch.Add(objP);
                    }
                }
            }
            ViewData["lstPrch"] = lstPrch;
            return PartialView("~/Areas/Admin/Views/Purchase/_AssignBillForPayment.cshtml");
        }

        [HttpPost]
        public string AssignBillToPayment(long PaymentId,long PurchaseId)
        {
            clsCommon objCommon = new clsCommon();
            var objPur = _db.tbl_Purchase.Where(o => o.PurchaseId == PurchaseId).FirstOrDefault();
            tbl_PurchasePayment objPay = _db.tbl_PurchasePayment.Where(o => o.PurchasePaymentId == PaymentId).FirstOrDefault();
            objPay.PurchaseId = PurchaseId;
            objPay.BillNumber = objPur.BillNo;
            _db.SaveChanges();
            return "Success";
        }

    }

}