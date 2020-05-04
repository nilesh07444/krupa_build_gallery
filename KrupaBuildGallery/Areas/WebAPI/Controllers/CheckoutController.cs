using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class CheckoutController : ApiController
    {
        krupagallarydbEntities _db;
        public CheckoutController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("GetCheckOutDetails"), HttpPost]
        public ResponseDataModel<CheckoutGenVM> GetCheckOutDetails(GeneralVM objGen)
        {
            ResponseDataModel<CheckoutGenVM> response = new ResponseDataModel<CheckoutGenVM>();
            CheckoutGenVM objChkout = new CheckoutGenVM();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoledId = Convert.ToInt64(objGen.RoleId);
                List<CartVM> lstCartItems = new List<CartVM>();
                decimal ShippingChargeTotal = 0;
                decimal TotalDiscount = 0;
                decimal TotalOrder = 0;
            
                if (UserId > 0)
                {
                    long ClientUserId = Convert.ToInt64(UserId);
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    where crt.ClientUserId == ClientUserId
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = RoledId == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                        GSTPer = i.GST_Per
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price, RoledId); });
                }              

                decimal creditlimitreminng = 0;
                if (UserId > 0 && RoledId == 2)
                {
                    long UserID = UserId;
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
                if (RoledId == 1)
                {
                    var lstCartItemsnew = lstCartItems.OrderByDescending(o => o.GSTPer).ToList();
                    DateTime dtNow = DateTime.UtcNow;
                    long clientusrrId = UserId;
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
                                lstInvItem.Add(objInvItm);
                            }
                        }
                    }
                }
                else if (RoledId == 2)
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
                                lstInvItem.Add(objInvItm);
                            }
                        }
                    }
                }

                objChkout.CreditRemaining = creditlimitreminng;                
                tbl_ClientOtherDetails objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == UserId).FirstOrDefault();
                objChkout.ClientOtherDetails = objotherdetails;
                objChkout.InvItems = lstInvItem;
                objChkout.ShippingChargeTotal = ShippingChargeTotal;
                objChkout.TotalDiscount = TotalDiscount;
                objChkout.TotalOrder = TotalOrder;
                objChkout.CartList = lstCartItems;
                var objGenralsetting = _db.tbl_GeneralSetting.FirstOrDefault();
                objChkout.shippingmsg = objGenralsetting.ShippingMessage;
                response.Data = objChkout;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }
           
            return response;
        }

        [Route("CheckItemsinStock"), HttpPost]
        public ResponseDataModel<string> CheckItemsinStock(GeneralVM objGen)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
         
            bool isOutofStock = false;
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                if (UserId > 0)
                {
                    var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == UserId).ToList();
                    if (cartlist != null && cartlist.Count() > 0)
                    {
                        foreach (var objcrt in cartlist)
                        {
                            if (objcrt != null)
                            {
                                int cntremingstk = RemainingStock(objcrt.CartItemId.Value);
                                if (cntremingstk < Convert.ToInt32(objcrt.CartItemQty))
                                {
                                    isOutofStock = true;
                                }
                            }
                        }
                    }

                }
                if (isOutofStock == true)
                {
                    response.AddError("OutofStock");
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

        [Route("PlaceOrder"), HttpPost]
        public ResponseDataModel<string> PlaceOrder(PlaceOrderVM objPlaceOrderVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();         
            try
            {
                long clientusrid = Convert.ToInt64(objPlaceOrderVM.ClientUserId);
                long RoleId = Convert.ToInt64(objPlaceOrderVM.RoleId);
                if (objPlaceOrderVM.razorpay_payment_id == "ByCredit")
                {
                    List<CartVM> lstCartItems = (from crt in _db.tbl_Cart
                                                 join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                                 where crt.ClientUserId == clientusrid
                                                 select new CartVM
                                                 {
                                                     CartId = crt.Cart_Id,
                                                     ItemName = i.ItemName,
                                                     ItemId = i.ProductItemId,
                                                     Price = RoleId == 1 ? i.CustomerPrice : i.DistributorPrice,
                                                     ItemImage = i.MainImage,
                                                     Qty = crt.CartItemQty.Value,
                                                     ItemSku = i.Sku,
                                                     GSTPer = i.GST_Per,
                                                     IGSTPer = i.IGST_Per
                                                 }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price, RoleId); });
                    // List<tbl_Cart> lstCarts = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                    string paymentmethod = "ByCredit";

                    int year = DateTime.UtcNow.Year;
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
                        if (objOrdertemp.InvoiceNo == null)
                        {
                            objOrdertemp.InvoiceNo = 1;
                        }
                        Invno = objOrdertemp.InvoiceNo.Value + 1;
                    }


                    tbl_Orders objOrder = new tbl_Orders();
                    objOrder.ClientUserId = clientusrid;
                    decimal ordramt = Convert.ToDecimal(objPlaceOrderVM.Orderamount);
                    decimal shippingcharge = 0;
                    if (objPlaceOrderVM.shippincode == "389001")
                    {
                        shippingcharge = Convert.ToDecimal(objPlaceOrderVM.shipamount);
                        ordramt = ordramt - shippingcharge;
                    }
                    objOrder.OrderAmount = ordramt;
                    objOrder.OrderShipCity = objPlaceOrderVM.shipcity;
                    objOrder.OrderShipAddress = objPlaceOrderVM.shipaddress;
                    objOrder.OrderShipState = objPlaceOrderVM.shipstate;
                    objOrder.OrderShipClientName = objPlaceOrderVM.shipfirstname + " " + objPlaceOrderVM.shiplastname;
                    objOrder.OrderShipClientPhone = objPlaceOrderVM.shipphone;
                    objOrder.OrderShipPincode = objPlaceOrderVM.shippincode;
                    objOrder.OrderStatusId = Convert.ToInt64(OrderStatus.NewOrder);
                    objOrder.PaymentType = paymentmethod;
                    objOrder.IsActive = true;
                    objOrder.IsDelete = false;
                    objOrder.CreatedBy = clientusrid;
                    objOrder.CreatedDate = DateTime.UtcNow;
                    objOrder.UpdatedBy = clientusrid;
                    objOrder.UpdatedDate = DateTime.UtcNow;
                    objOrder.AmountDue = ordramt;
                    objOrder.RazorpayOrderId = "";
                    objOrder.RazorpayPaymentId = "";
                    objOrder.InvoiceNo = Invno;
                    objOrder.InvoiceYear = year + "-" + toyear;
                    objOrder.RazorSignature = "";
                    if (objPlaceOrderVM.shippincode == "389001")
                    {
                        objOrder.ShippingCharge = Convert.ToDecimal(objPlaceOrderVM.shipamount);
                        objOrder.ShippingStatus = 2;
                    }
                    else
                    {
                        objOrder.ShippingCharge = 0;
                        objOrder.ShippingStatus = 1;
                    }
                    _db.tbl_Orders.Add(objOrder);
                    _db.SaveChanges();
                    objOrder.RazorpayOrderId = objOrder.OrderId.ToString();

                    var objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    if (objotherdetails != null)
                    {
                        decimal amtdue = 0;
                        if (objotherdetails.AmountDue != null)
                        {
                            amtdue = objotherdetails.AmountDue.Value;

                        }
                        objotherdetails.AmountDue = amtdue + ordramt;
                        objotherdetails.ShipAddress = objPlaceOrderVM.shipaddress;
                        objotherdetails.ShipCity = objPlaceOrderVM.shipcity;
                        objotherdetails.ShipFirstName = objPlaceOrderVM.shipfirstname;
                        objotherdetails.ShipLastName = objPlaceOrderVM.shiplastname;
                        objotherdetails.ShipPhoneNumber = objPlaceOrderVM.shipphone;
                        objotherdetails.ShipPostalcode = objPlaceOrderVM.shippincode;
                        objotherdetails.ShipState = objPlaceOrderVM.shipstate;
                        objotherdetails.ShipEmail = objPlaceOrderVM.shipemailaddress;
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
                            decimal beforetaxamount = Math.Round(totalItembasicprice - disc, 2);
                            decimal gstamt = Math.Round((beforetaxamount * objCart.GSTPer) / 100, 2);
                            decimal AfterTax = beforetaxamount + gstamt;
                            objOrderItem.GSTPer = objCart.GSTPer;
                            objOrderItem.GSTAmt = gstamt;
                            objOrderItem.Price = originalbasicprice;
                            objOrderItem.Discount = disc;
                            _db.tbl_OrderItemDetails.Add(objOrderItem);
                            var objCartforremove = _db.tbl_Cart.Where(o => o.Cart_Id == objCart.CartId).FirstOrDefault();
                            _db.tbl_Cart.Remove(objCartforremove);
                        }
                        _db.SaveChanges();
                    }

                    string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                    response.Data = "Success^" + orderid;
                }
                else
                {
                    Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(objPlaceOrderVM.razorpay_payment_id);
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
                                                             Price = RoleId == 1 ? i.CustomerPrice : i.DistributorPrice,
                                                             ItemImage = i.MainImage,
                                                             Qty = crt.CartItemQty.Value,
                                                             ItemSku = i.Sku,
                                                             GSTPer = i.GST_Per,
                                                             IGSTPer = i.IGST_Per
                                                         }).OrderByDescending(x => x.CartId).ToList();
                            lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,RoleId); });
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
                                if (objOrdertemp.InvoiceNo == null)
                                {
                                    objOrdertemp.InvoiceNo = 1;
                                }
                                Invno = objOrdertemp.InvoiceNo.Value + 1;
                            }
                            decimal ordramt = Convert.ToDecimal(objPlaceOrderVM.Orderamount);
                            decimal shippingcharge = 0;
                            if (objPlaceOrderVM.shippincode == "389001")
                            {
                                shippingcharge = Convert.ToDecimal(objPlaceOrderVM.shipamount);
                                ordramt = ordramt - shippingcharge;
                            }
                            tbl_Orders objOrder = new tbl_Orders();
                            objOrder.ClientUserId = clientusrid;
                            objOrder.OrderAmount = ordramt;
                            objOrder.OrderShipCity = objPlaceOrderVM.shipcity;
                            objOrder.OrderShipAddress = objPlaceOrderVM.shipaddress;
                            objOrder.OrderShipState = objPlaceOrderVM.shipstate;
                            objOrder.OrderShipClientName = objPlaceOrderVM.shipfirstname + " " + objPlaceOrderVM.shiplastname;
                            objOrder.OrderShipClientPhone = objPlaceOrderVM.shipphone;
                            objOrder.OrderShipPincode = objPlaceOrderVM.shippincode;
                            objOrder.OrderStatusId = Convert.ToInt64(OrderStatus.NewOrder);
                            objOrder.PaymentType = paymentmethod;
                            objOrder.IsActive = true;
                            objOrder.IsDelete = false;
                            objOrder.CreatedBy = clientusrid;
                            objOrder.CreatedDate = DateTime.UtcNow;
                            objOrder.UpdatedBy = clientusrid;
                            objOrder.UpdatedDate = DateTime.UtcNow;
                            objOrder.AmountDue = 0;
                            objOrder.InvoiceNo = Invno;
                            objOrder.InvoiceYear = year + "-" + toyear;
                            objOrder.RazorpayOrderId = "";
                            objOrder.RazorpayPaymentId = objPlaceOrderVM.razorpay_payment_id;
                            objOrder.RazorSignature = "";
                            if (objPlaceOrderVM.shippincode == "389001")
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
                            tbl_PaymentHistory objPyment = new tbl_PaymentHistory();
                            objPyment.OrderId = objOrder.OrderId;
                            objPyment.PaymentBy = paymentmethod;
                            objPyment.AmountDue = Convert.ToDecimal(ordramt);
                            objPyment.AmountPaid = Convert.ToDecimal(ordramt);
                            objPyment.DateOfPayment = DateTime.UtcNow;
                            objPyment.CreatedBy = clientusrid;
                            objPyment.CreatedDate = DateTime.UtcNow;
                            objPyment.RazorpayOrderId = "";
                            objPyment.RazorpayPaymentId = objPlaceOrderVM.razorpay_payment_id;
                            objPyment.RazorSignature = "";
                            objPyment.PaymentFor = "OrderPayment";
                            _db.tbl_PaymentHistory.Add(objPyment);
                            _db.SaveChanges();
                            decimal pointreamining = 0;
                            decimal totalremining = 0;
                            decimal TotalDiscount = 0;
                            List<tbl_PointDetails> lstpoints = new List<tbl_PointDetails>();
                            if (RoleId == 1)
                            {
                                DateTime dtNow = DateTime.UtcNow;                                
                                lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == clientusrid && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();

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
                                    decimal originalbasicprice = Math.Round(((objCart.Price / (100 + objCart.GSTPer)) * 100), 2);
                                    decimal totalItembasicprice = originalbasicprice * objCart.Qty;
                                    decimal disc = 0;
                                    if (RoleId == 1)
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
                                    decimal beforetaxamount = Math.Round(totalItembasicprice - disc, 2);
                                    decimal gstamt = Math.Round((beforetaxamount * objCart.GSTPer) / 100, 2);
                                    decimal AfterTax = beforetaxamount + gstamt;
                                    objOrderItem.GSTPer = objCart.GSTPer;
                                    objOrderItem.GSTAmt = gstamt;
                                    objOrderItem.Price = originalbasicprice;
                                    objOrderItem.Discount = disc;

                                    _db.tbl_OrderItemDetails.Add(objOrderItem);
                                    var objCartforremove = _db.tbl_Cart.Where(o => o.Cart_Id == objCart.CartId).FirstOrDefault();
                                    _db.tbl_Cart.Remove(objCartforremove);
                                }
                                _db.SaveChanges();
                            }

                            if (TotalDiscount > 0 && RoleId == 1)
                            {
                                if (lstpoints != null && lstpoints.Count() > 0)
                                {
                                    decimal reminingdisc = TotalDiscount;
                                    foreach (tbl_PointDetails objpoint in lstpoints)
                                    {
                                        if (objpoint != null)
                                        {
                                            decimal pnts = objpoint.Points.Value;
                                            decimal usedpnts = objpoint.UsedPoints.Value;
                                            decimal remainingpnts = pnts - usedpnts;
                                            if (remainingpnts <= reminingdisc)
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
                                }
                            }

                            var objotherdetails = _db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                            if (objotherdetails != null)
                            {
                                objotherdetails.ShipAddress = objPlaceOrderVM.shipaddress;
                                objotherdetails.ShipCity = objPlaceOrderVM.shipcity;
                                objotherdetails.ShipFirstName = objPlaceOrderVM.shipfirstname;
                                objotherdetails.ShipLastName = objPlaceOrderVM.shiplastname;
                                objotherdetails.ShipPhoneNumber = objPlaceOrderVM.shipphone;
                                objotherdetails.ShipPostalcode = objPlaceOrderVM.shippincode;
                                objotherdetails.ShipState = objPlaceOrderVM.shipstate;
                                objotherdetails.ShipEmail = objPlaceOrderVM.shipemailaddress;
                                _db.SaveChanges();
                            }
                            string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                            response.Data = "Success^" + orderid;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string AdminMobileNumber = objGensetting.AdminSMSNumber;
                            string msgsms = "New Order Received - Order No " + objOrder.OrderId + " - Krupa Build Gallery";
                            string msgsmscustomer = "Thank you for the Order. You order number is " + objOrder.OrderId + " - Krupa Build Gallery";
                            SendSMSForNewOrder(AdminMobileNumber, msgsms);
                            SendSMSForNewOrder(objPlaceOrderVM.MobileNumber, msgsmscustomer);
                        }
                        else
                        {
                            response.Data = "Payment " + objpymn["status"];
                        }

                    }
                    else
                    {
                        response.Data = "Payment Failed";
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

        public decimal GetPriceGenral(long Itemid, decimal price,long RoleId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                if (RoleId == 1)
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

        public int RemainingStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.Qty.Value);
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

        public string SendSMSForNewOrder(string MobileNumber, string Msg)
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

    }
}