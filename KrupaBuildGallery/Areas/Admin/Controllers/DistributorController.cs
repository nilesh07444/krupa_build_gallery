using ConstructionDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Text;
using System.Configuration;
using System.Net;
using System.Globalization;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using HiQPdf;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class DistributorController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public DistributorController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Distributor
        public ActionResult Index(int Status = -1, string StartDate = "", string EndDate = "")
        {
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
            try
            {
                DateTime dtStart = DateTime.MinValue;
                if (!string.IsNullOrEmpty(StartDate))
                {
                    dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
                }

                DateTime dtEnd = DateTime.MaxValue;
                if (!string.IsNullOrEmpty(StartDate))
                {
                    dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
                }
                lstClientUser = (from cu in _db.tbl_ClientUsers
                                 join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                                 where !cu.IsDelete && cu.ClientRoleId == 2 && cu.CreatedDate >= dtStart && cu.CreatedDate <= dtEnd
                                 select new ClientUserVM
                                 {
                                     ClientUserId = cu.ClientUserId,
                                     FirstName = cu.FirstName,
                                     LastName = cu.LastName,
                                     UserName = cu.UserName,
                                     Email = cu.Email,
                                     Password = cu.Password,
                                     RoleId = cu.ClientRoleId,
                                     CompanyName = cu.CompanyName,
                                     ProfilePic = cu.ProfilePicture,
                                     MobileNo = cu.MobileNo,
                                     IsActive = cu.IsActive,
                                     City = co.City,
                                     State = co.State,
                                     AddharCardNo = co.Addharcardno,
                                     PanCardNo = co.Pancardno,
                                     GSTNo = co.GSTno,
                                     AmountDue = co.AmountDue.HasValue ? co.AmountDue.Value : 0
                                 }).OrderBy(x => x.FirstName).ToList();

                if (Status == 2)
                {
                    lstClientUser = lstClientUser.Where(o => o.AmountDue > 0).ToList();
                }
                else if (Status == 1)
                {
                    List<long> clientuserids = lstClientUser.Select(o => o.ClientUserId).ToList();
                    List<long> pendingshippingdistri = _db.tbl_Orders.Where(o => o.ShippingStatus == 1 && clientuserids.Contains(o.ClientUserId)).Select(o => o.ClientUserId).Distinct().ToList();
                    lstClientUser = lstClientUser.Where(o => pendingshippingdistri.Contains(o.ClientUserId)).ToList();
                }
                ViewBag.Status = Status;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstClientUser);
        }
        public void Export(int Status = -1, string StartDate = "", string EndDate = "")
        {
            ExcelPackage excel = new ExcelPackage();
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
            DateTime dtStart = DateTime.MinValue;
            if (!string.IsNullOrEmpty(StartDate))
            {
                dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            }

            DateTime dtEnd = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(StartDate))
            {
                dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            }
            lstClientUser = (from cu in _db.tbl_ClientUsers
                             join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                             where !cu.IsDelete && cu.ClientRoleId == 2 && cu.CreatedDate >= dtStart && cu.CreatedDate <= dtEnd
                             select new ClientUserVM
                             {
                                 ClientUserId = cu.ClientUserId,
                                 FirstName = cu.FirstName,
                                 LastName = cu.LastName,
                                 UserName = cu.UserName,
                                 Email = cu.Email,
                                 Password = cu.Password,
                                 RoleId = cu.ClientRoleId,
                                 CompanyName = cu.CompanyName,
                                 ProfilePic = cu.ProfilePicture,
                                 MobileNo = cu.MobileNo,
                                 IsActive = cu.IsActive,
                                 City = co.City,
                                 State = co.State,
                                 AddharCardNo = co.Addharcardno,
                                 PanCardNo = co.Pancardno,
                                 GSTNo = co.GSTno,
                                 AmountDue = co.AmountDue.HasValue ? co.AmountDue.Value : 0
                             }).OrderBy(x => x.FirstName).ToList();

            if (Status == 2)
            {
                lstClientUser = lstClientUser.Where(o => o.AmountDue > 0).ToList();
            }
            else if (Status == 1)
            {
                List<long> clientuserids = lstClientUser.Select(o => o.ClientUserId).ToList();
                List<long> pendingshippingdistri = _db.tbl_Orders.Where(o => o.ShippingStatus == 1 && clientuserids.Contains(o.ClientUserId)).Select(o => o.ClientUserId).Distinct().ToList();
                lstClientUser = lstClientUser.Where(o => pendingshippingdistri.Contains(o.ClientUserId)).ToList();
            }

            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "First Name", "Last Name", "Mobile Number", "Email", "City", "State", "Created Date", "IsActive" };
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Distributors Report";
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
            if (lstClientUser != null && lstClientUser.Count() > 0)
            {
                foreach (var objj in lstClientUser)
                {
                    for (int j = 1; j < arrycolmns.Length + 1; j++)
                    {
                        workSheet.Cells[row1 + 2, j].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, j].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, j].AutoFitColumns(30, 70);
                        string vl = "";
                        if (arrycolmns[j - 1] == "First Name")
                        {
                            vl = objj.FirstName;
                        }
                        else if (arrycolmns[j - 1] == "Last Name")
                        {
                            vl = objj.LastName;
                        }
                        else if (arrycolmns[j - 1] == "Mobile Number")
                        {
                            vl = objj.MobileNo;
                        }
                        else if (arrycolmns[j - 1] == "Email")
                        {
                            vl = objj.Email;
                        }
                        else if (arrycolmns[j - 1] == "City")
                        {
                            vl = objj.City;
                        }
                        else if (arrycolmns[j - 1] == "State")
                        {
                            vl = objj.State;
                        }
                        else if (arrycolmns[j - 1] == "Created Date")
                        {
                            vl = objj.CreatedDate.ToString("dd-MMM-yyyy");
                        }
                        else if (arrycolmns[j - 1] == "IsActive")
                        {
                            vl = objj.IsActive == true ? "Active" : "Inactive";
                        }
                        workSheet.Cells[row1 + 2, j].Value = vl;

                    }



                    row1 = row1 + 1;
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Distributors.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        public ActionResult RequestList(int Status = 0)
        {
            List<DistributorRequestVM> lstDistriRequest = new List<DistributorRequestVM>();
            try
            {

                lstDistriRequest = (from cu in _db.tbl_DistributorRequestDetails
                                    where !cu.IsDelete.Value && (Status == -1 || cu.Status == Status)
                                    select new DistributorRequestVM
                                    {
                                        DistributorRequestId = cu.DistributorRequestId,
                                        FirstName = cu.FirstName,
                                        LastName = cu.LastName,
                                        Email = cu.Email,
                                        CompanyName = cu.CompanyName,
                                        MobileNo = cu.MobileNo,
                                        City = cu.City,
                                        State = cu.State,
                                        AddharCardNo = cu.AddharcardNo,
                                        PanCardNo = cu.PanCardNo,
                                        GSTNo = cu.GSTNo,
                                        Status = cu.Status.HasValue ? cu.Status.Value : 0
                                    }).OrderBy(x => x.FirstName).ToList();

                ViewBag.Status = Status;

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstDistriRequest);
        }

        public ActionResult Detail(long Id)
        {
            ClientUserVM objClientUserVM = (from cu in _db.tbl_ClientUsers
                                            join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                                            where !cu.IsDelete && cu.ClientUserId == Id
                                            select new ClientUserVM
                                            {
                                                ClientUserId = cu.ClientUserId,
                                                FirstName = cu.FirstName,
                                                LastName = cu.LastName,
                                                UserName = cu.UserName,
                                                Email = cu.Email,
                                                Password = cu.Password,
                                                RoleId = cu.ClientRoleId,
                                                CompanyName = cu.CompanyName,
                                                ProfilePic = cu.ProfilePicture,
                                                MobileNo = cu.MobileNo,
                                                IsActive = cu.IsActive,
                                                City = co.City,
                                                State = co.State,
                                                AddharCardNo = co.Addharcardno,
                                                PanCardNo = co.Pancardno,
                                                GSTNo = co.GSTno == "" ? "N/A" : co.GSTno,
                                                ProfilePhoto = cu.ProfilePicture,
                                                AddharPhoto = co.AddharPhoto,
                                                AlternateMobileNo = cu.AlternateMobileNo,
                                                ShopName = co.ShopName,
                                                GSTPhoto = co.GSTPhoto,
                                                PancardPhoto = co.PanCardPhoto,
                                                ShopPhoto = co.ShopPhoto,
                                                Prefix = cu.Prefix,
                                                CreditLimit = co.CreditLimitAmt.HasValue ? co.CreditLimitAmt.Value : 0,
                                                AmountDue = co.AmountDue.HasValue ? co.AmountDue.Value : 0
                                            }).FirstOrDefault();

            List<OrderVM> lstOrders = new List<OrderVM>();
            lstOrders = (from p in _db.tbl_Orders
                         where p.ClientUserId == Id
                         select new OrderVM
                         {
                             OrderId = p.OrderId,
                             ClientUserId = p.ClientUserId,
                             OrderAmount = p.OrderAmount,
                             OrderShipCity = p.OrderShipCity,
                             OrderShipState = p.OrderShipState,
                             OrderShipAddress = p.OrderShipAddress,
                             OrderPincode = p.OrderShipPincode,
                             OrderShipClientName = p.OrderShipClientName,
                             OrderShipClientPhone = p.OrderShipClientPhone,
                             OrderStatusId = p.OrderStatusId,
                             OrderAmountDue = p.AmountDue.Value,
                             PaymentType = p.PaymentType,
                             OrderDate = p.CreatedDate,
                             ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                             ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                         }).OrderByDescending(x => x.OrderDate).ToList();
            if (lstOrders != null && lstOrders.Count() > 0)
            {
                lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
            }
            objClientUserVM.OrderList = lstOrders;
            //if (lstOrders != null)
            //{
            //    objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
            //    List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
            //                                       join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
            //                                       where p.OrderId == Id
            //                                       select new OrderItemsVM
            //                                       {
            //                                           OrderId = p.OrderId.Value,
            //                                           OrderItemId = p.OrderDetailId,
            //                                           ProductItemId = p.ProductItemId.Value,
            //                                           ItemName = p.ItemName,
            //                                           Qty = p.Qty.Value,
            //                                           Price = p.Price.Value,
            //                                           Sku = p.Sku,
            //                                           GSTAmt = p.GSTAmt.Value,
            //                                           IGSTAmt = p.IGSTAmt.Value,
            //                                           ItemImg = c.MainImage
            //                                       }).OrderByDescending(x => x.OrderItemId).ToList();
            //    objOrder.OrderItems = lstOrderItms;
            //}
            return View(objClientUserVM);
        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }

        public ActionResult PaymentDetail(long Id)
        {
            List<PaymentHistoryVM> lstPaymentHist = new List<PaymentHistoryVM>();
            lstPaymentHist = (from p in _db.tbl_PaymentHistory
                              join o in _db.tbl_Orders on p.OrderId equals o.OrderId
                              where p.OrderId == Id
                              select new PaymentHistoryVM
                              {
                                  OrderId = p.OrderId,
                                  AmountDue = p.AmountDue,
                                  OrderTotalAmout = o.OrderAmount,
                                  AmountPaid = p.AmountPaid,
                                  PaymentDate = p.DateOfPayment,
                                  PaymentHistoryId = p.PaymentHistory_Id,
                                  Paymentthrough = p.PaymentBy,
                                  CurrentAmountDue = o.AmountDue.Value
                              }).OrderBy(x => x.PaymentDate).ToList();
            ViewData["orderobj"] = _db.tbl_Orders.Where(o => o.OrderId == Id).FirstOrDefault();
            return View(lstPaymentHist);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string MakePayment(FormCollection frm)
        {
            decimal AmountDue = 0, AmountPaid = 0;
            long OrderId = 0;
            DateTime PymtRecievedDate = new DateTime();
            if (frm["AmountDue"] != null)
            {
                AmountDue = Convert.ToDecimal(frm["AmountDue"].ToString());
            }
            if (frm["OrderId"] != null)
            {
                OrderId = Convert.ToInt64(frm["OrderId"].ToString());
            }
            if (frm["AmountPaid"] != null)
            {
                AmountPaid = Convert.ToDecimal(frm["AmountPaid"].ToString());
            }
            if (frm["PymtRecievedDate"] != null)
            {
                PymtRecievedDate = Convert.ToDateTime(frm["PymtRecievedDate"].ToString());
            }
            tbl_PaymentHistory objPyment = new tbl_PaymentHistory();
            objPyment.OrderId = OrderId;
            objPyment.PaymentBy = "Cash";
            objPyment.AmountDue = AmountDue;
            objPyment.AmountPaid = AmountPaid;
            objPyment.DateOfPayment = PymtRecievedDate;
            objPyment.CreatedBy = clsAdminSession.UserID;
            objPyment.CreatedDate = DateTime.Now;
            _db.tbl_PaymentHistory.Add(objPyment);
            _db.SaveChanges();

            var objOrder = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if (objOrder != null)
            {
                objOrder.AmountDue = AmountDue - AmountPaid;
                long ClientUserId = objOrder.ClientUserId;
                tbl_ClientOtherDetails objtbl_ClientOtherDetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                objtbl_ClientOtherDetails.AmountDue = objtbl_ClientOtherDetails.AmountDue - AmountPaid;
                _db.SaveChanges();
            }
            return "Success";
        }
        public string GenerateReceipt(long PaymentId)
        {
            tbl_PaymentHistory objPymt = _db.tbl_PaymentHistory.Where(o => o.PaymentHistory_Id == PaymentId).FirstOrDefault();
            string newhtmldata = "";
            if (objPymt != null)
            {
                StreamReader sr;
                string file = Server.MapPath("~/templates/ReceiptLatest.html");
                string htmldata = "";

                FileInfo fi = new FileInfo(file);
                sr = System.IO.File.OpenText(file);
                htmldata += sr.ReadToEnd();
                string ReceiptNo = "R-" + objPymt.PaymentHistory_Id;
                string DateOfReceipt = objPymt.DateOfPayment.ToString("dd-MM-yyyy");
                var objOrder = (from p in _db.tbl_Orders
                                join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                                where p.OrderId == objPymt.OrderId
                                select new OrderVM
                                {
                                    OrderId = p.OrderId,
                                    ClientUserName = c.FirstName + " " + c.LastName,
                                    ClientUserId = p.ClientUserId,
                                    OrderAmount = p.OrderAmount,
                                    OrderShipCity = p.OrderShipCity,
                                    OrderShipState = p.OrderShipState,
                                    OrderShipAddress = p.OrderShipAddress,
                                    OrderPincode = p.OrderShipPincode,
                                    OrderShipClientName = p.OrderShipClientName,
                                    OrderShipClientPhone = p.OrderShipClientPhone,
                                    OrderStatusId = p.OrderStatusId,
                                    PaymentType = p.PaymentType,
                                    OrderAmountDue = p.AmountDue.Value,
                                    OrderDate = p.CreatedDate
                                }).OrderByDescending(x => x.OrderDate).FirstOrDefault();
                string ClientUserName = objOrder.ClientUserName;
                string AmountWords = ConvertNumbertoWords(Convert.ToInt64(objPymt.AmountPaid));
                string PaymentBy = objPymt.PaymentBy;
                string Amount = objPymt.AmountPaid.ToString();
                string AmountDue = objPymt.AmountDue.ToString();
                string OrderNo = objPymt.OrderId.ToString();
                string OrderDate = objOrder.OrderDate.ToString("dd-MM-yyyy");
                newhtmldata = htmldata.Replace("--RECEIPTNO--", ReceiptNo).Replace("--DATE--", DateOfReceipt).Replace("--RECEIPTNO--", ReceiptNo).Replace("--NAME--", ClientUserName).Replace("--AMTWORD--", AmountWords).Replace("--OrderNo--", OrderNo).Replace("--OrderDate--", OrderDate).Replace("--PaymentBy--", PaymentBy).Replace("--PaidAmount--", Amount).Replace("--DueAmount--", AmountDue);
                //StringReader sr1 = new StringReader(newhtmldata);

                //Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);                
                //pdfDoc.Open();

                //XMLWorkerHelper objHelp = XMLWorkerHelper.GetInstance();
                //objHelp.ParseXHtml(writer, pdfDoc, sr1);
                //pdfDoc.Close();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "inline;filename=Receipt_" + objPymt.PaymentHistory_Id + ".pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Write(pdfDoc);
                //Response.End();

                //Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                ////PdfWriter writer = PdfWriter.GetInstance(pdfDoc,new FileStream(Server.MapPath("~/Receipt/")+ "Receipt_" + objPymt.PaymentHistory_Id+".pdf", FileMode.Create));
                ////writer.PageEvent = new App_Start.PDFGeneratePageEventHelper();
                //pdfDoc.Open();                
                //XMLWorkerHelper objHelp = XMLWorkerHelper.GetInstance();
                //objHelp.ParseXHtml(writer, pdfDoc, sr);
                //pdfDoc.Close();

                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "inline;filename=Receipt_" + objPymt.PaymentHistory_Id + ".pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Write(pdfDoc);
                //Response.End();
            }
            return newhtmldata;
            //return "Receipt_" + objPymt.PaymentHistory_Id+".pdf";
        }

        public string ConvertNumbertoWords(long number)
        {
            if (number == 0) return "ZERO";
            if (number < 0) return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " LAKES ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            //if ((number / 10) > 0)  
            //{  
            // words += ConvertNumbertoWords(number / 10) + " RUPEES ";  
            // number %= 10;  
            //}  
            if (number > 0)
            {
                if (words != "") words += "AND ";
                var unitsMap = new[]
        {
            "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
        };
                var tensMap = new[]
        {
            "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
        };
                if (number < 20) words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }

        public string PrintInvoice(long OrderId)
        {
            StreamReader sr;
            string newhtmldata = "";

            OrderVM objOrder = new OrderVM();
            objOrder = (from p in _db.tbl_Orders
                        join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                        join u in _db.tbl_ClientOtherDetails on c.ClientUserId equals u.ClientUserId
                        where p.OrderId == OrderId
                        select new OrderVM
                        {
                            OrderId = p.OrderId,
                            ClientUserName = c.FirstName + " " + c.LastName,
                            ClientUserId = p.ClientUserId,
                            ClientAddress = u.Address + ", " + u.City,
                            ClientEmail = c.Email,
                            ClientMobileNo = c.MobileNo,
                            OrderAmount = p.OrderAmount,
                            OrderShipCity = p.OrderShipCity,
                            OrderShipState = p.OrderShipState,
                            OrderShipAddress = p.OrderShipAddress,
                            OrderPincode = p.OrderShipPincode,
                            OrderShipClientName = p.OrderShipClientName,
                            OrderShipClientPhone = p.OrderShipClientPhone,
                            OrderStatusId = p.OrderStatusId,
                            PaymentType = p.PaymentType,
                            OrderDate = p.CreatedDate,
                            GSTNo = p.GSTNo,
                            InvoiceNo = p.InvoiceNo.Value,
                            InvoiceYear = p.InvoiceYear,
                            ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                            ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2,
                            ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();

            if (objOrder != null)
            {
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                   join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                   join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                                   where p.OrderId == OrderId
                                                   select new OrderItemsVM
                                                   {
                                                       OrderId = p.OrderId.Value,
                                                       OrderItemId = p.OrderDetailId,
                                                       ProductItemId = p.ProductItemId.Value,
                                                       ItemName = p.ItemName,
                                                       Qty = p.Qty.Value,
                                                       Price = p.Price.Value,
                                                       Sku = p.Sku,
                                                       GSTAmt = p.GSTAmt.Value,
                                                       IGSTAmt = p.IGSTAmt.Value,
                                                       ItemImg = c.MainImage,
                                                       HSNCode = c.HSNCode,
                                                       MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                                       VariantQtytxt = u.UnitQty,
                                                       GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                                       Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                       FinalAmt = p.FinalItemPrice.Value,
                                                       IsCombo = p.IsCombo.HasValue ? p.IsCombo.Value : false,
                                                       ComboId = p.ComboId.HasValue ? p.ComboId.Value : 0,
                                                       IsMainItem = p.IsMainItem.HasValue ? p.IsMainItem.Value : false,
                                                       ComboName = p.ComboOfferName,
                                                       ComboQty = p.ComboQty.HasValue ? p.ComboQty.Value : p.Qty.Value
                                                   }).OrderBy(x => x.OrderItemId).ToList();
                //}).OrderByDescending(x => x.GST_Per).ToList();

                objOrder.OrderItems = lstOrderItms;
                string file = Server.MapPath("~/templates/Invoice.html");
                string GSTTitle = "GST";
                if (objOrder.OrderShipState != "Gujarat")
                {
                    GSTTitle = "IGST";
                   // file = Server.MapPath("~/templates/InvoiceIGST.html");
                }
                string htmldata = "";

                FileInfo fi = new FileInfo(file);
                sr = System.IO.File.OpenText(file);
                htmldata += sr.ReadToEnd();
                string InvoiceNo = "S&S/" + objOrder.InvoiceYear + "/" + objOrder.InvoiceNo;
                string DateOfInvoice = CommonMethod.ConvertFromUTCOnlyDate(objOrder.OrderDate);
                string orderNo = objOrder.OrderId.ToString(); ;
                string ClientUserName = objOrder.ClientUserName;
                string ItemHtmls = "";
                decimal TotalFinal = 0;
                decimal SubTotal = 0;
                StringBuilder srBuild = new StringBuilder();
                if (lstOrderItms != null && lstOrderItms.Count() > 0)
                {
                    int cntsrNo = 1;

                    foreach (var objItem in lstOrderItms)
                    {
                        // decimal InclusiveGST = Math.Round(objItem.Price - objItem.Price * (100 / (100 + objItem.GST_Per)), 2);
                        // decimal PreGSTPrice = Math.Round(objItem.Price - InclusiveGST, 2);
                        decimal basicTotalPrice = Math.Round(objItem.Price * objItem.Qty, 2);
                        if (objItem.IsCombo && objItem.IsMainItem)
                        {
                            basicTotalPrice = Math.Round(objItem.Price * objItem.ComboQty, 2);
                        }

                        decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal CGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal IGSTAmt = Math.Round(objItem.GSTAmt);
                        decimal IGST = Math.Round(Convert.ToDecimal(objItem.GST_Per));
                        decimal FinalPrice = Math.Round(objItem.FinalAmt, 2);
                        decimal TaxableAmt = Math.Round(basicTotalPrice - objItem.Discount, 2);
                        TotalFinal = TotalFinal + FinalPrice;
                        if (objItem.IsCombo && objItem.IsMainItem)
                        {
                            srBuild.Append("<tr>");
                            srBuild.Append("<td>" + cntsrNo + "</td>");
                            srBuild.Append("<td colspan='2'>" + objItem.ComboName + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.ComboQty + "</td>");
                            srBuild.Append("<td class=\"text-center\"></td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.MRPPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.Price + "</td>");
                            //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.Discount + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");

                            //if (objOrder.OrderShipState != "Gujarat")
                            //{
                            //    srBuild.Append("<td class=\"text-center\">" + IGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + IGSTAmt + "</td>");
                            //}
                            //else
                            //{
                            //    srBuild.Append("<td class=\"text-center\">" + CGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + CGSTAmt + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + SGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + SGSTAmt + "</td>");

                            //}

                            srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%</td>");
                            srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                            srBuild.Append("</tr>");
                            cntsrNo = cntsrNo + 1;
                        }
                        srBuild.Append("<tr>");
                        if (objItem.IsCombo == false)
                        {
                            srBuild.Append("<td>" + cntsrNo + "</td>");
                        }
                        else
                        {
                            srBuild.Append("<td></td>");
                        }
                        srBuild.Append("<td>" + objItem.ItemName + "</td>");
                        srBuild.Append("<td>" + objItem.HSNCode + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Qty + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.VariantQtytxt + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.MRPPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Price + "</td>");
                        //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Discount + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");

                        //if (objOrder.OrderShipState != "Gujarat")
                        //{
                        //    srBuild.Append("<td class=\"text-center\">" + IGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + IGSTAmt + "</td>");
                        //}
                        //else
                        //{
                        //    srBuild.Append("<td class=\"text-center\">" + CGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + CGSTAmt + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + SGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + SGSTAmt + "</td>");

                        //}

                        srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%</td>");
                        if (objItem.IsCombo == false)
                        {
                            srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                            cntsrNo = cntsrNo + 1;
                        }
                        else
                        {
                            srBuild.Append("<td class=\"text-center\"></td>");
                        }
                        srBuild.Append("</tr>");


                    }
                }
                SubTotal = TotalFinal;
                TotalFinal = TotalFinal + objOrder.ShipmentCharge + objOrder.ExtraAmount;
                ItemHtmls = srBuild.ToString();

                string GST_HTML_DATA = getGSTCalculationHtmlDataByOrder(lstOrderItms, objOrder.OrderShipState != "Gujarat");
                string GSTNo = "";
                if (!string.IsNullOrEmpty(objOrder.GSTNo))
                {
                    GSTNo = "GST No." + objOrder.GSTNo;
                }
                double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(TotalFinal));
                double RoundedAmt = CommonMethod.GetRoundedValue(Convert.ToDouble(TotalFinal));
                string amtwrd = CommonMethod.ConvertToWords(RoundAmt.ToString());
                string address = objOrder.OrderShipAddress + "<br/>" + objOrder.OrderShipCity + "-" + objOrder.OrderPincode + "<br/>" + objOrder.OrderShipState;
                newhtmldata = htmldata.Replace("--INVOICENO--", InvoiceNo).Replace("--GSTTITLE--", GSTTitle).Replace("--GSTNo--", GSTNo).Replace("--INVOICEDATE--", DateOfInvoice).Replace("--ORDERNO--", orderNo).Replace("--CLIENTUSERNAME--", ClientUserName).Replace("--CLIENTUSERADDRESS--", address).Replace("--CLIENTUSEREMAIL--", objOrder.ClientEmail).Replace("--CLIENTUSERMOBILE--", objOrder.ClientMobileNo).Replace("--ITEMLIST--", ItemHtmls).Replace("--GSTCALCULATIONDATA--", GST_HTML_DATA).Replace("--SHIPPING--", Math.Round(objOrder.ShipmentCharge, 2).ToString()).Replace("--SUBTOTAL--", Math.Round(SubTotal, 2).ToString()).Replace("--TOTAL--", Math.Round(TotalFinal, 2).ToString()).Replace("--EXTRAAMOUNT--", Math.Round(objOrder.ExtraAmount, 2).ToString()).Replace("--ROUNDOFF--", Math.Round(RoundedAmt, 2).ToString()).Replace("--ROUNDTOTAL--", Math.Round(RoundAmt, 2).ToString()).Replace("--AMTWORD--", amtwrd);

            }

            return newhtmldata;
            //return "Receipt_" + objPymt.PaymentHistory_Id+".pdf";
        }

        private string getGSTCalculationHtmlDataByOrder(List<OrderItemsVM> lstOrderItms, bool IsIGST)
        {
            string htmlData = string.Empty;

            StringBuilder srBuild = new StringBuilder();


            decimal[] lstGSTPer = new decimal[] {0.00m,5.00m, 12.00m, 18.00m, 28.00m };

            decimal Grand_TotaltaxableAmount = 0;
            decimal Grand_IGST_Amt = 0;
            decimal Grand_CGST_Amt = 0;
            decimal Grand_SGST_Amt = 0;
            decimal Grand_FinalAmt = 0;

            lstGSTPer.ToList().ForEach(per =>
            {
                decimal TotaltaxableAmount = lstOrderItms.Where(x => x.GST_Per == per && x.IsCombo == false).Select(x => x.Price * x.Qty - x.Discount).Sum();
                decimal TotaltaxableAmount1 = lstOrderItms.Where(x => x.GST_Per == per && x.IsCombo == true).Select(x => x.Price * x.ComboQty - x.Discount).Sum();
                TotaltaxableAmount = TotaltaxableAmount + TotaltaxableAmount1;
                decimal IGST_Amt = 0;
                decimal CGST_Amt = 0;
                decimal SGST_Amt = 0;

                if (IsIGST)
                {
                    IGST_Amt = (TotaltaxableAmount * per) / 100;
                }
                else
                {
                    decimal half_per = per / 2;
                    CGST_Amt = (TotaltaxableAmount * half_per) / 100;
                    SGST_Amt = (TotaltaxableAmount * half_per) / 100;
                }

                decimal FinalAmt = TotaltaxableAmount + IGST_Amt + CGST_Amt + SGST_Amt;

                srBuild.Append("<tr>");
                srBuild.Append("<td class=\"text-center\"><strong> " + per.ToString("0.##") + "%</strong></td>");
                srBuild.Append("<td class=\"text-center\">" + TotaltaxableAmount.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + IGST_Amt.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + CGST_Amt.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + SGST_Amt.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + FinalAmt.ToString("0.##") + "</td>");
                srBuild.Append("</tr>");

                Grand_TotaltaxableAmount += TotaltaxableAmount;
                Grand_IGST_Amt += IGST_Amt;
                Grand_CGST_Amt += CGST_Amt;
                Grand_SGST_Amt += SGST_Amt;
                Grand_FinalAmt += FinalAmt;

            });

            // Taxable Amount
            srBuild.Append("<tr>");
            srBuild.Append("<td class=\"text-center\"><strong>Taxable Amount</strong></td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_TotaltaxableAmount.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_IGST_Amt.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_CGST_Amt.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_SGST_Amt.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_FinalAmt.ToString("0.##") + "</td>");
            srBuild.Append("</tr>");

            htmlData = srBuild.ToString();

            return htmlData;
        }

        public ActionResult RequestDetail(int Id)
        {
            DistributorRequestVM objDistReq = new DistributorRequestVM();
            try
            {
                objDistReq = (from cu in _db.tbl_DistributorRequestDetails
                              where !cu.IsDelete.Value && cu.DistributorRequestId == Id
                              select new DistributorRequestVM
                              {
                                  DistributorRequestId = cu.DistributorRequestId,
                                  FirstName = cu.FirstName,
                                  LastName = cu.LastName,
                                  Email = cu.Email,
                                  CompanyName = cu.CompanyName,
                                  MobileNo = cu.MobileNo,
                                  City = cu.City,
                                  State = cu.State,
                                  AddharCardNo = cu.AddharcardNo,
                                  PanCardNo = cu.PanCardNo,
                                  GSTNo = cu.GSTNo,
                                  ProfilePhoto = cu.ProfilePhoto,
                                  AddharPhoto = cu.AddharPhoto,
                                  AlternateMobile = cu.AlternateMobileNo,
                                  Dob = cu.Dob.Value,
                                  ShopName = cu.ShopName,
                                  GSTPhoto = cu.GSTPhoto,
                                  PancardPhoto = cu.PanCardPhoto,
                                  ShopPhoto = cu.ShopPhoto,
                                  Prefix = cu.Prefix,
                                  Status = cu.Status.HasValue ? cu.Status.Value : 0,
                                  Reason = cu.Reason,
                                  CancellationChequePhoto = cu.CancellationChequePhoto
                              }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return View(objDistReq);
        }

        [HttpPost]
        public string ApproveRejectDistributorRequest(long RequestId, string IsApprove, string CreditLimit = "0", string Password = "", string Reason = "")
        {
            string ReturnMessage = "";

            try
            {
                var objReq = _db.tbl_DistributorRequestDetails.Where(o => o.DistributorRequestId == RequestId).FirstOrDefault();
                if (objReq != null)
                {
                    if (IsApprove == "false")
                    {
                        objReq.Status = 2; //   0 For Pending  1 For Accept 2 For Reject
                        objReq.Reason = Reason;
                        _db.SaveChanges();
                        try
                        {
                            string ToEmail = objReq.Email;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;
                            string Subject = "Your Registration as a Distributor Rejected - Krupa Build Gallery";
                            string bodyhtml = "Following is the reason<br/>";
                            bodyhtml += "===============================<br/>";
                            bodyhtml += Reason;
                            clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                        }
                        catch (Exception e)
                        {
                            string ErrorMessage = e.Message.ToString();
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(111566, 999999);
                            string msg = "Your Registration As A Distributor Rejected - Shopping & Saving\n";
                            msg += "Following Is The Reason:\n";
                            msg += Reason;
                            string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                /// return "InvalidNumber";
                            }
                            else
                            {
                                //  return num.ToString();
                            }

                        }

                    }
                    else
                    {

                        tbl_ClientUsers objClient = new tbl_ClientUsers();
                        objClient.FirstName = objReq.FirstName;
                        objClient.LastName = objReq.LastName;
                        objClient.Email = objReq.Email;
                        objClient.MobileNo = objReq.MobileNo;
                        objClient.Password = clsCommon.EncryptString(Password);
                        objClient.IsActive = true;
                        objClient.IsDelete = false;
                        objClient.UserName = objReq.FirstName + objReq.LastName;
                        objClient.CompanyName = objReq.CompanyName;
                        objClient.CreatedBy = clsAdminSession.UserID;
                        objClient.CreatedDate = DateTime.Now;
                        objClient.AlternateMobileNo = objReq.AlternateMobileNo;
                        objClient.Prefix = objReq.Prefix;
                        objClient.ProfilePicture = objReq.ProfilePhoto;
                        objClient.ClientRoleId = 2;
                        _db.tbl_ClientUsers.Add(objClient);
                        _db.SaveChanges();
                        tbl_ClientOtherDetails objClientOther = new tbl_ClientOtherDetails();
                        objClientOther.ClientUserId = objClient.ClientUserId;
                        objClientOther.Addharcardno = objReq.AddharcardNo;
                        objClientOther.GSTno = objReq.GSTNo;
                        objClientOther.Pancardno = objReq.PanCardNo;
                        objClientOther.CreditLimitAmt = Convert.ToDecimal(CreditLimit);
                        objClientOther.AmountDue = 0;
                        objClientOther.IsActive = true;
                        objClientOther.IsDelete = false;
                        objClientOther.CreatedDate = DateTime.Now;
                        objClientOther.CreatedBy = clsAdminSession.UserID;
                        objClientOther.UpdatedDate = DateTime.Now;
                        objClientOther.UpdatedBy = clsAdminSession.UserID;
                        objClientOther.City = objReq.City;
                        objClientOther.State = objReq.State;
                        objClientOther.Dob = objReq.Dob;
                        objClientOther.PanCardPhoto = objReq.PanCardPhoto;
                        objClientOther.AddharPhoto = objReq.AddharPhoto;
                        objClientOther.ShopName = objReq.ShopName;
                        objClientOther.GSTPhoto = objReq.GSTPhoto;
                        objClientOther.ShopPhoto = objReq.ShopPhoto;
                        objClientOther.DistributorCode = "KBG/" + DateTime.Now.ToString("ddMMyyyy") + "/" + objClient.ClientUserId;
                        _db.tbl_ClientOtherDetails.Add(objClientOther);
                        objReq.Status = 1;
                        _db.SaveChanges();
                        try
                        {
                            string ToEmail = objReq.Email;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;
                            string Subject = "Your Registration As a Distributor Created - Shopping & Saving";
                            string bodyhtml = "Thank You For Become A Valuable Distributor Of Shopping & Saving<br/>";
                            bodyhtml += "Following Are The Login Details:<br/>";
                            bodyhtml += "===============================<br/>";
                            bodyhtml += "Email: " + objReq.Email + "<br/>";
                            bodyhtml += "Password: " + Password + "<br/>";
                            clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                        }
                        catch (Exception e)
                        {
                            string ErrorMessage = e.Message.ToString();
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(111566, 999999);
                            string msg = "Thank You For Become A Valuable Distributor Of Shopping & Saving\n";
                            msg += "Login Details:\n";
                            msg += "Email:" + objReq.Email + "\n";
                            msg += "Password:" + Password + "\n";
                            string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                /// return "InvalidNumber";
                            }
                            else
                            {
                                //  return num.ToString();
                            }

                        }


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

        [HttpPost]
        public string DeleteDistributorRequest(long RequestId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_DistributorRequestDetails distributorreq = _db.tbl_DistributorRequestDetails.Where(x => x.DistributorRequestId == RequestId).FirstOrDefault();

                if (distributorreq == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_DistributorRequestDetails.Remove(distributorreq);
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
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_ClientUsers objtbl_ClientUsers = _db.tbl_ClientUsers.Where(x => x.ClientUserId == Id).FirstOrDefault();

                if (objtbl_ClientUsers != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_ClientUsers.IsActive = true;
                    }
                    else
                    {
                        objtbl_ClientUsers.IsActive = false;
                    }

                    objtbl_ClientUsers.UpdatedBy = LoggedInUserId;
                    objtbl_ClientUsers.UpdatedDate = DateTime.UtcNow;

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
        public string ChangeCreditLimit(long ClientUserId, string CreditLimit)
        {
            try
            {
                string Mobilenum = "";
                tbl_ClientUsers objClient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                if (objClient != null)
                {
                    Mobilenum = objClient.MobileNo;
                }
                tbl_ClientOtherDetails objclientother = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                if (objclientother != null)
                {
                    decimal Credit = Convert.ToDecimal(CreditLimit);
                    objclientother.CreditLimitAmt = Credit;
                    _db.SaveChanges();
                    string msg = "Your Credit Limit Has Changed To Rs" + CreditLimit + " - Shopping & Saving";
                    SendSMSmsg(Mobilenum, msg);
                }
                return "Success";
            }
            catch (Exception e)
            {
                return "";
            }

        }

        public string SendSMSmsg(string MobileNumber, string msg)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return "Success";
                    }

                }
            }
            catch (WebException ex)
            {
                return "Error";
            }
        }

        public ActionResult Edit(int Id)
        {
            ClientUserVM objClientUserVM = (from cu in _db.tbl_ClientUsers
                                            join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                                            where !cu.IsDelete && cu.ClientUserId == Id
                                            select new ClientUserVM
                                            {
                                                ClientUserId = cu.ClientUserId,
                                                FirstName = cu.FirstName,
                                                LastName = cu.LastName,
                                                UserName = cu.UserName,
                                                Email = cu.Email,
                                                Password = cu.Password,
                                                RoleId = cu.ClientRoleId,
                                                CompanyName = cu.CompanyName,
                                                ProfilePic = cu.ProfilePicture,
                                                MobileNo = cu.MobileNo,
                                                IsActive = cu.IsActive,
                                                City = co.City,
                                                State = co.State,
                                                AddharCardNo = co.Addharcardno,
                                                PanCardNo = co.Pancardno,
                                                GSTNo = co.GSTno,
                                                ProfilePhoto = cu.ProfilePicture,
                                                AddharPhoto = co.AddharPhoto,
                                                AlternateMobileNo = cu.AlternateMobileNo,
                                                ShopName = co.ShopName,
                                                GSTPhoto = co.GSTPhoto,
                                                PancardPhoto = co.PanCardPhoto,
                                                ShopPhoto = co.ShopPhoto,
                                                Prefix = cu.Prefix,
                                                BirthDate = co.Dob.HasValue ? co.Dob.Value : DateTime.MinValue,
                                                CreditLimit = co.CreditLimitAmt.HasValue ? co.CreditLimitAmt.Value : 0,
                                                AmountDue = co.AmountDue.HasValue ? co.AmountDue.Value : 0
                                            }).FirstOrDefault();
            return View(objClientUserVM);
        }

        [HttpPost]
        public ActionResult EditDistributor(FormCollection frm, HttpPostedFileBase aadhharphoto, HttpPostedFileBase gstphoto, HttpPostedFileBase pancardnophoto, HttpPostedFileBase photofile, HttpPostedFileBase shopphoto)
        {
            try
            {
                string email = frm["email"].ToString();
                string firstnm = frm["fname"].ToString();
                string lastnm = frm["lname"].ToString();
                string mobileno = frm["mobileno"].ToString();
                string businessname = frm["bussinessname"].ToString();
                string addharno = frm["addharno"].ToString();
                string city = frm["city"].ToString();
                string state = frm["state"].ToString();
                string gstno = frm["gstno"].ToString();
                string prefix = frm["prefix"].ToString();
                string dob = frm["dob"].ToString();
                string alternatemobileno = frm["alternatemobileno"].ToString();
                string shopname = frm["shopname"].ToString();
                string pancardno = frm["pancardno"].ToString();
                string photo = string.Empty;
                string pancardphotoname = string.Empty;
                string gstphotoname = string.Empty;
                string addharphoto = string.Empty;
                string shopphotoname = string.Empty;
                long ClientUserId = Convert.ToInt64(frm["clientuserid"].ToString());

                string path = Server.MapPath("~/Images/UsersDocuments/");
                if (aadhharphoto != null)
                {
                    addharphoto = Guid.NewGuid() + "-" + Path.GetFileName(aadhharphoto.FileName);
                    aadhharphoto.SaveAs(path + addharphoto);
                }
                if (pancardnophoto != null)
                {
                    pancardphotoname = Guid.NewGuid() + "-" + Path.GetFileName(pancardnophoto.FileName);
                    pancardnophoto.SaveAs(path + pancardphotoname);
                }
                if (gstphoto != null)
                {
                    gstphotoname = Guid.NewGuid() + "-" + Path.GetFileName(gstphoto.FileName);
                    gstphoto.SaveAs(path + gstphotoname);
                }
                if (photofile != null)
                {
                    photo = Guid.NewGuid() + "-" + Path.GetFileName(photofile.FileName);
                    photofile.SaveAs(path + photo);
                }

                if (shopphoto != null)
                {
                    shopphotoname = Guid.NewGuid() + "-" + Path.GetFileName(shopphoto.FileName);
                    shopphoto.SaveAs(path + shopphotoname);
                }

                tbl_ClientUsers objclient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                if (objclient != null)
                {
                    objclient.FirstName = firstnm;
                    objclient.LastName = lastnm;
                    objclient.UserName = firstnm + lastnm;
                    //objclient.Email = email;
                    //objclient.MobileNo = mobileno;
                    objclient.CompanyName = businessname;
                    objclient.AlternateMobileNo = alternatemobileno;
                    objclient.Prefix = prefix;
                    if (!string.IsNullOrEmpty(photo))
                    {
                        objclient.ProfilePicture = photo;
                    }
                }
                tbl_ClientOtherDetails objclientoth = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                objclientoth.City = city;
                objclientoth.State = state;
                objclientoth.Addharcardno = addharno;
                objclientoth.GSTno = gstno;
                objclientoth.Pancardno = pancardno;
                objclientoth.ShopName = shopname;
                DateTime dt = DateTime.ParseExact(dob, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                objclientoth.Dob = dt;
                if (!string.IsNullOrEmpty(shopphotoname))
                {
                    objclientoth.ShopPhoto = shopphotoname;
                }
                if (!string.IsNullOrEmpty(addharphoto))
                {
                    objclientoth.AddharPhoto = addharphoto;
                }
                if (!string.IsNullOrEmpty(gstphotoname))
                {
                    objclientoth.GSTPhoto = gstphotoname;
                }
                if (!string.IsNullOrEmpty(pancardphotoname))
                {
                    objclientoth.PanCardPhoto = pancardphotoname;
                }
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["RegisterError"] = ErrorMessage;
            }

            return RedirectToAction("Index");

        }

        public string PrintInvoiceOrderItem(long OrderItemId, long OrderId)
        {
            StreamReader sr;
            string newhtmldata = "";
            var objSettings = _db.tbl_GeneralSetting.FirstOrDefault();
            OrderVM objOrder = new OrderVM();
            objOrder = (from p in _db.tbl_Orders
                        join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                        join u in _db.tbl_ClientOtherDetails on c.ClientUserId equals u.ClientUserId
                        where p.OrderId == OrderId
                        select new OrderVM
                        {
                            OrderId = p.OrderId,
                            ClientUserName = c.FirstName + " " + c.LastName,
                            ClientUserId = p.ClientUserId,
                            ClientAddress = u.Address + ", " + u.City,
                            ClientEmail = c.Email,
                            ClientMobileNo = c.MobileNo,
                            OrderAmount = p.OrderAmount,
                            OrderShipCity = p.OrderShipCity,
                            OrderShipState = p.OrderShipState,
                            OrderShipAddress = p.OrderShipAddress,
                            OrderPincode = p.OrderShipPincode,
                            OrderShipClientName = p.OrderShipClientName,
                            OrderShipClientPhone = p.OrderShipClientPhone,
                            OrderStatusId = p.OrderStatusId,
                            PaymentType = p.PaymentType,
                            OrderDate = p.CreatedDate,
                            InvoiceNo = p.InvoiceNo.Value,
                            GSTNo = p.GSTNo,
                            InvoiceYear = p.InvoiceYear,
                            ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                            ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2,
                            ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();

            if (objOrder != null)
            {
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                OrderItemsVM objItem = (from p in _db.tbl_OrderItemDetails
                                        join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                        join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                        where p.OrderId == OrderId && p.OrderDetailId == OrderItemId
                                        select new OrderItemsVM
                                        {
                                            OrderId = p.OrderId.Value,
                                            OrderItemId = p.OrderDetailId,
                                            ProductItemId = p.ProductItemId.Value,
                                            ItemName = p.ItemName,
                                            Qty = p.Qty.Value,
                                            Price = p.Price.Value,
                                            HSNCode = c.HSNCode,
                                            FinalAmt = p.FinalItemPrice.HasValue ? p.FinalItemPrice.Value : 0,
                                            Sku = p.Sku,
                                            IsDeleted = p.IsDelete,
                                            GSTAmt = p.GSTAmt.Value,
                                            IGSTAmt = p.IGSTAmt.Value,
                                            ItemImg = c.MainImage,
                                            MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                            ItemStatus = p.ItemStatus.HasValue ? p.ItemStatus.Value : 0,
                                            ShipingChargeOf1Item = c.ShippingCharge.HasValue ? c.ShippingCharge.Value : 0,
                                            VariantQtytxt = u.UnitQty,
                                            GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                            Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                            ComboName = p.ComboOfferName,
                                            IsCombo = p.IsCombo.HasValue ? p.IsCombo.Value : false,
                                            IsMainItem = p.IsMainItem.HasValue ? p.IsMainItem.Value : false,
                                            ComboQty = p.ComboQty.HasValue ? p.ComboQty.Value : 0,
                                            ComboId = p.ComboId.HasValue ? p.ComboId.Value : 0,
                                            modifieddate = p.UpdatedDate.HasValue ? p.UpdatedDate.Value : DateTime.Now
                                        }).OrderByDescending(x => x.GST_Per).FirstOrDefault();

                List<OrderItemsVM> lsttOrdrItms = new List<OrderItemsVM>();
                if (objItem.IsCombo == true)
                {
                    long ComboId = objItem.ComboId;
                    lsttOrdrItms = (from p in _db.tbl_OrderItemDetails
                                    join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                    join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                    where p.OrderId == OrderId && p.ComboId.Value == ComboId
                                    select new OrderItemsVM
                                    {
                                        OrderId = p.OrderId.Value,
                                        OrderItemId = p.OrderDetailId,
                                        ProductItemId = p.ProductItemId.Value,
                                        ItemName = p.ItemName,
                                        Qty = p.Qty.Value,
                                        Price = p.Price.Value,
                                        HSNCode = c.HSNCode,
                                        FinalAmt = p.FinalItemPrice.HasValue ? p.FinalItemPrice.Value : 0,
                                        Sku = p.Sku,
                                        IsDeleted = p.IsDelete,
                                        GSTAmt = p.GSTAmt.Value,
                                        IGSTAmt = p.IGSTAmt.Value,
                                        ItemImg = c.MainImage,
                                        MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                        ItemStatus = p.ItemStatus.HasValue ? p.ItemStatus.Value : 0,
                                        ShipingChargeOf1Item = c.ShippingCharge.HasValue ? c.ShippingCharge.Value : 0,
                                        VariantQtytxt = u.UnitQty,
                                        GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                        Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                        ComboName = p.ComboOfferName,
                                        IsCombo = p.IsCombo.HasValue ? p.IsCombo.Value : false,
                                        IsMainItem = p.IsMainItem.HasValue ? p.IsMainItem.Value : false,
                                        ComboQty = p.ComboQty.HasValue ? p.ComboQty.Value : 0,
                                        modifieddate = p.UpdatedDate.HasValue ? p.UpdatedDate.Value : DateTime.Now
                                    }).ToList();
                }

                string file = Server.MapPath("~/templates/Invoice.html");
                if (objItem.ItemStatus == 5 && objItem.IsDeleted == true)
                {
                    file = Server.MapPath("~/templates/InvoiceCancel.html");
                }
                else if (objItem.ItemStatus == 6 && objItem.IsDeleted == true)
                {
                    file = Server.MapPath("~/templates/InvoiceReturn.html");
                }
                else if (objItem.ItemStatus == 8 && objItem.IsDeleted == true)
                {
                    file = Server.MapPath("~/templates/InvoiceExchange.html");
                }

                string GSTTitle = "GST";
                if (objOrder.OrderShipState != "Gujarat")
                {
                    GSTTitle = "IGST";
                    // file = Server.MapPath("~/templates/InvoiceIGST.html");
                }
                //if (objOrder.OrderShipState != "Gujarat")
                //{
                //    file = Server.MapPath("~/templates/InvoiceIGST.html");
                //}
                string htmldata = "";

                FileInfo fi = new FileInfo(file);
                sr = System.IO.File.OpenText(file);
                htmldata += sr.ReadToEnd();
                string InvoiceNo = "S&S/" + objOrder.InvoiceYear + "/" + objOrder.InvoiceNo;
                string DateOfInvoice = CommonMethod.ConvertFromUTCOnlyDate(objOrder.OrderDate); 
                string orderNo = objOrder.OrderId.ToString(); ;
                string ClientUserName = objOrder.ClientUserName;
                string ItemHtmls = "";
                decimal TotalFinal = 0;
                decimal SubTotal = 0;
                string DateOfCancelReturnExchage = CommonMethod.ConvertFromUTCOnlyDate(objItem.modifieddate); 
                StringBuilder srBuild = new StringBuilder();

                int cntsrNo = 1;
                if (lsttOrdrItms != null && lsttOrdrItms.Count() > 0)
                {
                    foreach(var objItm in lsttOrdrItms)
                    {
                        decimal basicTotalPrice = Math.Round(objItm.Price * objItem.Qty, 2);
                        if (objItm.IsCombo && objItm.IsMainItem)
                        {
                            basicTotalPrice = Math.Round(objItm.Price * objItem.ComboQty, 2);
                        }
                       
                        decimal SGST = Math.Round(Convert.ToDecimal(objItm.GST_Per / 2), 2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItm.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round(objItm.GSTAmt / 2, 2);
                        decimal CGSTAmt = Math.Round(objItm.GSTAmt / 2, 2);
                        decimal IGSTAmt = Math.Round(objItm.GSTAmt);
                        decimal IGST = Math.Round(Convert.ToDecimal(objItm.GST_Per));
                        decimal FinalPrice = Math.Round(objItm.FinalAmt,2);
                        decimal TaxableAmt = Math.Round(basicTotalPrice - objItm.Discount, 2);
                        TotalFinal = TotalFinal + FinalPrice;
                        if(objItm.IsCombo && objItm.IsMainItem)
                        {
                            srBuild.Append("<tr>");
                            srBuild.Append("<td>" + cntsrNo + "</td>");
                            srBuild.Append("<td>" + objItm.ComboName + "</td>");
                            srBuild.Append("<td></td>");
                            srBuild.Append("<td class=\"text-center\">" + objItm.ComboQty + "</td>");
                            srBuild.Append("<td class=\"text-center\"></td>");
                            srBuild.Append("<td class=\"text-center\">" + objItm.MRPPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItm.Price + "</td>");
                            //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItm.Discount + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");


                            srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItm.GST_Per).ToString("0.##") + "%</td>");
                            srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                            srBuild.Append("</tr>");
                        }
                        srBuild.Append("<tr>");
                        srBuild.Append("<td></td>");
                        srBuild.Append("<td>" + objItm.ItemName + "</td>");
                        srBuild.Append("<td>" + objItm.HSNCode + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItm.Qty + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItm.VariantQtytxt + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItm.MRPPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItm.Price + "</td>");
                        //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItm.Discount + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");


                        srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItm.GST_Per).ToString("0.##") + "%</td>");
                        srBuild.Append("<td class=\"text-center\"></td>");
                        srBuild.Append("</tr>");
                    }
                }
                else
                {
                    decimal basicTotalPrice = Math.Round(objItem.Price * objItem.Qty, 2);
                    decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                    decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                    decimal SGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                    decimal CGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                    decimal IGSTAmt = Math.Round(objItem.GSTAmt);
                    decimal IGST = Math.Round(Convert.ToDecimal(objItem.GST_Per));
                    decimal FinalPrice = Math.Round(basicTotalPrice + objItem.GSTAmt - objItem.Discount, 2);
                    decimal TaxableAmt = Math.Round(basicTotalPrice - objItem.Discount, 2);
                    TotalFinal = TotalFinal + FinalPrice;
                    srBuild.Append("<tr>");
                    srBuild.Append("<td>" + cntsrNo + "</td>");
                    srBuild.Append("<td>" + objItem.ItemName + "</td>");
                    srBuild.Append("<td>" + objItem.HSNCode + "</td>");
                    srBuild.Append("<td class=\"text-center\">" + objItem.Qty + "</td>");
                    srBuild.Append("<td class=\"text-center\">" + objItem.VariantQtytxt + "</td>");
                    srBuild.Append("<td class=\"text-center\">" + objItem.MRPPrice + "</td>");
                    srBuild.Append("<td class=\"text-center\">" + objItem.Price + "</td>");
                    //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                    srBuild.Append("<td class=\"text-center\">" + objItem.Discount + "</td>");
                    srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");


                    srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%</td>");
                    srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                    srBuild.Append("</tr>");
                }
                // decimal InclusiveGST = Math.Round(objItem.Price - objItem.Price * (100 / (100 + objItem.GST_Per)), 2);
                // decimal PreGSTPrice = Math.Round(objItem.Price - InclusiveGST, 2);

               
                cntsrNo = cntsrNo + 1;


                decimal shipcharge = (objItem.ShipingChargeOf1Item * objItem.Qty);
                SubTotal = TotalFinal;
                if (objItem.ItemStatus == 5)
                {
                    if (objOrder.OrderPincode == "389001")
                    {                        
                        TotalFinal = TotalFinal + shipcharge;
                    }
                    else
                    {
                        shipcharge = 0;
                    }
                }

                decimal amtcut = 0;
                if (objItem.ItemStatus == 6)
                {
                    if (objOrder.OrderPincode == "389001")
                    {
                        amtcut = Math.Round((objItem.FinalAmt * objSettings.ReturnPerInGodhra.Value) / 100, 2);
                    }
                    else
                    {
                        amtcut = Math.Round((objItem.FinalAmt * objSettings.ReturnPerOutGodhra.Value) / 100, 2);
                    }

                    TotalFinal = TotalFinal - amtcut;
                }
                else if (objItem.ItemStatus == 8)
                {
                    amtcut = Math.Round((objItem.FinalAmt * objSettings.ExchangePer.Value) / 100, 2);
                    TotalFinal = TotalFinal - amtcut;
                }
                // decimal refundamtt = objOrderItm.FinalItemPrice.Value - amtcut;

                ItemHtmls = srBuild.ToString();
                List<OrderItemsVM> lstitms = new List<OrderItemsVM>();
                lstitms.Add(objItem);
                string GST_HTML_DATA = getGSTCalculationHtmlDataByOrder(lstitms, objOrder.OrderShipState != "Gujarat");
                string GSTNo = "";
                if (!string.IsNullOrEmpty(objOrder.GSTNo))
                {
                    GSTNo = "GST No." + objOrder.GSTNo;
                }
                double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(TotalFinal));
                double RoundedAmt = CommonMethod.GetRoundedValue(Convert.ToDouble(TotalFinal));
                string address = objOrder.OrderShipAddress + "<br/>" + objOrder.OrderShipCity + "-" + objOrder.OrderPincode + "<br/>" + objOrder.OrderShipState;
                newhtmldata = htmldata.Replace("--INVOICENO--", InvoiceNo).Replace("--GSTTITLE--", GSTTitle).Replace("--GSTNo--", GSTNo).Replace("--CANCELEDDATE--", DateOfCancelReturnExchage).Replace("--RETURNDATE--", DateOfCancelReturnExchage).Replace("--INVOICEDATE--", DateOfInvoice).Replace("--ORDERNO--", orderNo).Replace("--CLIENTUSERNAME--", ClientUserName).Replace("--CLIENTUSERADDRESS--", address).Replace("--CLIENTUSEREMAIL--", objOrder.ClientEmail).Replace("--CLIENTUSERMOBILE--", objOrder.ClientMobileNo).Replace("--ITEMLIST--", ItemHtmls).Replace("--GSTCALCULATIONDATA--", GST_HTML_DATA).Replace("--SHIPPING--", Math.Round(shipcharge, 2).ToString()).Replace("--SUBTOTAL--", Math.Round(SubTotal, 2).ToString()).Replace("--TOTAL--", Math.Round(TotalFinal, 2).ToString()).Replace("--EXTRAAMOUNT--", Math.Round(objOrder.ExtraAmount, 2).ToString()).Replace("--ExchangeCHARGE--", Math.Round(amtcut, 2).ToString()).Replace("--RETURNCHARGE--", Math.Round(amtcut, 2).ToString()).Replace("--ROUNDOFF--", Math.Round(RoundedAmt, 2).ToString()).Replace("--ROUNDTOTAL--", Math.Round(RoundAmt, 2).ToString()); ;

            }

            return newhtmldata;
            //return "Receipt_" + objPymt.PaymentHistory_Id+".pdf";
        }

        public void DownloadInvoice(long OrderId)
        {
            StreamReader sr;
            string newhtmldata = "";

            OrderVM objOrder = new OrderVM();
            objOrder = (from p in _db.tbl_Orders
                        join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                        join u in _db.tbl_ClientOtherDetails on c.ClientUserId equals u.ClientUserId
                        where p.OrderId == OrderId
                        select new OrderVM
                        {
                            OrderId = p.OrderId,
                            ClientUserName = c.FirstName + " " + c.LastName,
                            ClientUserId = p.ClientUserId,
                            ClientAddress = u.Address + ", " + u.City,
                            ClientEmail = c.Email,
                            ClientMobileNo = c.MobileNo,
                            OrderAmount = p.OrderAmount,
                            OrderShipCity = p.OrderShipCity,
                            OrderShipState = p.OrderShipState,
                            OrderShipAddress = p.OrderShipAddress,
                            OrderPincode = p.OrderShipPincode,
                            OrderShipClientName = p.OrderShipClientName,
                            OrderShipClientPhone = p.OrderShipClientPhone,
                            OrderStatusId = p.OrderStatusId,
                            PaymentType = p.PaymentType,
                            OrderDate = p.CreatedDate,
                            GSTNo = p.GSTNo,
                            InvoiceNo = p.InvoiceNo.Value,
                            InvoiceYear = p.InvoiceYear,
                            ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                            ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2,
                            ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();

            if (objOrder != null)
            {
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                   join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                   join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                                   where p.OrderId == OrderId
                                                   select new OrderItemsVM
                                                   {
                                                       OrderId = p.OrderId.Value,
                                                       OrderItemId = p.OrderDetailId,
                                                       ProductItemId = p.ProductItemId.Value,
                                                       ItemName = p.ItemName,
                                                       Qty = p.Qty.Value,
                                                       Price = p.Price.Value,
                                                       Sku = p.Sku,
                                                       GSTAmt = p.GSTAmt.Value,
                                                       IGSTAmt = p.IGSTAmt.Value,
                                                       ItemImg = c.MainImage,
                                                       HSNCode = c.HSNCode,
                                                       MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                                       VariantQtytxt = u.UnitQty,
                                                       GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                                       Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                       FinalAmt = p.FinalItemPrice.Value,
                                                       IsCombo = p.IsCombo.HasValue ? p.IsCombo.Value : false,
                                                       ComboId = p.ComboId.HasValue ? p.ComboId.Value : 0,
                                                       IsMainItem = p.IsMainItem.HasValue ? p.IsMainItem.Value : false,
                                                       ComboName = p.ComboOfferName,
                                                       ComboQty = p.ComboQty.HasValue ? p.ComboQty.Value : p.Qty.Value
                                                   }).OrderBy(x => x.OrderItemId).ToList();
                //}).OrderByDescending(x => x.GST_Per).ToList();

                objOrder.OrderItems = lstOrderItms;
                string file = Server.MapPath("~/templates/Invoice.html");
                string GSTTitle = "GST";
                if (objOrder.OrderShipState != "Gujarat")
                {
                    GSTTitle = "IGST";
                    // file = Server.MapPath("~/templates/InvoiceIGST.html");
                }
                string htmldata = "";

                FileInfo fi = new FileInfo(file);
                sr = System.IO.File.OpenText(file);
                htmldata += sr.ReadToEnd();
                string InvoiceNo = "S&S/" + objOrder.InvoiceYear + "/" + objOrder.InvoiceNo;
                string DateOfInvoice = objOrder.OrderDate.ToString("dd-MM-yyyy");
                string orderNo = objOrder.OrderId.ToString(); ;
                string ClientUserName = objOrder.ClientUserName;
                string ItemHtmls = "";
                decimal TotalFinal = 0;
                decimal SubTotal = 0;
                StringBuilder srBuild = new StringBuilder();
                if (lstOrderItms != null && lstOrderItms.Count() > 0)
                {
                    int cntsrNo = 1;

                    foreach (var objItem in lstOrderItms)
                    {
                        // decimal InclusiveGST = Math.Round(objItem.Price - objItem.Price * (100 / (100 + objItem.GST_Per)), 2);
                        // decimal PreGSTPrice = Math.Round(objItem.Price - InclusiveGST, 2);
                        decimal basicTotalPrice = Math.Round(objItem.Price * objItem.Qty, 2);
                        if (objItem.IsCombo && objItem.IsMainItem)
                        {
                            basicTotalPrice = Math.Round(objItem.Price * objItem.ComboQty, 2);
                        }

                        decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal CGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal IGSTAmt = Math.Round(objItem.GSTAmt);
                        decimal IGST = Math.Round(Convert.ToDecimal(objItem.GST_Per));
                        decimal FinalPrice = Math.Round(objItem.FinalAmt, 2);
                        decimal TaxableAmt = Math.Round(basicTotalPrice - objItem.Discount, 2);
                        TotalFinal = TotalFinal + FinalPrice;
                        if (objItem.IsCombo && objItem.IsMainItem)
                        {
                            srBuild.Append("<tr>");
                            srBuild.Append("<td>" + cntsrNo + "</td>");
                            srBuild.Append("<td colspan='2'>" + objItem.ComboName + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.ComboQty + "</td>");
                            srBuild.Append("<td class=\"text-center\"></td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.MRPPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.Price + "</td>");
                            //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.Discount + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");

                            //if (objOrder.OrderShipState != "Gujarat")
                            //{
                            //    srBuild.Append("<td class=\"text-center\">" + IGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + IGSTAmt + "</td>");
                            //}
                            //else
                            //{
                            //    srBuild.Append("<td class=\"text-center\">" + CGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + CGSTAmt + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + SGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + SGSTAmt + "</td>");

                            //}

                            srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%</td>");
                            srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                            srBuild.Append("</tr>");
                            cntsrNo = cntsrNo + 1;
                        }
                        srBuild.Append("<tr>");
                        if (objItem.IsCombo == false)
                        {
                            srBuild.Append("<td>" + cntsrNo + "</td>");
                        }
                        else
                        {
                            srBuild.Append("<td></td>");
                        }
                        srBuild.Append("<td>" + objItem.ItemName + "</td>");
                        srBuild.Append("<td>" + objItem.HSNCode + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Qty + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.VariantQtytxt + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.MRPPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Price + "</td>");
                        //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Discount + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");

                        //if (objOrder.OrderShipState != "Gujarat")
                        //{
                        //    srBuild.Append("<td class=\"text-center\">" + IGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + IGSTAmt + "</td>");
                        //}
                        //else
                        //{
                        //    srBuild.Append("<td class=\"text-center\">" + CGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + CGSTAmt + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + SGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + SGSTAmt + "</td>");

                        //}

                        srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%</td>");
                        if (objItem.IsCombo == false)
                        {
                            srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                            cntsrNo = cntsrNo + 1;
                        }
                        else
                        {
                            srBuild.Append("<td class=\"text-center\"></td>");
                        }
                        srBuild.Append("</tr>");


                    }
                }
                SubTotal = TotalFinal;
                TotalFinal = TotalFinal + objOrder.ShipmentCharge + objOrder.ExtraAmount;
                ItemHtmls = srBuild.ToString();

                string GST_HTML_DATA = getGSTCalculationHtmlDataByOrder(lstOrderItms, objOrder.OrderShipState != "Gujarat");
                string GSTNo = "";
                if (!string.IsNullOrEmpty(objOrder.GSTNo))
                {
                    GSTNo = "GST No." + objOrder.GSTNo;
                }
                double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(TotalFinal));
                double RoundedAmt = CommonMethod.GetRoundedValue(Convert.ToDouble(TotalFinal));
                string address = objOrder.OrderShipAddress + "<br/>" + objOrder.OrderShipCity + "-" + objOrder.OrderPincode + "<br/>" + objOrder.OrderShipState;
                newhtmldata = htmldata.Replace("--INVOICENO--", InvoiceNo).Replace("--GSTTITLE--", GSTTitle).Replace("--GSTNo--", GSTNo).Replace("--INVOICEDATE--", DateOfInvoice).Replace("--ORDERNO--", orderNo).Replace("--CLIENTUSERNAME--", ClientUserName).Replace("--CLIENTUSERADDRESS--", address).Replace("--CLIENTUSEREMAIL--", objOrder.ClientEmail).Replace("--CLIENTUSERMOBILE--", objOrder.ClientMobileNo).Replace("--ITEMLIST--", ItemHtmls).Replace("--GSTCALCULATIONDATA--", GST_HTML_DATA).Replace("--SHIPPING--", Math.Round(objOrder.ShipmentCharge, 2).ToString()).Replace("--SUBTOTAL--", Math.Round(SubTotal, 2).ToString()).Replace("--TOTAL--", Math.Round(TotalFinal, 2).ToString()).Replace("--EXTRAAMOUNT--", Math.Round(objOrder.ExtraAmount, 2).ToString()).Replace("--ROUNDOFF--", Math.Round(RoundedAmt, 2).ToString()).Replace("--ROUNDTOTAL--", Math.Round(RoundAmt, 2).ToString());

            }

            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set browser width
            htmlToPdfConverter.BrowserWidth = 1200;

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;

            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);

            // convert HTML code to a PDF memory buffer
            // htmlToPdfConverter.ConvertHtmlToFile(htmldata, "", Server.MapPath("~/Certificates/") + flName);
            // convert HTML to PDF
            byte[] pdfBuffer = null;
            pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(newhtmldata, "");
            // inform the browser about the binary data format
            Response.AddHeader("Content-Type", "application/pdf");

            // let the browser know how to open the PDF document, attachment or inline, and the file name
            Response.AddHeader("Content-Disposition", String.Format("{0}; filename=HtmlToPdf.pdf; size={1}",
               "inline", pdfBuffer.Length.ToString()));

            // write the PDF buffer to HTTP response
            Response.BinaryWrite(pdfBuffer);

            // call End() method of HTTP response to stop ASP.NET page processing
            Response.End();
        }
    }
}