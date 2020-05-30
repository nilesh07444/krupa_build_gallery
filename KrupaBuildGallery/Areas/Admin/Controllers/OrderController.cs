using ConstructionDiary.Models;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.ViewModel;
using System.Net;
using System.Configuration;
using Razorpay.Api;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{    
    [CustomAuthorize] 
    public class OrderController : Controller
    {
		private readonly krupagallarydbEntities _db;
        public OrderController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Order
        public ActionResult Index(int Status = -1)
        {
            
            List<OrderVM> lstOrders = new List<OrderVM>();
            try
            {

                lstOrders = (from p in _db.tbl_Orders
                             join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                               where !p.IsDelete && (Status == -1 || Status == 10 || Status == 11 || p.OrderStatusId == Status)
                             select new OrderVM
                               {
                                   OrderId = p.OrderId,
                                   ClientUserName = c.FirstName+" "+c.LastName,
                                   ClientUserId = p.ClientUserId,
                                   OrderAmount = p.OrderAmount,
                                   OrderShipCity = p.OrderShipCity,
                                   OrderShipState = p.OrderShipState,
                                   OrderShipAddress = p.OrderShipAddress,
                                   OrderPincode = p.OrderShipPincode,
                                   OrderShipClientName = p.OrderShipClientName,
                                   OrderShipClientPhone = p.OrderShipClientPhone,
                                   OrderStatusId =  p.OrderStatusId,       
                                   PaymentType = p.PaymentType,
                                   OrderDate = p.CreatedDate,
                                   IsCashOnDelivery = p.IsCashOnDelivery.HasValue ? p.IsCashOnDelivery.Value : false,
                                   OrderTypeId = p.OrderType.HasValue ? p.OrderType.Value : 1,
                                   ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                                   ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                               }).OrderByDescending(x => x.OrderDate).ToList();

                if(lstOrders != null && lstOrders.Count() > 0)
                {
                    lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
                }
                if(Status == 10)
                {
                    lstOrders = lstOrders.Where(o => o.IsCashOnDelivery == true).ToList();
                }
                else if(Status == 11)
                {
                    lstOrders = lstOrders.Where(o => o.OrderTypeId == 2).ToList();
                }
                ViewBag.Status = Status;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstOrders);
        }

        public ActionResult Detail(long Id)
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
                             ClientRoleId = c.ClientRoleId,
                             ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                             ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2,
                             CreditUsed = p.CreditAmountUsed.HasValue ? p.CreditAmountUsed.Value : 0,
                             WalletAmtUsed = p.WalletAmountUsed.HasValue ? p.WalletAmountUsed.Value : 0,
                             OrderTypeId = p.OrderType.HasValue ? p.OrderType.Value : 1,
                             ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0
                         }).OrderByDescending(x => x.OrderDate).FirstOrDefault();          
            if(objOrder != null)
            {   
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                            join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                            join vr in _db.tbl_ItemVariant on p.VariantItemId equals vr.VariantItemId
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
                                ItemStatus = p.ItemStatus.Value,
                                VariantQtytxt = vr.UnitQty,
                                Discount = p.Discount.HasValue ? p.Discount.Value : 0
                            }).OrderByDescending(x => x.OrderItemId).ToList();
                if (lstOrderItms != null && lstOrderItms.Count() > 0)
                {
                    lstOrderItms.ForEach(x => x.ItemStatustxt = GetItemStatus(x.ItemStatus));
                }
                objOrder.OrderItems = lstOrderItms;
            }
            return View(objOrder);
        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }

        [HttpPost]
        public string ChangeOrderStatus(long OrderId,int Status,string Dispatchtime)
        {
            tbl_Orders objordr =  _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            clsCommon objcmn = new clsCommon();
            if (objordr != null)
            {
                objordr.OrderStatusId = Status;
                long clientusrid = objordr.ClientUserId;
                _db.SaveChanges();
                if(Status == 2)
                {
                    tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    List<tbl_OrderItemDetails> lstItms = _db.tbl_OrderItemDetails.Where(o => o.OrderId == OrderId).ToList();
                    if(lstItms != null && lstItms.Count() > 0)
                    {
                        foreach(tbl_OrderItemDetails ob in lstItms)
                        {
                            if(ob.ItemStatus == 1)
                            {
                                ob.ItemStatus = 2;
                            }
                            objcmn.SaveTransaction(ob.ProductItemId.Value,ob.OrderDetailId, ob.OrderId.Value, "Change Item Status to Confirm", ob.FinalItemPrice.Value,0,clsAdminSession.UserID, DateTime.UtcNow, "Item Status Change");
                        }
                        _db.SaveChanges();
                    }
                    if (objclntusr != null)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            string msg = "Your order no."+ objordr.OrderId+" has been confirmed. We will dispatch your order within " + Dispatchtime;
                            string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objclntusr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                return "InvalidNumber";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(objclntusr.Email))
                                {
                                    tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                                    string FromEmail = objGensetting.FromEmail;
                                  
                                    string msg1 = "Your order #" + objordr.OrderId + " has been confirmed. We will dispatch your order within " + Dispatchtime;
                                    clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order has been confirmed - Krupa Build Gallery", msg1);
                                }
                            }

                        }
                    }
                }
                else if(Status == 3)
                {
                    tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    if (objclntusr != null)
                    {
                        using (WebClient webClient = new WebClient())
                        {                            
                         
                            string msg = "Your order no." + objordr.OrderId + " has been dispatched";
                            string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objclntusr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                return "InvalidNumber";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(objclntusr.Email))
                                {
                                    tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                                    string FromEmail = objGensetting.FromEmail;
                                  
                                    string msg1 = "Your order #" + objordr.OrderId + " has been dispatched";
                                    clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order has been dispatched - Krupa Build Gallery", msg1);
                                }
                            }

                        }
                    }
                }              
            }
            
            return "";
        }

        [HttpPost]
        public string SetShipCharge(long OrderId,decimal ShippingCharge)
        {
            tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if(objordr != null)
            {
                objordr.ShippingCharge = ShippingCharge;
            }
            long clientusrid = objordr.ClientUserId;
            tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
            if (objclntusr != null)
            {
                using (WebClient webClient = new WebClient())
                {
                    string msg = "Shipping Charges for Your order no." + objordr.OrderId + " is: Rs " + ShippingCharge + ". Please pay from your order details you can find button to pay.";
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objclntusr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(objclntusr.Email))
                        {
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;

                            string msg1 = "Shipping Charges for Your order no." + objordr.OrderId + " is: Rs " + ShippingCharge + ". Please pay from your order details you can find button to pay.";
                             clsCommon.SendEmail(objclntusr.Email, FromEmail, "Shipping Charge - Krupa Build Gallery", msg1);
                           
                        }
                    }

                }
            }
            _db.SaveChanges();
            return "";
        }

        public ActionResult OrderItemClientRequests(int Status = 0)
        {

            List<OrderItemRequestsVM> lstItemClientRequests = new List<OrderItemRequestsVM>();
            try
            {
                lstItemClientRequests = (from p in _db.tbl_ItemReturnCancelReplace
                             join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                             join ordritems in _db.tbl_OrderItemDetails on p.ItemId equals ordritems.OrderDetailId
                             join Itm in _db.tbl_ProductItems on ordritems.ProductItemId equals Itm.ProductItemId
                             where (Status == 0 && p.IsApproved == null) || (p.ItemStatus == Status && p.IsApproved == true)
                             select new OrderItemRequestsVM
                             {
                                 OrderItemRequestId = p.ItemReturnCancelReplaceId,
                                 OrderId = p.OrderId.Value,                                 
                                 ItemName = Itm.ItemName,
                                 Amount = p.Amount.Value,
                                 Reason = p.Reason,
                                 OrderItemId = p.ItemId.Value,
                                 ItemStatus = p.ItemStatus.Value,                              
                                 DateCreated = p.DateCreated.Value,
                                 OrderItemStatus = ordritems.ItemStatus.Value
                             }).OrderByDescending(x => x.DateCreated).ToList();

              
                ViewBag.Status = Status;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstItemClientRequests);
        }

        [HttpPost]
        public string ApproveRejectItemRequest(string requestid,string aprprovereject)
        {
            long ItmRequestId = Convert.ToInt64(requestid);
            string msgsms = "";
            tbl_ItemReturnCancelReplace objReq =_db.tbl_ItemReturnCancelReplace.Where(o => o.ItemReturnCancelReplaceId == ItmRequestId).FirstOrDefault();
            tbl_ClientUsers objClient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == objReq.ClientUserId).FirstOrDefault();
            tbl_OrderItemDetails objOrderItm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == objReq.ItemId).FirstOrDefault();
            clsCommon objCommon = new clsCommon();
            if (aprprovereject == "reject")
            {
                objReq.IsApproved = false;
                string mobilenumber = objClient.MobileNo;
                if (objReq.ItemStatus == 6)
                {
                    msgsms = "Item Return Request Rejected for Order No." + objReq.OrderId;
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Return Request Rejected", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Reject Return Item Request");
                    objOrderItm.ItemStatus = 4;                    
                }
                else if (objReq.ItemStatus == 7)
                {
                    objOrderItm.ItemStatus = 4;
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Replace Request Rejected", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Reject Replace Item Request");
                    msgsms = "Item Replace Request Rejected for Order No." + objReq.OrderId;                   
                }
                else if (objReq.ItemStatus == 8)
                {
                    objOrderItm.ItemStatus = 4;
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Exchange Request Rejected", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Reject Exchange Item Request");
                    msgsms = "Item Exchange Request Rejected for Order No." + objReq.OrderId;                   
                }
                objReq.DateModified = DateTime.UtcNow;
                objReq.ModifiedBy = clsAdminSession.UserID;
                _db.SaveChanges();
                SendMessageSMS(mobilenumber, msgsms);

            }
            else
            {
                string mobilenumber = objClient.MobileNo;
                if (objReq.ItemStatus == 6)
                {
                    decimal amtCrd = 0;
                    decimal amronl = 0;
                    decimal amtwlt1 = 0;
                    tbl_Orders objtbl_Orders = _db.tbl_Orders.Where(o => o.OrderId == objReq.OrderId).FirstOrDefault();
                    if(objtbl_Orders != null)
                    {
                        decimal amtcut = 0;
                        if(objtbl_Orders.OrderShipPincode == "389001")
                        {
                            amtcut = Math.Round((objOrderItm.FinalItemPrice.Value * 5) / 100, 2);
                        }
                        else
                        {
                            amtcut = Math.Round((objOrderItm.FinalItemPrice.Value * 7) / 100, 2);
                        }
                        decimal refundamtt = objOrderItm.FinalItemPrice.Value - amtcut;
                        decimal remaing = refundamtt;
                      
                        if(objtbl_Orders.CreditAmountUsed > 0 && remaing > 0 && objtbl_Orders.IsCashOnDelivery == false)
                        {
                            decimal credtrefuned = objtbl_Orders.CreditAmountRefund.HasValue ? objtbl_Orders.CreditAmountRefund.Value : 0;
                            decimal remaingtorefund = objtbl_Orders.CreditAmountUsed.Value - credtrefuned;
                            decimal refndToCredit = 0;
                            tbl_ClientOtherDetails objClientOthr = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == objtbl_Orders.ClientUserId).FirstOrDefault();
                            if (objClientOthr != null)
                            {
                                if (remaing <= remaingtorefund)
                                {
                                    refndToCredit = remaing;
                                    if(objtbl_Orders.AmountDue >= refndToCredit)
                                    {
                                        objtbl_Orders.AmountDue = objtbl_Orders.AmountDue - refndToCredit;
                                        objClientOthr.AmountDue = objClientOthr.AmountDue - refndToCredit;
                                        objtbl_Orders.CreditAmountRefund = credtrefuned + refndToCredit;
                                        amtCrd = refndToCredit;
                                        remaing = 0;
                                    }
                                    else
                                    {
                                        decimal amtduee = objtbl_Orders.AmountDue.Value;
                                        objClientOthr.AmountDue = objClientOthr.AmountDue - amtduee;
                                        objtbl_Orders.AmountDue = 0;                                      
                                        objtbl_Orders.CreditAmountRefund = credtrefuned + amtduee;
                                        remaing = remaing - amtduee;
                                        amtCrd = amtduee;
                                    }
                                 
                                }
                                else
                                {
                                    if (objtbl_Orders.AmountDue >= remaingtorefund)
                                    {
                                        objtbl_Orders.AmountDue = objtbl_Orders.AmountDue - remaingtorefund;
                                        objClientOthr.AmountDue = objClientOthr.AmountDue - remaingtorefund;
                                        objtbl_Orders.CreditAmountRefund = credtrefuned + remaingtorefund;
                                        remaing = remaing - remaingtorefund;
                                        amtCrd = remaingtorefund;
                                    }
                                    else
                                    {
                                        decimal amtduee = objtbl_Orders.AmountDue.Value;
                                        objClientOthr.AmountDue = objClientOthr.AmountDue - amtduee;
                                        objtbl_Orders.AmountDue = 0;
                                        objtbl_Orders.CreditAmountRefund = credtrefuned + amtduee;
                                        remaing = remaing - amtduee;
                                        amtCrd = amtduee;
                                    }                                  
                                }                                                             
                            }
                        }
                        if ((objtbl_Orders.WalletAmountUsed > 0 && remaing > 0) || (objtbl_Orders.IsCashOnDelivery == true))
                        {
                            decimal wltamtrefuned = objtbl_Orders.WalletAmountRefund.HasValue ? objtbl_Orders.WalletAmountRefund.Value : 0;
                            decimal remaingtorefund = objtbl_Orders.WalletAmountUsed.Value - wltamtrefuned;
                            decimal refndTowallet = 0;
                            if(remaing <= remaingtorefund)
                            {
                                refndTowallet = remaing;
                                remaing = 0;
                            }
                            else
                            {
                                remaing = remaing - remaingtorefund;
                                refndTowallet = remaingtorefund;
                            }
                            objtbl_Orders.WalletAmountRefund = wltamtrefuned + refndTowallet;
                            tbl_Wallet objWlt = new tbl_Wallet();
                            objWlt.Amount = refndTowallet;
                            objWlt.CreditDebit = "Credit";
                            objWlt.ItemId = objReq.ItemId;
                            objWlt.OrderId = objReq.OrderId;
                            objWlt.ClientUserId = objReq.ClientUserId;
                            objWlt.WalletDate = DateTime.UtcNow;
                            objWlt.Description = "Amount Refund to Wallet Order #" + objReq.OrderId;
                            _db.tbl_Wallet.Add(objWlt);
                            if (objClient != null)
                            {
                                decimal amtwlt = objClient.WalletAmt.HasValue ? objClient.WalletAmt.Value : 0;
                                amtwlt = amtwlt + refndTowallet;
                                objClient.WalletAmt = amtwlt;
                                amtwlt1 = refndTowallet;
                                _db.SaveChanges();
                            }
                        }
                        if (remaing > 0 && objtbl_Orders.IsCashOnDelivery == false)
                        {
                            //decimal onlnamtrefuned = objtbl_Orders.OnlinePaymentAmtRefund.HasValue ? objtbl_Orders.OnlinePaymentAmtRefund.Value : 0;
                            //decimal remaingtorefund = objtbl_Orders.AmountByRazorPay.Value - onlnamtrefuned;
                            //decimal refndToonlin = 0;
                            //if (remaing <= remaingtorefund)
                            //{
                              //  refndToonlin = remaing;
                               // remaing = 0;
                            //}
                            //else
                            //{
                              //  remaing = remaing - remaingtorefund;
                               // refndToonlin = remaingtorefund;
                            //}
                            //initialize the SDK client
                            string key = "rzp_test_DMsPlGIBp3SSnI";
                            string secret = "YMkpd9LbnaXViePncLLXhqms";
                            RazorpayClient client = new RazorpayClient(key, secret);
                            List<tbl_PaymentHistory> lstPymtn = _db.tbl_PaymentHistory.Where(o => o.PaymentBy == "online" && o.OrderId == objtbl_Orders.OrderId && o.PaymentFor == "OrderPayment").OrderBy(o => o.DateOfPayment).ToList();
                            if (lstPymtn != null && lstPymtn.Count() > 0)
                            {
                                amronl = remaing;
                                foreach (var objPaymen in lstPymtn)
                                {
                                    if(objPaymen.AmountPaid >= remaing)
                                    {
                                        // payment to be refunded, payment must be a captured payment
                                        Payment payment = client.Payment.Fetch(objPaymen.RazorpayPaymentId);
                                        int refundAmtOnline = Convert.ToInt32(Math.Round(remaing,2) * 100);
                                        //Partial Refund
                                        Dictionary<string, object> data = new Dictionary<string, object>();
                                        data.Add("amount", refundAmtOnline);
                                        Refund refund = payment.Refund(data);                                     
                                        break;
                                    }
                                    else
                                    {
                                        // payment to be refunded, payment must be a captured payment
                                        Payment payment = client.Payment.Fetch(objPaymen.RazorpayPaymentId);
                                        int refundAmtOnline = Convert.ToInt32(Math.Round(objPaymen.AmountPaid, 2) * 100);
                                        //Partial Refund
                                        Dictionary<string, object> data = new Dictionary<string, object>();
                                        data.Add("amount", refundAmtOnline);
                                        Refund refund = payment.Refund(data);
                                        remaing = remaing - objPaymen.AmountPaid;                                        
                                    }
                                }
                            }                           
                        }
                    }
                    objReq.IsApproved = true;
                    objReq.DateModified = DateTime.UtcNow;
                    objReq.ModifiedBy = clsAdminSession.UserID;
                    string amtrefundtext = "";
                    if(amtCrd > 0)
                    {
                        amtrefundtext = amtrefundtext + "\n Credit : Rs." + amtCrd;
                    }
                    if (amtwlt1 > 0)
                    {
                        amtrefundtext = amtrefundtext + "\n Wallet : Rs." + amtwlt1;
                    }
                    if (amronl > 0)
                    {
                        amtrefundtext = amtrefundtext + "\n Online : Rs." + amronl;
                    }
                    msgsms = "You Item is Returned for Order No." + objReq.OrderId + " . Amount Refunded to "+ amtrefundtext;
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Return Request Accepted", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Return Item Request");
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Refunded amount to "+ amtrefundtext, 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Return Item Request Refund");
                    SendMessageSMS(mobilenumber, msgsms);
                    _db.SaveChanges();
                }
                else if (objReq.ItemStatus == 7)
                {
                    objReq.IsApproved = true;
                    objReq.DateModified = DateTime.UtcNow;
                    objReq.ModifiedBy = clsAdminSession.UserID;
                    _db.SaveChanges();
                    msgsms = "You Item to Return for Order No." + objReq.OrderId +" is Accepted. You will get Item asap";
                    SendMessageSMS(mobilenumber, msgsms);
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Replace Request Accepted", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Replace Item Request");
                }
                else if (objReq.ItemStatus == 8)
                {
                    objReq.IsApproved = true;
                    objOrderItm.IsDelete = true;
                    decimal amt = objReq.Amount.Value;
                    decimal deprc = Math.Round((objReq.Amount.Value * 3) / 100, 2);
                    decimal amtredund = amt - deprc;
                    tbl_Wallet objWlt = new tbl_Wallet();
                    objWlt.Amount = amtredund;
                    objWlt.CreditDebit = "Credit";
                    objWlt.ItemId = objReq.ItemId;
                    objWlt.OrderId = objReq.OrderId;
                    objWlt.ClientUserId = objReq.ClientUserId;
                    objWlt.WalletDate = DateTime.UtcNow;
                    objWlt.Description = "Amount Refund to Wallet Order #" + objReq.OrderId;
                    _db.tbl_Wallet.Add(objWlt);                   
                    if (objClient != null)
                    {
                        decimal amtwlt = objClient.WalletAmt.HasValue ? objClient.WalletAmt.Value : 0;
                        amtwlt = amtwlt + amtredund;
                        objClient.WalletAmt = amtwlt;
                        _db.SaveChanges();
                    }
                    objReq.DateModified = DateTime.UtcNow;
                    objReq.ModifiedBy = clsAdminSession.UserID;
                    _db.SaveChanges();
                    msgsms = "You Item is Exchanged for Order No." + objReq.OrderId + " . Amount Rs." + amtredund + " Refunded to your wallet";
                    SendMessageSMS(mobilenumber, msgsms);
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Exchanged Request Accepted", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Exchanged Item Request");
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Amount Rs." + amtredund + " Refunded to your wallet", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Exchanged Item Request Refund");
                }
            }
             

            return "Success";
        }

        public string SendMessageSMS(string mobile, string msg)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + mobile + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return "success";
                    }

                }
            }
            catch (WebException ex)
            {
                return "fail";
            }
        }

        public string GetItemStatus(long itemstatusid)
        {
            return Enum.GetName(typeof(OrderItemStatus), itemstatusid);
        }

        public ActionResult GetAndAssignDeliveryPerson()
        {
          List<AdminUserVM> lstAdminUsers = (from a in _db.tbl_AdminUsers
                             join r in _db.tbl_AdminRoles on a.AdminRoleId equals r.AdminRoleId
                             where !a.IsDeleted
                             select new AdminUserVM
                             {
                                 AdminUserId = a.AdminUserId,
                                 AdminRoleId = a.AdminRoleId,
                                 RoleName = r.AdminRoleName,
                                 FirstName = a.FirstName,
                                 LastName = a.LastName,
                                 Email = a.Email,
                                 MobileNo = a.MobileNo,
                                 ProfilePicture = a.ProfilePicture,
                                 IsActive = a.IsActive
                             }).ToList();
            ViewData["lstAdminUsers"] = lstAdminUsers;
            return PartialView("~/Areas/Admin/Views/Order/_AssignDeliveryPerson.cshtml");
        }

        [HttpPost]
        public string AssignDeliveryPerson(long OrderId,long OrderItemId,long PersonId)
        {
            clsCommon objCommon = new clsCommon();
            tbl_OrderItemDetails objOrderItm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == OrderItemId).FirstOrDefault();
            tbl_AdminUsers objAdminUsr = _db.tbl_AdminUsers.Where(o => o.AdminUserId == PersonId).FirstOrDefault();
            objOrderItm.ItemStatus = 3;
            _db.SaveChanges();
            tbl_Orders objOrdr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if (objOrdr.OrderStatusId == 2)
            {
                List<tbl_OrderItemDetails> lstOrderTms = _db.tbl_OrderItemDetails.Where(o => o.OrderId == OrderId && o.ItemStatus != 5 && o.ItemStatus != 3).ToList();
                if(lstOrderTms == null || lstOrderTms.Count == 0)
                {
                    objOrdr.OrderStatusId = 3;
                }
            }
            tbl_OrderItemDelivery objOrderItmDlv = new tbl_OrderItemDelivery();
            objOrderItmDlv.OrderId = OrderId;
            objOrderItmDlv.OrderItemId = OrderItemId;
            objOrderItmDlv.Status = 3;
            objOrderItmDlv.DelieveryPersonId = PersonId;
            objOrderItmDlv.AssignedBy = clsAdminSession.UserID;
            objOrderItmDlv.AssignedDate = DateTime.UtcNow;
            _db.tbl_OrderItemDelivery.Add(objOrderItmDlv);
            _db.SaveChanges();
            objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value,"Delivery Person "+ objAdminUsr.FirstName+" "+ objAdminUsr.LastName+" Assign to Dispatch Item", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Item Status Changed");
            tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == objOrdr.ClientUserId).FirstOrDefault();
            if (objclntusr != null)
            {
                using (WebClient webClient = new WebClient())
                {

                    string msg = "Your order no." + objOrdr.OrderId + "\n Item: " + objOrderItm.ItemName + "\n has been dispatched";
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objclntusr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(objclntusr.Email))
                        {
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;

                            string msg1 = "Your order #" + objOrdr.OrderId + "Item: "+objOrderItm.ItemName+ "\n has been dispatched";
                            clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order has been dispatched - Krupa Build Gallery", msg1);
                        }
                    }

                }

                using (WebClient webClient = new WebClient())
                {
                    string msg = "Order no." + objOrdr.OrderId + " \nItem: " + objOrderItm.ItemName + " \nhas been assigned to you for delivery";
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objAdminUsr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(objAdminUsr.Email))
                        {
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;

                            string msg1 = "Order no." + objOrdr.OrderId + "Item: " + objOrderItm.ItemName + "has been assigned to you for delivery";
                            clsCommon.SendEmail(objAdminUsr.Email, FromEmail, "Assigned New Item Delivery - Krupa Build Gallery", msg1);
                        }
                    }

                }
            }

            return "Success";
        }


    }
}