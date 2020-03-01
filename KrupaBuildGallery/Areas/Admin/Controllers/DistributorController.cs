﻿using ConstructionDiary.Models;
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
        public ActionResult Index()
        {
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
            try
            {

                lstClientUser = (from cu in _db.tbl_ClientUsers
                                 join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                                 where !cu.IsDelete && cu.ClientRoleId == 2
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
                                     GSTNo = co.GSTno
                                 }).OrderBy(x => x.FirstName).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstClientUser);
        }

        public ActionResult RequestList()
        {
            List<DistributorRequestVM> lstDistriRequest = new List<DistributorRequestVM>();
            try
            {

                lstDistriRequest = (from cu in _db.tbl_DistributorRequestDetails                                  
                                 where !cu.IsDelete.Value
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
                                     GSTNo = cu.GSTNo
                                 }).OrderBy(x => x.FirstName).ToList();

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
                                 GSTNo = co.GSTno == "" ?"N/A":co.GSTno
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
                            OrderDate = p.CreatedDate
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
           ViewData["orderobj"] =_db.tbl_Orders.Where(o => o.OrderId == Id).FirstOrDefault();
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

            var objOrder =_db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if(objOrder != null)
            {
                objOrder.AmountDue = AmountDue - AmountPaid;
                long ClientUserId = objOrder.ClientUserId;
                tbl_ClientOtherDetails objtbl_ClientOtherDetails =_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
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
                string file = Server.MapPath("~/ReceiptLatest.html");
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
                            ClientAddress = u.Address +", "+u.City,
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
                            OrderDate = p.CreatedDate
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();
            if (objOrder != null)
            {
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                   join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
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
                                                       GST_Per = c.GST_Per                                                       
                                                   }).OrderByDescending(x => x.OrderItemId).ToList();
                objOrder.OrderItems = lstOrderItms;
                string file = Server.MapPath("~/Invoice.html");
                string htmldata = "";

                FileInfo fi = new FileInfo(file);
                sr = System.IO.File.OpenText(file);
                htmldata += sr.ReadToEnd();
                string InvoiceNo = "INV-" + objOrder.OrderId;
                string DateOfInvoice = objOrder.OrderDate.ToString("dd-MM-yyyy");
                string orderNo = objOrder.OrderId.ToString(); ;
                string ClientUserName = objOrder.ClientUserName;
                string ItemHtmls = "";
                decimal TotalFinal = 0;
                StringBuilder srBuild = new StringBuilder();
                if(lstOrderItms != null && lstOrderItms.Count() > 0)
                {
                    int cntsrNo = 1;
                 
                    foreach(var objItem in lstOrderItms)
                    {
                        decimal InclusiveGST = Math.Round(objItem.Price - objItem.Price * (100 / (100 + objItem.GST_Per)),2);
                        decimal PreGSTPrice =  Math.Round(objItem.Price - InclusiveGST,2);
                        decimal basicTotalPrice = Math.Round(PreGSTPrice * objItem.Qty,2);
                        decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2),2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round((basicTotalPrice * SGST) / 100,2);
                        decimal CGSTAmt = Math.Round((basicTotalPrice * CGST) / 100,2);
                        decimal FinalPrice = Math.Round(basicTotalPrice + SGSTAmt + CGSTAmt,2);
                        TotalFinal = TotalFinal + FinalPrice;
                        srBuild.Append("<tr>");
                        srBuild.Append("<td>"+ cntsrNo + "</td>");
                        srBuild.Append("<td>"+objItem.ItemName+"</td>");
                        srBuild.Append("<td class=\"text-center\">"+objItem.Qty+"</td>");
                        srBuild.Append("<td class=\"text-center\">"+ PreGSTPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + CGST + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + CGSTAmt + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + SGST + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + SGSTAmt + "</td>");
                        srBuild.Append("<td class=\"text-center\">" +Math.Round(FinalPrice,2) + "</td>");
                        srBuild.Append("</tr>");
                        cntsrNo = cntsrNo + 1;


                    }
                }
                ItemHtmls = srBuild.ToString();
                newhtmldata = htmldata.Replace("--INVOICENO--", InvoiceNo).Replace("--INVOICEDATE--", DateOfInvoice).Replace("--ORDERNO--", orderNo).Replace("--CLIENTUSERNAME--", ClientUserName).Replace("--CLIENTUSERADDRESS--", objOrder.ClientAddress).Replace("--CLIENTUSEREMAIL--", objOrder.ClientEmail).Replace("--CLIENTUSERMOBILE--", objOrder.ClientMobileNo).Replace("--ITEMLIST--",ItemHtmls).Replace("--TOTAL--",Math.Round(TotalFinal,2).ToString());

            }
          
            return newhtmldata;
            //return "Receipt_" + objPymt.PaymentHistory_Id+".pdf";
        }
    }
}