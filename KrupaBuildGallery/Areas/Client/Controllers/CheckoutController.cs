using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Model;
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
            Session["ClientUserId"] = 1;
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
                if (Session["ClientUserId"] != null)
                {
                    long ClientUserId = Convert.ToInt64(Session["ClientUserId"]);
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    where crt.ClientUserId == ClientUserId
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = i.CustomerPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
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
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            ViewData["lstCartItems"] = lstCartItems;
            return View();
        }

        public ActionResult OrderSuccess(int Id)
        {
            return View();
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
        public string PlaceOrder(CheckoutVM objCheckout,string razorpay_payment_id,string razorpay_order_id,string razorpay_signature)
        {
            string ReturnMessage = "";
            long clientusrid = Convert.ToInt64(Session["ClientUserId"]);
            try
            {
                Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(razorpay_payment_id);
                if(objpymn != null)
                {                    
                    if(objpymn["status"] != null && Convert.ToString(objpymn["status"]) == "Captured")
                    {
                      List<CartVM> lstCartItems = (from crt in _db.tbl_Cart
                                        join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                        where crt.ClientUserId == clientusrid
                                        select new CartVM
                                        {
                                            CartId = crt.Cart_Id,
                                            ItemName = i.ItemName,
                                            ItemId = i.ProductItemId,
                                            Price = i.CustomerPrice,
                                            ItemImage = i.MainImage,
                                            Qty = crt.CartItemQty.Value,
                                            ItemSku = i.Sku,
                                            GSTPer = i.GST_Per,
                                            IGSTPer = i.IGST_Per
                                        }).OrderByDescending(x => x.CartId).ToList();
                       // List<tbl_Cart> lstCarts = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                        string paymentmethod = objpymn["method"];
                        string paymentdetails = "";
                        if(paymentmethod == "upi")
                        {
                            paymentdetails = objpymn["vpa"];
                        }
                        else if(paymentmethod == "card")
                        {
                            string cardid = objpymn["cardid"];
                            Razorpay.Api.Card objcard = new Razorpay.Api.Card().Fetch(cardid);
                            if(objcard != null)
                            {
                                paymentdetails = objcard["network"] + " ****"+ objcard["last4"];
                            }
                        }
                        tbl_Orders objOrder = new tbl_Orders();
                        objOrder.ClientUserId = clientusrid;
                        objOrder.OrderAmount = objCheckout.Orderamout;
                        objOrder.OrderShipCity = objCheckout.shipcity;
                        objOrder.OrderShipAddress = objCheckout.shipaddress;
                        objOrder.OrderShipState = objCheckout.shipstate;
                        objOrder.OrderShipClientName = objCheckout.shipfirstname + " " + objCheckout.shiplastname;
                        objOrder.OrderShipClientPhone = objCheckout.shipphone;
                        objOrder.OrderStatusId = Convert.ToInt64(OrderStatus.Processing);
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
                        if(lstCartItems != null && lstCartItems.Count() > 0)
                        {
                            foreach(var objCart in lstCartItems)
                            {
                                tbl_OrderItemDetails objOrderItem = new tbl_OrderItemDetails();
                                objOrderItem.OrderId = objOrder.OrderId;
                                objOrderItem.ProductItemId = objCart.ItemId;
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


                        ReturnMessage = "Success";
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
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }
    }
}