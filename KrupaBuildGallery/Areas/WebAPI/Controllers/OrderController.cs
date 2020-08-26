using HiQPdf;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.AccessControl;
using System.Text;
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
                                ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0,
                                HasFreeItems = p.HasFreeItems.HasValue ? p.HasFreeItems.Value : false,
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
                                                           FinalAmt = p.FinalItemPrice.Value,
                                                           Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                           VariantQtytxt = vr.UnitQty,                                                           
                                                           ItemStatus = p.ItemStatus.HasValue ? p.ItemStatus.Value : 1,
                                                           IsReturnable = c.IsReturnable.HasValue ? c.IsReturnable.Value : false,
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
                        objCom.SavePaymentTransaction(0, objordr.OrderId, true, objordr.ShippingCharge.Value, "Payment By Online for Shipping Charge", UserId, false, DateTime.UtcNow, "Online Payment");
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
                        objCom.SavePaymentTransaction(0, orderid64, true, amountpaid, "Payment By Online for Due Amount", UserId, false, DateTime.UtcNow, "Online Payment");
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
                                                                   IsReturnable = c.IsReturnable.HasValue ? c.IsReturnable.Value : false,
                                                                   IsCombo = p.IsCombo.HasValue ? p.IsCombo.Value : false,
                                                                   ComboQty = p.ComboQty.HasValue ? p.ComboQty.Value : 0,
                                                                   ComboId = p.ComboId.HasValue ? p.ComboId.Value : 0
                                                               }).OrderByDescending(x => x.OrderItemId).ToList();

                            decimal totlmt = 0;
                            decimal OldOrderTotl = 0;
                            decimal shipingchrgs = 0;
                            if (lstOrderItms != null && lstOrderItms.Count() > 0)
                            {
                                foreach (var objj in lstOrderItms)
                                {
                                    totlmt = totlmt + objj.FinalAmt;
                                    if (objj.IsCombo)
                                    {
                                        shipingchrgs = shipingchrgs + Math.Round(objj.ShipingChargeOf1Item * objj.ComboQty, 2);
                                    }
                                    else
                                    {
                                        shipingchrgs = shipingchrgs + Math.Round(objj.ShipingChargeOf1Item * objj.Qty, 2);
                                    }                                 
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
                            if (objitm.IsCombo == true)
                            {
                                shipchrge = Math.Round(objproditm.ShippingCharge.Value * objitm.Qty.Value, 2);
                            }
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
                            objitm.UpdatedDate = DateTime.UtcNow;
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
                                objCom.SavePaymentTransaction(objitm.OrderDetailId, objitm.OrderId.Value, false, amtrefund, "Payment To Wallet Refund", UserId, false, DateTime.UtcNow, "Wallet");
                                //SendMessageSMS(objClient.MobileNo,);
                                _db.SaveChanges();
                            }
                        }

                        tbl_StockReport objstkreport = new tbl_StockReport();
                        objstkreport.FinancialYear = clsCommon.GetCurrentFinancialYear();
                        objstkreport.StockDate = DateTime.UtcNow;
                        objstkreport.Qty = Convert.ToInt64(objitm.QtyUsed);
                        objstkreport.IsCredit = true;
                        objstkreport.IsAdmin = false;
                        objstkreport.CreatedBy = UserId;
                        objstkreport.ItemId = objitm.ProductItemId;
                        objstkreport.Remarks = "Ordered Item Cancelled:" + objitm.OrderId;
                        _db.tbl_StockReport.Add(objstkreport);
                        _db.SaveChanges();
                        if (objitm.IsCombo == true)
                        {
                            List<tbl_OrderItemDetails> lstitmms = _db.tbl_OrderItemDetails.Where(o => o.ComboId == objitm.ComboId && o.OrderDetailId != objitm.OrderDetailId).ToList();
                            if (lstitmms != null && lstitmms.Count() > 0)
                            {
                                foreach (tbl_OrderItemDetails objj in lstitmms)
                                {
                                    objj.ItemStatus = 5;
                                    objj.IsDelete = true;
                                    objj.UpdatedDate = DateTime.UtcNow;
                                    tbl_StockReport objstkreports = new tbl_StockReport();
                                    objstkreports.FinancialYear = clsCommon.GetCurrentFinancialYear();
                                    objstkreports.StockDate = DateTime.UtcNow;
                                    objstkreports.Qty = Convert.ToInt64(objitm.QtyUsed);
                                    objstkreports.IsCredit = true;
                                    objstkreports.IsAdmin = false;
                                    objstkreports.CreatedBy = UserId;
                                    objstkreports.ItemId = objj.ProductItemId;
                                    objstkreports.Remarks = "Ordered Item Cancelled:" + objitm.OrderId;
                                    _db.tbl_StockReport.Add(objstkreports);
                                    _db.SaveChanges();
                                }
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
                    objitmreplce.IsCombo = objitm.IsCombo.HasValue ? objitm.IsCombo.Value : false;
                    objitmreplce.ComboId = objitm.ComboId.HasValue ? objitm.ComboId.Value : 0;
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


        [Route("GetOrderListForAgent"), HttpPost]
        public ResponseDataModel<List<OrderVM>> GetOrderListForAgent(GeneralVM objGen)
        {
            ResponseDataModel<List<OrderVM>> response = new ResponseDataModel<List<OrderVM>>();
            List<OrderVM> lstOrder = new List<OrderVM>();
            try
            {
                long AdminUser = Convert.ToInt64(objGen.ClientUserId);
                long StatusId = Convert.ToInt64(objGen.StatusId);
                List<long> lstOrderIds = _db.tbl_OrderItemDelivery.Where(o => o.DelieveryPersonId == AdminUser).Where(o => o.Status == StatusId).Select(o => o.OrderId.Value).Distinct().ToList();
                lstOrder = (from p in _db.tbl_Orders
                            join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                            where lstOrderIds.Contains(p.OrderId) 
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
                            }).OrderByDescending(x => x.OrderDate).ToList();
                if (lstOrder != null && lstOrder.Count() > 0)
                {
                    lstOrder.ForEach(x => { x.OrderStatus = GetOrderStatus(x.OrderStatusId); x.OrderDateString = CommonMethod.ConvertFromUTC(x.OrderDate); });
                }

                response.Data = lstOrder;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetOrderDetailsForAgentByOrderId"), HttpPost]
        public ResponseDataModel<OrderVM> GetOrderDetailsForAgentByOrderId(GeneralVM objGen)
        {
            ResponseDataModel<OrderVM> response = new ResponseDataModel<OrderVM>();
            OrderVM objOrdr = new OrderVM();
            try
            {
                long OrderId = Convert.ToInt64(objGen.OrderId);
                long AgntUsrId = Convert.ToInt64(objGen.ClientUserId);
                long AgentId = Convert.ToInt64(objGen.AgentId);
                objOrdr = (from p in _db.tbl_Orders
                            join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                            where p.OrderId == OrderId
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
                                ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0,
                                IsExtraAmountReceived = p.IsExtraAmountReceived.HasValue ? p.IsExtraAmountReceived.Value : false
                                
                            }).OrderByDescending(x => x.OrderDate).FirstOrDefault();

                if (objOrdr != null)
                {
                    objOrdr.OrderStatus = GetOrderStatus(objOrdr.OrderStatusId);
                   List<long> lstOrdritmid = _db.tbl_OrderItemDelivery.Where(o => o.OrderId == OrderId && o.DelieveryPersonId == AgntUsrId).Select(x => x.OrderItemId.Value).ToList();

                    List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                       join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                       join vr in _db.tbl_ItemVariant on p.VariantItemId equals vr.VariantItemId
                                                       where p.OrderId == OrderId && lstOrdritmid.Contains(p.OrderDetailId)
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
                                                           FinalAmt = p.FinalItemPrice.Value,
                                                           IGSTAmt = p.IGSTAmt.Value,
                                                           ItemImg = c.MainImage,
                                                           ShipingChargeOf1Item = c.ShippingCharge.HasValue ? c.ShippingCharge.Value : 0,
                                                           Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                           VariantQtytxt = vr.UnitQty,
                                                           IsReplace = p.IsReplacedItem.HasValue ? p.IsReplacedItem.Value : false,
                                                           ItemStatus = p.IsReplacedItem == true ? 4 : p.ItemStatus.HasValue ? p.ItemStatus.Value : 1,
                                                           IsReturnable = c.IsReturnable.HasValue ? c.IsReturnable.Value : false
                                                       }).OrderByDescending(x => x.OrderItemId).ToList();

                    List<OrderItemsVM> lstOrderItmsReplace = (from r in _db.tbl_ItemReplace
                                                              join p in _db.tbl_OrderItemDetails on r.ItemDetailId equals p.OrderDetailId
                                                              join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                              join vr in _db.tbl_ItemVariant on p.VariantItemId equals vr.VariantItemId
                                                              where p.OrderId == OrderId && lstOrdritmid.Contains(r.ItemDetailId.Value)
                                                              select new OrderItemsVM
                                                              {
                                                                  OrderId = p.OrderId.Value,
                                                                  OrderItemId = p.OrderDetailId,
                                                                  ProductItemId = p.ProductItemId.Value,
                                                                  ItemName = p.ItemName,
                                                                  Qty = p.Qty.Value,
                                                                  Price = 0,
                                                                  Sku = p.Sku,
                                                                  GSTAmt = p.GSTAmt.Value,
                                                                  FinalAmt = 0,
                                                                  IGSTAmt = p.IGSTAmt.Value,
                                                                  ItemImg = c.MainImage,
                                                                  ShipingChargeOf1Item = 0,
                                                                  Discount = 0,
                                                                  VariantQtytxt = vr.UnitQty,
                                                                  ItemStatus = r.ItemStatus.Value,
                                                                  IsReplace = false,
                                                                  IsReturnable = c.IsReturnable.HasValue ? c.IsReturnable.Value : false
                                                              }).OrderByDescending(x => x.OrderItemId).ToList();

                    lstOrderItms.AddRange(lstOrderItmsReplace);
                List<AdminUserVM> lstAdminusers = (from p in _db.tbl_AdminUsers                              
                                select new AdminUserVM
                                {
                                    AdminUserId = p.AdminUserId,
                                    FirstName = p.FirstName,
                                    LastName = p.LastName,
                                    MobileNo = p.MobileNo,
                                    Address = p.Address,
                                    WorkingTime = p.WorkingTime
                                }).OrderBy(x => x.FirstName).ToList();
                    if (lstOrderItms != null && lstOrderItms.Count() > 0)
                    {
                        lstOrderItms.ForEach(x => { x.ItemStatustxt = GetItemStatus(x.ItemStatus); x.DeliveryPersonName = GetAssignPersonName(x.OrderItemId, lstAdminusers); });
                    }
                    objOrdr.OrderItems = lstOrderItms;
                }

                response.Data = objOrdr;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }
            return response;
        }

        public string GetAssignPersonName(long OrderItemId, List<AdminUserVM> lstuser)
        {
           var objPersonN = _db.tbl_OrderItemDelivery.Where(o => o.OrderItemId == OrderItemId).OrderByDescending(x => x.OrderItemDeliveryId).FirstOrDefault();
           if(objPersonN != null && lstuser != null)
            {
                var objusr = lstuser.Where(o => o.AdminUserId == objPersonN.DelieveryPersonId).FirstOrDefault();
                if(objusr != null)
                {
                   return objusr.FirstName + " " + objusr.LastName;
                }
            }
            return "";
        }

        [Route("AssignDelieveryPerson"), HttpPost]
        public ResponseDataModel<string> AssignDelieveryPerson(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();         
            try
            {
                long DelieveryPrsnId = objGen.DelieveryPersonId;
                long OrdrId = Convert.ToInt64(objGen.OrderId);
                long AgentId = Convert.ToInt64(objGen.AgentId);
                string OrderItemIds = objGen.OrderDetailId;
                clsCommon objCommon = new clsCommon();
                tbl_AdminUsers objAdminUsr = _db.tbl_AdminUsers.Where(o => o.AdminUserId == DelieveryPrsnId).FirstOrDefault();
                string ItmsText = "";
                OrderItemIds = OrderItemIds.Trim('^');
                if (!string.IsNullOrEmpty(OrderItemIds))
                {
                    string[] strordditm = OrderItemIds.Split('^');
                    foreach (string ss in strordditm)
                    {
                        long OrderItemId = Convert.ToInt64(ss);
                        tbl_OrderItemDetails objOrderItm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == OrderItemId).FirstOrDefault();
                     
                        tbl_ItemVariant objVrnt = _db.tbl_ItemVariant.Where(o => o.VariantItemId == objOrderItm.VariantItemId).FirstOrDefault();
                        if (objVrnt != null)
                        {
                            ItmsText = ItmsText + objOrderItm.ItemName + "-" + objVrnt.UnitQty + ",";
                        }
                        else
                        {
                            ItmsText = ItmsText + objOrderItm.ItemName + ",";
                        }
                        tbl_OrderItemDelivery objOrderItmDlv = _db.tbl_OrderItemDelivery.Where(o => o.OrderItemId == OrderItemId && o.Status == 3 && o.DelieveryPersonId != AgentId).FirstOrDefault();
                        if(objOrderItmDlv == null)
                        {
                            objOrderItmDlv = new tbl_OrderItemDelivery();
                            objOrderItmDlv.OrderId = OrdrId;
                            objOrderItmDlv.OrderItemId = OrderItemId;
                            objOrderItmDlv.Status = 3;
                            objOrderItmDlv.DelieveryPersonId = DelieveryPrsnId;
                            objOrderItmDlv.AssignedBy = AgentId;
                            objOrderItmDlv.AssignedDate = DateTime.UtcNow;
                            _db.tbl_OrderItemDelivery.Add(objOrderItmDlv);
                            _db.SaveChanges();
                        }
                        else
                        {
                            objOrderItmDlv.OrderItemId = OrderItemId;
                            objOrderItmDlv.Status = 3;
                            objOrderItmDlv.DelieveryPersonId = DelieveryPrsnId;
                            objOrderItmDlv.AssignedBy = AgentId;
                            objOrderItmDlv.AssignedDate = DateTime.UtcNow;                            
                            _db.SaveChanges();
                        }
                        
                        objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Delivery Person " + objAdminUsr.FirstName + " " + objAdminUsr.LastName + " Assign to Dispatch Item", 0, 0, AgentId, DateTime.UtcNow, "Delievery Person Assigned");
                    }
                    _db.SaveChanges();
                }
                using (WebClient webClient = new WebClient())
                {
                    string msg = "Order no." + OrdrId + " \nItem: " + ItmsText + " \nhas been assigned to you for delivery";
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objAdminUsr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(objAdminUsr.Email))
                        {
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;

                            string msg1 = "Order no." + OrdrId + "Item: " + ItmsText.Trim(',') + "Has Been Assigned To You For Delivery.";
                            clsCommon.SendEmail(objAdminUsr.Email, FromEmail, "Assigned New Item Delivery - Shopping & Saving", msg1);
                        }
                    }

                }


                response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SetDeliveredItems"), HttpPost]
        public ResponseDataModel<string> SetDeliveredItems(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                long DelieveryPrsnId = objGen.DelieveryPersonId;
                long OrdrId = Convert.ToInt64(objGen.OrderId);
                long AgentId = Convert.ToInt64(objGen.AgentId);
                string OrderItemIds = objGen.OrderDetailId;
                clsCommon objCommon = new clsCommon();
                tbl_AdminUsers objAdminUsr = _db.tbl_AdminUsers.Where(o => o.AdminUserId == DelieveryPrsnId).FirstOrDefault();
                if(objAdminUsr.ParentAgentId != null && objAdminUsr.ParentAgentId != 0)
                {
                    AgentId = objAdminUsr.ParentAgentId.Value;
                }
                
                tbl_Orders objrd = _db.tbl_Orders.Where(o => o.OrderId == OrdrId).FirstOrDefault();
                long ClientUserId = 0;
                if(objrd != null)
                {
                    ClientUserId = objrd.ClientUserId;                    
                }
                string mobileclient = "";
                tbl_ClientUsers objClient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                if(objClient != null)
                {
                    mobileclient = objClient.MobileNo;
                }

                string ItmsText = "";
                OrderItemIds = OrderItemIds.Trim('^');
                bool IsExtrapaid = false;
                if (!string.IsNullOrEmpty(OrderItemIds))
                {
                    string[] strordditm = OrderItemIds.Split('^');
                    foreach (string ss in strordditm)
                    {
                        long OrderItemId = Convert.ToInt64(ss);
                        if (DelieveryPrsnId == AgentId)
                        {
                            var objDelvItmm = _db.tbl_OrderItemDelivery.Where(o => o.DelieveryPersonId != AgentId && o.Status == 3 && o.OrderItemId == OrderItemId).FirstOrDefault();
                            if (objDelvItmm != null)
                            {
                                _db.tbl_OrderItemDelivery.Remove(objDelvItmm);
                                _db.SaveChanges();
                            }
                        }
                        tbl_OrderItemDetails objOrderItm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == OrderItemId).FirstOrDefault();
                      
                        tbl_ItemVariant objVrnt = _db.tbl_ItemVariant.Where(o => o.VariantItemId == objOrderItm.VariantItemId).FirstOrDefault();
                        if (objVrnt != null)
                        {
                            ItmsText = ItmsText + objOrderItm.ItemName + "-" + objVrnt.UnitQty + ",";
                        }
                        else
                        {
                            ItmsText = ItmsText + objOrderItm.ItemName + ",";
                        }
                     
                        List<tbl_OrderItemDelivery> lstorddelv = _db.tbl_OrderItemDelivery.Where(o => o.OrderItemId == OrderItemId && o.Status == 3).ToList();
                        if(lstorddelv != null && lstorddelv.Count() > 0)
                        {
                            bool IsExtrp = true;
                            foreach(var objj in lstorddelv)
                            {
                                objj.Status = 4;
                                if (objrd.IsCashOnDelivery == true && objrd.OrderShipPincode == "389001")
                                {
                                    if(objj.ReplaceId != null && objj.ReplaceId > 0)
                                    {
                                        IsExtrp = false;
                                        objj.AmountToReceived = 0;
                                        var objItmrplc = _db.tbl_ItemReplace.Where(o => o.ItemReplaceId == objj.ReplaceId).FirstOrDefault();
                                        objItmrplc.ItemStatus = 4;
                                    }
                                    else
                                    {
                                        if(objOrderItm.FinalItemPrice > 0)
                                        {
                                            tbl_ProductItems objprod = _db.tbl_ProductItems.Where(o => o.ProductItemId == objOrderItm.ProductItemId).FirstOrDefault();
                                            decimal shipcharge = Math.Round(objOrderItm.Qty.Value * objprod.ShippingCharge.Value, 2);
                                            decimal extramtt = objrd.ExtraAmount.HasValue ? objrd.ExtraAmount.Value : 0;
                                            bool IsExtramtrec = objrd.IsExtraAmountReceived.HasValue ? objrd.IsExtraAmountReceived.Value : false;
                                            if (IsExtrapaid == false && IsExtramtrec == false)
                                            {
                                                objj.AmountToReceived = shipcharge + objOrderItm.FinalItemPrice + extramtt;
                                            }
                                            else
                                            {
                                                objj.AmountToReceived = shipcharge + objOrderItm.FinalItemPrice;
                                            }
                                            objCommon.SavePaymentTransaction(0, objrd.OrderId, true, objj.AmountToReceived.Value, "Payment By Cash", ClientUserId, false, DateTime.UtcNow, "Cash");
                                            objrd.AmountDue = objrd.AmountDue - objj.AmountToReceived;
                                        }
                                        else
                                        {
                                            IsExtrp = false;
                                        }
                                        
                                    }
                                   
                                }
                                else if(objrd.IsCashOnDelivery == true && objrd.OrderShipPincode != "389001")
                                {
                                    if (objj.ReplaceId != null && objj.ReplaceId > 0)
                                    {
                                        IsExtrp = false;
                                        objj.AmountToReceived = 0;
                                        var objItmrplc = _db.tbl_ItemReplace.Where(o => o.ItemReplaceId == objj.ReplaceId).FirstOrDefault();
                                        objItmrplc.ItemStatus = 4;
                                    }
                                    else
                                    {
                                        if (objOrderItm.FinalItemPrice > 0)
                                        {
                                            decimal extramtt = objrd.ExtraAmount.HasValue ? objrd.ExtraAmount.Value : 0;
                                            bool IsExtramtrec = objrd.IsExtraAmountReceived.HasValue ? objrd.IsExtraAmountReceived.Value : false;
                                            if (IsExtrapaid == false && IsExtramtrec == false)
                                            {
                                                objj.AmountToReceived = objOrderItm.FinalItemPrice + extramtt;
                                            }
                                            else
                                            {
                                                objj.AmountToReceived = objOrderItm.FinalItemPrice;
                                            }
                                            objrd.AmountDue = objrd.AmountDue - objj.AmountToReceived;
                                            objCommon.SavePaymentTransaction(0, objrd.OrderId, true, objj.AmountToReceived.Value, "Payment By Cash", ClientUserId, false, DateTime.UtcNow, "Cash");
                                        }
                                        else
                                        {
                                            IsExtrp = false;
                                        }
                                    }
                                   
                                }
                                if(objj.DelieveryPersonId != DelieveryPrsnId)
                                {
                                    objj.AmountToReceived = 0;
                                }
                            }
                            if(IsExtrp == true)
                            {
                                IsExtrapaid = true;
                                objrd.IsExtraAmountReceived = true;
                            }                            
                            _db.SaveChanges();
                        }
                        objOrderItm.ItemStatus = 4;
                        objOrderItm.IsReplacedItem = false;
                        _db.SaveChanges();
                        
                        objCommon.SaveTransaction(objOrderItm.ProductItemId.Value, objOrderItm.OrderDetailId, objOrderItm.OrderId.Value, "Delivered By " + objAdminUsr.FirstName + " " + objAdminUsr.LastName, 0, 0, AgentId, DateTime.UtcNow, "Delievered Items");
                    }
                    _db.SaveChanges();
                    if(objrd.OrderStatusId == 3)
                    {
                        List<tbl_OrderItemDetails> lstOrderTms1 = _db.tbl_OrderItemDetails.Where(o => o.OrderId == OrdrId && o.ItemStatus != 5 && o.ItemStatus != 3 && o.ItemStatus != 4).ToList();
                        if (lstOrderTms1 == null || lstOrderTms1.Count == 0)
                        {
                            objrd.OrderStatusId = 4;
                        }
                    }
                    _db.SaveChanges();
                }
                using (WebClient webClient = new WebClient())
                {
                    string msg = "Order no." + OrdrId + " \nItem: " + ItmsText + " \nhas been delivered";
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + mobileclient + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {

                    }
                    else
                    {
                       
                    }

                }


                response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SendOTPForDelivery"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTPForDelivery(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                long orderid = Convert.ToInt64(objOtpVM.OrderId);
                string GetCashMessage = objOtpVM.PriceString;
                var objOrder = _db.tbl_Orders.Where(o => o.OrderId == orderid).FirstOrDefault();
                string mobilnumber = "";
                if(objOrder != null)
                {
                    mobilnumber = objOrder.OrderShipClientPhone;
                }
                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    if(!string.IsNullOrEmpty(GetCashMessage))
                    {
                        GetCashMessage = "Please Give Cash" + GetCashMessage + " And ";
                    }
                    string msg = GetCashMessage+"Your OTP code for Item Delivery is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + mobilnumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
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

        [Route("GetStatsOfDelivery"), HttpPost]
        public ResponseDataModel<GeneralVM> GetStatsOfDelivery(GeneralVM objGeneralVM)
        {
            ResponseDataModel<GeneralVM> response = new ResponseDataModel<GeneralVM>();
            GeneralVM objGeneralVM1 = new GeneralVM();
            try
            {
                long CustId = Convert.ToInt64(objGeneralVM.ClientUserId);
                int TotalPendingDelv =  _db.tbl_OrderItemDelivery.Where(o => o.Status == 3 && o.DelieveryPersonId == CustId).ToList().Count();
                int TotalDelv = _db.tbl_OrderItemDelivery.Where(o => o.Status == 4 && o.DelieveryPersonId == CustId).ToList().Count();
                objGeneralVM1.TotalPendingDeliveryItems = TotalPendingDelv.ToString();
                objGeneralVM1.TotalDeliveredItem = TotalDelv.ToString();
                response.Data = objGeneralVM1;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetRatingReview"), HttpPost]
        public ResponseDataModel<tbl_ReviewRating> GetRatingReview(GeneralVM objGen)
        {
            ResponseDataModel<tbl_ReviewRating> response = new ResponseDataModel<tbl_ReviewRating>();
            try
            {
                long OrderDetailId = Convert.ToInt64(objGen.OrderDetailId);
                tbl_ReviewRating objtbl_ReviewRating = _db.tbl_ReviewRating.Where(o => o.OrderDetailId == OrderDetailId).FirstOrDefault();
                if (objtbl_ReviewRating == null)
                {
                    objtbl_ReviewRating = new tbl_ReviewRating();
                }
                response.Data = objtbl_ReviewRating;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SaveRatingReview"), HttpPost]
        public ResponseDataModel<string> SaveRatingReview(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                long OrdrDtlid = Convert.ToInt64(objGen.OrderDetailId);
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                string Ratings = objGen.Ratings;
                string Reviews = objGen.Reviews;
                tbl_ReviewRating objrt = _db.tbl_ReviewRating.Where(o => o.OrderDetailId == OrdrDtlid).FirstOrDefault();
                if (objrt == null)
                {
                    var objItms = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == OrdrDtlid).FirstOrDefault();
                    objrt = new tbl_ReviewRating();
                    objrt.OrderDetailId = OrdrDtlid;
                    objrt.ProductItemId = objItms.ProductItemId;
                    objrt.ClientUserId = UserId;
                    objrt.Rating = Convert.ToDecimal(Ratings);
                    objrt.Review = Reviews;
                    objrt.CreatedDate = DateTime.UtcNow;
                    _db.tbl_ReviewRating.Add(objrt);
                }
                else
                {
                    objrt.Rating = Convert.ToDecimal(Ratings);
                    objrt.Review = Reviews;
                }
                _db.SaveChanges();

                response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("UpdateOrderShipMobileNumber"), HttpPost]
        public ResponseDataModel<string> UpdateOrderShipMobileNumber(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                long OrderId = Convert.ToInt64(objGen.OrderId);
                string MobileNumber = Convert.ToString(objGen.MobileNumber);
                tbl_Orders objOrdr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
                if(objOrdr != null)
                {
                    objOrdr.OrderShipClientPhone = MobileNumber;
                }
                _db.SaveChanges();

                response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("DownloadInvoice"), HttpPost]
        public ResponseDataModel<string> DownloadInvoice(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                long OrdrDtlid = Convert.ToInt64(objGen.OrderId);
                StreamReader sr;
                string newhtmldata = "";

                OrderVM objOrder = new OrderVM();
                objOrder = (from p in _db.tbl_Orders
                            join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                            join u in _db.tbl_ClientOtherDetails on c.ClientUserId equals u.ClientUserId
                            where p.OrderId == OrdrDtlid
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
                                                       where p.OrderId == OrdrDtlid
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
                    string file = HttpContext.Current.Server.MapPath("~/templates/Invoice.html");
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
                string invfile = "Inv" + objOrder.OrderId + ".pdf";
                // convert HTML code to a PDF memory buffer
                htmlToPdfConverter.ConvertHtmlToFile(newhtmldata, "", HttpContext.Current.Server.MapPath("~/Documents/") + invfile);

                response.Data = invfile;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        private string getGSTCalculationHtmlDataByOrder(List<OrderItemsVM> lstOrderItms, bool IsIGST)
        {
            string htmlData = string.Empty;

            StringBuilder srBuild = new StringBuilder();


            decimal[] lstGSTPer = new decimal[] { 0.00m, 5.00m, 12.00m, 18.00m, 28.00m };

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
    }
}