using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class CartController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public CartController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Cart
        public ActionResult Index()
        {
            List<CartVM> lstCartItems = new List<CartVM>();

            try
            {          
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
                                    where crt.ClientUserId == ClientUserId 
                                 select new CartVM
                                 {
                                     CartId = crt.Cart_Id,
                                     ItemName = i.ItemName,
                                     ItemId = i.ProductItemId,
                                     //Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,  
                                     VariantQtytxt = vr.UnitQty,
                                     ItemImage = i.MainImage,
                                     Qty = crt.CartItemQty.Value,
                                     IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false,
                                     VariantId = crt.VariantItemId.Value,
                                     Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                 }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,x.VariantId);x.StockQty = RemainingStock(x.ItemId);});
                }
                else
                {
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.CartSessionId == cookiesessionval
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        //Price = i.CustomerPrice,
                                        VariantQtytxt = vr.UnitQty,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false,
                                        VariantId = crt.VariantItemId.Value,
                                        Price = vr.CustomerPrice.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price, x.VariantId); x.StockQty = RemainingStock(x.ItemId);});
                }
                   
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            ViewData["CashCartItems"] = lstCartItems.Where(o => o.IsCashonDelivery == true).ToList();
            ViewData["OnlinePaymentCartItems"] = lstCartItems.Where(o => o.IsCashonDelivery == false).ToList();
            return View();
        }

        [HttpPost]
        public string AddtoCart(long VarintId,long ItemId, long Qty,string IsCash)
        {
            string ReturnMessage = "";
            bool isOutofStock = false;
            bool IsCashOrdr = false;
            if(IsCash == "true")
            {
                IsCashOrdr = true;
            }
            try
            {
                int TotalStk = ItemStock(ItemId);
                int TotalSold = SoldItems(ItemId);
                int InStock = TotalStk - TotalSold;
                string GuidNew = Guid.NewGuid().ToString();
                string cookiesessionval = "";
                if (Request.Cookies["sessionkeyval"] != null)
                {
                    cookiesessionval = Request.Cookies["sessionkeyval"].Value;
                }
                else
                {
                    Response.Cookies["sessionkeyval"].Value = GuidNew;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);                    
                }
                if (clsClientSession.UserID == 0)
                {
                    var cartlist = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                    if(cartlist != null && cartlist.Count() > 0)
                    {
                        //var crtobj = cartlist.Where(o => o.CartItemId == ItemId).FirstOrDefault();
                        var lstcrt = cartlist.Where(o => o.CartItemId == ItemId).ToList();
                        if(lstcrt != null && lstcrt.Count() > 0)
                        {
                            decimal Qtnty = GetVarintQnty(VarintId);
                            decimal TotlQty = (lstcrt.Sum(x => x.CartItemQty).Value + Qty) *Qtnty;
                            var crtobj1 = lstcrt.Where(o => o.VariantItemId == VarintId && o.IsCashonDelivery == IsCashOrdr).FirstOrDefault();
                            if(crtobj1 != null)
                            {
                                crtobj1.CartItemQty = crtobj1.CartItemQty + Qty;
                            }
                            else
                            {
                                tbl_Cart crtobj = new tbl_Cart();
                                crtobj.CartItemId = ItemId;
                                crtobj.CartItemQty = Qty;
                                crtobj.CartSessionId = cookiesessionval;
                                crtobj.ClientUserId = 0;
                                crtobj.VariantItemId = VarintId;
                                crtobj.IsCashonDelivery = IsCashOrdr;
                                crtobj.CreatedDate = DateTime.Now;
                                _db.tbl_Cart.Add(crtobj);
                            }
                            if (Convert.ToDecimal(InStock) < TotlQty)
                            {
                                isOutofStock = true;
                            }
                        }
                        else
                        {
                            decimal Qtnty = GetVarintQnty(VarintId);
                            tbl_Cart crtobj = new tbl_Cart();
                            crtobj.CartItemId = ItemId;
                            crtobj.CartItemQty = Qty;
                            crtobj.CartSessionId = cookiesessionval;
                            crtobj.ClientUserId = 0;
                            crtobj.IsCashonDelivery = IsCashOrdr;
                            crtobj.VariantItemId = VarintId;
                            crtobj.CreatedDate = DateTime.Now;
                            _db.tbl_Cart.Add(crtobj);
                            if (InStock < (crtobj.CartItemQty * Qtnty))
                            {
                                isOutofStock = true;
                            }
                        }             
                    }
                    else
                    {
                        tbl_Cart crtobj = new tbl_Cart();
                        crtobj.CartItemId = ItemId;
                        crtobj.CartItemQty = Qty;
                        crtobj.CartSessionId = cookiesessionval;
                        crtobj.ClientUserId = 0;
                        crtobj.VariantItemId = VarintId;
                        crtobj.CreatedDate = DateTime.Now;
                        crtobj.IsCashonDelivery = IsCashOrdr;
                        _db.tbl_Cart.Add(crtobj);
                        decimal Qtnty = GetVarintQnty(VarintId);
                        if (InStock < (crtobj.CartItemQty * Qtnty))
                        {
                            isOutofStock = true;
                        }

                    }
                  
                }
                else
                {
                    long clientusrid = Convert.ToInt64(clsClientSession.UserID);
                    var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                    if(cartlist != null && cartlist.Count() > 0)
                    {
                        var lstcrt = cartlist.Where(o => o.CartItemId == ItemId).ToList();
                        if (lstcrt != null && lstcrt.Count() > 0)
                        {                            
                            decimal Qtnty = GetVarintQnty(VarintId);
                            decimal TotlQty = (lstcrt.Sum(x => x.CartItemQty).Value + Qty) * Qtnty;
                            var crtobj1 = lstcrt.Where(o => o.VariantItemId == VarintId && o.IsCashonDelivery == IsCashOrdr).FirstOrDefault();
                            if (crtobj1 != null)
                            {
                                crtobj1.CartItemQty = crtobj1.CartItemQty + Qty;
                            }
                            else
                            {
                                tbl_Cart crtobj11 = new tbl_Cart();
                                crtobj11.CartItemId = ItemId;
                                crtobj11.CartItemQty = Qty;
                                crtobj11.CartSessionId = cartlist.FirstOrDefault().CartSessionId;
                                crtobj11.ClientUserId = clientusrid;
                                crtobj11.IsCashonDelivery = IsCashOrdr;
                                crtobj11.CreatedDate = DateTime.Now;
                                crtobj11.VariantItemId = VarintId;
                                _db.tbl_Cart.Add(crtobj11);
                            }
                            if (Convert.ToDecimal(InStock) < TotlQty)
                            {
                                isOutofStock = true;
                            }
                        }
                        else
                        {
                            tbl_Cart crtobj2 = new tbl_Cart();
                            crtobj2.CartItemId = ItemId;
                            crtobj2.CartItemQty = Qty;
                            crtobj2.CartSessionId = cartlist.FirstOrDefault().CartSessionId;
                            crtobj2.ClientUserId = clientusrid;
                            crtobj2.IsCashonDelivery = IsCashOrdr;
                            crtobj2.VariantItemId = VarintId;
                            crtobj2.CreatedDate = DateTime.Now;
                            _db.tbl_Cart.Add(crtobj2);
                            decimal Qtnty = GetVarintQnty(VarintId);
                            if (InStock < (crtobj2.CartItemQty * Qtnty))
                            {
                                isOutofStock = true;
                            }
                        }
                    }
                    else
                    {
                        var crtobj = new tbl_Cart();
                        crtobj.CartItemId = ItemId;
                        crtobj.CartItemQty = Qty;
                        crtobj.CartSessionId = cookiesessionval;
                        crtobj.ClientUserId = clientusrid;
                        crtobj.IsCashonDelivery = IsCashOrdr;
                        crtobj.VariantItemId = VarintId;
                        crtobj.CreatedDate = DateTime.Now;
                        _db.tbl_Cart.Add(crtobj);
                        decimal Qtnty = GetVarintQnty(VarintId);
                        if (InStock < (crtobj.CartItemQty * Qtnty))
                        {
                            isOutofStock = true;
                        }

                    }
                }
                if (isOutofStock == false)
                {
                    _db.SaveChanges();
                    ReturnMessage = "Success";
                }
                else
                {
                    ReturnMessage = "OutofStock";
                }               
                
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public void UpdatCarts()
        {
            
            string GuidNew = Guid.NewGuid().ToString();
            string cookiesessionval = "";
            if (Request.Cookies["sessionkeyval"] != null)
            {
                cookiesessionval = Request.Cookies["sessionkeyval"].Value;
            }
            else
            {
                Response.Cookies["sessionkeyval"].Value = GuidNew;
                Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
            }
            if (clsClientSession.UserID > 0)
            {
                if(string.IsNullOrEmpty(cookiesessionval))
                {
                    cookiesessionval = GuidNew;
                }
                long clientusrid = Convert.ToInt64(clsClientSession.UserID);
                var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();             
                if (cartlist != null && cartlist.Count() > 0)
                {
                    string sessioncrtid = cartlist.FirstOrDefault().CartSessionId;
                    Response.Cookies["sessionkeyval"].Value = sessioncrtid;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                    var cartlistsessions = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval).ToList(); 
                    if(cartlistsessions != null && cartlistsessions.Count() > 0)
                    {
                        foreach(var obj in cartlistsessions)
                        {
                           var objcrtsession = cartlist.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                           if(objcrtsession != null)
                            {
                                objcrtsession.CartItemQty = objcrtsession.CartItemQty + obj.CartItemQty;
                                _db.tbl_Cart.Remove(obj);
                            }
                            else
                            {
                               var crtobj1 = new tbl_Cart();
                                crtobj1.CartItemId = obj.CartItemId;
                                crtobj1.CartItemQty = obj.CartItemQty;
                                crtobj1.CartSessionId = sessioncrtid;
                                crtobj1.ClientUserId = clientusrid;
                                crtobj1.CreatedDate = DateTime.Now;
                                _db.tbl_Cart.Add(crtobj1);
                                _db.tbl_Cart.Remove(obj);
                            }
                            _db.SaveChanges();
                        }
                    }                
                }

            }         
        }

        [HttpPost]
        public string Removecartitem(long CartItemId)
        {
            string ReturnMessage = "";

            try
            {
                var objCart = _db.tbl_Cart.Where(o => o.Cart_Id == CartItemId).FirstOrDefault();
                _db.tbl_Cart.Remove(objCart);
                _db.SaveChanges();                
                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }
        
        public ActionResult CartItemsListTop()
        {
            List<CartVM> lstCartItems = new List<CartVM>();
            try
            {
                
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
                                    where crt.ClientUserId == ClientUserId
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        VariantQtytxt = vr.UnitQty,
                                        //Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false,
                                        VariantId = crt.VariantItemId.Value,
                                        Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,x.VariantId); });
                }
                else
                {
                    lstCartItems = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                    join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                    where crt.CartSessionId == cookiesessionval
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        VariantQtytxt = vr.UnitQty,
                                        // Price = i.CustomerPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false,
                                        VariantId = crt.VariantItemId.Value,
                                        Price = vr.CustomerPrice.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price,x.VariantId); });
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            ViewData["CashCartItems"] = lstCartItems.Where(o => o.IsCashonDelivery == true).ToList();
            ViewData["OnlinePaymentCartItems"] = lstCartItems.Where(o => o.IsCashonDelivery == false).ToList();
            return PartialView("~/Areas/Client/Views/Cart/_CartItemsTop.cshtml");
        }

        [HttpPost]
        public string UpdateCart(string CartItems)
        {
            string ReturnMessage = "";

            try
            {
                if(!string.IsNullOrEmpty(CartItems))
                {
                   string[] arrycrtitems =  CartItems.Split('|');
                   if(arrycrtitems != null && arrycrtitems.Count() > 0)
                   {
                        foreach(string str in arrycrtitems)
                        {
                           string[] arrstr = str.Split('^');
                            if(arrstr != null && arrstr.Count() > 0)
                            {
                                long cartids = Convert.ToInt64(arrstr[0]);
                                if (!string.IsNullOrEmpty(arrstr[1].ToString()))
                                {
                                    var objCart = _db.tbl_Cart.Where(o => o.Cart_Id == cartids).FirstOrDefault();
                                    objCart.CartItemQty = Convert.ToInt32(arrstr[1]);
                                    _db.SaveChanges();
                                }
                            }
                        }
                   }
                }           
                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public decimal GetOfferPrice(long Itemid, decimal price,long VariantId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {                 
                return GetVarintPrc(VariantId, objItem.OfferPrice);
            }

            return price;
        }

        public decimal GetDistributorOfferPrice(long Itemid, decimal price,long VariantId)
        {
       
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return GetVarintPrc(VariantId, objItem.OfferPriceforDistributor.Value);                
            }

            return price;
        }

        public decimal GetPriceGenral(long Itemid, decimal price,long VariantId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                if(clsClientSession.RoleID == 1)
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

        public int ItemStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            if(TotalStock == null)
            {
                TotalStock = 0;
            }
            return Convert.ToInt32(TotalStock);
        }
        public int SoldItems(long ItemId)
        {
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.QtyUsed.Value);
            if(TotalSold == null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
        }
        public int RemainingStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.QtyUsed.Value);
            if(TotalStock == null)
            {
                TotalStock = 0;
            }
            if(TotalSold == null)
            {
                TotalSold = 0;
            }
            long remiaing = TotalStock.Value - TotalSold.Value;
            return Convert.ToInt32(remiaing);
        }

        public ActionResult SecondCart()
        {
            List<CartVM> lstCartItems = new List<CartVM>();

            try
            {
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
                                        VariantQtytxt = vr.UnitQty,
                                        ItemId = i.ProductItemId,
                                        VariantId = crt.VariantItemId.Value,
                                        //Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,x.VariantId); x.StockQty = RemainingStock(x.ItemId); });
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
                                        VariantQtytxt = vr.UnitQty,
                                        VariantId = crt.VariantItemId.Value,
                                        //Price = i.CustomerPrice,
                                        Price = vr.CustomerPrice.Value,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price,x.VariantId); x.StockQty = RemainingStock(x.ItemId); });
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstCartItems);
        }

        [HttpPost]
        public string AddtoSecondCart(long VarintId,long ItemId, long Qty)
        {
            string ReturnMessage = "";
            bool isOutofStock = false;
            try
            {
                int TotalStk = ItemStock(ItemId);
                int TotalSold = SoldItems(ItemId);
                int InStock = TotalStk - TotalSold;
                string GuidNew = Guid.NewGuid().ToString();
                string cookiesessionval = "";
                if (Request.Cookies["sessionkeyval"] != null)
                {
                    cookiesessionval = Request.Cookies["sessionkeyval"].Value;
                }
                else
                {
                    Response.Cookies["sessionkeyval"].Value = GuidNew;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                }
                if (clsClientSession.UserID == 0)
                {
                    var cartlist = _db.tbl_SecondCart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                    if (cartlist != null && cartlist.Count() > 0)
                    {
                        var crtobj = cartlist.Where(o => o.CartItemId == ItemId && o.VariantItemId == VarintId).FirstOrDefault();
                        if (crtobj != null)
                        {
                            crtobj.CartItemQty = crtobj.CartItemQty + Qty;
                        }
                        else
                        {
                            crtobj = new tbl_SecondCart();
                            crtobj.CartItemId = ItemId;
                            crtobj.CartItemQty = Qty;
                            crtobj.CartSessionId = cookiesessionval;
                            crtobj.ClientUserId = 0;
                            crtobj.VariantItemId = VarintId;
                            crtobj.CreatedDate = DateTime.Now;
                            _db.tbl_SecondCart.Add(crtobj);                            
                        }
                    }
                    else
                    {
                        tbl_SecondCart crtobj = new tbl_SecondCart();
                        crtobj.CartItemId = ItemId;
                        crtobj.CartItemQty = Qty;
                        crtobj.CartSessionId = cookiesessionval;
                        crtobj.ClientUserId = 0;
                        crtobj.VariantItemId = VarintId;
                        crtobj.CreatedDate = DateTime.Now;
                        _db.tbl_SecondCart.Add(crtobj);
                     }

                }
                else
                {
                    long clientusrid = Convert.ToInt64(clsClientSession.UserID);
                    var cartlist = _db.tbl_SecondCart.Where(o => o.ClientUserId == clientusrid).ToList();
                    if (cartlist != null && cartlist.Count() > 0)
                    {
                        var crtobj = cartlist.Where(o => o.CartItemId == ItemId && o.VariantItemId == VarintId).FirstOrDefault();
                        if (crtobj != null)
                        {
                            crtobj.CartItemQty = crtobj.CartItemQty + Qty;
                          
                        }
                        else
                        {
                            crtobj = new tbl_SecondCart();
                            crtobj.CartItemId = ItemId;
                            crtobj.CartItemQty = Qty;
                            crtobj.CartSessionId = cartlist.FirstOrDefault().CartSessionId;
                            crtobj.ClientUserId = clientusrid;
                            crtobj.VariantItemId = VarintId;
                            crtobj.CreatedDate = DateTime.Now;
                            _db.tbl_SecondCart.Add(crtobj);
                         
                        }

                    }
                    else
                    {
                        var crtobj = new tbl_SecondCart();
                        crtobj.CartItemId = ItemId;
                        crtobj.CartItemQty = Qty;
                        crtobj.CartSessionId = cookiesessionval;
                        crtobj.ClientUserId = clientusrid;
                        crtobj.CreatedDate = DateTime.Now;
                        crtobj.VariantItemId = VarintId;
                        _db.tbl_SecondCart.Add(crtobj);                      

                    }
                }
                if (isOutofStock == false)
                {
                    _db.SaveChanges();
                    ReturnMessage = "Success";
                }
                else
                {
                    ReturnMessage = "OutofStock";
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string UpdateSecondCart(string CartItems)
        {
            string ReturnMessage = "";

            try
            {
                if (!string.IsNullOrEmpty(CartItems))
                {
                    string[] arrycrtitems = CartItems.Split('|');
                    if (arrycrtitems != null && arrycrtitems.Count() > 0)
                    {
                        foreach (string str in arrycrtitems)
                        {
                            string[] arrstr = str.Split('^');
                            if (arrstr != null && arrstr.Count() > 0)
                            {
                                long cartids = Convert.ToInt64(arrstr[0]);
                                if (!string.IsNullOrEmpty(arrstr[1].ToString()))
                                {
                                    var objCart = _db.tbl_SecondCart.Where(o => o.SecondCartId == cartids).FirstOrDefault();
                                    objCart.CartItemQty = Convert.ToInt32(arrstr[1]);
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string Removesecondcartitem(long CartItemId)
        {
            string ReturnMessage = "";

            try
            {
                var objCart = _db.tbl_SecondCart.Where(o => o.SecondCartId == CartItemId).FirstOrDefault();
                _db.tbl_SecondCart.Remove(objCart);
                _db.SaveChanges();
                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult SecondCartItemsListTop()
        {
            List<CartVM> lstCartItems = new List<CartVM>();
            try
            {

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
                                        VariantQtytxt = vr.UnitQty,
                                        //Price = clsClientSession.RoleID == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        VariantId = crt.VariantItemId.Value,
                                        Price = clsClientSession.RoleID == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value
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
                                        VariantQtytxt = vr.UnitQty,
                                        //Price = i.CustomerPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        VariantId = crt.VariantItemId.Value,
                                        Price = vr.CustomerPrice.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartItems.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price,x.VariantId); });
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return PartialView("~/Areas/Client/Views/Cart/_SecondCartItemsTop.cshtml", lstCartItems);
        }

        public decimal GetVarintPrc(long VariantId,decimal Price)
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
    }
}