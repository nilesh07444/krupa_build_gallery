using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    [CustomClientAuthorize]
    public class MyProfileController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public MyProfileController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/MyProfile
        public ActionResult Index()
        {
            ViewData["clientdetails"] = new tbl_ClientUsers();
            ViewData["clientotherdetails"] = new tbl_ClientOtherDetails();
            ViewBag.OrderCount = 0;
            ViewBag.WalletAmt = 0;
            ViewBag.CreditBls = 0;
            ViewBag.PointsRemaining = 0;
            if (clsClientSession.UserID > 0)
            {
                long userid = clsClientSession.UserID;
                ViewBag.OrderCount = _db.tbl_Orders.Where(o => o.ClientUserId == userid).ToList().Count;
                tbl_ClientUsers objClientUser = _db.tbl_ClientUsers.Where(o => o.ClientUserId == userid).FirstOrDefault();
                tbl_ClientOtherDetails objClientOtherdetails =_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == userid).FirstOrDefault();
                decimal waltamt = objClientUser.WalletAmt.HasValue ? objClientUser.WalletAmt.Value : 0;
                decimal credit = objClientOtherdetails.CreditLimitAmt.HasValue ? objClientOtherdetails.CreditLimitAmt.Value : 0;
                if(credit > 0)
                {
                    decimal amtdue = objClientOtherdetails.AmountDue.HasValue ? objClientOtherdetails.AmountDue.Value : 0;
                    ViewBag.CreditBls = credit - amtdue;
                }
                DateTime dtNow = DateTime.UtcNow;
              
                List<tbl_PointDetails> lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == userid && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();
                decimal pointreamining = 0;
                if (lstpoints != null && lstpoints.Count() > 0)
                {
                    pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
                }
                ViewBag.PointsRemaining = pointreamining;
                ViewBag.WalletAmt = waltamt;               
                ViewData["clientdetails"] = objClientUser;
                ViewData["clientotherdetails"] = objClientOtherdetails;
            }
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();
                    Random random = new Random();
                    int num = random.Next(310450,789899);
                    //string msg = "Your change password OTP code is " + num;
                    int SmsId = (int)SMSType.ChangePwdOtp;
                    clsCommon objcm = new clsCommon();
                    string msg = objcm.GetSmsContent(SmsId);
                    msg = msg.Replace("{{OTP}}", num + "");
                    msg = HttpUtility.UrlEncode(msg);
                    //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", MobileNumber).Replace("--MSG--", msg);
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return num.ToString();
                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public string ChangePasswordSubmit(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string CurrentPassword = frm["currentpwd"];
                string NewPassword = frm["newpwd"];

                long LoggedInUserId = Int64.Parse(clsClientSession.UserID.ToString());
                tbl_ClientUsers objUser = _db.tbl_ClientUsers.Where(x => x.ClientUserId == LoggedInUserId).FirstOrDefault();

                if (objUser != null)
                {
                    string EncryptedCurrentPassword = clsCommon.EncryptString(CurrentPassword); 
                    if (objUser.Password == EncryptedCurrentPassword)
                    {
                        objUser.Password = clsCommon.EncryptString(NewPassword);
                        _db.SaveChanges();

                        ReturnMessage = "SUCCESS";
                        Session.Clear();
                    }
                    else
                    {
                        ReturnMessage = "CP_NOT_MATCHED";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public ActionResult SaveProfile(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string fname = Convert.ToString(frm["firstname"]);
                string lname = Convert.ToString(frm["lastname"]);
                string mobile = Convert.ToString(frm["mobilenumber"]);
                string altmobile = Convert.ToString(frm["alternatemobile"]);
                string email = Convert.ToString(frm["email"]);
                string prefix = Convert.ToString(frm["prefix"]);
                string shipfnam = Convert.ToString(frm["shipfirstname"]);
                string shiplname = Convert.ToString(frm["shiplastname"]);
                string shipmobile = Convert.ToString(frm["shipmobilenumber"]);
                string shipaddress = Convert.ToString(frm["shipaddress"]);
                string shipcity = Convert.ToString(frm["shipcity"]);
                string shippincode = Convert.ToString(frm["shippincode"]);
                string shipstate = Convert.ToString(frm["shipstate"]);
                
                long LoggedInUserId = Int64.Parse(clsClientSession.UserID.ToString());
                tbl_ClientUsers objUser = _db.tbl_ClientUsers.Where(x => x.ClientUserId == LoggedInUserId).FirstOrDefault();
                tbl_ClientOtherDetails objotherdetails = _db.tbl_ClientOtherDetails.Where(x => x.ClientUserId == LoggedInUserId).FirstOrDefault();
                objUser.FirstName = fname;
                objUser.LastName = lname;
                objUser.Email = email;
                objUser.AlternateMobileNo = altmobile;
                objUser.Prefix = prefix;
                objotherdetails.ShipFirstName = shipfnam;
                objotherdetails.ShipLastName = shiplname;
                objotherdetails.ShipPhoneNumber = shipmobile;
                objotherdetails.ShipPostalcode = shippincode;
                objotherdetails.ShipState = shipstate;
                objotherdetails.ShipCity = shipcity;
                objotherdetails.ShipAddress = shipaddress;
                if(clsClientSession.RoleID == 2)
                {
                    string addharcard = Convert.ToString(frm["addharcardno"]);
                    string companyname = Convert.ToString(frm["companyname"]);
                    string gstno = Convert.ToString(frm["gstno"]);
                    string panno = Convert.ToString(frm["panno"]);
                    objotherdetails.Addharcardno = addharcard;
                    objUser.CompanyName = companyname;
                    objotherdetails.GSTno = gstno;
                    objotherdetails.Pancardno = panno;
                }

                _db.SaveChanges();
                clsClientSession.Prefix = prefix;
                clsClientSession.FirstName = fname;
                clsClientSession.LastName = lname;
                clsClientSession.Email = email;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return RedirectToAction("Index");
        }

        public ActionResult PaymentReport()
        {
            return View();
        }
        public void ExportPaymentReport(string StartDate, string EndDate, string PaymentMode)
        {
            ExcelPackage excel = new ExcelPackage();
            if (PaymentMode == "OnlinePayment")
            {
                PaymentMode = "Online Payment";
            }
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing", "PaymentMethod", "Remarks","InvoiceNo." };
         
            lstClients = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clsClientSession.UserID).ToList();
            if (lstClients != null && lstClients.Count() > 0)
            {
                foreach (var client in lstClients)
                {
                    string strRol = "Distributor";
                    if (client.ClientRoleId == 1)
                    {
                        strRol = "Customer";
                    }
                    var workSheet = excel.Workbook.Worksheets.Add(strRol + " - Report");
                    workSheet.Cells[1, 1].Style.Font.Bold = true;
                    workSheet.Cells[1, 1].Style.Font.Size = 20;
                    workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Cells[1, 1].Value = "Payment Report: " + client.FirstName + " " + client.LastName + " - " + StartDate + " to " + EndDate;
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

                    var lstordes = _db.tbl_Orders.Where(o => o.ClientUserId == client.ClientUserId).ToList();
                    List<long> orderIds = new List<long>();
                    if (lstordes != null && lstordes.Count() > 0)
                    {
                        orderIds = lstordes.Select(o => o.OrderId).ToList();
                        List<tbl_PaymentTransaction> lstCrdt = _db.tbl_PaymentTransaction.Where(o => orderIds.Contains(o.OrderId) && o.TransactionDate < dtStart && o.IsCredit == true && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                        List<tbl_PaymentTransaction> lstDebt = _db.tbl_PaymentTransaction.Where(o => orderIds.Contains(o.OrderId) && o.TransactionDate < dtStart && o.IsCredit == false && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                        decimal TotalCredit = 0;
                        decimal TotalDebit = 0;
                        TotalCredit = lstCrdt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                        TotalDebit = lstDebt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                        decimal TotalOpening = TotalCredit - TotalDebit;
                        List<tbl_PaymentTransaction> lstAllTransaction = _db.tbl_PaymentTransaction.Where(o => orderIds.Contains(o.OrderId) && o.TransactionDate >= dtStart && o.TransactionDate <= dtEnd && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                        int row1 = 1;
                        if (lstAllTransaction != null && lstAllTransaction.Count() > 0)
                        {
                            foreach (var objTrn in lstAllTransaction)
                            {
                                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = false;
                                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                                workSheet.Cells[row1 + 2, 1].Value = objTrn.TransactionDate.Value.ToString("dd-MM-yyyy");
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
                                workSheet.Cells[row1 + 2, 2].Value = TotalOpening;
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
                                if (objTrn.IsCredit == false)
                                {
                                    workSheet.Cells[row1 + 2, 3].Value = objTrn.Amount.Value;
                                    TotalOpening = TotalOpening + objTrn.Amount.Value;
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
                                if (objTrn.IsCredit == true)
                                {
                                    workSheet.Cells[row1 + 2, 4].Value = objTrn.Amount.Value;
                                    TotalOpening = TotalOpening - objTrn.Amount.Value;
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
                                workSheet.Cells[row1 + 2, 5].Value = TotalOpening;
                                workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                                workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);

                                workSheet.Cells[row1 + 2, 6].Style.Font.Bold = false;
                                workSheet.Cells[row1 + 2, 6].Style.Font.Size = 12;
                                workSheet.Cells[row1 + 2, 6].Value = objTrn.ModeOfPayment;
                                workSheet.Cells[row1 + 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                workSheet.Cells[row1 + 2, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[row1 + 2, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 6].Style.WrapText = true;
                                workSheet.Cells[row1 + 2, 6].AutoFitColumns(30, 70);

                                workSheet.Cells[row1 + 2, 7].Style.Font.Bold = false;
                                workSheet.Cells[row1 + 2, 7].Style.Font.Size = 12;
                                workSheet.Cells[row1 + 2, 7].Value = objTrn.Remarks;
                                workSheet.Cells[row1 + 2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                workSheet.Cells[row1 + 2, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[row1 + 2, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 7].Style.WrapText = true;
                                workSheet.Cells[row1 + 2, 7].AutoFitColumns(30, 70);

                                var objj = lstordes.Where(o => o.OrderId == objTrn.OrderId).FirstOrDefault();
                                string invno = "";
                                if (objj != null)
                                {
                                    invno = "S&S/" + objj.InvoiceYear + "/" + objj.InvoiceNo;
                                }
                                workSheet.Cells[row1 + 2, 8].Style.Font.Bold = false;
                                workSheet.Cells[row1 + 2, 8].Style.Font.Size = 12;
                                workSheet.Cells[row1 + 2, 8].Value = invno;
                                workSheet.Cells[row1 + 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                workSheet.Cells[row1 + 2, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                workSheet.Cells[row1 + 2, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                workSheet.Cells[row1 + 2, 8].Style.WrapText = true;
                                workSheet.Cells[row1 + 2, 8].AutoFitColumns(30, 70);

                                row1 = row1 + 1;
                            }
                        }
                    }

                }
            }
         

            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=PaymentReport.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult GetPaymentReport(string StartDate, string EndDate,string PaymentMode)
        {
            List<ReportVM> lstReportVm = new List<ReportVM>();
            ExcelPackage excel = new ExcelPackage();
            if (PaymentMode == "OnlinePayment")
            {
                PaymentMode = "Online Payment";
            }
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing", "PaymentMethod", "Remarks","InvoiceNo" };

            lstClients = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clsClientSession.UserID).ToList();
            if (lstClients != null && lstClients.Count() > 0)
            {
                foreach (var client in lstClients)
                {
                    string strRol = "Distributor";
                    if (client.ClientRoleId == 1)
                    {
                        strRol = "Customer";
                    }
                   
                    var lstordes = _db.tbl_Orders.Where(o => o.ClientUserId == client.ClientUserId).ToList();
                    List<long> orderIds = new List<long>();
                    if (lstordes != null && lstordes.Count() > 0)
                    {
                        orderIds = lstordes.Select(o => o.OrderId).ToList();
                        List<tbl_PaymentTransaction> lstCrdt = _db.tbl_PaymentTransaction.Where(o => orderIds.Contains(o.OrderId) && o.TransactionDate < dtStart && o.IsCredit == true && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                        List<tbl_PaymentTransaction> lstDebt = _db.tbl_PaymentTransaction.Where(o => orderIds.Contains(o.OrderId) && o.TransactionDate < dtStart && o.IsCredit == false && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                        decimal TotalCredit = 0;
                        decimal TotalDebit = 0;
                        TotalCredit = lstCrdt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                        TotalDebit = lstDebt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                        decimal TotalOpening = TotalCredit - TotalDebit;
                        List<tbl_PaymentTransaction> lstAllTransaction = _db.tbl_PaymentTransaction.Where(o => orderIds.Contains(o.OrderId) && o.TransactionDate >= dtStart && o.TransactionDate <= dtEnd && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                        int row1 = 1;
                        if (lstAllTransaction != null && lstAllTransaction.Count() > 0)
                        {
                            foreach (var objTrn in lstAllTransaction)
                            {
                                ReportVM objrp = new ReportVM();
                                objrp.Date = objTrn.TransactionDate.Value.ToString("dd-MM-yyyy");
                                objrp.Opening = TotalOpening.ToString();
                                if (objTrn.IsCredit == true)
                                {
                                    objrp.Credit = objTrn.Amount.Value.ToString();
                                    TotalOpening = TotalOpening + objTrn.Amount.Value;
                                }
                                else
                                {
                                    objrp.Credit = "";
                                }


                                if (objTrn.IsCredit == false)
                                {
                                    objrp.Debit = objTrn.Amount.Value.ToString();
                                    TotalOpening = TotalOpening - objTrn.Amount.Value;
                                }
                                else
                                {
                                    objrp.Debit = "";
                                }

                                objrp.Closing = TotalOpening.ToString();
                                objrp.PaymentMethod = objTrn.ModeOfPayment;
                                objrp.Remarks = objTrn.Remarks;
                                var objj = lstordes.Where(o => o.OrderId == objTrn.OrderId).FirstOrDefault();
                                string invno = "";
                                if (objj != null)
                                {
                                    invno = "S&S/" + objj.InvoiceYear + "/" + objj.InvoiceNo;
                                }
                                objrp.InvoiceNo = invno;
                                lstReportVm.Add(objrp);
                                row1 = row1 + 1;
                            }
                        }
                    }

                }
            }
        
            return PartialView("~/Areas/Admin/Views/Order/_PaymentReport.cshtml", lstReportVm);
        }

        [HttpPost]
        public string CheckEmail(string Email)
        {
            string ReturnMessage = "";
            try
            {
                long ClientUsrId = clsClientSession.UserID;
                long RoleIds = clsClientSession.RoleID;
                tbl_ClientUsers objclien = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == Email.ToLower() && o.ClientUserId != ClientUsrId && o.ClientRoleId == RoleIds && o.IsDelete == false).FirstOrDefault();
                if(objclien != null)
                {
                    return "Email is Already Exist";
                }
                else
                {
                    return "Success";
                }
               
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }


    }
}