﻿using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class OrdersController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public OrdersController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Orders
        public ActionResult Index()
        {
            
            List<OrderVM> lstOrders = new List<OrderVM>();
            try
            {
                long UserId = clsClientSession.UserID;
                lstOrders = (from p in _db.tbl_Orders
                             join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                             where !p.IsDelete && p.ClientUserId == UserId
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
                                 OrderDate = p.CreatedDate
                             }).OrderByDescending(x => x.OrderDate).ToList();

                if (lstOrders != null && lstOrders.Count() > 0)
                {
                    lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstOrders);
        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }
        public ActionResult OrderDetails(int Id)
        {
            OrderVM objOrder = new OrderVM();
            objOrder = (from p in _db.tbl_Orders
                        join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                        where p.OrderId == Id
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
                            OrderDate = p.CreatedDate,
                            ClientEmail = c.Email,
                            ClientMobileNo = c.MobileNo,
                            OrderAmountDue = p.AmountDue.HasValue ?p.AmountDue.Value : 0,
                            ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                            ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2                            
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();
            if (objOrder != null)
            {
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                   join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                   where p.OrderId == Id
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
                                                       Discount = p.Discount.HasValue ? p.Discount.Value : 0
                                                   }).OrderByDescending(x => x.OrderItemId).ToList();
                objOrder.OrderItems = lstOrderItms;
            }

            if(objOrder.ShipmentCharge > 0)
            {
                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", objOrder.ShipmentCharge * 100); // this amount should be same as transaction amount
                input.Add("currency", "INR");
                input.Add("receipt", "12121");
                input.Add("payment_capture", 1);

                string key = "rzp_test_DMsPlGIBp3SSnI";
                string secret = "YMkpd9LbnaXViePncLLXhqms";

                RazorpayClient client = new RazorpayClient(key, secret);

                Razorpay.Api.Order order = client.Order.Create(input);
                ViewBag.OrderId = order["id"];
                ViewBag.Description = "Shipping Charge for Order #"+ objOrder.OrderId;
                ViewBag.Amount = objOrder.ShipmentCharge * 100;
            }
            else
            {
                ViewBag.OrderId = 0;
                ViewBag.Description = "Shipping Charge for Order #" + objOrder.OrderId;
                ViewBag.Amount = objOrder.ShipmentCharge * 100;
            }

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

            ViewData["lstPaymentHist"] = lstPaymentHist;
            ViewData["orderobj"] = _db.tbl_Orders.Where(o => o.OrderId == Id).FirstOrDefault();
            return View(objOrder);
        }

        [HttpPost]
        public string SaveShippingCharge(string razorpymentid,string razororderid,string razorsignature,string orderid)
        {
            long OrderID64 = Convert.ToInt64(orderid);
          
            Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(razorpymentid);
            if (objpymn != null)
            {
                if (objpymn["status"] != null && Convert.ToString(objpymn["status"]) == "captured")
                {
                    tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == OrderID64).FirstOrDefault();
                    if (objordr != null)
                    {
                        objordr.ShippingStatus = 2;
                    }
                    _db.SaveChanges();
                    string paymentmethod = objpymn["method"];
                    tbl_PaymentHistory objPayment = new tbl_PaymentHistory();
                    objPayment.AmountPaid = objordr.ShippingCharge.Value;
                    objPayment.AmountDue = objordr.ShippingCharge.Value;
                    objPayment.DateOfPayment = DateTime.UtcNow;
                    objPayment.OrderId = objordr.OrderId;
                    objPayment.PaymentBy = paymentmethod;
                    objPayment.CreatedBy = clsClientSession.UserID;
                    objPayment.CreatedDate = DateTime.UtcNow;
                    objPayment.RazorpayOrderId = razororderid;
                    objPayment.RazorpayPaymentId = razorpymentid;
                    objPayment.RazorSignature = razorsignature;
                    objPayment.PaymentFor = "ShippingCharge";
                    _db.tbl_PaymentHistory.Add(objPayment);
                    _db.SaveChanges();
                    return "Success";
                }            
            }

            return "";
        }

        [HttpPost]
        public string MakePayment(string razorpymentid,string razororderid, string razorsignature, string orderid,string amount)
        {
            long orderid64 = Convert.ToInt64(orderid); 
            var objOrder = _db.tbl_Orders.Where(o => o.OrderId == orderid64).FirstOrDefault();
            decimal amountdue = 0;
            decimal amountpaid = Convert.ToDecimal(amount);
            if (objOrder != null)
            {
                amountdue = objOrder.AmountDue.Value;
                objOrder.AmountDue = amountdue - amountpaid;
                long ClientUserId = objOrder.ClientUserId;
                tbl_ClientOtherDetails objtbl_ClientOtherDetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                objtbl_ClientOtherDetails.AmountDue = objtbl_ClientOtherDetails.AmountDue - amountpaid;
                _db.SaveChanges();
            }

            Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(razorpymentid);
            if (objpymn != null)
            {
                if (objpymn["status"] != null && Convert.ToString(objpymn["status"]) == "captured")
                {                  
                    string paymentmethod = objpymn["method"];
                    tbl_PaymentHistory objPayment = new tbl_PaymentHistory();
                    objPayment.AmountPaid = amountpaid;
                    objPayment.AmountDue = amountdue;
                    objPayment.DateOfPayment = DateTime.UtcNow;
                    objPayment.OrderId = orderid64;
                    objPayment.PaymentBy = paymentmethod;
                    objPayment.CreatedBy = clsClientSession.UserID;
                    objPayment.CreatedDate = DateTime.UtcNow;
                    objPayment.RazorpayOrderId = razororderid;
                    objPayment.RazorpayPaymentId = razorpymentid;
                    objPayment.RazorSignature = razorsignature;
                    objPayment.PaymentFor = "Order Amount";
                    _db.tbl_PaymentHistory.Add(objPayment);
                    _db.SaveChanges();
                    return "Success";
                }
            }

            return "Success";
        }

        public PartialViewResult CreateRazorPaymentOrder(decimal Amount, string description)
        {
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", Amount * 100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", "12000");
            input.Add("payment_capture", 1);

            string key = "rzp_test_DMsPlGIBp3SSnI";
            string secret = "YMkpd9LbnaXViePncLLXhqms";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.OrderId = order["id"];
            ViewBag.Description = description;
            ViewBag.Amount = Amount * 100;
            return PartialView("~/Areas/Client/Views/Orders/_Razorpaymentpartial.cshtml");
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
    }
}