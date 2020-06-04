using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Razorpay.Api;
namespace KrupaBuildGallery.Areas.Client.Controllers
{
    [CustomClientAuthorize]
    public class CheckoutController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public CheckoutController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Checkout
        public ActionResult Index(string type = "Online")
        {
            List<CartVM> lstCartItems = new List<CartVM>();          
            try
            {

                decimal ShippingChargeTotal = 0;
                decimal TotalDiscount = 0;
                decimal TotalOrder = 0;
                ViewBag.WebsiteOrderId = "1";
                bool IsCashOrd = false;
                if(type == "Cash")
                {
                    IsCashOrd = true;
                }
               
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
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.ClientUserId == ClientUserId && crt.IsCashonDelivery == IsCashOrd
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        //Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                        VariantId = crt.VariantItemId.Value,
                                        VariantQtytxt = vr.UnitQty,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                        GSTPer = i.GST_Per,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,x.VariantId); });
                }
                else
                {
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.CartSessionId == cookiesessionval && crt.IsCashonDelivery == IsCashOrd
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = vr.CustomerPrice.Value,
                                       // Price = i.CustomerPrice,
                                        ItemImage = i.MainImage,
                                        VariantId = crt.VariantItemId.Value,
                                        VariantQtytxt = vr.UnitQty,
                                        Qty = crt.CartItemQty.Value,
                                        ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false,
                                        GSTPer = i.GST_Per
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price,x.VariantId); });
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
                List<InvoiceItemVM> lstInvItem = new List<InvoiceItemVM>();
                if (clsClientSession.RoleID  == 1)
                {
                    var lstCartItemsnew = lstCartItems.OrderByDescending(o => o.GSTPer).ToList();
                    DateTime dtNow = DateTime.UtcNow;
                    long clientusrrId = clsClientSession.UserID;
                    List<tbl_PointDetails> lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == clientusrrId && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();
                    decimal pointreamining = 0;
                    if(lstpoints != null && lstpoints.Count() > 0)
                    {
                        pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
                    }
                    decimal totalremining = pointreamining;
                    if (lstCartItemsnew.Count() > 0)
                    {
                        foreach(var objcr in lstCartItemsnew)
                        {
                            if(objcr != null)
                            {
                                //decimal InclusiveGST = Math.Round(objcr.Price - objcr.Price * (100 / (100 + objcr.GSTPer)), 2);
                                //decimal PreGSTPrice = Math.Round(objcr.Price - InclusiveGST, 2);
                                decimal originalbasicprice = Math.Round(((objcr.Price / (100 + objcr.GSTPer)) * 100),2);
                                decimal totalItembasicprice = originalbasicprice * objcr.Qty;
                                decimal disc = Math.Round((totalItembasicprice * 5)/100,2);
                                if(disc <= totalremining)
                                {
                                    totalremining = totalremining - disc;
                                }
                                else
                                {
                                    disc = totalremining;
                                    totalremining = totalremining - disc;
                                }
                                TotalDiscount = TotalDiscount + disc;
                                ShippingChargeTotal = ShippingChargeTotal + (objcr.ShippingCharge * objcr.Qty);
                                decimal beforetaxamount = Math.Round(totalItembasicprice - disc,2);
                                decimal gstamt = Math.Round((beforetaxamount * objcr.GSTPer)/100, 2);
                                decimal AfterTax = beforetaxamount + gstamt;
                                InvoiceItemVM objInvItm = new InvoiceItemVM();
                                objInvItm.ItemName = objcr.ItemName;
                                objInvItm.GSTPer = objcr.GSTPer;
                                objInvItm.GSTAmount = gstamt;
                                objInvItm.Qty = Convert.ToInt32(objcr.Qty);
                                objInvItm.BasicAmount = originalbasicprice;
                                objInvItm.Discount = disc;
                                objInvItm.VariantQtytxt = objcr.VariantQtytxt;
                                objInvItm.ItemAmount = AfterTax;
                                objInvItm.beforetaxamount = beforetaxamount;
                                TotalOrder = TotalOrder + AfterTax;
                                lstInvItem.Add(objInvItm);
                            }
                        }
                    }
                }
                else if(clsClientSession.RoleID == 2)
                {
                    var lstCartItemsnew = lstCartItems.OrderByDescending(o => o.GSTPer).ToList();
                    decimal pointreamining = 0;
                    decimal totalremining = pointreamining;
                  
                    if (lstCartItemsnew.Count() > 0)
                    {
                        foreach (var objcr in lstCartItemsnew)
                        {
                            if (objcr != null)
                            {
                                decimal originalbasicprice = Math.Round(((objcr.Price / (100 + objcr.GSTPer)) * 100), 2);
                                decimal totalItembasicprice = originalbasicprice * objcr.Qty;
                                decimal disc = 0;// Math.Round((totalItembasicprice * 5) / 100, 2);
                                ShippingChargeTotal = ShippingChargeTotal + (objcr.ShippingCharge * objcr.Qty);
                                decimal beforetaxamount = Math.Round(totalItembasicprice - disc,2);
                                decimal gstamt = Math.Round((beforetaxamount * objcr.GSTPer) / 100, 2);
                                decimal AfterTax = beforetaxamount + gstamt;
                                InvoiceItemVM objInvItm = new InvoiceItemVM();
                                objInvItm.ItemName = objcr.ItemName;
                                objInvItm.GSTPer = objcr.GSTPer;
                                objInvItm.GSTAmount = gstamt;
                                objInvItm.Qty = Convert.ToInt32(objcr.Qty);
                                objInvItm.BasicAmount = originalbasicprice;
                                objInvItm.VariantQtytxt = objcr.VariantQtytxt;
                                objInvItm.Discount = disc;
                                objInvItm.ItemAmount = AfterTax;
                                objInvItm.beforetaxamount = beforetaxamount;
                                TotalOrder = TotalOrder + AfterTax;
                                lstInvItem.Add(objInvItm);
                            }
                        }
                    }
                }

                ViewBag.CreditRemaining = creditlimitreminng;
                tbl_ClientOtherDetails objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                ViewData["objotherdetails"] = objotherdetails;
                ViewData["lstInvItem"] = lstInvItem;
                ViewBag.ShippingChargeTotal = ShippingChargeTotal;
                ViewBag.TotalDiscount = TotalDiscount;
                ViewBag.TotalOrder = TotalOrder;
                var objtbl_ExtraAmount = _db.tbl_ExtraAmount.Where(o => o.AmountFrom <= TotalOrder && o.AmountTo >= TotalOrder).FirstOrDefault();
                ViewBag.ExtraAmt = 0;
                if (objtbl_ExtraAmount != null)
                {
                    ViewBag.ExtraAmt = objtbl_ExtraAmount.ExtraAmount;
                }
                decimal WalletAmt = 0;
                var objuserclient =_db.tbl_ClientUsers.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                if(objuserclient != null)
                {
                    WalletAmt = objuserclient.WalletAmt.HasValue ? objuserclient.WalletAmt.Value : 0;
                }
                //_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                ViewBag.WalletAmt = WalletAmt;
                ViewBag.IsCashOnDelivery = IsCashOrd;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            ViewData["lstCartItems"] = lstCartItems;
            var objGenralsetting = _db.tbl_GeneralSetting.FirstOrDefault();
            ViewBag.ShippingMsg = objGenralsetting.ShippingMessage;
            ViewBag.CashOrderAmtMax = objGenralsetting.CashLimitPerOrder;
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
                            OrderDate = p.CreatedDate,
                            ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0,
                            ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();
            if (objOrder != null)
            {
             //   objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                   join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                   join vr in _db.tbl_ItemVariant on p.VariantItemId equals vr.VariantItemId
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
                                                       VariantQtytxt = vr.UnitQty,
                                                       GSTAmt = p.GSTAmt.Value,
                                                       IGSTAmt = p.IGSTAmt.Value,
                                                       ItemImg = c.MainImage,
                                                       Discount = p.Discount.HasValue ? p.Discount.Value : 0
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

            var objGsetting = _db.tbl_GeneralSetting.FirstOrDefault();
            string key = objGsetting.RazorPayKey;  //"rzp_test_DMsPlGIBp3SSnI";
            string secret = objGsetting.RazorPaySecret; // "YMkpd9LbnaXViePncLLXhqms";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.OrderId = order["id"];
            ViewBag.Description = description;
            ViewBag.Amount = Amount * 100;
            ViewBag.Key = key;
            return PartialView("~/Areas/Client/Views/Checkout/_RazorPayPayment.cshtml");
        }

        [HttpPost]
        public JsonResult PlaceOrder(CheckoutVM objCheckout,string razorpay_payment_id,string razorpay_order_id,string razorpay_signature)
        {
            string ReturnMessage = "";
            long clientusrid = clsClientSession.UserID;
            bool Iscashondelivery = false;
            clsCommon objcmn = new clsCommon();
            try
            {
                decimal amtwallet = Convert.ToDecimal(objCheckout.walletamtinorder);
                decimal amtcredit = Convert.ToDecimal(objCheckout.creditamtinorder);
                decimal amtonline = Convert.ToDecimal(objCheckout.onlineamtinorder);
                if (razorpay_payment_id == "ByCredit")
                {
                    string paymentmethod = "ByCredit";
                    decimal amtorderdue = 0;
                    if (objCheckout.isCashondelivery.ToLower() == "true")
                    {
                        paymentmethod = "Cash on Delivery";
                        Iscashondelivery = true;
                    }
                    List<CartVM> lstCartItems = new List<CartVM>();
                    if (objCheckout.ordertype == "1")
                    {
                        lstCartItems = (from crt in _db.tbl_Cart
                                                     join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                        join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                        where crt.ClientUserId == clientusrid && crt.IsCashonDelivery == Iscashondelivery
                                                     select new CartVM
                                                     {
                                                         CartId = crt.Cart_Id,
                                                         ItemName = i.ItemName,
                                                         ItemId = i.ProductItemId,
                                                         //Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                                         Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                                         ItemImage = i.MainImage,
                                                         VariantId = crt.VariantItemId.Value,
                                                         VariantQtytxt = vr.UnitQty,
                                                         Qty = crt.CartItemQty.Value,
                                                         ItemSku = i.Sku,
                                                         GSTPer = i.GST_Per,
                                                         IGSTPer = i.IGST_Per
                                                     }).OrderByDescending(x => x.CartId).ToList();
                    }
                    else
                    {
                        lstCartItems = (from crt in _db.tbl_SecondCart
                                        join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                        join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                        where crt.ClientUserId == clientusrid 
                                        select new CartVM
                                        {
                                            CartId = crt.SecondCartId,
                                            ItemName = i.ItemName,
                                            ItemId = i.ProductItemId,
                                            Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                            ItemImage = i.MainImage,
                                            Qty = crt.CartItemQty.Value,
                                            VariantId = crt.VariantItemId.Value,
                                            VariantQtytxt = vr.UnitQty,
                                            ItemSku = i.Sku,
                                            GSTPer = i.GST_Per,
                                            IGSTPer = i.IGST_Per
                                        }).OrderByDescending(x => x.CartId).ToList();
                    }
                 
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,x.VariantId); });
                    // List<tbl_Cart> lstCarts = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                   
                    int year = DateTime.Now.Year;
                    int toyear = year + 1;
                    if (DateTime.UtcNow.Month <= 3)
                    {
                        year = year - 1;
                        toyear = year;
                    }
                    DateTime dtfincialyear = new DateTime(year,4,1);
                    DateTime dtendyear = new DateTime(toyear,3,31);
                    var objOrdertemp = _db.tbl_Orders.Where(o => o.CreatedDate >= dtfincialyear && o.CreatedDate <= dtendyear).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                    long Invno = 1;
                    if(objOrdertemp != null)
                    {
                        if (objOrdertemp.InvoiceNo == null)
                        {
                            objOrdertemp.InvoiceNo = 1;
                        }
                        Invno = objOrdertemp.InvoiceNo.Value + 1;
                    }
                    

                    tbl_Orders objOrder = new tbl_Orders();
                    objOrder.ClientUserId = clientusrid;
                    decimal ordramt = Convert.ToDecimal(objCheckout.Orderamount);
                    decimal shippingcharge = 0;
                    decimal extraamt = 0;
                    if (objCheckout.shippincode == "389001")
                    {
                        shippingcharge = Convert.ToDecimal(objCheckout.shipamount);
                        extraamt = Convert.ToDecimal(objCheckout.extraamount);
                        ordramt = ordramt - shippingcharge - extraamt;
                    }
                    if (objCheckout.isCashondelivery.ToLower() == "true")
                    {
                        amtorderdue = ordramt + shippingcharge + extraamt;                        
                    }
                    else
                    {
                        if(amtwallet > 0 && amtcredit > 0)
                        {
                            paymentmethod = "Wallet and Credit";
                        }
                        else if(amtwallet > 0)
                        {
                            paymentmethod = "Wallet";
                        }
                        else if(amtcredit > 0)
                        {
                            paymentmethod = "Credit";
                        }
                        if (objCheckout.ordertype == "1")
                        {
                            amtorderdue = amtcredit;
                        }                       
                        else
                        {
                            decimal advncpay = 0;
                            if(!string.IsNullOrEmpty(objCheckout.advanceamtpay))
                            {
                                advncpay = Convert.ToDecimal(objCheckout.advanceamtpay);
                                decimal totlordewithship = ordramt + shippingcharge + extraamt;
                                decimal remaingammt = totlordewithship - advncpay;
                                amtorderdue = remaingammt + amtcredit;
                            }
                        }
                    }
                    objOrder.OrderAmount = ordramt;
                    objOrder.OrderShipCity = objCheckout.shipcity;
                    objOrder.OrderShipAddress = objCheckout.shipaddress;
                    objOrder.OrderShipState = objCheckout.shipstate;
                    objOrder.OrderShipClientName = objCheckout.shipfirstname + " " + objCheckout.shiplastname;
                    objOrder.OrderShipClientPhone = objCheckout.shipphone;
                    objOrder.OrderShipPincode = objCheckout.shippincode;
                    objOrder.OrderStatusId = Convert.ToInt64(OrderStatus.NewOrder);
                    objOrder.PaymentType = paymentmethod;
                    objOrder.IsActive = true;
                    objOrder.IsDelete = false;
                    objOrder.CreatedBy = clientusrid;
                    objOrder.CreatedDate = DateTime.UtcNow;
                    objOrder.UpdatedBy = clientusrid;
                    objOrder.UpdatedDate = DateTime.UtcNow;
                    objOrder.AmountDue = amtorderdue;
                    objOrder.RazorpayOrderId = razorpay_order_id;
                    objOrder.RazorpayPaymentId = "";
                    objOrder.InvoiceNo = Invno;
                    objOrder.InvoiceYear = year + "-" + toyear;
                    objOrder.ExtraAmount = extraamt;
                    objOrder.OrderType = Convert.ToInt32(objCheckout.ordertype);
                   
                    objOrder.RazorSignature = "";
                    if(objCheckout.shippincode == "389001")
                    {
                        objOrder.ShippingCharge = Convert.ToDecimal(objCheckout.shipamount);
                        objOrder.ShippingStatus = 2;
                    }                   
                    else
                    {
                        objOrder.ShippingCharge = 0;
                        objOrder.ShippingStatus = 1;
                    }
                    objOrder.WalletAmountUsed = amtwallet;
                    objOrder.CreditAmountUsed = amtcredit;
                    objOrder.AmountByRazorPay = amtonline;
                    if (objCheckout.isCashondelivery.ToLower() == "true")
                    {
                        objOrder.IsCashOnDelivery = true;
                    }
                    else
                    {
                        objOrder.IsCashOnDelivery = false;
                    }
                        _db.tbl_Orders.Add(objOrder);
                    _db.SaveChanges();
                    if(objOrder.IsCashOnDelivery == true)
                    {
                        objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Cash On Delivery Order : Rs" + amtorderdue, amtorderdue, clsClientSession.UserID, 0, DateTime.UtcNow, "Cash On Delivery Order");
                    }
                    objOrder.RazorpayOrderId = objOrder.OrderId.ToString();

                    if(amtwallet > 0)
                    {
                        tbl_Wallet objwlt = new tbl_Wallet();
                        objwlt.Amount = amtwallet;
                        objwlt.CreditDebit = "Debit";
                        objwlt.OrderId = objOrder.OrderId;
                        objwlt.ClientUserId = clientusrid;
                        objwlt.WalletDate = DateTime.UtcNow;
                        objwlt.Description = "Paid Amount for order no." + objOrder.OrderId;
                        _db.tbl_Wallet.Add(objwlt);                    
                       var objclientuss = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                        if(objclientuss != null)
                        {
                            objclientuss.WalletAmt = objclientuss.WalletAmt - amtwallet;
                        }
                        _db.SaveChanges();
                    }
                    objcmn.SaveTransaction(0,0,objOrder.OrderId, "Payment By Wallet : Rs"+ amtwallet,amtwallet, clsClientSession.UserID, 0, DateTime.UtcNow, "Wallet Payment");
                    if(amtcredit > 0)
                    {
                        objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Payment By Credit Used : Rs" + amtcredit, amtcredit, clsClientSession.UserID, 0, DateTime.UtcNow, "Credit Used Payment");
                    }
                    
                    var objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    if(objotherdetails != null)
                    {
                        decimal amtdue = 0;
                        if(objotherdetails.AmountDue != null)
                        {
                            amtdue = objotherdetails.AmountDue.Value;
                            
                        }
                        objotherdetails.AmountDue = amtdue + amtcredit;
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
                    decimal TotalDiscount = 0;
                    if (lstCartItems != null && lstCartItems.Count() > 0)
                    {
                        foreach (var objCart in lstCartItems)
                        {
                            tbl_OrderItemDetails objOrderItem = new tbl_OrderItemDetails();
                            objOrderItem.OrderId = objOrder.OrderId;
                            objOrderItem.ProductItemId = objCart.ItemId;
                            objOrderItem.VariantItemId = objCart.VariantId;
                            objOrderItem.ItemName = objCart.ItemName;
                            objOrderItem.IGSTAmt = 0;
                            objOrderItem.Qty = objCart.Qty;
                            objOrderItem.Price = objCart.Price;
                            objOrderItem.Sku = objCart.ItemSku;
                            objOrderItem.IsActive = true;
                            objOrderItem.IsDelete = false;
                            objOrderItem.CreatedBy = clientusrid;
                            objOrderItem.CreatedDate = DateTime.UtcNow;
                            objOrderItem.UpdatedBy = clientusrid;
                            objOrderItem.UpdatedDate = DateTime.UtcNow;
                            decimal qtty = GetVarintQtyy(objCart.VariantQtytxt);
                            objOrderItem.QtyUsed = qtty * objCart.Qty;
                            //decimal InclusiveGST = Math.Round(Convert.ToDecimal(objOrderItem.Price) - Convert.ToDecimal(objOrderItem.Price) * (100 / (100 + objCart.GSTPer)), 2);
                            //decimal PreGSTPrice = Math.Round(Convert.ToDecimal(objOrderItem.Price) - InclusiveGST, 2);
                            //decimal basicTotalPrice = Math.Round(Convert.ToDecimal(PreGSTPrice * objOrderItem.Qty), 2);
                            //decimal SGST = Math.Round(Convert.ToDecimal(objCart.GSTPer / 2), 2);
                            //decimal CGST = Math.Round(Convert.ToDecimal(objCart.GSTPer / 2), 2);
                            //decimal SGSTAmt = Math.Round((basicTotalPrice * SGST) / 100, 2);
                            //decimal CGSTAmt = Math.Round((basicTotalPrice * CGST) / 100, 2);
                            //objOrderItem.GSTAmt = SGSTAmt + CGSTAmt;
                            decimal originalbasicprice = Math.Round(((objCart.Price / (100 + objCart.GSTPer)) * 100), 2);
                            decimal totalItembasicprice = originalbasicprice * objCart.Qty;
                            decimal disc = 0;
                            decimal beforetaxamount = Math.Round(totalItembasicprice - disc,2);
                            decimal gstamt = Math.Round((beforetaxamount * objCart.GSTPer) / 100, 2);
                            decimal AfterTax = beforetaxamount + gstamt;
                            objOrderItem.GSTPer = objCart.GSTPer;
                            objOrderItem.GSTAmt = gstamt;
                            objOrderItem.Price = originalbasicprice;
                            objOrderItem.Discount = disc;
                            objOrderItem.ItemStatus = 1;
                            objOrderItem.FinalItemPrice = AfterTax;
                            _db.tbl_OrderItemDetails.Add(objOrderItem);
                            _db.SaveChanges();
                            objcmn.SaveTransaction(objCart.ItemId, objOrderItem.OrderDetailId, objOrderItem.OrderId.Value, "Order Placed for Item", objOrderItem.FinalItemPrice.Value, clsClientSession.UserID, 0, DateTime.UtcNow, "New Order Item");
                            if (objCheckout.ordertype == "1")
                            {
                                var objCartforremove = _db.tbl_Cart.Where(o => o.Cart_Id == objCart.CartId).FirstOrDefault();
                                _db.tbl_Cart.Remove(objCartforremove);
                            }
                            else
                            {
                                var objCartforremove = _db.tbl_SecondCart.Where(o => o.SecondCartId == objCart.CartId).FirstOrDefault();
                                _db.tbl_SecondCart.Remove(objCartforremove);
                            }
                        }
                        _db.SaveChanges();
                    }
                    decimal amttTotlordepy = ordramt + shippingcharge + extraamt;
                    if (amtwallet > 0)
                    {
                        tbl_PaymentHistory objPyment1 = new tbl_PaymentHistory();
                        objPyment1.OrderId = objOrder.OrderId;
                        objPyment1.PaymentBy = "wallet";
                        objPyment1.AmountDue = Convert.ToDecimal(amttTotlordepy);
                        objPyment1.AmountPaid = Convert.ToDecimal(amtwallet);
                        objPyment1.DateOfPayment = DateTime.UtcNow;
                        objPyment1.CreatedBy = clientusrid;
                        objPyment1.CreatedDate = DateTime.UtcNow;
                        objPyment1.RazorpayOrderId = "";
                        objPyment1.RazorpayPaymentId = "";
                        objPyment1.RazorSignature = "";
                        objPyment1.PaymentFor = "OrderPayment";
                        _db.tbl_PaymentHistory.Add(objPyment1);
                        amttTotlordepy = amttTotlordepy - amtwallet;
                    }
                    if (amtonline > 0)
                    {
                        tbl_PaymentHistory objPyment = new tbl_PaymentHistory();
                        objPyment.OrderId = objOrder.OrderId;
                        objPyment.PaymentBy = "online";
                        objPyment.AmountDue = Convert.ToDecimal(amttTotlordepy);
                        objPyment.AmountPaid = Convert.ToDecimal(amtonline);
                        objPyment.DateOfPayment = DateTime.UtcNow;
                        objPyment.CreatedBy = clientusrid;
                        objPyment.CreatedDate = DateTime.UtcNow;
                        objPyment.RazorpayOrderId = razorpay_order_id;
                        objPyment.RazorpayPaymentId = razorpay_payment_id;
                        objPyment.RazorSignature = razorpay_signature;
                        objPyment.PaymentFor = "OrderPayment";
                        _db.tbl_PaymentHistory.Add(objPyment);
                        objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Order Online Payment : Rs" + amtonline, amtonline, clsClientSession.UserID, 0, DateTime.UtcNow, "Online Payment");
                    }
                    _db.SaveChanges();
                    string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                   
                    ReturnMessage = "Success^" + orderid;
                }
                else
                {
                    decimal amountdue = 0;
                    Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(razorpay_payment_id);
                    if (objpymn != null)
                    {
                        if (objpymn["status"] != null && Convert.ToString(objpymn["status"]) == "captured")
                        {
                            List<CartVM> lstCartItems = new List<CartVM>();
                            if(objCheckout.ordertype == "1")
                            {
                                
                                lstCartItems = (from crt in _db.tbl_Cart
                                                             join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                                join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                                where crt.ClientUserId == clientusrid && crt.IsCashonDelivery == false
                                                             select new CartVM
                                                             {
                                                                 CartId = crt.Cart_Id,
                                                                 ItemName = i.ItemName,
                                                                 ItemId = i.ProductItemId,
                                                                 Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                                                 ItemImage = i.MainImage,
                                                                 VariantId = crt.VariantItemId.Value,
                                                                 VariantQtytxt = vr.UnitQty,
                                                                 Qty = crt.CartItemQty.Value,
                                                                 ItemSku = i.Sku,
                                                                 GSTPer = i.GST_Per,
                                                                 IGSTPer = i.IGST_Per
                                                             }).OrderByDescending(x => x.CartId).ToList();
                            }
                            else
                            {
                                lstCartItems = (from crt in _db.tbl_SecondCart
                                                join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                                join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                                where crt.ClientUserId == clientusrid 
                                                select new CartVM
                                                {
                                                    CartId = crt.SecondCartId,
                                                    ItemName = i.ItemName,
                                                    ItemId = i.ProductItemId,
                                                    Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                                    ItemImage = i.MainImage,
                                                    VariantId = crt.VariantItemId.Value,
                                                    Qty = crt.CartItemQty.Value,
                                                    VariantQtytxt = vr.UnitQty,
                                                    ItemSku = i.Sku,
                                                    GSTPer = i.GST_Per,
                                                    IGSTPer = i.IGST_Per
                                                }).OrderByDescending(x => x.CartId).ToList();
                            }
                          
                            lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,x.VariantId); });
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
                            int year = DateTime.Now.Year;
                            int toyear = year + 1;
                            if (DateTime.UtcNow.Month <= 3)
                            {
                                year = year - 1;
                                toyear = year;
                            }
                            DateTime dtfincialyear = new DateTime(year, 4, 1);
                            DateTime dtendyear = new DateTime(toyear, 3, 31);
                            var objOrdertemp = _db.tbl_Orders.Where(o => o.CreatedDate >= dtfincialyear && o.CreatedDate <= dtendyear).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                            long Invno = 1;
                            if (objOrdertemp != null)
                            {
                                if(objOrdertemp.InvoiceNo == null)
                                {
                                    objOrdertemp.InvoiceNo = 1;
                                }
                                Invno = objOrdertemp.InvoiceNo.Value + 1;
                            }
                            decimal ordramt = Convert.ToDecimal(objCheckout.Orderamount);
                            decimal shippingcharge = 0;
                            decimal extraamount = 0;
                            if (objCheckout.shippincode == "389001")
                            {
                                shippingcharge = Convert.ToDecimal(objCheckout.shipamount);
                                extraamount = Convert.ToDecimal(objCheckout.extraamount);
                                ordramt = ordramt - shippingcharge - extraamount;
                            }

                            List<string> lstpymenymthod = new List<string>();
                            if (amtwallet > 0)
                            {
                                lstpymenymthod.Add("Wallet");
                            }
                            if(amtcredit > 0)
                            {
                                lstpymenymthod.Add("Credit");
                            }
                            
                            if(amtonline > 0)
                            {
                                lstpymenymthod.Add(paymentmethod);
                            }

                            if (objCheckout.ordertype == "1")
                            {
                                amountdue = amtcredit;
                            }
                            else
                            {
                                decimal advncpay = 0;
                                if (!string.IsNullOrEmpty(objCheckout.advanceamtpay))
                                {
                                    advncpay = Convert.ToDecimal(objCheckout.advanceamtpay);
                                    decimal totlordewithship = ordramt + shippingcharge + extraamount;
                                    decimal remaingammt = totlordewithship - advncpay;
                                    amountdue = remaingammt + amtcredit;
                                }
                            }
                            paymentmethod = string.Join(",", lstpymenymthod);
                            tbl_Orders objOrder = new tbl_Orders();
                            objOrder.ClientUserId = clientusrid;
                            objOrder.OrderAmount = ordramt;
                            objOrder.OrderShipCity = objCheckout.shipcity;
                            objOrder.OrderShipAddress = objCheckout.shipaddress;
                            objOrder.OrderShipState = objCheckout.shipstate;
                            objOrder.OrderShipClientName = objCheckout.shipfirstname + " " + objCheckout.shiplastname;
                            objOrder.OrderShipClientPhone = objCheckout.shipphone;
                            objOrder.OrderShipPincode = objCheckout.shippincode;
                            objOrder.OrderStatusId = Convert.ToInt64(OrderStatus.NewOrder);
                            objOrder.PaymentType = paymentmethod;
                            objOrder.IsActive = true;
                            objOrder.IsDelete = false;
                            objOrder.CreatedBy = clientusrid;
                            objOrder.CreatedDate = DateTime.UtcNow;
                            objOrder.UpdatedBy = clientusrid;
                            objOrder.UpdatedDate = DateTime.UtcNow;
                            objOrder.AmountDue = amountdue;
                            objOrder.InvoiceNo = Invno;
                            objOrder.InvoiceYear = year + "-" + toyear;
                            objOrder.RazorpayOrderId = razorpay_order_id;
                            objOrder.RazorpayPaymentId = razorpay_payment_id;
                            objOrder.RazorSignature = razorpay_signature;
                            objOrder.OrderType = Convert.ToInt32(objCheckout.ordertype);
                            objOrder.IsCashOnDelivery = false;
                            objOrder.WalletAmountUsed = amtwallet;
                            objOrder.CreditAmountUsed = amtcredit;
                            objOrder.AmountByRazorPay = amtonline;
                            objOrder.ExtraAmount = extraamount;
                            if (objCheckout.shippincode == "389001")
                            {
                                objOrder.ShippingCharge = shippingcharge;
                                objOrder.ShippingStatus = 2;
                            }
                            else
                            {
                                objOrder.ShippingCharge = 0;
                                objOrder.ShippingStatus = 1;
                            }
                            _db.tbl_Orders.Add(objOrder);
                            _db.SaveChanges();

                            if (amtwallet > 0)
                            {
                                tbl_Wallet objwlt = new tbl_Wallet();
                                objwlt.Amount = amtwallet;
                                objwlt.CreditDebit = "Debit";
                                objwlt.OrderId = objOrder.OrderId;
                                objwlt.ClientUserId = clientusrid;
                                objwlt.WalletDate = DateTime.UtcNow;
                                objwlt.Description = "Paid Amount for order no." + objOrder.OrderId;
                                _db.tbl_Wallet.Add(objwlt);
                                var objclientuss = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                                if (objclientuss != null)
                                {
                                    objclientuss.WalletAmt = objclientuss.WalletAmt - amtwallet;
                                }
                                _db.SaveChanges();
                                objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Payment By Wallet : Rs" + amtwallet, amtwallet, clsClientSession.UserID, 0, DateTime.UtcNow, "Wallet Payment");
                            }

                            decimal amttTotlordepy = ordramt + shippingcharge + extraamount;
                            if(amtwallet > 0)
                            {
                                tbl_PaymentHistory objPyment1 = new tbl_PaymentHistory();
                                objPyment1.OrderId = objOrder.OrderId;
                                objPyment1.PaymentBy = "wallet";
                                objPyment1.AmountDue = Convert.ToDecimal(amttTotlordepy);
                                objPyment1.AmountPaid = Convert.ToDecimal(amtwallet);
                                objPyment1.DateOfPayment = DateTime.UtcNow;
                                objPyment1.CreatedBy = clientusrid;
                                objPyment1.CreatedDate = DateTime.UtcNow;
                                objPyment1.RazorpayOrderId = "";
                                objPyment1.RazorpayPaymentId = "";
                                objPyment1.RazorSignature = "";
                                objPyment1.PaymentFor = "OrderPayment";
                                _db.tbl_PaymentHistory.Add(objPyment1);
                                amttTotlordepy = amttTotlordepy - amtwallet;
                            }
                            if(amtonline > 0)
                            {
                                tbl_PaymentHistory objPyment = new tbl_PaymentHistory();
                                objPyment.OrderId = objOrder.OrderId;
                                objPyment.PaymentBy = "online";
                                objPyment.AmountDue = Convert.ToDecimal(amttTotlordepy);
                                objPyment.AmountPaid = Convert.ToDecimal(amtonline);
                                objPyment.DateOfPayment = DateTime.UtcNow;
                                objPyment.CreatedBy = clientusrid;
                                objPyment.CreatedDate = DateTime.UtcNow;
                                objPyment.RazorpayOrderId = razorpay_order_id;
                                objPyment.RazorpayPaymentId = razorpay_payment_id;
                                objPyment.RazorSignature = razorpay_signature;
                                objPyment.PaymentFor = "OrderPayment";
                                _db.tbl_PaymentHistory.Add(objPyment);
                                objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Order Online Payment : Rs" + amtonline, amtonline, clsClientSession.UserID, 0, DateTime.UtcNow, "Online Payment");
                            }                           
                            _db.SaveChanges();
                            decimal pointreamining = 0;
                            decimal totalremining = 0;
                            decimal TotalDiscount = 0;
                            List<tbl_PointDetails> lstpoints = new List<tbl_PointDetails>();
                            if (clsClientSession.RoleID == 1)
                            {
                                DateTime dtNow = DateTime.UtcNow;
                                long clientusrrId = clsClientSession.UserID;
                                lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == clientusrrId && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();
                             
                                if (lstpoints != null && lstpoints.Count() > 0)
                                {
                                    pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
                                }
                                totalremining = pointreamining;
                            }
                            var lstCartItemsnew = lstCartItems.OrderByDescending(o => o.GSTPer).ToList();
                            if (lstCartItemsnew != null && lstCartItemsnew.Count() > 0)
                            {
                                foreach (var objCart in lstCartItemsnew)
                                {
                                    tbl_OrderItemDetails objOrderItem = new tbl_OrderItemDetails();
                                    objOrderItem.OrderId = objOrder.OrderId;
                                    objOrderItem.ProductItemId = objCart.ItemId;
                                    objOrderItem.VariantItemId = objCart.VariantId;
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
                                     decimal qtty = GetVarintQtyy(objCart.VariantQtytxt);
                                    objOrderItem.QtyUsed = qtty * objCart.Qty;
                                    decimal originalbasicprice = Math.Round(((objCart.Price / (100 + objCart.GSTPer)) * 100), 2);
                                    decimal totalItembasicprice = originalbasicprice * objCart.Qty;
                                    decimal disc = 0;
                                    if (clsClientSession.RoleID == 1)
                                    {
                                        disc = Math.Round((totalItembasicprice * 5) / 100, 2);
                                        if (disc <= totalremining)
                                        {
                                            totalremining = totalremining - disc;
                                        }
                                        else
                                        {
                                            disc = totalremining;
                                        }
                                    }
                                   
                                    TotalDiscount = TotalDiscount + disc;
                                    decimal beforetaxamount = Math.Round(totalItembasicprice - disc,2);
                                    decimal gstamt = Math.Round((beforetaxamount * objCart.GSTPer) / 100, 2);
                                    decimal AfterTax = beforetaxamount + gstamt;
                                    objOrderItem.GSTPer = objCart.GSTPer;
                                    objOrderItem.GSTAmt = gstamt;
                                    objOrderItem.Price = originalbasicprice;
                                    objOrderItem.Discount = disc;
                                    objOrderItem.FinalItemPrice = AfterTax;
                                    objOrderItem.ItemStatus = 1;
                                    _db.tbl_OrderItemDetails.Add(objOrderItem);
                                    _db.SaveChanges();
                                    objcmn.SaveTransaction(objCart.ItemId, objOrderItem.OrderDetailId, objOrderItem.OrderId.Value, "Order Placed for Item", objOrderItem.FinalItemPrice.Value, clsClientSession.UserID, 0, DateTime.UtcNow, "New Order Item");
                                    if (objCheckout.ordertype == "1")
                                    {
                                        var objCartforremove = _db.tbl_Cart.Where(o => o.Cart_Id == objCart.CartId).FirstOrDefault();
                                        _db.tbl_Cart.Remove(objCartforremove);
                                    }
                                    else
                                    {
                                        var objCartforremove = _db.tbl_SecondCart.Where(o => o.SecondCartId == objCart.CartId).FirstOrDefault();
                                        _db.tbl_SecondCart.Remove(objCartforremove);
                                    }
                                    
                                }
                                _db.SaveChanges();
                            }

                            if(TotalDiscount > 0 && clsClientSession.RoleID == 1)
                            {
                                if (lstpoints != null && lstpoints.Count() > 0)
                                {
                                    decimal reminingdisc = TotalDiscount;
                                    foreach (tbl_PointDetails objpoint in lstpoints)
                                    {
                                        if(objpoint != null)
                                        {
                                            decimal pnts = objpoint.Points.Value;
                                            decimal usedpnts = objpoint.UsedPoints.Value;
                                            decimal remainingpnts = pnts - usedpnts;
                                            if(remainingpnts <= reminingdisc)
                                            {
                                                objpoint.UsedPoints = objpoint.UsedPoints + remainingpnts;
                                                reminingdisc = reminingdisc - remainingpnts;
                                            }
                                            else
                                            {
                                                objpoint.UsedPoints = objpoint.UsedPoints + reminingdisc;                                                
                                            }
                                        }
                                        _db.SaveChanges();
                                    }
                                    objcmn.SaveTransaction(0,0, objOrder.OrderId, "Points Used", TotalDiscount, clsClientSession.UserID, 0, DateTime.UtcNow, "Points Used");
                                }
                            }
                          
                            var objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                            if (objotherdetails != null)
                            {
                                decimal amtdue = 0;
                                if (objotherdetails.AmountDue != null)
                                {
                                    amtdue = objotherdetails.AmountDue.Value;

                                }
                                objotherdetails.AmountDue = amtdue + amtcredit;
                                objotherdetails.ShipAddress = objCheckout.shipaddress;
                                objotherdetails.ShipCity = objCheckout.shipcity;
                                objotherdetails.ShipFirstName = objCheckout.shipfirstname;
                                objotherdetails.ShipLastName = objCheckout.shiplastname;
                                objotherdetails.ShipPhoneNumber = objCheckout.shipphone;
                                objotherdetails.ShipPostalcode = objCheckout.shippincode;
                                objotherdetails.ShipState = objCheckout.shipstate;
                                objotherdetails.ShipEmail = objCheckout.shipemailaddress;
                                _db.SaveChanges();
                                if(amtcredit > 0)
                                {
                                    objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Payment By Credit Used : Rs" + amtcredit, amtcredit, clsClientSession.UserID, 0, DateTime.UtcNow, "Credit Used Payment");
                                }
                            }
                            string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                            ReturnMessage = "Success^" + orderid;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string AdminMobileNumber = objGensetting.AdminSMSNumber;
                            string msgsms = "New Order Received - Order No " + objOrder.OrderId + " - Krupa Build Gallery";
                            string msgsmscustomer = "Thank you for the Order. You order number is " + objOrder.OrderId + " - Krupa Build Gallery";
                            SendSMSForNewOrder(AdminMobileNumber, msgsms);
                            SendSMSForNewOrder(clsClientSession.MobileNumber, msgsmscustomer);
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

        public decimal GetOfferPrice(long Itemid, decimal price, long VariantId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return GetVarintPrc(VariantId, objItem.OfferPrice);
            }

            return price;
        }

        public decimal GetDistributorOfferPrice(long Itemid, decimal price, long VariantId)
        {

            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return GetVarintPrc(VariantId, objItem.OfferPriceforDistributor.Value);
            }

            return price;
        }

        public decimal GetPriceGenral(long Itemid, decimal price, long VariantId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                if (clsClientSession.RoleID == 1)
                {
                    return GetVarintPrc(VariantId, objItem.OfferPrice);
                }
                else
                {
                    return GetVarintPrc(VariantId, objItem.OfferPriceforDistributor.Value);
                }

            }

            return price;
        }
        [HttpPost]
        public string CheckItemsinStock(string IsCash = "false")
        {
            string ReturnMessage = "";
            bool isOutofStock = false;
            try
            {
                if(clsClientSession.UserID > 0)
                {
                    bool IsCashOrdr = false;
                    if(IsCash == "true")
                    {
                        IsCashOrdr = true;
                    }
                    var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == clsClientSession.UserID && o.IsCashonDelivery == IsCashOrdr).ToList();
                    if(cartlist != null && cartlist.Count() > 0)
                    {                        
                        foreach (var objcrt in cartlist)
                        {
                            if(objcrt != null)
                            {
                              int cntremingstk = RemainingStock(objcrt.CartItemId.Value);
                               decimal qtyy = GetVarintQnty(objcrt.VariantItemId.Value);
                              if (cntremingstk < (Convert.ToInt32(objcrt.CartItemQty) * qtyy))
                              {
                                    isOutofStock = true; 
                              }
                            }
                        }
                    }
                 
                }
                if (isOutofStock == true)
                {
                    ReturnMessage = "OutofStock";
                }
                else
                {
                    ReturnMessage = "Success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public int RemainingStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.QtyUsed.Value);
            if (TotalStock == null)
            {
                TotalStock = 0;
            }
            if (TotalSold == null)
            {
                TotalSold = 0;
            }
            long remiaing = TotalStock.Value - TotalSold.Value;
            return Convert.ToInt32(remiaing);
        }

        public string SendSMSForNewOrder(string MobileNumber,string Msg)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                { 
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + Msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }

                    return "Success";
                }
            }
            catch (WebException ex)
            {
                return "Fail";
            }
        }

        public ActionResult secondcartcheckout()
        {
            List<CartVM> lstCartItems = new List<CartVM>();
            try
            {

                decimal ShippingChargeTotal = 0;
                decimal TotalDiscount = 0;
                decimal TotalOrder = 0;
                ViewBag.WebsiteOrderId = "1";
                decimal AdvancePaymentAmt = 0;
                bool IsCashOrd = false;
                ViewBag.IsCashOnDelivery = IsCashOrd;
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
                    lstCartItems = (from crt in _db.tbl_SecondCart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.ClientUserId == ClientUserId
                                    select new CartVM
                                    {
                                        CartId = crt.SecondCartId,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        //Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        VariantId = crt.VariantItemId.Value,
                                        VariantQtytxt = vr.UnitQty,
                                        ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                        GSTPer = i.GST_Per,
                                        IsCashonDelivery = i.IsCashonDeliveryUse.HasValue?i.IsCashonDeliveryUse.Value : false,
                                        AdvncePayPer = i.PayAdvancePer.HasValue ? i.PayAdvancePer.Value : 0
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,x.VariantId); });
                }
                else
                {
                    lstCartItems = (from crt in _db.tbl_SecondCart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.CartSessionId == cookiesessionval
                                    select new CartVM
                                    {
                                        CartId = crt.SecondCartId,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = vr.CustomerPrice.Value,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        VariantId = crt.VariantItemId.Value,
                                        VariantQtytxt = vr.UnitQty,
                                        ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                        IsCashonDelivery = i.IsCashonDeliveryUse.HasValue ? i.IsCashonDeliveryUse.Value : false,
                                        GSTPer = i.GST_Per,
                                        AdvncePayPer = i.PayAdvancePer.HasValue ? i.PayAdvancePer.Value : 0
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price,x.VariantId); });
                }

                decimal creditlimitreminng = 0;
                if (clsClientSession.UserID > 0 && clsClientSession.RoleID == 2)
                {
                    long UserID = clsClientSession.UserID;
                    tbl_ClientOtherDetails objclientothr = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == UserID).FirstOrDefault();
                    if (objclientothr != null && objclientothr.CreditLimitAmt != null && objclientothr.CreditLimitAmt > 0)
                    {
                        decimal amountdue = 0;
                        if (objclientothr.AmountDue != null)
                        {
                            amountdue = objclientothr.AmountDue.Value;
                        }
                        creditlimitreminng = objclientothr.CreditLimitAmt.Value - amountdue;
                    }
                }
                List<InvoiceItemVM> lstInvItem = new List<InvoiceItemVM>();
                if (clsClientSession.RoleID == 1)
                {
                    var lstCartItemsnew = lstCartItems.OrderByDescending(o => o.GSTPer).ToList();
                    DateTime dtNow = DateTime.UtcNow;
                    long clientusrrId = clsClientSession.UserID;
                    List<tbl_PointDetails> lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == clientusrrId && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();
                    decimal pointreamining = 0;
                    if (lstpoints != null && lstpoints.Count() > 0)
                    {
                        pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
                    }
                    decimal totalremining = pointreamining;
                    if (lstCartItemsnew.Count() > 0)
                    {
                        foreach (var objcr in lstCartItemsnew)
                        {
                            if (objcr != null)
                            {
                                //decimal InclusiveGST = Math.Round(objcr.Price - objcr.Price * (100 / (100 + objcr.GSTPer)), 2);
                                //decimal PreGSTPrice = Math.Round(objcr.Price - InclusiveGST, 2);
                                decimal originalbasicprice = Math.Round(((objcr.Price / (100 + objcr.GSTPer)) * 100), 2);
                                decimal totalItembasicprice = originalbasicprice * objcr.Qty;
                                decimal disc = Math.Round((totalItembasicprice * 5) / 100, 2);
                                if (disc <= totalremining)
                                {
                                    totalremining = totalremining - disc;
                                }
                                else
                                {
                                    disc = totalremining;
                                    totalremining = totalremining - disc;
                                }
                                TotalDiscount = TotalDiscount + disc;
                                ShippingChargeTotal = ShippingChargeTotal + (objcr.ShippingCharge * objcr.Qty);
                                decimal beforetaxamount = Math.Round(totalItembasicprice - disc, 2);
                                decimal gstamt = Math.Round((beforetaxamount * objcr.GSTPer) / 100, 2);
                                decimal AfterTax = beforetaxamount + gstamt;
                                InvoiceItemVM objInvItm = new InvoiceItemVM();
                                objInvItm.ItemName = objcr.ItemName;
                                objInvItm.GSTPer = objcr.GSTPer;
                                objInvItm.GSTAmount = gstamt;
                                objInvItm.Qty = Convert.ToInt32(objcr.Qty);
                                objInvItm.BasicAmount = originalbasicprice;
                                objInvItm.Discount = disc;
                                objInvItm.ItemAmount = AfterTax;
                                objInvItm.beforetaxamount = beforetaxamount;
                                TotalOrder = TotalOrder + AfterTax;
                                objInvItm.AdvncePayAMt = Math.Round((AfterTax * objcr.AdvncePayPer) / 100, 2);
                                AdvancePaymentAmt = objInvItm.AdvncePayAMt + AdvancePaymentAmt;
                                lstInvItem.Add(objInvItm);
                            }
                        }
                    }
                }
                else if (clsClientSession.RoleID == 2)
                {
                    var lstCartItemsnew = lstCartItems.OrderByDescending(o => o.GSTPer).ToList();
                    decimal pointreamining = 0;
                    decimal totalremining = pointreamining;

                    if (lstCartItemsnew.Count() > 0)
                    {
                        foreach (var objcr in lstCartItemsnew)
                        {
                            if (objcr != null)
                            {
                                decimal originalbasicprice = Math.Round(((objcr.Price / (100 + objcr.GSTPer)) * 100), 2);
                                decimal totalItembasicprice = originalbasicprice * objcr.Qty;
                                decimal disc = 0;// Math.Round((totalItembasicprice * 5) / 100, 2);
                                ShippingChargeTotal = ShippingChargeTotal + (objcr.ShippingCharge * objcr.Qty);
                                decimal beforetaxamount = Math.Round(totalItembasicprice - disc, 2);
                                decimal gstamt = Math.Round((beforetaxamount * objcr.GSTPer) / 100, 2);
                                decimal AfterTax = beforetaxamount + gstamt;
                                InvoiceItemVM objInvItm = new InvoiceItemVM();
                                objInvItm.ItemName = objcr.ItemName;
                                objInvItm.GSTPer = objcr.GSTPer;
                                objInvItm.GSTAmount = gstamt;
                                objInvItm.Qty = Convert.ToInt32(objcr.Qty);
                                objInvItm.BasicAmount = originalbasicprice;
                                objInvItm.Discount = disc;
                                objInvItm.ItemAmount = AfterTax;
                                objInvItm.beforetaxamount = beforetaxamount;
                                TotalOrder = TotalOrder + AfterTax;
                                objInvItm.AdvncePayAMt = Math.Round((AfterTax * objcr.AdvncePayPer) / 100, 2);
                                AdvancePaymentAmt = objInvItm.AdvncePayAMt + AdvancePaymentAmt;
                                lstInvItem.Add(objInvItm);
                            }
                        }
                    }
                }

                ViewBag.CreditRemaining = creditlimitreminng;
                tbl_ClientOtherDetails objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                ViewData["objotherdetails"] = objotherdetails;
                ViewData["lstInvItem"] = lstInvItem;
                ViewBag.ShippingChargeTotal = ShippingChargeTotal;
                ViewBag.TotalDiscount = TotalDiscount;
                ViewBag.TotalOrder = TotalOrder;
                var objtbl_ExtraAmount =  _db.tbl_ExtraAmount.Where(o => o.AmountFrom <= TotalOrder && o.AmountTo >= TotalOrder).FirstOrDefault();
                ViewBag.ExtraAmt = 0;
                if (objtbl_ExtraAmount != null)
                {
                    ViewBag.ExtraAmt = objtbl_ExtraAmount.ExtraAmount;
                }
                decimal WalletAmt = 0;
                var objuserclient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                if (objuserclient != null)
                {
                    WalletAmt = objuserclient.WalletAmt.HasValue ? objuserclient.WalletAmt.Value : 0;
                }              
                //_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                ViewBag.WalletAmt = WalletAmt;
                ViewBag.AdvancePaymentAmt = AdvancePaymentAmt;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            ViewData["lstCartItems"] = lstCartItems;
            var objGenralsetting = _db.tbl_GeneralSetting.FirstOrDefault();
            ViewBag.ShippingMsg = objGenralsetting.ShippingMessage;
           
            return View();
        }

        public decimal GetVarintPrc(long VariantId, decimal Price)
        {
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Killo", "2 Killo", "5 Killo" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 litre", "2 litres", "5 litres" };
            string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

            string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
            string[] sheetsqty = { "32", "28", "21", "24", "18" };
            tbl_ItemVariant objVarints = _db.tbl_ItemVariant.Where(o => o.VariantItemId == VariantId).FirstOrDefault();
            if (objVarints != null)
            {
                if (Array.IndexOf(kgs, objVarints.UnitQty) >= 0)
                {
                    int idxxx = Array.IndexOf(kgs, objVarints.UnitQty);
                    decimal qtt = Convert.ToDecimal(kgsQty[idxxx].ToString());
                    if (qtt >= 1)
                    {
                        return Math.Round((Price * qtt * objVarints.PricePecentage.Value) / 100, 2);
                    }
                    else
                    {
                        return Math.Round((Price * objVarints.PricePecentage.Value) / 100, 2);
                    }
                }
                else if (Array.IndexOf(ltrs, objVarints.UnitQty) >= 0)
                {
                    int idxxx = Array.IndexOf(ltrs, objVarints.UnitQty);
                    decimal qtt = Convert.ToDecimal(ltrsQty[idxxx].ToString());
                    if (qtt >= 1)
                    {
                        return Math.Round((Price * qtt * objVarints.PricePecentage.Value) / 100, 2);
                    }
                    else
                    {
                        return Math.Round((Price * objVarints.PricePecentage.Value) / 100, 2);
                    }
                }
                else if (Array.IndexOf(sheets, objVarints.UnitQty) >= 0)
                {
                    int idxxx = Array.IndexOf(sheets, objVarints.UnitQty);
                    decimal sqft = Convert.ToDecimal(sheetsqty[idxxx]);
                    return Math.Round((Price * sqft) / 100, 2);
                }
                else
                {
                    return Price;
                }
            }

            return Price;
        }

        public decimal GetVarintQtyy(string varintqty)
        {
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Killo", "2 Killo", "5 Killo" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 litre", "2 litres", "5 litres" };
            string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

            string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
            string[] sheetsqty = { "32", "28", "21", "24", "18" };
            if (Array.IndexOf(kgs, varintqty) >= 0)
            {
                int idxxx = Array.IndexOf(kgs, varintqty);
                decimal qtt = Convert.ToDecimal(kgsQty[idxxx].ToString());
                return qtt;
            }
            else if (Array.IndexOf(ltrs, varintqty) >= 0)
            {
                int idxxx = Array.IndexOf(ltrs, varintqty);
                decimal qtt = Convert.ToDecimal(ltrsQty[idxxx].ToString());
                return qtt;
            }
            else 
            {
                return 1;
            }            
        }

        public decimal GetVarintQnty(long VariantId)
        {
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Killo", "2 Killo", "5 Killo" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 litre", "2 litres", "5 litres" };
            string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

            string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
            string[] sheetsqty = { "32", "28", "21", "24", "18" };
            tbl_ItemVariant objVarints = _db.tbl_ItemVariant.Where(o => o.VariantItemId == VariantId).FirstOrDefault();
            if (objVarints != null)
            {
                if (Array.IndexOf(kgs, objVarints.UnitQty) >= 0)
                {
                    int idxxx = Array.IndexOf(kgs, objVarints.UnitQty);
                    decimal qtt = Convert.ToDecimal(kgsQty[idxxx].ToString());
                    return qtt;
                }
                else if (Array.IndexOf(ltrs, objVarints.UnitQty) >= 0)
                {
                    int idxxx = Array.IndexOf(ltrs, objVarints.UnitQty);
                    decimal qtt = Convert.ToDecimal(ltrsQty[idxxx].ToString());
                    return qtt;
                }
                else
                {
                    return 1;
                }
            }

            return 1;
        }
    }
}