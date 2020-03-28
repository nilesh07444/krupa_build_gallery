using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Razorpay.Api;
namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public CheckoutController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Checkout
        public ActionResult Index()
        {
            List<CartVM> lstCartItems = new List<CartVM>();          
            try
            {
              
                ViewBag.WebsiteOrderId = "1";

               
                string GuidNew = Guid.NewGuid().ToString();
                string cookiesessionval = "";
                if (Request.Cookies["sessionkeyval"] != null)
                {
                    cookiesessionval = Request.Cookies["sessionkeyval"].Value;
                }
                else
                {
                    cookiesessionval = GuidNew;
                    Response.Cookies["sessionkeyval"].Value = GuidNew;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                }
                if (clsClientSession.UserID > 0)
                {
                    long ClientUserId = Convert.ToInt64(clsClientSession.UserID);
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    where crt.ClientUserId == ClientUserId
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price); });
                }
                else
                {
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    where crt.CartSessionId == cookiesessionval
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = i.CustomerPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price); });
                }

                decimal creditlimitreminng = 0;
                if (clsClientSession.UserID > 0 && clsClientSession.RoleID == 2)
                {
                    long UserID = clsClientSession.UserID;
                    tbl_ClientOtherDetails objclientothr = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == UserID).FirstOrDefault();
                    if(objclientothr != null && objclientothr.CreditLimitAmt != null && objclientothr.CreditLimitAmt > 0)
                    {
                        decimal amountdue = 0;
                        if(objclientothr.AmountDue != null)
                        {
                            amountdue = objclientothr.AmountDue.Value;                            
                        }
                        creditlimitreminng = objclientothr.CreditLimitAmt.Value - amountdue;
                    }
                }

                ViewBag.CreditRemaining = creditlimitreminng;
                tbl_ClientOtherDetails objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                ViewData["objotherdetails"] = objotherdetails;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            ViewData["lstCartItems"] = lstCartItems;
            return View();
        }

        public ActionResult OrderSuccess(string Id)
        {
            string orderid = clsCommon.DecryptString(Id.ToString());
            long OrderID64 = Convert.ToInt64(orderid);
            OrderVM objOrder = new OrderVM();
            objOrder = (from p in _db.tbl_Orders
                        join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                        where p.OrderId == OrderID64
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
                            RazorpayOrderId = p.RazorpayOrderId,
                            OrderDate = p.CreatedDate
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();
            if (objOrder != null)
            {
             //   objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                   join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                   where p.OrderId == OrderID64
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
                                                       ItemImg = c.MainImage
                                                   }).OrderByDescending(x => x.OrderItemId).ToList();
                objOrder.OrderItems = lstOrderItms;
            }
            return View(objOrder);
        }

        public PartialViewResult CreateRazorPaymentOrder(decimal Amount,string description)
        {            
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", Amount * 100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", "12121");
            input.Add("payment_capture", 1);

            string key = "rzp_test_DMsPlGIBp3SSnI";
            string secret = "YMkpd9LbnaXViePncLLXhqms";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.OrderId = order["id"];
            ViewBag.Description = description;
            ViewBag.Amount = Amount * 100;
            return PartialView("~/Areas/Client/Views/Checkout/_RazorPayPayment.cshtml");
        }

        [HttpPost]
        public JsonResult PlaceOrder(CheckoutVM objCheckout,string razorpay_payment_id,string razorpay_order_id,string razorpay_signature)
        {
            string ReturnMessage = "";
            long clientusrid = clsClientSession.UserID;
            try
            {
                if(razorpay_payment_id == "ByCredit")
                {
                    List<CartVM> lstCartItems = (from crt in _db.tbl_Cart
                                                 join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                                 where crt.ClientUserId == clientusrid
                                                 select new CartVM
                                                 {
                                                     CartId = crt.Cart_Id,
                                                     ItemName = i.ItemName,
                                                     ItemId = i.ProductItemId,
                                                     Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                                     ItemImage = i.MainImage,
                                                     Qty = crt.CartItemQty.Value,
                                                     ItemSku = i.Sku,
                                                     GSTPer = i.GST_Per,
                                                     IGSTPer = i.IGST_Per
                                                 }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price); });
                    // List<tbl_Cart> lstCarts = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                    string paymentmethod = "ByCredit";
                    
                    tbl_Orders objOrder = new tbl_Orders();
                    objOrder.ClientUserId = clientusrid;
                    objOrder.OrderAmount = Convert.ToDecimal(objCheckout.Orderamount);
                    objOrder.OrderShipCity = objCheckout.shipcity+" - "+ objCheckout.shippincode;
                    objOrder.OrderShipAddress = objCheckout.shipaddress;
                    objOrder.OrderShipState = objCheckout.shipstate;
                    objOrder.OrderShipClientName = objCheckout.shipfirstname + " " + objCheckout.shiplastname;
                    objOrder.OrderShipClientPhone = objCheckout.shipphone;
                    objOrder.OrderStatusId = Convert.ToInt64(OrderStatus.NewOrder);
                    objOrder.PaymentType = paymentmethod;
                    objOrder.IsActive = true;
                    objOrder.IsDelete = false;
                    objOrder.CreatedBy = clientusrid;
                    objOrder.CreatedDate = DateTime.Now;
                    objOrder.UpdatedBy = clientusrid;
                    objOrder.UpdatedDate = DateTime.Now;
                    objOrder.AmountDue = Convert.ToDecimal(objCheckout.Orderamount); 
                    objOrder.RazorpayOrderId = razorpay_order_id;
                    objOrder.RazorpayPaymentId = "";
                    objOrder.RazorSignature = "";
                    _db.tbl_Orders.Add(objOrder);
                    _db.SaveChanges();
                    objOrder.RazorpayOrderId = objOrder.OrderId.ToString();

                    var objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    if(objotherdetails != null)
                    {
                        decimal amtdue = 0;
                        if(objotherdetails.AmountDue != null)
                        {
                            amtdue = objotherdetails.AmountDue.Value;
                            
                        }
                        objotherdetails.AmountDue = amtdue + Convert.ToDecimal(objCheckout.Orderamount);
                        objotherdetails.ShipAddress = objCheckout.shipaddress;
                        objotherdetails.ShipCity = objCheckout.shipcity;
                        objotherdetails.ShipFirstName = objCheckout.shipfirstname;
                        objotherdetails.ShipLastName = objCheckout.shiplastname;
                        objotherdetails.ShipPhoneNumber = objCheckout.shipphone;
                        objotherdetails.ShipPostalcode = objCheckout.shippincode;
                        objotherdetails.ShipState = objCheckout.shipstate;
                        objotherdetails.ShipEmail = objCheckout.shipemailaddress;
                    }
                    _db.SaveChanges();

                    if (lstCartItems != null && lstCartItems.Count() > 0)
                    {
                        foreach (var objCart in lstCartItems)
                        {
                            tbl_OrderItemDetails objOrderItem = new tbl_OrderItemDetails();
                            objOrderItem.OrderId = objOrder.OrderId;
                            objOrderItem.ProductItemId = objCart.ItemId;
                            objOrderItem.ItemName = objCart.ItemName;
                            objOrderItem.IGSTAmt = 0;
                            objOrderItem.Qty = objCart.Qty;
                            objOrderItem.Price = objCart.Price;
                            objOrderItem.Sku = objCart.ItemSku;
                            objOrderItem.IsActive = true;
                            objOrderItem.IsDelete = false;
                            objOrderItem.CreatedBy = clientusrid;
                            objOrderItem.CreatedDate = DateTime.Now;
                            objOrderItem.UpdatedBy = clientusrid;
                            objOrderItem.UpdatedDate = DateTime.Now;
                            decimal InclusiveGST = Math.Round(Convert.ToDecimal(objOrderItem.Price) - Convert.ToDecimal(objOrderItem.Price) * (100 / (100 + objCart.GSTPer)), 2);
                            decimal PreGSTPrice = Math.Round(Convert.ToDecimal(objOrderItem.Price) - InclusiveGST, 2);
                            decimal basicTotalPrice = Math.Round(Convert.ToDecimal(PreGSTPrice * objOrderItem.Qty), 2);
                            decimal SGST = Math.Round(Convert.ToDecimal(objCart.GSTPer / 2), 2);
                            decimal CGST = Math.Round(Convert.ToDecimal(objCart.GSTPer / 2), 2);
                            decimal SGSTAmt = Math.Round((basicTotalPrice * SGST) / 100, 2);
                            decimal CGSTAmt = Math.Round((basicTotalPrice * CGST) / 100, 2);
                            objOrderItem.GSTAmt = SGSTAmt + CGSTAmt;
                            _db.tbl_OrderItemDetails.Add(objOrderItem);
                            var objCartforremove = _db.tbl_Cart.Where(o => o.Cart_Id == objCart.CartId).FirstOrDefault();
                            _db.tbl_Cart.Remove(objCartforremove);
                        }
                        _db.SaveChanges();
                    }

                    string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                    ReturnMessage = "Success^" + orderid;
                }
                else
                {
                    Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(razorpay_payment_id);
                    if (objpymn != null)
                    {
                        if (objpymn["status"] != null && Convert.ToString(objpymn["status"]) == "captured")
                        {
                            List<CartVM> lstCartItems = (from crt in _db.tbl_Cart
                                                         join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                                         where crt.ClientUserId == clientusrid
                                                         select new CartVM
                                                         {
                                                             CartId = crt.Cart_Id,
                                                             ItemName = i.ItemName,
                                                             ItemId = i.ProductItemId,
                                                             Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                                             ItemImage = i.MainImage,
                                                             Qty = crt.CartItemQty.Value,
                                                             ItemSku = i.Sku,
                                                             GSTPer = i.GST_Per,
                                                             IGSTPer = i.IGST_Per
                                                         }).OrderByDescending(x => x.CartId).ToList();
                            lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price); });
                            // List<tbl_Cart> lstCarts = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                            string paymentmethod = objpymn["method"];
                            string paymentdetails = "";
                            if (paymentmethod == "upi")
                            {
                                paymentdetails = objpymn["vpa"];
                            }
                            else if (paymentmethod == "card")
                            {
                                string cardid = objpymn["cardid"];
                                Razorpay.Api.Card objcard = new Razorpay.Api.Card().Fetch(cardid);
                                if (objcard != null)
                                {
                                    paymentdetails = objcard["network"] + " ****" + objcard["last4"];
                                }
                            }
                            tbl_Orders objOrder = new tbl_Orders();
                            objOrder.ClientUserId = clientusrid;
                            objOrder.OrderAmount = Convert.ToDecimal(objCheckout.Orderamount);
                            objOrder.OrderShipCity = objCheckout.shipcity;
                            objOrder.OrderShipAddress = objCheckout.shipaddress;
                            objOrder.OrderShipState = objCheckout.shipstate;
                            objOrder.OrderShipClientName = objCheckout.shipfirstname + " " + objCheckout.shiplastname;
                            objOrder.OrderShipClientPhone = objCheckout.shipphone;
                            objOrder.OrderStatusId = Convert.ToInt64(OrderStatus.NewOrder);
                            objOrder.PaymentType = paymentmethod;
                            objOrder.IsActive = true;
                            objOrder.IsDelete = false;
                            objOrder.CreatedBy = clientusrid;
                            objOrder.CreatedDate = DateTime.Now;
                            objOrder.UpdatedBy = clientusrid;
                            objOrder.UpdatedDate = DateTime.Now;
                            objOrder.AmountDue = 0;
                            objOrder.RazorpayOrderId = razorpay_order_id;
                            objOrder.RazorpayPaymentId = razorpay_payment_id;
                            objOrder.RazorSignature = razorpay_signature;
                            _db.tbl_Orders.Add(objOrder);
                            _db.SaveChanges();
                            tbl_PaymentHistory objPyment = new tbl_PaymentHistory();
                            objPyment.OrderId = objOrder.OrderId; 
                            objPyment.PaymentBy = paymentmethod;
                            objPyment.AmountDue = Convert.ToDecimal(objCheckout.Orderamount);
                            objPyment.AmountPaid = Convert.ToDecimal(objCheckout.Orderamount);
                            objPyment.DateOfPayment = DateTime.Now; 
                            objPyment.CreatedBy = clientusrid;
                            objPyment.CreatedDate = DateTime.Now;
                            _db.tbl_PaymentHistory.Add(objPyment);
                            _db.SaveChanges();

                            if (lstCartItems != null && lstCartItems.Count() > 0)
                            {
                                foreach (var objCart in lstCartItems)
                                {
                                    tbl_OrderItemDetails objOrderItem = new tbl_OrderItemDetails();
                                    objOrderItem.OrderId = objOrder.OrderId;
                                    objOrderItem.ProductItemId = objCart.ItemId;
                                    objOrderItem.ItemName = objCart.ItemName;
                                    objOrderItem.IGSTAmt = 0;
                                    objOrderItem.Qty = objCart.Qty;
                                    objOrderItem.Price = objCart.Price;
                                    objOrderItem.Sku = objCart.ItemSku;
                                    objOrderItem.IsActive = true;
                                    objOrderItem.IsDelete = false;
                                    objOrderItem.CreatedBy = clientusrid;
                                    objOrderItem.CreatedDate = DateTime.Now;
                                    objOrderItem.UpdatedBy = clientusrid;
                                    objOrderItem.UpdatedDate = DateTime.Now;
                                    decimal InclusiveGST = Math.Round(Convert.ToDecimal(objOrderItem.Price) - Convert.ToDecimal(objOrderItem.Price) * (100 / (100 + objCart.GSTPer)), 2);
                                    decimal PreGSTPrice = Math.Round(Convert.ToDecimal(objOrderItem.Price) - InclusiveGST, 2);
                                    decimal basicTotalPrice = Math.Round(Convert.ToDecimal(PreGSTPrice * objOrderItem.Qty), 2);
                                    decimal SGST = Math.Round(Convert.ToDecimal(objCart.GSTPer / 2), 2);
                                    decimal CGST = Math.Round(Convert.ToDecimal(objCart.GSTPer / 2), 2);
                                    decimal SGSTAmt = Math.Round((basicTotalPrice * SGST) / 100, 2);
                                    decimal CGSTAmt = Math.Round((basicTotalPrice * CGST) / 100, 2);
                                    objOrderItem.GSTAmt = SGSTAmt + CGSTAmt;
                                    _db.tbl_OrderItemDetails.Add(objOrderItem);
                                    var objCartforremove = _db.tbl_Cart.Where(o => o.Cart_Id == objCart.CartId).FirstOrDefault();
                                    _db.tbl_Cart.Remove(objCartforremove);
                                }
                                _db.SaveChanges();
                            }
                            var objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                            if (objotherdetails != null)
                            {                               
                                objotherdetails.ShipAddress = objCheckout.shipaddress;
                                objotherdetails.ShipCity = objCheckout.shipcity;
                                objotherdetails.ShipFirstName = objCheckout.shipfirstname;
                                objotherdetails.ShipLastName = objCheckout.shiplastname;
                                objotherdetails.ShipPhoneNumber = objCheckout.shipphone;
                                objotherdetails.ShipPostalcode = objCheckout.shippincode;
                                objotherdetails.ShipState = objCheckout.shipstate;
                                objotherdetails.ShipEmail = objCheckout.shipemailaddress;
                                _db.SaveChanges();
                            }
                            string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                            ReturnMessage = "Success^" + orderid;
                        }
                        else
                        {
                            ReturnMessage = "Payment " + objpymn["status"];
                        }
                    }
                    else
                    {
                        ReturnMessage = "Payment Failed";
                    }

                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return Json(ReturnMessage);
        }

        public decimal GetOfferPrice(long Itemid, decimal price)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return objItem.OfferPrice;
            }

            return price;
        }

        public decimal GetDistributorOfferPrice(long Itemid, decimal price)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return objItem.OfferPriceforDistributor.Value;
            }

            return price;
        }

        public decimal GetPriceGenral(long Itemid, decimal price)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                if (clsClientSession.RoleID == 1)
                {
                    return objItem.OfferPrice;
                }
                else
                {
                    return objItem.OfferPriceforDistributor.Value;
                }

            }

            return price;
        }
    }
}