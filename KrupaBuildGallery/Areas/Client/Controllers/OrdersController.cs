using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Razorpay.Api;
using System;
using System.Collections.Generic;
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
    }
}