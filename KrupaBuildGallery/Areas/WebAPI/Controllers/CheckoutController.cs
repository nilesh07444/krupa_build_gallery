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
                string type = objGen.CheckoutType;
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoledId = Convert.ToInt64(objGen.RoleId);
                List<CartVM> lstCartItems = new List<CartVM>();
                decimal ShippingChargeTotal = 0;
                decimal Ordetotlyearly = 0;
                decimal TotalDiscount = 0;
                decimal TotalOrder = 0;
                bool IsCashOrd = false;
                if (type == "Cash")
                {
                    IsCashOrd = true;
                    int year1 = DateTime.UtcNow.Year;
                    int toyear = year1 + 1;
                    if (DateTime.UtcNow.Month <= 3)
                    {
                        year1 = year1 - 1;
                        toyear = year1;
                    }
                    DateTime dtfincialyear = new DateTime(year1, 4, 1);
                    DateTime dtendyear = new DateTime(toyear, 3, 31);
                    List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                       join c in _db.tbl_Orders on p.OrderId equals c.OrderId
                                                       where c.ClientUserId == UserId && c.IsCashOnDelivery == true && p.ItemStatus != 5 && p.ItemStatus != 6 && p.ItemStatus != 8 && c.CreatedDate >= dtfincialyear && c.CreatedDate <= dtendyear
                                                       select new OrderItemsVM
                                                       {
                                                           FinalAmt = p.FinalItemPrice.Value
                                                       }).ToList();

                    if (lstOrderItms != null && lstOrderItms.Count() > 0)
                    {
                        Ordetotlyearly = lstOrderItms.Sum(x => x.FinalAmt);
                    }
                }
                if (UserId > 0)
                {
                    long ClientUserId = Convert.ToInt64(UserId);
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.ClientUserId == ClientUserId && crt.IsCashonDelivery == IsCashOrd
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = RoledId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                        ItemImage = i.MainImage,
                                        VariantId = crt.VariantItemId.Value,
                                        VariantQtytxt = vr.UnitQty,
                                        Qty = crt.CartItemQty.Value,
                                        ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                        GSTPer = i.GST_Per,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price, RoledId,x.VariantId); });
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

                var objtbl_ExtraAmount = _db.tbl_ExtraAmount.Where(o => o.AmountFrom <= TotalOrder && o.AmountTo >= TotalOrder).FirstOrDefault();
                objChkout.ExtraAmount = 0;
                if (objtbl_ExtraAmount != null)
                {
                    objChkout.ExtraAmount = objtbl_ExtraAmount.ExtraAmount.Value;
                }
                decimal WalletAmt = 0;
                var objuserclient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == UserId).FirstOrDefault();
                if (objuserclient != null)
                {
                    WalletAmt = objuserclient.WalletAmt.HasValue ? objuserclient.WalletAmt.Value : 0;
                }
                //_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                objChkout.WalletAmount = WalletAmt;
                objChkout.IsCashOnDelivery = IsCashOrd;
                objChkout.YearlyOrderPlaced = Ordetotlyearly;
                objChkout.CashOrderAmtMax = objGenralsetting.CashLimitPerOrder.Value;
                objChkout.CashOrderAmtYerly = objGenralsetting.CashLimitPerYear.Value;
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
        public ResponseDataModel<List<string>> CheckItemsinStock(GeneralVM objGen)
        {
            ResponseDataModel<List<string>> response = new ResponseDataModel<List<string>>();
            List<string> strlst = new List<string>();
            bool isOutofStock = false;
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                decimal amt = Convert.ToDecimal(objGen.Amount);
                string IsCash = objGen.CheckoutType;
                if (UserId > 0)
                {
                    bool IsCashOrdr = false;
                    if (IsCash == "true")
                    {
                        IsCashOrdr = true;
                    }
                    var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == UserId && o.IsCashonDelivery == IsCashOrdr).ToList();
                    if (cartlist != null && cartlist.Count() > 0)
                    {
                        foreach (var objcrt in cartlist)
                        {
                            if (objcrt != null)
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
                    response.AddError("OutofStock");
                }
                else
                {
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    input.Add("amount", amt * 100); // this amount should be same as transaction amount
                    input.Add("currency", "INR");
                    input.Add("receipt", "rec_"+ UserId+"_"+ DateTime.Now.ToString("ddmmyyyy"));
                    input.Add("payment_capture", 1);

                    var objGsetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    string key = objGsetting.RazorPayKey;  //"rzp_test_DMsPlGIBp3SSnI";
                    string secret = objGsetting.RazorPaySecret; // "YMkpd9LbnaXViePncLLXhqms";

                    RazorpayClient client = new RazorpayClient(key, secret);
                    Razorpay.Api.Order order = client.Order.Create(input);
                    strlst.Add("Success");
                    strlst.Add(Convert.ToString(order["id"]));
                    response.Data = strlst;
                        
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
                clsCommon objcmn = new clsCommon();
                long clientusrid = Convert.ToInt64(objPlaceOrderVM.ClientUserId);
                long RoleId = Convert.ToInt64(objPlaceOrderVM.RoleId);
                bool Iscashondelivery = false;
                decimal amtwallet = Convert.ToDecimal(objPlaceOrderVM.walletamtinorder);
                decimal amtcredit = Convert.ToDecimal(objPlaceOrderVM.creditamtinorder);
                decimal amtonline = Convert.ToDecimal(objPlaceOrderVM.onlineamtinorder);
                if (objPlaceOrderVM.razorpay_payment_id == "ByCredit")
                {
                    decimal amtorderdue = 0;
                    string paymentmethod = "ByCredit";
                    if (objPlaceOrderVM.isCashondelivery.ToLower() == "true")
                    {
                        paymentmethod = "Cash on Delivery";
                        Iscashondelivery = true;
                    }
                    List<CartVM> lstCartItems = new List<CartVM>();
                    if (objPlaceOrderVM.ordertype == "1")
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
                                            Price = RoleId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
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
                                            Price = RoleId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                            ItemImage = i.MainImage,
                                            Qty = crt.CartItemQty.Value,
                                            VariantId = crt.VariantItemId.Value,
                                            VariantQtytxt = vr.UnitQty,
                                            ItemSku = i.Sku,
                                            GSTPer = i.GST_Per,
                                            IGSTPer = i.IGST_Per
                                        }).OrderByDescending(x => x.CartId).ToList();
                    }
                      
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price, RoleId,x.VariantId); });
                    // List<tbl_Cart> lstCarts = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                    
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
                    decimal extraamt = 0;
                    if (objPlaceOrderVM.shippincode == "389001")
                    {
                        shippingcharge = Convert.ToDecimal(objPlaceOrderVM.shipamount);
                        extraamt = Convert.ToDecimal(objPlaceOrderVM.extraamount);
                        ordramt = ordramt - shippingcharge - extraamt;                      
                    }
                    if (objPlaceOrderVM.isCashondelivery.ToLower() == "true")
                    {
                        amtorderdue = ordramt + shippingcharge + extraamt;
                    }
                    else
                    {
                        if (amtwallet > 0 && amtcredit > 0)
                        {
                            paymentmethod = "Wallet and Credit";
                        }
                        else if (amtwallet > 0)
                        {
                            paymentmethod = "Wallet";
                        }
                        else if (amtcredit > 0)
                        {
                            paymentmethod = "Credit";
                        }
                        if (objPlaceOrderVM.ordertype == "1")
                        {
                            amtorderdue = amtcredit;
                        }
                        else
                        {
                            decimal advncpay = 0;
                            if (!string.IsNullOrEmpty(objPlaceOrderVM.advanceamtpay))
                            {
                                advncpay = Convert.ToDecimal(objPlaceOrderVM.advanceamtpay);
                                decimal totlordewithship = ordramt + shippingcharge + extraamt;
                                decimal remaingammt = totlordewithship - advncpay;
                                amtorderdue = remaingammt + amtcredit;
                            }
                        }
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
                    objOrder.AmountDue = amtorderdue;
                    objOrder.RazorpayOrderId = "";
                    objOrder.RazorpayPaymentId = objPlaceOrderVM.razorpay_payment_id;
                    objOrder.InvoiceNo = Invno;
                    objOrder.InvoiceYear = year + "-" + toyear;
                    objOrder.RazorSignature = "";
                    objOrder.ExtraAmount = extraamt;
                    objOrder.OrderType = Convert.ToInt32(objPlaceOrderVM.ordertype);
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
                    objOrder.WalletAmountUsed = amtwallet;
                    objOrder.CreditAmountUsed = amtcredit;
                    objOrder.AmountByRazorPay = amtonline;
                    if (objPlaceOrderVM.isCashondelivery.ToLower() == "true")
                    {
                        objOrder.IsCashOnDelivery = true;
                    }
                    else
                    {
                        objOrder.IsCashOnDelivery = false;
                    }
                    _db.tbl_Orders.Add(objOrder);
                    _db.SaveChanges();
                    if (objOrder.IsCashOnDelivery == true)
                    {
                        objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Cash On Delivery Order : Rs" + amtorderdue, amtorderdue, clientusrid, 0, DateTime.UtcNow, "Cash On Delivery Order");
                    }
                    objOrder.RazorpayOrderId = objOrder.OrderId.ToString();
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
                    }
                    objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Payment By Wallet : Rs" + amtwallet, amtwallet, clientusrid, 0, DateTime.UtcNow, "Wallet Payment");
                    if (amtcredit > 0)
                    {
                        objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Payment By Credit Used : Rs" + amtcredit, amtcredit,clientusrid, 0, DateTime.UtcNow, "Credit Used Payment");
                    }
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
                    decimal pointreamining = 0;
                    decimal totalremining = 0;
                    decimal TotalDiscount = 0;
                    List<tbl_PointDetails> lstpoints = new List<tbl_PointDetails>();
                    if (RoleId == 1)
                    {
                        DateTime dtNow = DateTime.UtcNow;
                        long clientusrrId = clientusrid;
                        lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == clientusrrId && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();

                        if (lstpoints != null && lstpoints.Count() > 0)
                        {
                            pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
                        }
                        totalremining = pointreamining;
                    }
                    decimal disc = 0;
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
                                    totalremining = totalremining - disc;
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
                            objOrderItem.ItemStatus = Convert.ToInt32(OrderStatus.NewOrder); 
                            objOrderItem.FinalItemPrice = AfterTax;
                            _db.tbl_OrderItemDetails.Add(objOrderItem);
                            objcmn.SaveTransaction(objCart.ItemId, objOrderItem.OrderDetailId, objOrderItem.OrderId.Value, "Order Placed for Item", objOrderItem.FinalItemPrice.Value, clientusrid, 0, DateTime.UtcNow, "New Order Item");
                            if (objPlaceOrderVM.ordertype == "1")
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
                    if (amtonline > 0 && Iscashondelivery == false)
                    {
                        tbl_PaymentHistory objPyment = new tbl_PaymentHistory();
                        objPyment.OrderId = objOrder.OrderId;
                        objPyment.PaymentBy = "online";
                        objPyment.AmountDue = Convert.ToDecimal(amttTotlordepy);
                        objPyment.AmountPaid = Convert.ToDecimal(amtonline);
                        objPyment.DateOfPayment = DateTime.UtcNow;
                        objPyment.CreatedBy = clientusrid;
                        objPyment.CreatedDate = DateTime.UtcNow;
                        objPyment.RazorpayOrderId = "";
                        objPyment.RazorpayPaymentId = objPlaceOrderVM.razorpay_payment_id;
                        objPyment.RazorSignature = "";
                        objPyment.PaymentFor = "OrderPayment";
                        _db.tbl_PaymentHistory.Add(objPyment);
                        objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Order Online Payment : Rs" + amtonline, amtonline,clientusrid, 0, DateTime.UtcNow, "Online Payment");
                    }
                    _db.SaveChanges();
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
                            objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Points Used", TotalDiscount, clientusrid, 0, DateTime.UtcNow, "Points Used");
                        }
                    }
                    string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                    response.Data = "Success^" + orderid;
                    tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    string AdminMobileNumber = objGensetting.AdminSMSNumber;
                    string msgsms = "New Order Received - Order No " + objOrder.OrderId + " - Shopping & Saving";
                    string msgsmscustomer = "Thank you for the Order. You Order Number Is " + objOrder.OrderId + " - Shopping & Saving";
                    SendSMSForNewOrder(AdminMobileNumber, msgsms);
                    SendSMSForNewOrder(objPlaceOrderVM.MobileNumber, msgsmscustomer);
                }
                else
                {
                    decimal amountdue = 0;
                    Razorpay.Api.Payment objpymn = new Razorpay.Api.Payment().Fetch(objPlaceOrderVM.razorpay_payment_id);
                    if (objpymn != null)
                    {
                        if (objpymn["status"] != null && Convert.ToString(objpymn["status"]) == "captured")
                        {
                            List<CartVM> lstCartItems = new List<CartVM>();
                            if (objPlaceOrderVM.ordertype == "1")
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
                                                    Price = RoleId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
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
                                                    Price = RoleId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                                    ItemImage = i.MainImage,
                                                    VariantId = crt.VariantItemId.Value,
                                                    Qty = crt.CartItemQty.Value,
                                                    VariantQtytxt = vr.UnitQty,
                                                    ItemSku = i.Sku,
                                                    GSTPer = i.GST_Per,
                                                    IGSTPer = i.IGST_Per
                                                }).OrderByDescending(x => x.CartId).ToList();
                            }
                            lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,RoleId,x.VariantId); });
                            // List<tbl_Cart> lstCarts = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                            string paymentmethod = objpymn["method"];
                            string paymentdetails = "";
                            if (paymentmethod == "upi")
                            {
                                paymentdetails = objpymn["vpa"];
                            }
                            else if (paymentmethod == "card")
                            {
                                string cardid = objpymn["card_id"];
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
                            decimal extraamount = 0;
                            if (objPlaceOrderVM.shippincode == "389001")
                            {
                                shippingcharge = Convert.ToDecimal(objPlaceOrderVM.shipamount);
                                extraamount = Convert.ToDecimal(objPlaceOrderVM.extraamount);
                                ordramt = ordramt - shippingcharge - extraamount;
                            }
                            List<string> lstpymenymthod = new List<string>();
                            if (amtwallet > 0)
                            {
                                lstpymenymthod.Add("Wallet");
                            }
                            if (amtcredit > 0)
                            {
                                lstpymenymthod.Add("Credit");
                            }

                            if (amtonline > 0)
                            {
                                lstpymenymthod.Add(paymentmethod);
                            }

                            if (objPlaceOrderVM.ordertype == "1")
                            {
                                amountdue = amtcredit;
                            }
                            else
                            {
                                decimal advncpay = 0;
                                if (!string.IsNullOrEmpty(objPlaceOrderVM.advanceamtpay))
                                {
                                    advncpay = Convert.ToDecimal(objPlaceOrderVM.advanceamtpay);
                                    decimal totlordewithship = ordramt + shippingcharge + extraamount;
                                    decimal remaingammt = totlordewithship - advncpay;
                                    amountdue = remaingammt + amtcredit;
                                }
                            }
                            paymentmethod = string.Join(",", lstpymenymthod);
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
                            objOrder.AmountDue = amountdue;
                            objOrder.InvoiceNo = Invno;
                            objOrder.InvoiceYear = year + "-" + toyear;
                            objOrder.RazorpayOrderId = "";
                            objOrder.RazorpayPaymentId = objPlaceOrderVM.razorpay_payment_id;
                            objOrder.RazorSignature = "";
                            objOrder.OrderType = Convert.ToInt32(objPlaceOrderVM.ordertype);
                            objOrder.IsCashOnDelivery = false;
                            objOrder.WalletAmountUsed = amtwallet;
                            objOrder.CreditAmountUsed = amtcredit;
                            objOrder.AmountByRazorPay = amtonline;
                            objOrder.ExtraAmount = extraamount;
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
                                objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Payment By Wallet : Rs" + amtwallet, amtwallet,clientusrid, 0, DateTime.UtcNow, "Wallet Payment");
                            }
                            decimal amttTotlordepy = ordramt + shippingcharge + extraamount;
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
                                objPyment.RazorpayOrderId = "";
                                objPyment.RazorpayPaymentId = objPlaceOrderVM.razorpay_payment_id;
                                objPyment.RazorSignature = "";
                                objPyment.PaymentFor = "OrderPayment";
                                _db.tbl_PaymentHistory.Add(objPyment);
                                objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Order Online Payment : Rs" + amtonline, amtonline,clientusrid, 0, DateTime.UtcNow, "Online Payment");
                            }
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
                            decimal disc = 0;
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
                                            totalremining = totalremining - disc;
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
                                    objOrderItem.FinalItemPrice = AfterTax;
                                    objOrderItem.ItemStatus = Convert.ToInt32(OrderStatus.NewOrder); 
                                    _db.tbl_OrderItemDetails.Add(objOrderItem);
                                    _db.SaveChanges();
                                    objcmn.SaveTransaction(objCart.ItemId, objOrderItem.OrderDetailId, objOrderItem.OrderId.Value, "Order Placed for Item", objOrderItem.FinalItemPrice.Value,clientusrid, 0, DateTime.UtcNow, "New Order Item");
                                    if (objPlaceOrderVM.ordertype == "1")
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
                                    objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Points Used", TotalDiscount,clientusrid, 0, DateTime.UtcNow, "Points Used");
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
                                objotherdetails.ShipAddress = objPlaceOrderVM.shipaddress;
                                objotherdetails.ShipCity = objPlaceOrderVM.shipcity;
                                objotherdetails.ShipFirstName = objPlaceOrderVM.shipfirstname;
                                objotherdetails.ShipLastName = objPlaceOrderVM.shiplastname;
                                objotherdetails.ShipPhoneNumber = objPlaceOrderVM.shipphone;
                                objotherdetails.ShipPostalcode = objPlaceOrderVM.shippincode;
                                objotherdetails.ShipState = objPlaceOrderVM.shipstate;
                                objotherdetails.ShipEmail = objPlaceOrderVM.shipemailaddress;
                                _db.SaveChanges();
                                if (amtcredit > 0)
                                {
                                    objcmn.SaveTransaction(0, 0, objOrder.OrderId, "Payment By Credit Used : Rs" + amtcredit, amtcredit,clientusrid, 0, DateTime.UtcNow, "Credit Used Payment");
                                }
                            }
                            string orderid = clsCommon.EncryptString(objOrder.OrderId.ToString());
                            response.Data = "Success^" + orderid;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string AdminMobileNumber = objGensetting.AdminSMSNumber;
                            string msgsms = "New Order Received - Order No " + objOrder.OrderId + " - Shopping & Saving";
                            string msgsmscustomer = "Thank You For The Order. Your Order Number Is " + objOrder.OrderId + " - Shopping & Saving";
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

        public decimal GetPriceGenral(long Itemid, decimal price,long RoleId, long VariantId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                if (RoleId == 1)
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

        public decimal GetVarintPrc(long VariantId, decimal Price)
        {
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
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
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
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
            string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
            string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
            string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
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

        [Route("GetSecondCartCheckOutDetails"), HttpPost]
        public ResponseDataModel<CheckoutGenVM> GetSecondCartCheckOutDetails(GeneralVM objGen)
        {
            ResponseDataModel<CheckoutGenVM> response = new ResponseDataModel<CheckoutGenVM>();
            CheckoutGenVM objChkout = new CheckoutGenVM();
            try
            {
                string type = objGen.CheckoutType;
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoledId = Convert.ToInt64(objGen.RoleId);
                List<CartVM> lstCartItems = new List<CartVM>();
                decimal ShippingChargeTotal = 0;
                decimal Ordetotlyearly = 0;
                decimal AdvancePaymentAmt = 0;
                decimal TotalDiscount = 0;
                decimal TotalOrder = 0;
                bool IsCashOrd = false;
          
                if (UserId > 0)
                {
                    long ClientUserId = Convert.ToInt64(UserId);
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.ClientUserId == ClientUserId && crt.IsCashonDelivery == IsCashOrd
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = RoledId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                        ItemImage = i.MainImage,
                                        VariantId = crt.VariantItemId.Value,
                                        VariantQtytxt = vr.UnitQty,
                                        Qty = crt.CartItemQty.Value,
                                        ShippingCharge = i.ShippingCharge.HasValue ? i.ShippingCharge.Value : 0,
                                        GSTPer = i.GST_Per,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false,
                                        AdvncePayPer = i.PayAdvancePer.HasValue ? i.PayAdvancePer.Value : 0
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price, RoledId, x.VariantId); });
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
                                objInvItm.AdvncePayAMt = Math.Round((AfterTax * objcr.AdvncePayPer) / 100, 2);
                                AdvancePaymentAmt = objInvItm.AdvncePayAMt + AdvancePaymentAmt;
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
                                objInvItm.AdvncePayAMt = Math.Round((AfterTax * objcr.AdvncePayPer) / 100, 2);
                                AdvancePaymentAmt = objInvItm.AdvncePayAMt + AdvancePaymentAmt;
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

                var objtbl_ExtraAmount = _db.tbl_ExtraAmount.Where(o => o.AmountFrom <= TotalOrder && o.AmountTo >= TotalOrder).FirstOrDefault();
                objChkout.ExtraAmount = 0;
                if (objtbl_ExtraAmount != null)
                {
                    objChkout.ExtraAmount = objtbl_ExtraAmount.ExtraAmount.Value;
                }
                decimal WalletAmt = 0;
                var objuserclient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == UserId).FirstOrDefault();
                if (objuserclient != null)
                {
                    WalletAmt = objuserclient.WalletAmt.HasValue ? objuserclient.WalletAmt.Value : 0;
                }
                //_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == clsClientSession.UserID).FirstOrDefault();
                objChkout.WalletAmount = WalletAmt;
                objChkout.IsCashOnDelivery = IsCashOrd;
                objChkout.YearlyOrderPlaced = Ordetotlyearly;
                objChkout.CashOrderAmtMax = objGenralsetting.CashLimitPerOrder.Value;
                objChkout.CashOrderAmtYerly = objGenralsetting.CashLimitPerYear.Value;
                objChkout.AdvancePaymentAmt = AdvancePaymentAmt;
                response.Data = objChkout;
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