using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class OrderController : ApiController
    {
        krupagallarydbEntities _db;
        public OrderController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("GetOrderList"), HttpPost]
        public ResponseDataModel<List<OrderVM>> GetOrderList(GeneralVM objGen)
        {
            ResponseDataModel<List<OrderVM>> response = new ResponseDataModel<List<OrderVM>>();
            List<OrderVM> lstOrders = new List<OrderVM>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
                int StatusFilter = Convert.ToInt32(objGen.SortBy);
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
                                 OrderDate = p.CreatedDate,
                                 IsCashOnDelivery = p.IsCashOnDelivery.HasValue ? p.IsCashOnDelivery.Value : false,
                                 OrderTypeId = p.OrderType.HasValue ? p.OrderType.Value : 1,
                                 OrderAmountDue = p.AmountDue.HasValue ? p.AmountDue.Value : 0,
                                 ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                                 ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                             }).OrderByDescending(x => x.OrderDate).ToList();

                if (lstOrders != null && lstOrders.Count() > 0)
                {
                    lstOrders.ForEach(x => {x.OrderStatus = GetOrderStatus(x.OrderStatusId);x.OrderDateString = CommonMethod.ConvertFromUTC(x.OrderDate); });
                }

                if (StatusFilter == 1)
                {
                    lstOrders = lstOrders.Where(o => o.ShippingStatus == 1).ToList();
                }
                else if (StatusFilter == 2)
                {
                    lstOrders = lstOrders.Where(o => o.OrderAmountDue > 0).ToList();
                }
                else if (StatusFilter == 3)
                {
                    lstOrders = lstOrders.Where(o => o.IsCashOnDelivery == true).ToList();
                }
                else if (StatusFilter == 4)
                {
                    lstOrders = lstOrders.Where(o => o.OrderTypeId == 2).ToList();
                }

                response.Data = lstOrders;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }

        public string GetItemStatus(long itemstatusid)
        {
            return Enum.GetName(typeof(OrderItemStatus), itemstatusid);
        }

        [Route("GetOrderDetails"), HttpPost]
        public ResponseDataModel<OrderVM> GetOrderDetails(GeneralVM objGen)
        {
            ResponseDataModel<OrderVM> response = new ResponseDataModel<OrderVM>();
            OrderVM objOrder = new OrderVM();
            try
            {
                long Id = Convert.ToInt64(objGen.OrderId);                
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
                                OrderAmountDue = p.AmountDue.HasValue ? p.AmountDue.Value : 0,
                                ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                                ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2,
                                WalletAmtUsed = p.WalletAmountUsed.HasValue ? p.WalletAmountUsed.Value : 0,
                                CreditUsed = p.CreditAmountUsed.HasValue ? p.CreditAmountUsed.Value : 0,
                                OnlineUsed = p.AmountByRazorPay.HasValue ? p.AmountByRazorPay.Value : 0,
                                OrderTypeId = p.OrderType.HasValue ? p.OrderType.Value : 1,
                                IsCashOnDelivery = p.IsCashOnDelivery.HasValue ? p.IsCashOnDelivery.Value : false,
                                ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0
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
                                                           Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                           VariantQtytxt = vr.UnitQty,                                                           
                                                           ItemStatus = p.ItemStatus.HasValue ? p.ItemStatus.Value : 1,
                                                           IsReturnable = c.IsReturnable.HasValue ? c.IsReturnable.Value : false
                                                       }).OrderByDescending(x => x.OrderItemId).ToList();
                    if (lstOrderItms != null && lstOrderItms.Count() > 0)
                    {
                        lstOrderItms.ForEach(x => x.ItemStatustxt = GetItemStatus(x.ItemStatus));
                    }
                    objOrder.OrderItems = lstOrderItms;
                }

                if (objOrder.ShipmentCharge > 0 && objOrder.ShippingStatus == 1)
                {
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input.Add("amount", objOrder.ShipmentCharge * 100); // this amount should be same as transaction amount
                    input.Add("currency", "INR");
                    input.Add("receipt", "recpt_shipping_"+ objOrder.OrderId);
                    input.Add("payment_capture", 1);

                    var objGsetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    string key = objGsetting.RazorPayKey;  //"rzp_test_DMsPlGIBp3SSnI";
                    string secret = objGsetting.RazorPaySecret; // "YMkpd9LbnaXViePncLLXhqms";

                    RazorpayClient client = new RazorpayClient(key, secret);

                    Razorpay.Api.Order order = client.Order.Create(input);
                    objOrder.RazorpayOrderId = Convert.ToString(order["id"]);                                        
                }
                else
                {
                    objOrder.RazorpayOrderId = "0";
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

                objOrder.PaymentHistory = lstPaymentHist;                
                response.Data = objOrder;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SaveShippingCharge"), HttpPost]
        public ResponseDataModel<string> SaveShippingCharge(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
        
            try
            {
                clsCommon objCom = new clsCommon();
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long OrderId = Convert.ToInt64(objGen.OrderId);
                string razrpaymentid = objGen.RazorPaymentId;
                string razorderid = objGen.RazorOrderid;
                response.Data = "Fail";
                Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(razrpaymentid);
                if (objpymn != null)
                {
                    if (objpymn["status"] != null && Convert.ToString(objpymn["status"]) == "captured")
                    {
                        tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
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
                        objPayment.CreatedBy = UserId;
                        objPayment.CreatedDate = DateTime.UtcNow;
                        objPayment.RazorpayOrderId = razorderid;
                        objPayment.RazorpayPaymentId = razrpaymentid;
                        objPayment.RazorSignature = "";
                        objPayment.PaymentFor = "ShippingCharge";
                        _db.tbl_PaymentHistory.Add(objPayment);
                        _db.SaveChanges();
                        objCom.SaveTransaction(0, 0, objordr.OrderId, "Shipping Price Paid Online Amount: Rs" + objordr.ShippingCharge.Value, objordr.ShippingCharge.Value, UserId, 0, DateTime.UtcNow, "Shipping Charge Payment");
                        response.Data = "Success";
                    }
                }               

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("MakePayment"), HttpPost]
        public ResponseDataModel<string> MakePayment(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();

            try
            {
                clsCommon objCom = new clsCommon();
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long OrderId = Convert.ToInt64(objGen.OrderId);
                string razrpaymentid = objGen.RazorPaymentId;
                string amt = objGen.Amount;
                string razorderid = objGen.RazorOrderid;
                response.Data = "Fail";
                long orderid64 = Convert.ToInt64(OrderId);
                var objOrder = _db.tbl_Orders.Where(o => o.OrderId == orderid64).FirstOrDefault();
                decimal amountdue = 0;
                decimal amountpaid = Convert.ToDecimal(amt);
                if (objOrder != null)
                {
                    amountdue = objOrder.AmountDue.Value;
                    objOrder.AmountDue = amountdue - amountpaid;
                    long ClientUserId = objOrder.ClientUserId;
                    tbl_ClientOtherDetails objtbl_ClientOtherDetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                    objtbl_ClientOtherDetails.AmountDue = objtbl_ClientOtherDetails.AmountDue - amountpaid;
                    _db.SaveChanges();
                }

                Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(razrpaymentid);
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
                        objPayment.CreatedBy = UserId;
                        objPayment.CreatedDate = DateTime.UtcNow;
                        objPayment.RazorpayOrderId = razorderid;
                        objPayment.RazorpayPaymentId = razrpaymentid;
                        objPayment.RazorSignature = "";
                        objPayment.PaymentFor = "Order Amount";
                        _db.tbl_PaymentHistory.Add(objPayment);
                        _db.SaveChanges();
                        objCom.SaveTransaction(0, 0, orderid64, "Due Order Amount Paid Online: Rs" + amountpaid, amountpaid, UserId, 0, DateTime.UtcNow, "Amount Due Paid");
                        response.Data = "Success";
                    }
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("CreateRazorPayOrder"), HttpPost]
        public ResponseDataModel<string> CreateRazorPayOrder(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();

            try
            {
                string amt = objGen.Amount;
                decimal Amount = Convert.ToDecimal(amt);
                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", Amount * 100); // this amount should be same as transaction amount
                input.Add("currency", "INR");
                input.Add("receipt", "12000");
                input.Add("payment_capture", 1);

                var objGsetting = _db.tbl_GeneralSetting.FirstOrDefault();
                string key = objGsetting.RazorPayKey;  //"rzp_test_DMsPlGIBp3SSnI";
                string secret = objGsetting.RazorPaySecret; // "YMkpd9LbnaXViePncLLXhqms";

                RazorpayClient client = new RazorpayClient(key, secret);

                Razorpay.Api.Order order = client.Order.Create(input);
                response.Data = Convert.ToString(order["id"]);            

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SaveItemAction"), HttpPost]
        public ResponseDataModel<string> SaveItemAction(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string strmsgretun = "";
            bool IsRetunfalse = false;
            try
            {
                string orderitemid = objGen.OrderDetailId;
                string status = objGen.StatusId;
                string reason = objGen.Reason;
                int UserId = Convert.ToInt32(objGen.ClientUserId);
                clsCommon objCom = new clsCommon();
                long OrderItmID64 = Convert.ToInt64(orderitemid);
                decimal amtrefund = 0;
                bool IsApprov = false;
                string msgsms = "";
                string mobilenum = "";
                string adminmobilenumber = _db.tbl_GeneralSetting.FirstOrDefault().AdminSMSNumber;
                tbl_OrderItemDetails objitm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == OrderItmID64).FirstOrDefault();
                if (objitm != null)
                {
                    long proditmid = objitm.ProductItemId.Value;
                    long ordrid = objitm.OrderId.Value;
                    string resn = System.Web.HttpUtility.UrlDecode(reason);
                    tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == ordrid).FirstOrDefault();
                    var objproditm = _db.tbl_ProductItems.Where(o => o.ProductItemId == proditmid).FirstOrDefault();
                    if (status == "5")
                    {
                        IsApprov = true;
                      
                        amtrefund = objitm.FinalItemPrice.Value;
                        if (objordr.OrderShipPincode == "389001")
                        {
                            List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                               join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                               join vr in _db.tbl_ItemVariant on p.VariantItemId equals vr.VariantItemId
                                                               where p.OrderId == ordrid && p.IsDelete == false && p.ItemStatus != 5
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
                                                                   VariantQtytxt = vr.UnitQty,
                                                                   ShipingChargeOf1Item = c.ShippingCharge.HasValue ? c.ShippingCharge.Value : 0,
                                                                   FinalAmt = p.FinalItemPrice.HasValue ? p.FinalItemPrice.Value : 0,
                                                                   Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                                   ItemStatus = p.ItemStatus.HasValue ? p.ItemStatus.Value : 1,
                                                                   IsReturnable = c.IsReturnable.HasValue ? c.IsReturnable.Value : false
                                                               }).OrderByDescending(x => x.OrderItemId).ToList();

                            decimal totlmt = 0;
                            decimal OldOrderTotl = 0;
                            decimal shipingchrgs = 0;
                            if (lstOrderItms != null && lstOrderItms.Count() > 0)
                            {
                                foreach (var objj in lstOrderItms)
                                {
                                    totlmt = totlmt + objj.FinalAmt;
                                    shipingchrgs = shipingchrgs + Math.Round(objj.ShipingChargeOf1Item * objj.Qty, 2);
                                }
                            }
                            OldOrderTotl = totlmt;
                            var objtbl_ExtraAmount = _db.tbl_ExtraAmount.Where(o => o.AmountFrom <= totlmt && o.AmountTo >= totlmt).FirstOrDefault();
                            decimal extramt = 0;
                            if (objtbl_ExtraAmount != null)
                            {
                                extramt = objtbl_ExtraAmount.ExtraAmount.Value;
                            }
                            OldOrderTotl = OldOrderTotl + extramt + shipingchrgs;
                            decimal shipchrge = Math.Round(objproditm.ShippingCharge.Value * objitm.Qty.Value, 2);
                            decimal NewShipingCharge = shipingchrgs - shipchrge;
                            decimal NewOrdeAmt = totlmt - objitm.FinalItemPrice.Value;
                            var objtbl_ExtraAmountnew = _db.tbl_ExtraAmount.Where(o => o.AmountFrom <= NewOrdeAmt && o.AmountTo >= NewOrdeAmt).FirstOrDefault();
                            decimal amtextrnew = 0;
                            decimal OrderTotlNew = 0;
                            if (objtbl_ExtraAmountnew != null)
                            {
                                amtextrnew = objtbl_ExtraAmountnew.ExtraAmount.Value;
                            }
                            OrderTotlNew = NewOrdeAmt + amtextrnew + NewShipingCharge;
                            decimal refund = OldOrderTotl - OrderTotlNew;
                            if (refund >= 0)
                            {
                                amtrefund = refund;
                            }
                            else
                            {
                                IsRetunfalse = true;
                                strmsgretun = "Can Not Cancel";
                            }
                            //amtrefund = shipchrge + amtrefund;
                        }
                        if(IsRetunfalse == false)
                        {
                            objitm.ItemStatus = 5;
                            objitm.IsDelete = true;
                            if (objordr.IsCashOnDelivery.Value == true)
                            {
                                objordr.AmountDue = objordr.AmountDue - amtrefund;
                            }
                            else
                            {
                                tbl_Wallet objWlt = new tbl_Wallet();
                                objWlt.Amount = amtrefund;
                                objWlt.CreditDebit = "Credit";
                                objWlt.ItemId = objitm.OrderDetailId;
                                objWlt.OrderId = objitm.OrderId;
                                objWlt.ClientUserId = objordr.ClientUserId;
                                objWlt.WalletDate = DateTime.UtcNow;
                                objWlt.Description = "Amount Refund to Wallet Order #" + objitm.OrderId;
                                _db.tbl_Wallet.Add(objWlt);
                                var objClient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == objordr.ClientUserId).FirstOrDefault();
                                if (objClient != null)
                                {
                                    decimal amtwlt = objClient.WalletAmt.HasValue ? objClient.WalletAmt.Value : 0;
                                    amtwlt = amtwlt + amtrefund;
                                    objClient.WalletAmt = amtwlt;
                                    _db.SaveChanges();
                                }
                                msgsms = "You Item is Cancelled for Order No." + objitm.OrderId + " . Amount Rs." + amtrefund + " Refunded to your wallet";
                                SendMessageSMS(objClient.MobileNo, msgsms);
                                objCom.SaveTransaction(objproditm.ProductItemId, objitm.OrderDetailId, objitm.OrderId.Value, "Item Cancel Request", objitm.FinalItemPrice.Value, UserId, 0, DateTime.UtcNow, "Item Cancel Request");
                                msgsms = "Items has been Cancelled for Order No." + objitm.OrderId;
                                SendMessageSMS(adminmobilenumber, msgsms);
                                objCom.SaveTransaction(objproditm.ProductItemId, objitm.OrderDetailId, objitm.OrderId.Value, "Cancel Item amount Refund to Wallet Rs" + amtrefund, amtrefund, UserId, 0, DateTime.UtcNow, "Item Cancel Refund");
                                //SendMessageSMS(objClient.MobileNo,);
                                _db.SaveChanges();
                            }
                        }                      
                    }
                    else if (status == "6")
                    {
                        objitm.ItemStatus = 6;
                        msgsms = "Item Return Request Received for Order No." + objitm.OrderId;
                        amtrefund = objitm.FinalItemPrice.Value;
                        SendMessageSMS(adminmobilenumber, msgsms);
                        objCom.SaveTransaction(objproditm.ProductItemId, objitm.OrderDetailId, objitm.OrderId.Value, "Item Return Request Sent", objitm.FinalItemPrice.Value, UserId, 0, DateTime.UtcNow, "Item Return Request Sent");
                    }
                    else if (status == "7")
                    {
                        objitm.ItemStatus = 7;
                        msgsms = "Item Replace Request Received for Order No." + objitm.OrderId;
                        amtrefund = objitm.FinalItemPrice.Value;
                        SendMessageSMS(adminmobilenumber, msgsms);
                        objCom.SaveTransaction(objproditm.ProductItemId, objitm.OrderDetailId, objitm.OrderId.Value, "Item Replace Request Sent", objitm.FinalItemPrice.Value,UserId, 0, DateTime.UtcNow, "Item Replace Request Sent");
                    }
                    else if (status == "8")
                    {
                        objitm.ItemStatus = 8;
                        msgsms = "Item Exchange Request Received for Order No." + objitm.OrderId;
                        amtrefund = objitm.FinalItemPrice.Value;
                        SendMessageSMS(adminmobilenumber, msgsms);
                        objCom.SaveTransaction(objproditm.ProductItemId, objitm.OrderDetailId, objitm.OrderId.Value, "Item Exchange Request Sent", objitm.FinalItemPrice.Value, UserId, 0, DateTime.UtcNow, "Item Exchange Request Sent");
                    }
                    tbl_ItemReturnCancelReplace objitmreplce = new tbl_ItemReturnCancelReplace();
                    objitmreplce.ItemId = objitm.OrderDetailId;
                    objitmreplce.OrderId = objitm.OrderId;
                    objitmreplce.Amount = amtrefund;
                    objitmreplce.Reason = resn;
                    objitmreplce.ItemStatus = Convert.ToInt32(status);
                    objitmreplce.ClientUserId = objordr.ClientUserId;
                    if (status == "5")
                    {
                        objitmreplce.IsApproved = IsApprov;
                    }
                    objitmreplce.DateCreated = DateTime.UtcNow;
                    objitmreplce.ModifiedBy = objordr.ClientUserId;
                    objitmreplce.DateModified = DateTime.UtcNow;
                    _db.tbl_ItemReturnCancelReplace.Add(objitmreplce);
                    _db.SaveChanges();
                }

                if (IsRetunfalse == true)
                {
                    response.Data = strmsgretun;
                }
                else
                {
                    response.Data = "Success";
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

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

        [Route("SendOTPForItemAction"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTPForItemAction(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    string msg = "Your OTP code for Item action is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNum + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        response.AddError("Invalid Mobile Number");
                    }
                    else
                    {                       
                        objOtp.Otp = num.ToString();
                        response.Data = objOtp;
                    }
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }
    }
}