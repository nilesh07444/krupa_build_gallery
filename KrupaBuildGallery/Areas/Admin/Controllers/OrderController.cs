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
using OfficeOpenXml;
using System.Text;
using OfficeOpenXml.Style;
using System.IO;
using System.Drawing;

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
                                 ClientUserName = c.FirstName + " " + c.LastName,
                                 ClientUserId = p.ClientUserId,
                                 OrderAmount = p.OrderAmount + (p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0) + (p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0),
                                 OrderShipCity = p.OrderShipCity,
                                 OrderShipState = p.OrderShipState,
                                 OrderShipAddress = p.OrderShipAddress,
                                 OrderPincode = p.OrderShipPincode,
                                 OrderShipClientName = p.OrderShipClientName,
                                 OrderShipClientPhone = p.OrderShipClientPhone,
                                 OrderStatusId = p.OrderStatusId,
                                 PaymentType = p.PaymentType,
                                 OrderDate = p.CreatedDate,
                                 IsCashOnDelivery = p.IsCashOnDelivery.HasValue ? p.IsCashOnDelivery.Value : false,
                                 OrderTypeId = p.OrderType.HasValue ? p.OrderType.Value : 1,
                                 ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                                 ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                             }).OrderByDescending(x => x.OrderDate).ToList();

                if (lstOrders != null && lstOrders.Count() > 0)
                {
                    lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
                }
                if (Status == 10)
                {
                    lstOrders = lstOrders.Where(o => o.IsCashOnDelivery == true).ToList();
                }
                else if (Status == 11)
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
                            OrderAmount = p.OrderAmount + (p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0) + (p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0),
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
                            OrderAmountDue = p.AmountDue.HasValue ? p.AmountDue.Value : 0,
                            WalletAmtUsed = p.WalletAmountUsed.HasValue ? p.WalletAmountUsed.Value : 0,
                            OrderTypeId = p.OrderType.HasValue ? p.OrderType.Value : 1,
                            ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0,
                            AdvancePay = p.AdvancePaymentRecieved.HasValue ? p.AdvancePaymentRecieved.Value : 0,
                            Remarks = p.Remarks
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();
            if (objOrder != null)
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
                                                       IsDeleted = p.IsDelete,
                                                       ItemStatus = p.ItemStatus.Value,
                                                       FinalAmt = p.FinalItemPrice.Value,
                                                       VariantQtytxt = vr.UnitQty,
                                                       Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                       IsCombo = p.IsCombo.HasValue ? p.IsCombo.Value : false,
                                                       ComboId = p.ComboId.HasValue ? p.ComboId.Value : 0,
                                                       IsMainItem = p.IsMainItem.HasValue ? p.IsMainItem.Value : false,
                                                       ComboName = p.ComboOfferName,
                                                       ComboQty = p.ComboQty.HasValue ? p.ComboQty.Value : p.Qty.Value
                                                   }).OrderBy(x => x.OrderItemId).ToList();
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
        public string ChangeOrderStatus(long OrderId, int Status, string Dispatchtime)
        {
            tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            clsCommon objcmn = new clsCommon();
            if (objordr != null)
            {
                objordr.OrderStatusId = Status;
                long clientusrid = objordr.ClientUserId;
                _db.SaveChanges();
                if (Status == 2)
                {
                    tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    List<tbl_OrderItemDetails> lstItms = _db.tbl_OrderItemDetails.Where(o => o.OrderId == OrderId).ToList();
                    if (lstItms != null && lstItms.Count() > 0)
                    {
                        foreach (tbl_OrderItemDetails ob in lstItms)
                        {
                            if (ob.ItemStatus == 1)
                            {
                                ob.ItemStatus = 2;
                            }
                            objcmn.SaveTransaction(ob.ProductItemId.Value, ob.OrderDetailId, ob.OrderId.Value, "Change Item Status to Confirm", ob.FinalItemPrice.Value, 0, clsAdminSession.UserID, DateTime.UtcNow, "Item Status Change");
                        }
                        _db.SaveChanges();
                    }
                    if (objclntusr != null)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            string msg = "Your Order No.: " + objordr.OrderId + " Has Been Confirmed. We Will Dispatch Your Order Within " + Dispatchtime;
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

                                    string msg1 = "Your Order No.: " + objordr.OrderId + " Has Been Confirmed. We Will Dispatch Your Order Within " + Dispatchtime;
                                    clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order Has Been Confirmed - Shopping & Saving", msg1);
                                }
                            }

                        }
                    }
                }
                else if (Status == 3)
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

                                    string msg1 = "Your Order No.: " + objordr.OrderId + " Has Been Dispatched";
                                    clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order Has Been Dispatched - Shopping & Saving", msg1);
                                }
                            }

                        }
                    }
                }
            }

            return "";
        }



        [HttpPost]
        public string SendMessageForDueAmount(long OrderId)
        {
            tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            clsCommon objcmn = new clsCommon();
            if (objordr != null)
            {
                long clientusrid = objordr.ClientUserId;
                tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                if (objclntusr != null)
                {
                    using (WebClient webClient = new WebClient())
                    {

                        string msg = "Your order no." + objordr.OrderId + ". Please pay remaining amount of order to confirm order.";
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

                                string msg1 = "Your order no." + objordr.OrderId + ". Please pay remaining amount of order to confirm order.";
                                clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order's Remaining Payment - Shopping & Saving", msg1);
                            }
                        }

                    }
                }

            }

            return "";
        }

        [HttpPost]
        public string SetShipCharge(long OrderId, decimal ShippingCharge)
        {
            tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if (objordr != null)
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

                            string msg1 = "Shipping Charges For Your Order No.: " + objordr.OrderId + " Is: Rs " + ShippingCharge + ". Please Pay From Your Order Details You Can Find Button To Pay.";
                            clsCommon.SendEmail(objclntusr.Email, FromEmail, "Shipping Charge - Shopping & Saving", msg1);

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
        public string ApproveRejectItemRequest(string requestid, string aprprovereject)
        {
            long ItmRequestId = Convert.ToInt64(requestid);
            string msgsms = "";
            tbl_ItemReturnCancelReplace objReq = _db.tbl_ItemReturnCancelReplace.Where(o => o.ItemReturnCancelReplaceId == ItmRequestId).FirstOrDefault();
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
                var objSettings = _db.tbl_GeneralSetting.FirstOrDefault();
                string mobilenumber = objClient.MobileNo;
                if (objReq.ItemStatus == 6)
                {
                    decimal amtCrd = 0;
                    decimal amronl = 0;
                    decimal amtwlt1 = 0;
                    objOrderItm.IsDelete = true;
                    objOrderItm.UpdatedDate = DateTime.UtcNow;
                    tbl_Orders objtbl_Orders = _db.tbl_Orders.Where(o => o.OrderId == objReq.OrderId).FirstOrDefault();
                    if (objtbl_Orders != null)
                    {
                        decimal amtcut = 0;
                        if (objtbl_Orders.OrderShipPincode == "389001")
                        {
                            amtcut = Math.Round((objOrderItm.FinalItemPrice.Value * objSettings.ReturnPerInGodhra.Value) / 100, 2);
                        }
                        else
                        {
                            amtcut = Math.Round((objOrderItm.FinalItemPrice.Value * objSettings.ReturnPerOutGodhra.Value) / 100, 2);
                        }
                        decimal refundamtt = objOrderItm.FinalItemPrice.Value - amtcut;
                        double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(refundamtt));
                        decimal remaing = Convert.ToDecimal(RoundAmt);
                         
                        if (objtbl_Orders.CreditAmountUsed > 0 && remaing > 0 && objtbl_Orders.IsCashOnDelivery == false)
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
                                    if (objtbl_Orders.AmountDue >= refndToCredit)
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
                            if (remaing <= remaingtorefund)
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
                            var objGsetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string key = objGsetting.RazorPayKey;  //"rzp_test_DMsPlGIBp3SSnI";
                            string secret = objGsetting.RazorPaySecret; // "YMkpd9LbnaXViePncLLXhqms";
                            RazorpayClient client = new RazorpayClient(key, secret);
                            List<tbl_PaymentHistory> lstPymtn = _db.tbl_PaymentHistory.Where(o => o.PaymentBy == "online" && o.OrderId == objtbl_Orders.OrderId && o.PaymentFor == "OrderPayment").OrderBy(o => o.DateOfPayment).ToList();
                            if (lstPymtn != null && lstPymtn.Count() > 0)
                            {
                                amronl = remaing;
                                foreach (var objPaymen in lstPymtn)
                                {
                                    if (objPaymen.AmountPaid >= remaing)
                                    {
                                        // payment to be refunded, payment must be a captured payment
                                        Payment payment = client.Payment.Fetch(objPaymen.RazorpayPaymentId);
                                        int refundAmtOnline = Convert.ToInt32(Math.Round(remaing, 2) * 100);
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
                    if (amtCrd > 0)
                    {
                        amtrefundtext = amtrefundtext + "\n Credit : Rs." + amtCrd;
                        objCommon.SavePaymentTransaction(objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, false, amtCrd, "Payment To Credit Refund", clsAdminSession.UserID, true, DateTime.UtcNow, "Credit");
                    }
                    if (amtwlt1 > 0)
                    {
                        amtrefundtext = amtrefundtext + "\n Wallet : Rs." + amtwlt1;
                        objCommon.SavePaymentTransaction(objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, false, amtwlt1, "Payment To Wallet Refund", clsAdminSession.UserID, true, DateTime.UtcNow, "Wallet");
                    }
                    if (amronl > 0)
                    {
                        amtrefundtext = amtrefundtext + "\n Online : Rs." + amronl;
                        objCommon.SavePaymentTransaction(objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, false, amronl, "Payment To Online Refund", clsAdminSession.UserID, true, DateTime.UtcNow, "Online Payment");
                    }

                    msgsms = "You Item is Returned for Order No." + objReq.OrderId + " . Amount Refunded to " + amtrefundtext;
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Return Request Accepted", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Return Item Request");
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Refunded amount to " + amtrefundtext, 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Return Item Request Refund");
                    SendMessageSMS(mobilenumber, msgsms);
                    _db.SaveChanges();
                    tbl_StockReport objstkreport = new tbl_StockReport();
                    objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                    objstkreport.StockDate = DateTime.UtcNow;
                    objstkreport.Qty = Convert.ToInt64(objOrderItm.QtyUsed);
                    objstkreport.IsCredit = true;
                    objstkreport.IsAdmin = false;
                    objstkreport.CreatedBy = clsClientSession.UserID;
                    objstkreport.ItemId = objOrderItm.ProductItemId;
                    objstkreport.Remarks = "Ordered Item Returned:" + objOrderItm.OrderId;
                    _db.tbl_StockReport.Add(objstkreport);
                    _db.SaveChanges();
                }
                else if (objReq.ItemStatus == 7)
                {
                    objReq.IsApproved = true;
                    objReq.DateModified = DateTime.UtcNow;
                    objReq.ModifiedBy = clsAdminSession.UserID;
                    objOrderItm.UpdatedDate = DateTime.UtcNow;
                    objOrderItm.IsReplacedItem = true;
                    _db.SaveChanges();
                    msgsms = "Your Item to Replace for Order No." + objReq.OrderId + " is Accepted. You will get Item asap";
                    SendMessageSMS(mobilenumber, msgsms);
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Replace Request Accepted", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Replace Item Request");
                }
                else if (objReq.ItemStatus == 8)
                {
                    objReq.IsApproved = true;
                    objOrderItm.IsDelete = true;
                    objOrderItm.UpdatedDate = DateTime.UtcNow;
                    decimal amt = objReq.Amount.Value;
                    decimal deprc = Math.Round((objReq.Amount.Value * objSettings.ExchangePer.Value) / 100, 2);
                    decimal amtredund = amt - deprc;
                    double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(amtredund));
                    tbl_Wallet objWlt = new tbl_Wallet();
                    objWlt.Amount = Convert.ToDecimal(RoundAmt);
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
                        amtwlt = amtwlt + Convert.ToDecimal(RoundAmt);
                        objClient.WalletAmt = amtwlt;
                        _db.SaveChanges();
                    }
                    objReq.DateModified = DateTime.UtcNow;
                    objReq.ModifiedBy = clsAdminSession.UserID;
                    _db.SaveChanges();
                    tbl_StockReport objstkreport = new tbl_StockReport();
                    objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                    objstkreport.StockDate = DateTime.UtcNow;
                    objstkreport.Qty = Convert.ToInt64(objOrderItm.QtyUsed);
                    objstkreport.IsCredit = true;
                    objstkreport.IsAdmin = false;
                    objstkreport.CreatedBy = clsClientSession.UserID;
                    objstkreport.ItemId = objOrderItm.ProductItemId;
                    objstkreport.Remarks = "Ordered Item Exchanged:" + objOrderItm.OrderId;
                    _db.tbl_StockReport.Add(objstkreport);
                    _db.SaveChanges();
                    msgsms = "You Item is Exchanged for Order No." + objReq.OrderId + " . Amount Rs." + RoundAmt + " Refunded to your wallet";
                    SendMessageSMS(mobilenumber, msgsms);
                    objCommon.SavePaymentTransaction(objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, false, Convert.ToDecimal(RoundAmt), "Payment To Wallet Refund", clsAdminSession.UserID, true, DateTime.UtcNow, "Wallet");
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Item Exchanged Request Accepted", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Exchanged Item Request");
                    objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Amount Rs." + RoundAmt + " Refunded to your wallet", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Accepted Exchanged Item Request Refund");
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
        public string AssignDeliveryPerson(long OrderId, long OrderItemId, long PersonId)
        {
            clsCommon objCommon = new clsCommon();
            string ItemList = "";
            tbl_OrderItemDetails objOrderItm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == OrderItemId).FirstOrDefault();
            tbl_AdminUsers objAdminUsr = _db.tbl_AdminUsers.Where(o => o.AdminUserId == PersonId).FirstOrDefault();
            tbl_Orders objOrdr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if (objOrderItm.IsCombo == true)
            {
                long comboid = objOrderItm.ComboId.Value;
                List<tbl_OrderItemDetails> lstordritms =_db.tbl_OrderItemDetails.Where(o => o.ComboId == comboid && o.OrderId == OrderId).ToList();
                if(lstordritms != null && lstordritms.Count() > 0)
                {
                    foreach(var objorr in lstordritms)
                    {
                        long RplceId = 0;
                        if(objorr.IsReplacedItem == true)
                        {
                            tbl_ItemReplace objItemReplc = new tbl_ItemReplace();
                            objItemReplc.ItemDetailId = objorr.OrderDetailId;
                            objItemReplc.OrderId = objorr.OrderId;
                            objItemReplc.ItemStatus = 3;
                            objItemReplc.ReplaceDate = DateTime.UtcNow;
                            _db.tbl_ItemReplace.Add(objItemReplc);
                            _db.SaveChanges();
                            RplceId = objItemReplc.ItemReplaceId;
                            objorr.ItemStatus = 3;
                        }
                        else
                        {
                            objorr.ItemStatus = 3;
                        }
                        
                        _db.SaveChanges();
                       
                        tbl_OrderItemDelivery objOrderItmDlv = new tbl_OrderItemDelivery();
                        objOrderItmDlv.OrderId = OrderId;
                        objOrderItmDlv.OrderItemId = objorr.OrderDetailId;
                        objOrderItmDlv.Status = 3;
                        objOrderItmDlv.DelieveryPersonId = PersonId;
                        objOrderItmDlv.AssignedBy = clsAdminSession.UserID;
                        objOrderItmDlv.AssignedDate = DateTime.UtcNow;
                        objOrderItmDlv.ReplaceId = RplceId;
                        _db.tbl_OrderItemDelivery.Add(objOrderItmDlv);
                        _db.SaveChanges();
                        tbl_ItemVariant objVrnt = _db.tbl_ItemVariant.Where(o => o.VariantItemId == objorr.VariantItemId).FirstOrDefault();
                        if (objVrnt != null)
                        {
                            ItemList = ItemList + objorr.ItemName + "-" + objVrnt.UnitQty + ",";
                        }
                        else
                        {
                            ItemList = ItemList + objorr.ItemName + ",";
                        }
                        objCommon.SaveTransaction(objorr.ProductItemId.Value, objorr.OrderDetailId, objorr.OrderId.Value, "Delivery Person " + objAdminUsr.FirstName + " " + objAdminUsr.LastName + " Assign to Dispatch Item", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Item Status Changed");
                    }
                }
            
                if (objOrdr.OrderStatusId == 2)
                {
                    List<tbl_OrderItemDetails> lstOrderTms = _db.tbl_OrderItemDetails.Where(o => o.OrderId == OrderId && o.ItemStatus != 5 && o.ItemStatus != 3).ToList();
                    if (lstOrderTms == null || lstOrderTms.Count == 0)
                    {
                        objOrdr.OrderStatusId = 3;
                    }
                }             
            }
            else
            {
                ItemList = objOrderItm.ItemName;
                long RplceId = 0;
                if (objOrderItm.IsReplacedItem == true)
                {
                    tbl_ItemReplace objItemReplc = new tbl_ItemReplace();
                    objItemReplc.ItemDetailId = objOrderItm.OrderDetailId;
                    objItemReplc.OrderId = objOrderItm.OrderId;
                    objItemReplc.ItemStatus = 3;
                    objItemReplc.ReplaceDate = DateTime.UtcNow;
                    _db.tbl_ItemReplace.Add(objItemReplc);
                    _db.SaveChanges();
                    RplceId = objItemReplc.ItemReplaceId;
                    objOrderItm.ItemStatus = 3;
                }
                else
                {
                    objOrderItm.ItemStatus = 3;
                }

                _db.SaveChanges();

                if (objOrdr.OrderStatusId == 2)
                {
                    List<tbl_OrderItemDetails> lstOrderTms = _db.tbl_OrderItemDetails.Where(o => o.OrderId == OrderId && o.ItemStatus != 5 && o.ItemStatus != 3).ToList();
                    if (lstOrderTms == null || lstOrderTms.Count == 0)
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
                objOrderItmDlv.ReplaceId = RplceId;
                _db.tbl_OrderItemDelivery.Add(objOrderItmDlv);
                _db.SaveChanges();
                objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Delivery Person " + objAdminUsr.FirstName + " " + objAdminUsr.LastName + " Assign to Dispatch Item", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Item Status Changed");

            }          
          
            
            tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == objOrdr.ClientUserId).FirstOrDefault();
            if (objclntusr != null)
            {
                using (WebClient webClient = new WebClient())
                {

                    string msg = "Your Order No: " + objOrdr.OrderId + "\n Item: " + ItemList + "\n Has Been Dispatched";
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

                            string msg1 = "Your Order #" + objOrdr.OrderId + "Item: " + ItemList + "\n Has Been Dispatched";
                            clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order has been dispatched - Shopping & Saving", msg1);
                        }
                    }

                }

                using (WebClient webClient = new WebClient())
                {
                    string msg = "Order No: " + objOrdr.OrderId + " \nItem: " + ItemList + " \nHas Been Assigned To You for Delivery";
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

                            string msg1 = "Order No: " + objOrdr.OrderId + "Item: " + ItemList + "Has Been Assigned To You for Delivery";
                            clsCommon.SendEmail(objAdminUsr.Email, FromEmail, "Assigned New Item Delivery - Shopping & Saving", msg1);
                        }
                    }

                }
            }

            return "Success";
        }

        [HttpPost]
        public string AssignMultiItemDeliveryPerson(long OrderId, string OrderItemIds, long PersonId)
        {
            clsCommon objCommon = new clsCommon();
            tbl_AdminUsers objAdminUsr = _db.tbl_AdminUsers.Where(o => o.AdminUserId == PersonId).FirstOrDefault();
            string ItmsText = "";
            OrderItemIds = OrderItemIds.Trim('^');
            if (!string.IsNullOrEmpty(OrderItemIds))
            {
                string[] strordditm = OrderItemIds.Split('^');
                foreach (string ss in strordditm)
                {
                    long OrderItemId = Convert.ToInt64(ss);
                    tbl_OrderItemDetails objOrderItm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == OrderItemId).FirstOrDefault();
                    if (objOrderItm.IsCombo == true)
                    {
                        long comboid = objOrderItm.ComboId.Value;
                        List<tbl_OrderItemDetails> lstordritms = _db.tbl_OrderItemDetails.Where(o => o.ComboId == comboid && o.OrderId == OrderId).ToList();
                        if (lstordritms != null && lstordritms.Count() > 0)
                        {
                            foreach (var objorr in lstordritms)
                            {
                                long RplceId = 0;
                                if (objorr.IsReplacedItem == true)
                                {
                                    tbl_ItemReplace objItemReplc = new tbl_ItemReplace();
                                    objItemReplc.ItemDetailId = objorr.OrderDetailId;
                                    objItemReplc.OrderId = objorr.OrderId;
                                    objItemReplc.ItemStatus = 3;
                                    objItemReplc.ReplaceDate = DateTime.UtcNow;
                                    _db.tbl_ItemReplace.Add(objItemReplc);
                                    _db.SaveChanges();
                                    RplceId = objItemReplc.ItemReplaceId;
                                    objorr.ItemStatus = 3;
                                }
                                else
                                {
                                    objorr.ItemStatus = 3;
                                }
                              
                                _db.SaveChanges();
                                tbl_OrderItemDelivery objOrderItmDlv1 = new tbl_OrderItemDelivery();
                                objOrderItmDlv1.OrderId = OrderId;
                                objOrderItmDlv1.OrderItemId = objorr.OrderDetailId;
                                objOrderItmDlv1.Status = 3;
                                objOrderItmDlv1.DelieveryPersonId = PersonId;
                                objOrderItmDlv1.AssignedBy = clsAdminSession.UserID;
                                objOrderItmDlv1.AssignedDate = DateTime.UtcNow;
                                objOrderItmDlv1.ReplaceId = RplceId;
                                _db.tbl_OrderItemDelivery.Add(objOrderItmDlv1);
                                _db.SaveChanges();
                                tbl_ItemVariant objVrnt1 = _db.tbl_ItemVariant.Where(o => o.VariantItemId == objorr.VariantItemId).FirstOrDefault();
                                if (objVrnt1 != null)
                                {
                                    ItmsText = ItmsText + objorr.ItemName + "-" + objVrnt1.UnitQty + ",";
                                }
                                else
                                {
                                    ItmsText = ItmsText + objorr.ItemName + ",";
                                }
                                objCommon.SaveTransaction(objorr.ProductItemId.Value, objorr.OrderDetailId, objorr.OrderId.Value, "Delivery Person " + objAdminUsr.FirstName + " " + objAdminUsr.LastName + " Assign to Dispatch Item", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Item Status Changed");
                            }
                        }
                    }
                    else
                    {
                        long RplceId = 0;
                        if (objOrderItm.IsReplacedItem == true)
                        {
                            tbl_ItemReplace objItemReplc = new tbl_ItemReplace();
                            objItemReplc.ItemDetailId = objOrderItm.OrderDetailId;
                            objItemReplc.OrderId = objOrderItm.OrderId;
                            objItemReplc.ItemStatus = 3;
                            objItemReplc.ReplaceDate = DateTime.UtcNow;
                            _db.tbl_ItemReplace.Add(objItemReplc);
                            _db.SaveChanges();
                            RplceId = objItemReplc.ItemReplaceId;
                            objOrderItm.ItemStatus = 3;
                        }
                        else
                        {
                            objOrderItm.ItemStatus = 3;
                        }
                     
                        _db.SaveChanges();
                        tbl_ItemVariant objVrnt = _db.tbl_ItemVariant.Where(o => o.VariantItemId == objOrderItm.VariantItemId).FirstOrDefault();
                        if (objVrnt != null)
                        {
                            ItmsText = ItmsText + objOrderItm.ItemName + "-" + objVrnt.UnitQty + ",";
                        }
                        else
                        {
                            ItmsText = ItmsText + objOrderItm.ItemName + ",";
                        }
                        tbl_OrderItemDelivery objOrderItmDlv = new tbl_OrderItemDelivery();
                        objOrderItmDlv.OrderId = OrderId;
                        objOrderItmDlv.OrderItemId = OrderItemId;
                        objOrderItmDlv.Status = 3;
                        objOrderItmDlv.DelieveryPersonId = PersonId;
                        objOrderItmDlv.AssignedBy = clsAdminSession.UserID;
                        objOrderItmDlv.AssignedDate = DateTime.UtcNow;
                        objOrderItmDlv.ReplaceId = RplceId;
                        _db.tbl_OrderItemDelivery.Add(objOrderItmDlv);
                        objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Delivery Person " + objAdminUsr.FirstName + " " + objAdminUsr.LastName + " Assign to Dispatch Item", 0, 0, clsAdminSession.UserID, DateTime.UtcNow, "Item Status Changed");
                    }
                  
                }
                _db.SaveChanges();
            }

            _db.SaveChanges();

            tbl_Orders objOrdr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if (objOrdr.OrderStatusId == 2)
            {
                List<tbl_OrderItemDetails> lstOrderTms = _db.tbl_OrderItemDetails.Where(o => o.OrderId == OrderId && o.ItemStatus != 5 && o.ItemStatus != 3).ToList();
                if (lstOrderTms == null || lstOrderTms.Count == 0)
                {
                    objOrdr.OrderStatusId = 3;
                }
            }
            _db.SaveChanges();
            tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == objOrdr.ClientUserId).FirstOrDefault();
            if (objclntusr != null)
            {
                using (WebClient webClient = new WebClient())
                {

                    string msg = "Your order no." + objOrdr.OrderId + "\n Item: " + ItmsText + "\n has been dispatched";
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

                            string msg1 = "Your Order No: " + objOrdr.OrderId + "Item: " + ItmsText.Trim(',') + "\n Has Been Dispatched.";
                            clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order Has Been Dispatched - Shopping & Saving", msg1);
                        }
                    }

                }

                using (WebClient webClient = new WebClient())
                {
                    string msg = "Order no." + objOrdr.OrderId + " \nItem: " + ItmsText + " \nhas been assigned to you for delivery";
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

                            string msg1 = "Order No.: " + objOrdr.OrderId + "Item: " + ItmsText.Trim(',') + "Has Been Assigned To You For Delivery.";
                            clsCommon.SendEmail(objAdminUsr.Email, FromEmail, "Assigned New Item Delivery - Shopping & Saving", msg1);
                        }
                    }

                }
            }

            return "Success";
        }

        public ActionResult PaymentReport()
        {
            return View();
        }

        public void ExportPaymentReport(string StartDate, string EndDate, string MobileNo, string PaymentMode)
        {
            ExcelPackage excel = new ExcelPackage();
            if (PaymentMode == "OnlinePayment")
            {
                PaymentMode = "Online Payment";
            }
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing", "PaymentMethod", "Remarks" };
            if (!string.IsNullOrEmpty(MobileNo))
            {
                lstClients = _db.tbl_ClientUsers.Where(o => o.MobileNo == MobileNo).ToList();
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
                                    double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(objTrn.Amount));
                                    objTrn.Amount = Convert.ToDecimal(RoundAmt);
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
                                    if (objTrn.IsCredit == true)
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
                                    if (objTrn.IsCredit == false)
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
                                    row1 = row1 + 1;
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                var workSheet = excel.Workbook.Worksheets.Add("Report");
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 20;
                workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Cells[1, 1].Value = "Payment Report: " + StartDate + " to " + EndDate;
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

                List<tbl_PaymentTransaction> lstCrdt = _db.tbl_PaymentTransaction.Where(o => o.TransactionDate < dtStart && o.IsCredit == true && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                List<tbl_PaymentTransaction> lstDebt = _db.tbl_PaymentTransaction.Where(o => o.TransactionDate < dtStart && o.IsCredit == false && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                decimal TotalCredit = 0;
                decimal TotalDebit = 0;
                TotalCredit = lstCrdt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                TotalDebit = lstDebt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                decimal TotalOpening = TotalCredit - TotalDebit;
                List<tbl_PaymentTransaction> lstAllTransaction = _db.tbl_PaymentTransaction.Where(o => o.TransactionDate >= dtStart && o.TransactionDate <= dtEnd && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                int row1 = 1;
                if (lstAllTransaction != null && lstAllTransaction.Count() > 0)
                {
                    foreach (var objTrn in lstAllTransaction)
                    {
                        double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(objTrn.Amount));
                        objTrn.Amount = Convert.ToDecimal(RoundAmt);
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
                        if (objTrn.IsCredit == true)
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
                        if (objTrn.IsCredit == false)
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
                        row1 = row1 + 1;
                    }
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=PaymentReport.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult GetPaymentReport(string StartDate, string EndDate, string MobileNo, string PaymentMode)
        {
            List<ReportVM> lstReportVm = new List<ReportVM>();
            if (PaymentMode == "OnlinePayment")
            {
                PaymentMode = "Online Payment";
            }
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing", "PaymentMethod", "Remarks" };
            if (!string.IsNullOrEmpty(MobileNo))
            {
                lstClients = _db.tbl_ClientUsers.Where(o => o.MobileNo == MobileNo).ToList();
                if (lstClients != null && lstClients.Count() > 0)
                {
                    var client = lstClients.FirstOrDefault();
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
                                double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(objTrn.Amount));
                                objTrn.Amount = Convert.ToDecimal(RoundAmt);
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
                                lstReportVm.Add(objrp);
                                row1 = row1 + 1;
                            }
                        }
                    }
                }
            }
            else
            {

                List<tbl_PaymentTransaction> lstCrdt = _db.tbl_PaymentTransaction.Where(o => o.TransactionDate < dtStart && o.IsCredit == true && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                List<tbl_PaymentTransaction> lstDebt = _db.tbl_PaymentTransaction.Where(o => o.TransactionDate < dtStart && o.IsCredit == false && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                decimal TotalCredit = 0;
                decimal TotalDebit = 0;
                TotalCredit = lstCrdt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                TotalDebit = lstDebt.Sum(x => x.Amount.HasValue ? x.Amount.Value : 0);
                decimal TotalOpening = TotalCredit - TotalDebit;
                List<tbl_PaymentTransaction> lstAllTransaction = _db.tbl_PaymentTransaction.Where(o => o.TransactionDate >= dtStart && o.TransactionDate <= dtEnd && (PaymentMode == "All" || o.ModeOfPayment == PaymentMode)).ToList();
                int row1 = 1;
                if (lstAllTransaction != null && lstAllTransaction.Count() > 0)
                {
                    foreach (var objTrn in lstAllTransaction)
                    {
                        double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(objTrn.Amount));
                        objTrn.Amount = Convert.ToDecimal(RoundAmt);
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
                        lstReportVm.Add(objrp);
                        row1 = row1 + 1;
                    }
                }
            }

            return PartialView("~/Areas/Admin/Views/Order/_PaymentReport.cshtml", lstReportVm);
        }

        public ActionResult SalesReport()
        {
            return View();
        }

        public void ExportSalesReport(string StartDate, string EndDate, string ReportType)
        {
           
            
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            if(ReportType == "Sales")
            {
                SalesReportData(dtStart, dtEnd, StartDate, EndDate);
            }
            else if(ReportType == "SalesCancel")
            {
                SalesCancelReportData(dtStart, dtEnd, StartDate, EndDate);
            }
            else if (ReportType == "SalesReturn")
            {
                SalesReturnReportData(dtStart, dtEnd, StartDate, EndDate);
            }
            else if (ReportType == "SalesExchange")
            {
                SalesExchangeReportData(dtStart, dtEnd, StartDate, EndDate);
            }
        }

        public void SalesReportData(DateTime dtStart, DateTime dtEnd,string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            decimal TotalWhole = 0;
            decimal TotalWholeQty = 0;
            List<string> dtlist = new List<string>();
            List<OrderVM> lstOrderss = new List<OrderVM>();
            lstOrderss = (from p in _db.tbl_Orders
                          join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                          where p.CreatedDate >= dtStart && p.CreatedDate <= dtEnd && p.IsDelete == false
                          select new OrderVM
                          {
                              OrderId = p.OrderId,
                              ClientUserName = c.FirstName + " " + c.LastName,
                              ClientUserId = p.ClientUserId,
                              OrderAmount = p.OrderAmount + (p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0) + (p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0),
                              OrderShipCity = p.OrderShipCity,
                              OrderShipState = p.OrderShipState,
                              OrderShipAddress = p.OrderShipAddress,
                              OrderPincode = p.OrderShipPincode,
                              InvoiceYear = p.InvoiceYear,
                              InvoiceNo = p.InvoiceNo.Value,
                              OrderShipClientName = p.OrderShipClientName,
                              OrderShipClientPhone = p.OrderShipClientPhone,
                              OrderStatusId = p.OrderStatusId,
                              PaymentType = p.PaymentType,
                              OrderDate = p.CreatedDate,
                              ClientRoleId = c.ClientRoleId,
                              ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                              ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2,
                              CreditUsed = p.CreditAmountUsed.HasValue ? p.CreditAmountUsed.Value : 0,
                              OrderAmountDue = p.AmountDue.HasValue ? p.AmountDue.Value : 0,
                              WalletAmtUsed = p.WalletAmountUsed.HasValue ? p.WalletAmountUsed.Value : 0,
                              OrderTypeId = p.OrderType.HasValue ? p.OrderType.Value : 1,
                              ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0,
                              AdvancePay = p.AdvancePaymentRecieved.HasValue ? p.AdvancePaymentRecieved.Value : 0
                          }).OrderBy(x => x.OrderDate).ToList();

            if (lstOrderss != null && lstOrderss.Count() > 0)
            {
                dtlist = lstOrderss.Select(x => x.OrderDate.ToString("dd-MMM-yy")).Distinct().ToList();
            }
            // var llst = lstorders.Where(o => o.CreatedDate.ToShortDateString() == "7/11/2020").ToList();
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Invoice No", "Name", "Item", "HSNCode", "Qty", "Unit", "MRP Price", "Price", "Point Discount", "Taxable Amount", "GST", "Total" };
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Sales Report: " + StartDate + " to " + EndDate;
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
            foreach (string dtstr in dtlist)
            {
                decimal TotalDateWise = 0;
                decimal TotalDateWiseQty = 0;
                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = true;
                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                workSheet.Cells[row1 + 2, 1].Value = dtstr;
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
                var llstordrs = lstOrderss.Where(o => o.OrderDate.ToString("dd-MMM-yy") == dtstr).ToList();
                if (llstordrs != null && llstordrs.Count() > 0)
                {
                    foreach (var ordrr in llstordrs)
                    {
                        string InvoiceNo = "S&S/" + ordrr.InvoiceYear + "/" + ordrr.InvoiceNo;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 2].Value = InvoiceNo;
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
                        workSheet.Cells[row1 + 2, 3].Value = ordrr.ClientUserName;
                        workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Merge = true;
                        workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);
                        row1 = row1 + 1;
                        decimal TotalFinal = 0;
                        decimal TotlQty = 0;
                        List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                           join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                           join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                                           where p.OrderId == ordrr.OrderId
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
                                                               MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                                               VariantQtytxt = u.UnitQty,
                                                               GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                                               Discount = p.Discount.HasValue ? p.Discount.Value : 0
                                                           }).OrderByDescending(x => x.GST_Per).ToList();
                        if (lstOrderItms != null && lstOrderItms.Count() > 0)
                        {
                            foreach (var objItem in lstOrderItms)
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
                                TotlQty = TotlQty + objItem.Qty;
                                for (var col = 4; col < arrycolmns.Length + 1; col++)
                                {
                                    if (arrycolmns[col - 1] == "Item")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.ItemName;
                                    }
                                    else if (arrycolmns[col - 1] == "HSNCode")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.HSNCode;
                                    }
                                    else if (arrycolmns[col - 1] == "Qty")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Qty;
                                    }
                                    else if (arrycolmns[col - 1] == "Unit")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.VariantQtytxt;
                                    }
                                    else if (arrycolmns[col - 1] == "MRP Price")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.MRPPrice;
                                    }
                                    else if (arrycolmns[col - 1] == "Price")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Price;
                                    }
                                    else if (arrycolmns[col - 1] == "Point Discount")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Discount;
                                    }
                                    else if (arrycolmns[col - 1] == "Taxable Amount")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = TaxableAmt;
                                    }
                                    else if (arrycolmns[col - 1] == "GST")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%";
                                    }
                                    else if (arrycolmns[col - 1] == "Total")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = Math.Round(FinalPrice, 2);
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

                        workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
                        workSheet.Cells[row1 + 2, 4].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 4].Value = "Bill Wise Sum:";
                        workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 5].Style.Font.Bold = true;
                        workSheet.Cells[row1 + 2, 5].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 5].Value = "Grand Total: " + TotalFinal;
                        workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 6].Style.Font.Bold = true;
                        workSheet.Cells[row1 + 2, 6].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 6].Value = "Shipping Charge: " + ordrr.ShipmentCharge;
                        workSheet.Cells[row1 + 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 6].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 7].Style.Font.Bold = true;
                        workSheet.Cells[row1 + 2, 7].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 7].Value = "Extra Charge: " + ordrr.ExtraAmount;
                        workSheet.Cells[row1 + 2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 7].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 7].AutoFitColumns(30, 70);
                        decimal netamt = TotalFinal + ordrr.ShipmentCharge + ordrr.ExtraAmount;
                        double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(netamt));
                        netamt = Convert.ToDecimal(RoundAmt);
                        TotalDateWise = TotalDateWise + netamt;
                        TotalDateWiseQty = TotalDateWiseQty + TotlQty;
                        TotalWhole = TotalWhole + netamt;
                        TotalWholeQty = TotalWholeQty + TotlQty;                  
                        workSheet.Cells[row1 + 2, 8].Style.Font.Bold = true;
                        workSheet.Cells[row1 + 2, 8].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 8].Value = "Net Amount: " + netamt;
                        workSheet.Cells[row1 + 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 8].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 8].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 9].Style.Font.Bold = true;
                        workSheet.Cells[row1 + 2, 9].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 9].Value = "Total Qty: " + TotlQty;
                        workSheet.Cells[row1 + 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 9].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 9].AutoFitColumns(30, 70);
                        row1 = row1 + 1;
                    }
                    row1 = row1 + 1;
                    workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 2].Value = "Date Wise Total: ";
                    workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 3].Value = "Total Net Amount: " + TotalDateWise;
                    workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 4].Value = "Total Qty: " + TotalDateWiseQty;
                    workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);
                    row1 = row1 + 1;
                }
            }

            row1 = row1 + 1;
            workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 2].Value = "Total Net Sell Amount: ";
            workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 3].Value = TotalWhole;
            workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 4].Value = "Total Qty: " + TotalWholeQty;
            workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);
            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=SalesReport.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public void SalesCancelReportData(DateTime dtStart, DateTime dtEnd, string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            decimal TotalWhole = 0;
            decimal TotalWholeQty = 0;
            List<string> dtlist = new List<string>();

            List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                               join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                               join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                               join o in _db.tbl_Orders on p.OrderId equals o.OrderId
                                               where p.UpdatedDate >= dtStart && p.UpdatedDate <= dtEnd && p.ItemStatus == 5 && p.IsDelete == true
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
                                                   MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                                   VariantQtytxt = u.UnitQty,
                                                   modifieddate = p.UpdatedDate.HasValue ? p.UpdatedDate.Value : DateTime.MinValue,
                                                   GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                                   Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                   InvoiceNo = o.InvoiceNo.HasValue ? o.InvoiceNo.Value : 0,
                                                   InvoiceYear = o.InvoiceYear,
                                                   ClientUserId = o.ClientUserId
                                               }).OrderBy(x => x.modifieddate).ToList();


            if (lstOrderItms != null && lstOrderItms.Count() > 0)
            {
                dtlist = lstOrderItms.Select(x => x.modifieddate.ToString("dd-MMM-yy")).Distinct().ToList();
            }
            // var llst = lstorders.Where(o => o.CreatedDate.ToShortDateString() == "7/11/2020").ToList();
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Invoice No", "Item", "HSNCode", "Qty", "Unit", "MRP Price", "Price", "Point Discount", "Taxable Amount", "GST", "Total" };
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Sales Cancel Report: " + StartDate + " to " + EndDate;
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
            foreach (string dtstr in dtlist)
            {
                decimal TotalDateWise = 0;
                decimal TotalDateWiseQty = 0;
                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = true;
                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                workSheet.Cells[row1 + 2, 1].Value = dtstr;
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
                var llstordrs = lstOrderItms.Where(o => o.modifieddate.ToString("dd-MMM-yy") == dtstr).ToList();
                if (llstordrs != null && llstordrs.Count() > 0)
                {
                    foreach (var objItem in llstordrs)           
                    {
                        string InvoiceNo = "S&S/" + objItem.InvoiceYear + "/" + objItem.InvoiceNo;                       

                        decimal basicTotalPrice = Math.Round(objItem.Price * objItem.Qty, 2);
                        decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal CGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal IGSTAmt = Math.Round(objItem.GSTAmt);
                        decimal IGST = Math.Round(Convert.ToDecimal(objItem.GST_Per));
                        decimal FinalPrice = Math.Round(basicTotalPrice + objItem.GSTAmt - objItem.Discount, 2);
                        decimal TaxableAmt = Math.Round(basicTotalPrice - objItem.Discount, 2);
                        double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(FinalPrice));
                        FinalPrice = Convert.ToDecimal(RoundAmt);
                        TotalDateWise = TotalDateWise + FinalPrice;
                        TotalDateWiseQty = TotalDateWiseQty + objItem.Qty;
                        TotalWhole = TotalWhole + FinalPrice;
                        TotalWholeQty = TotalWholeQty + objItem.Qty;
                        for (var col = 2; col < arrycolmns.Length + 1; col++)
                        {

                            if (arrycolmns[col - 1] == "Invoice No")
                            {
                                workSheet.Cells[row1 + 2, col].Value = InvoiceNo;
                            }
                            else if (arrycolmns[col - 1] == "Item")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.ItemName;
                            }
                            else if (arrycolmns[col - 1] == "HSNCode")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.HSNCode;
                            }
                            else if (arrycolmns[col - 1] == "Qty")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Qty;
                            }
                            else if (arrycolmns[col - 1] == "Unit")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.VariantQtytxt;
                            }
                            else if (arrycolmns[col - 1] == "MRP Price")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.MRPPrice;
                            }
                            else if (arrycolmns[col - 1] == "Price")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Price;
                            }
                            else if (arrycolmns[col - 1] == "Point Discount")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Discount;
                            }
                            else if (arrycolmns[col - 1] == "Taxable Amount")
                            {
                                workSheet.Cells[row1 + 2, col].Value = TaxableAmt;
                            }
                            else if (arrycolmns[col - 1] == "GST")
                            {
                                workSheet.Cells[row1 + 2, col].Value = Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%";
                            }
                            else if (arrycolmns[col - 1] == "Total")
                            {
                                workSheet.Cells[row1 + 2, col].Value = Math.Round(FinalPrice, 2);
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
                    workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 2].Value = "Date Wise Total: ";
                    workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 3].Value = "Total Net Cancel Amount: ";
                    workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 4].Value = TotalDateWise; // "Total Qty: " + TotalDateWiseQty;
                    workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 5].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 5].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 5].Value = "Total Qty: " + TotalDateWiseQty;
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

            row1 = row1 + 1;
            workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 2].Value = "Total Net Cancel Amount: ";
            workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 3].Value = TotalWhole;
            workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 4].Value = "Total Qty: " + TotalWholeQty;
            workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);
            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=SalesCancelReport.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public void SalesReturnReportData(DateTime dtStart, DateTime dtEnd, string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            decimal TotalWhole = 0;
            decimal TotalWholeQty = 0;
            List<string> dtlist = new List<string>();

            List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                               join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                               join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                               join o in _db.tbl_Orders on p.OrderId equals o.OrderId
                                               where p.UpdatedDate >= dtStart && p.UpdatedDate <= dtEnd && p.ItemStatus == 6 && p.IsDelete == true
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
                                                   MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                                   VariantQtytxt = u.UnitQty,
                                                   modifieddate = p.UpdatedDate.HasValue ? p.UpdatedDate.Value : DateTime.MinValue,
                                                   GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                                   Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                   InvoiceNo = o.InvoiceNo.HasValue ? o.InvoiceNo.Value : 0,
                                                   InvoiceYear = o.InvoiceYear,
                                                   ClientUserId = o.ClientUserId
                                               }).OrderBy(x => x.modifieddate).ToList();


            if (lstOrderItms != null && lstOrderItms.Count() > 0)
            {
                dtlist = lstOrderItms.Select(x => x.modifieddate.ToString("dd-MMM-yy")).Distinct().ToList();
            }
            // var llst = lstorders.Where(o => o.CreatedDate.ToShortDateString() == "7/11/2020").ToList();
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Invoice No", "Item", "HSNCode", "Qty", "Unit", "MRP Price", "Price", "Point Discount", "Taxable Amount", "GST", "Total" };
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Sales Return Report: " + StartDate + " to " + EndDate;
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
            foreach (string dtstr in dtlist)
            {
                decimal TotalDateWise = 0;
                decimal TotalDateWiseQty = 0;
                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = true;
                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                workSheet.Cells[row1 + 2, 1].Value = dtstr;
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
                var llstordrs = lstOrderItms.Where(o => o.modifieddate.ToString("dd-MMM-yy") == dtstr).ToList();
                if (llstordrs != null && llstordrs.Count() > 0)
                {
                    foreach (var objItem in llstordrs)
                    {
                        string InvoiceNo = "S&S/" + objItem.InvoiceYear + "/" + objItem.InvoiceNo;

                        decimal basicTotalPrice = Math.Round(objItem.Price * objItem.Qty, 2);
                        decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal CGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal IGSTAmt = Math.Round(objItem.GSTAmt);
                        decimal IGST = Math.Round(Convert.ToDecimal(objItem.GST_Per));
                        decimal FinalPrice = Math.Round(basicTotalPrice + objItem.GSTAmt - objItem.Discount, 2);
                        decimal TaxableAmt = Math.Round(basicTotalPrice - objItem.Discount, 2);
                        double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(FinalPrice));
                        FinalPrice = Convert.ToDecimal(RoundAmt);
                        TotalDateWise = TotalDateWise + FinalPrice;
                        TotalDateWiseQty = TotalDateWiseQty + objItem.Qty;
                        TotalWhole = TotalWhole + FinalPrice;
                        TotalWholeQty = TotalWholeQty + objItem.Qty;
                        for (var col = 2; col < arrycolmns.Length + 1; col++)
                        {

                            if (arrycolmns[col - 1] == "Invoice No")
                            {
                                workSheet.Cells[row1 + 2, col].Value = InvoiceNo;
                            }
                            else if (arrycolmns[col - 1] == "Item")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.ItemName;
                            }
                            else if (arrycolmns[col - 1] == "HSNCode")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.HSNCode;
                            }
                            else if (arrycolmns[col - 1] == "Qty")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Qty;
                            }
                            else if (arrycolmns[col - 1] == "Unit")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.VariantQtytxt;
                            }
                            else if (arrycolmns[col - 1] == "MRP Price")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.MRPPrice;
                            }
                            else if (arrycolmns[col - 1] == "Price")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Price;
                            }
                            else if (arrycolmns[col - 1] == "Point Discount")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Discount;
                            }
                            else if (arrycolmns[col - 1] == "Taxable Amount")
                            {
                                workSheet.Cells[row1 + 2, col].Value = TaxableAmt;
                            }
                            else if (arrycolmns[col - 1] == "GST")
                            {
                                workSheet.Cells[row1 + 2, col].Value = Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%";
                            }
                            else if (arrycolmns[col - 1] == "Total")
                            {
                                workSheet.Cells[row1 + 2, col].Value = Math.Round(FinalPrice, 2);
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
                    workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 2].Value = "Date Wise Total: ";
                    workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 3].Value = "Total Net Return Amount: ";
                    workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 4].Value = TotalDateWise;
                    workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 5].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 5].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 5].Value = "Total Qty: " + TotalDateWiseQty;
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

            row1 = row1 + 1;
            workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 2].Value = "Total Net Return Amount: ";
            workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 3].Value = TotalWhole;
            workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 4].Value = "Total Qty: " + TotalWholeQty;
            workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);
            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=SalesReturnReport.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public void SalesExchangeReportData(DateTime dtStart, DateTime dtEnd, string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            decimal TotalWhole = 0;
            decimal TotalWholeQty = 0;
            List<string> dtlist = new List<string>();

            List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                               join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                               join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                               join o in _db.tbl_Orders on p.OrderId equals o.OrderId
                                               where p.UpdatedDate >= dtStart && p.UpdatedDate <= dtEnd && p.ItemStatus == 8 && p.IsDelete == true
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
                                                   MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                                   VariantQtytxt = u.UnitQty,
                                                   modifieddate = p.UpdatedDate.HasValue ? p.UpdatedDate.Value : DateTime.MinValue,
                                                   GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                                   Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                   InvoiceNo = o.InvoiceNo.HasValue ? o.InvoiceNo.Value : 0,
                                                   InvoiceYear = o.InvoiceYear,
                                                   ClientUserId = o.ClientUserId
                                               }).OrderBy(x => x.modifieddate).ToList();


            if (lstOrderItms != null && lstOrderItms.Count() > 0)
            {
                dtlist = lstOrderItms.Select(x => x.modifieddate.ToString("dd-MMM-yy")).Distinct().ToList();
            }
            // var llst = lstorders.Where(o => o.CreatedDate.ToShortDateString() == "7/11/2020").ToList();
            List<tbl_ClientUsers> lstClients = new List<tbl_ClientUsers>();
            string[] arrycolmns = new string[] { "Date", "Invoice No", "Item", "HSNCode", "Qty", "Unit", "MRP Price", "Price", "Point Discount", "Taxable Amount", "GST", "Total" };
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Sales Exchange Report: " + StartDate + " to " + EndDate;
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
            foreach (string dtstr in dtlist)
            {
                decimal TotalDateWise = 0;
                decimal TotalDateWiseQty = 0;
                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = true;
                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                workSheet.Cells[row1 + 2, 1].Value = dtstr;
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
                var llstordrs = lstOrderItms.Where(o => o.modifieddate.ToString("dd-MMM-yy") == dtstr).ToList();
                if (llstordrs != null && llstordrs.Count() > 0)
                {
                    foreach (var objItem in llstordrs)
                    {
                        string InvoiceNo = "S&S/" + objItem.InvoiceYear + "/" + objItem.InvoiceNo;

                        decimal basicTotalPrice = Math.Round(objItem.Price * objItem.Qty, 2);
                        decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal CGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal IGSTAmt = Math.Round(objItem.GSTAmt);
                        decimal IGST = Math.Round(Convert.ToDecimal(objItem.GST_Per));
                        decimal FinalPrice = Math.Round(basicTotalPrice + objItem.GSTAmt - objItem.Discount, 2);
                        decimal TaxableAmt = Math.Round(basicTotalPrice - objItem.Discount, 2);
                        double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(FinalPrice));
                        FinalPrice = Convert.ToDecimal(RoundAmt);
                        TotalDateWise = TotalDateWise + FinalPrice;
                        TotalDateWiseQty = TotalDateWiseQty + objItem.Qty;
                        TotalWhole = TotalWhole + FinalPrice;
                        TotalWholeQty = TotalWholeQty + objItem.Qty;
                        for (var col = 2; col < arrycolmns.Length + 1; col++)
                        {

                            if (arrycolmns[col - 1] == "Invoice No")
                            {
                                workSheet.Cells[row1 + 2, col].Value = InvoiceNo;
                            }
                            else if (arrycolmns[col - 1] == "Item")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.ItemName;
                            }
                            else if (arrycolmns[col - 1] == "HSNCode")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.HSNCode;
                            }
                            else if (arrycolmns[col - 1] == "Qty")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Qty;
                            }
                            else if (arrycolmns[col - 1] == "Unit")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.VariantQtytxt;
                            }
                            else if (arrycolmns[col - 1] == "MRP Price")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.MRPPrice;
                            }
                            else if (arrycolmns[col - 1] == "Price")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Price;
                            }
                            else if (arrycolmns[col - 1] == "Point Discount")
                            {
                                workSheet.Cells[row1 + 2, col].Value = objItem.Discount;
                            }
                            else if (arrycolmns[col - 1] == "Taxable Amount")
                            {
                                workSheet.Cells[row1 + 2, col].Value = TaxableAmt;
                            }
                            else if (arrycolmns[col - 1] == "GST")
                            {
                                workSheet.Cells[row1 + 2, col].Value = Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%";
                            }
                            else if (arrycolmns[col - 1] == "Total")
                            {
                                workSheet.Cells[row1 + 2, col].Value = Math.Round(FinalPrice, 2);
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
                    workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 2].Value = "Date Wise Total: ";
                    workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 3].Value = "Total Net Exchange Amount: ";
                    workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 4].Value = TotalDateWise;
                    workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                    workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);

                    workSheet.Cells[row1 + 2, 5].Style.Font.Bold = true;
                    workSheet.Cells[row1 + 2, 5].Style.Font.Size = 13;
                    workSheet.Cells[row1 + 2, 5].Value = "Total Qty: " + TotalDateWiseQty;
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

            row1 = row1 + 1;
            workSheet.Cells[row1 + 2, 2].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 2].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 2].Value = "Total Net Exchange Amount: ";
            workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 3].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 3].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 3].Value = TotalWhole;
            workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

            workSheet.Cells[row1 + 2, 4].Style.Font.Bold = true;
            workSheet.Cells[row1 + 2, 4].Style.Font.Size = 13;
            workSheet.Cells[row1 + 2, 4].Value = "Total Qty: " + TotalWholeQty;
            workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
            workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);
            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=SalesExchangeReport.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
    }
}