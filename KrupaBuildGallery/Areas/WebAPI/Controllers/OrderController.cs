using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}