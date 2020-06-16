using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class CartController :  ApiController
    {
        krupagallarydbEntities _db;
        public CartController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("GetCartList"), HttpPost]
        public ResponseDataModel<List<CartVM>> GetCartList(GeneralVM objGen)
        {
            ResponseDataModel<List<CartVM>> response = new ResponseDataModel<List<CartVM>>();
            List<CartVM> lstCartList = new List<CartVM>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
               // long CategoryId = Convert.ToInt64(objGen.CategoryId);
                string GuidNew = Guid.NewGuid().ToString();
                string cookiesessionval = "";
                if (!string.IsNullOrEmpty(objGen.SessionUniqueId))
                {
                    cookiesessionval = objGen.SessionUniqueId;
                }
              
                if (UserId > 0)
                {
                    long ClientUserId = Convert.ToInt64(UserId);
                    lstCartList = (from crt in _db.tbl_Cart
                                    join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                   join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                   where crt.ClientUserId == ClientUserId
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        //Price = RoleId == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        VariantQtytxt = vr.UnitQty,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value,
                                        IsCashonDelivery = crt.IsCashonDelivery.HasValue ? crt.IsCashonDelivery.Value : false,
                                        VariantId = crt.VariantItemId.Value,
                                        Price = RoleId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartList.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,RoleId,x.VariantId); x.StockQty = RemainingStock(x.ItemId); });
                }
                else
                {
                    lstCartList = (from crt in _db.tbl_Cart
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
                    lstCartList.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price,x.VariantId); x.StockQty = RemainingStock(x.ItemId); });
                }
                response.Data = lstCartList;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("AddToCart"), HttpPost]
        public ResponseDataModel<int> AddToCart(CartVM objcart)
        {
            ResponseDataModel<int> response = new ResponseDataModel<int>();
            int CartCount = 0;
            try
            {
                bool isOutofStock = false;
                bool IsCashOrdr = false;
                if (objcart.IsCash == "true")
                {
                    IsCashOrdr = true;
                }
                long ItemId = objcart.ItemId;
                long VarintId = objcart.VariantId;
                long UserId = Convert.ToInt64(objcart.ClientUserId);
                int TotalStk = ItemStock(ItemId);
                int TotalSold = SoldItems(ItemId);
                int InStock = TotalStk - TotalSold;
                long Qty = objcart.Qty; 
                string GuidNew = Guid.NewGuid().ToString();
                string cookiesessionval = "";
                if (!string.IsNullOrEmpty(objcart.SessionUniqueId))
                {
                    cookiesessionval = objcart.SessionUniqueId;
                }
                
                if (objcart.ClientUserId == 0)
                {
                    var cartlist = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                    if (cartlist != null && cartlist.Count() > 0)
                    {
                        //var crtobj = cartlist.Where(o => o.CartItemId == ItemId).FirstOrDefault();
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
                    long clientusrid = Convert.ToInt64(objcart.ClientUserId);
                    var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                    if (cartlist != null && cartlist.Count() > 0)
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
                    if (objcart.ClientUserId == 0)
                    {
                        var cartlist = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                        CartCount = cartlist.Count();
                    }
                    else
                    {
                        long clientusrid = Convert.ToInt64(objcart.ClientUserId);
                        var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                        CartCount = cartlist.Count();
                    }
                        
                }
                else
                {
                    response.AddError("Item Out of Stock.");                    
                }
                response.Data = CartCount;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("RemoveCartItem"), HttpPost]
        public ResponseDataModel<CartVM> RemoveCartItem(CartVM objcart)
        {
            ResponseDataModel<CartVM> response = new ResponseDataModel<CartVM>();
            CartVM objcart1 = new CartVM();
            try
            {
                long CartItemId = objcart.CartId;
                var objCart = _db.tbl_Cart.Where(o => o.Cart_Id == CartItemId).FirstOrDefault();
                _db.tbl_Cart.Remove(objCart);
                _db.SaveChanges();
                response.Data = objcart1;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("UpdateCartItems"), HttpPost]
        public ResponseDataModel<CartVM> UpdateCartItems(GeneralVM objGen)
        {
            ResponseDataModel<CartVM> response = new ResponseDataModel<CartVM>();
            CartVM objcart1 = new CartVM();
            try
            {
                string CartItems = objGen.StrCartItems;
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
                                    var objCart = _db.tbl_Cart.Where(o => o.Cart_Id == cartids).FirstOrDefault();
                                    objCart.CartItemQty = Convert.ToInt32(arrstr[1]);
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                response.Data = objcart1;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

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

        public decimal GetDistributorOfferPrice(long Itemid, decimal price, long VariantId)
        {
            var objItem = _db.tbl_Offers.Where(o => o.ProductItemId == Itemid && DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate).FirstOrDefault();
            if (objItem != null)
            {
                return GetVarintPrc(VariantId, objItem.OfferPriceforDistributor.Value);
            }

            return price;
        }

        public decimal GetPriceGenral(long Itemid, decimal price,long RoleId,long VariantId)
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

        public int ItemStock(long ItemId)
        {
            long? TotalStock = _db.tbl_ItemStocks.Where(o => o.IsActive == true && o.IsDelete == false && o.ProductItemId == ItemId).Sum(o => (long?)o.Qty);
            if (TotalStock == null)
            {
                TotalStock = 0;
            }
            return Convert.ToInt32(TotalStock);
        }
        public int SoldItems(long ItemId)
        {
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.QtyUsed.Value);
            if (TotalSold == null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
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

        [Route("UpdateCart"), HttpPost]
        public ResponseDataModel<int> UpdateCart(CartVM objcart)
        {
            ResponseDataModel<int> response = new ResponseDataModel<int>();
            int CartCount = 0;
            try
            {
                bool isOutofStock = false;
                long CartId = objcart.CartId;
                var objCrt =_db.tbl_Cart.Where(o => o.Cart_Id == CartId).FirstOrDefault();
                if(objCrt != null)
                {
                    long ItemId = objCrt.CartItemId.Value;
                    long UserId = Convert.ToInt64(objCrt.ClientUserId);
                    int TotalStk = ItemStock(ItemId);
                    int TotalSold = SoldItems(ItemId);
                    int InStock = TotalStk - TotalSold;
                    long Qty = objcart.Qty;

                    if (InStock < Qty)
                    {
                        isOutofStock = true;
                    }
                    else
                    {
                        objCrt.CartItemQty = Qty;
                        _db.SaveChanges();
                    }

                    if (isOutofStock == true)
                    {
                        response.AddError("Item Out of Stock.");
                    }
                }
                else
                {
                    response.AddError("Cart Item Not Exist");
                }              
              
                
                response.Data = CartCount;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

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

        [Route("GetSecondCartList"), HttpPost]
        public ResponseDataModel<List<CartVM>> GetSecondCartList(GeneralVM objGen)
        {
            ResponseDataModel<List<CartVM>> response = new ResponseDataModel<List<CartVM>>();
            List<CartVM> lstCartList = new List<CartVM>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);
                
                string GuidNew = Guid.NewGuid().ToString();
                string cookiesessionval = "";
                if (!string.IsNullOrEmpty(objGen.SessionUniqueId))
                {
                    cookiesessionval = objGen.SessionUniqueId;
                }

                if (UserId > 0)
                {
                    long ClientUserId = Convert.ToInt64(UserId);
                    lstCartList = (from crt in _db.tbl_SecondCart
                                   join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                   join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                   where crt.ClientUserId == ClientUserId
                                   select new CartVM
                                   {
                                       CartId = crt.SecondCartId,
                                       ItemName = i.ItemName,
                                       ItemId = i.ProductItemId,
                                       //Price = RoleId == 1 ? i.CustomerPrice : i.DistributorPrice,
                                       VariantQtytxt = vr.UnitQty,
                                       ItemImage = i.MainImage,
                                       Qty = crt.CartItemQty.Value,
                                       VariantId = crt.VariantItemId.Value,
                                       Price = RoleId == 1 ? vr.CustomerPrice.Value : vr.DistributorPrice.Value,
                                   }).OrderByDescending(x => x.CartId).ToList();
                    lstCartList.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price, RoleId, x.VariantId); x.StockQty = RemainingStock(x.ItemId); });
                }
                else
                {
                    lstCartList = (from crt in _db.tbl_SecondCart
                                   join i in _db.tbl_ProductItems on crt.CartItemId equals i.ProductItemId
                                   join vr in _db.tbl_ItemVariant on crt.VariantItemId equals vr.VariantItemId
                                   where crt.CartSessionId == cookiesessionval
                                   select new CartVM
                                   {
                                       CartId = crt.SecondCartId,
                                       ItemName = i.ItemName,
                                       ItemId = i.ProductItemId,
                                       //Price = i.CustomerPrice,
                                       VariantQtytxt = vr.UnitQty,
                                       ItemImage = i.MainImage,
                                       Qty = crt.CartItemQty.Value,
                                       VariantId = crt.VariantItemId.Value,
                                       Price = vr.CustomerPrice.Value
                                   }).OrderByDescending(x => x.CartId).ToList();
                    lstCartList.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price, x.VariantId); x.StockQty = RemainingStock(x.ItemId); });
                }
                response.Data = lstCartList;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("AddToSecondCart"), HttpPost]
        public ResponseDataModel<int> AddToSecondCart(CartVM objcart)
        {
            ResponseDataModel<int> response = new ResponseDataModel<int>();
            int CartCount = 0;
            try
            {
                bool isOutofStock = false;
                bool IsCashOrdr = false;              
                long ItemId = objcart.ItemId;
                long VarintId = objcart.VariantId;
                long UserId = Convert.ToInt64(objcart.ClientUserId);
                int TotalStk = ItemStock(ItemId);
                int TotalSold = SoldItems(ItemId);
                int InStock = TotalStk - TotalSold;
                long Qty = objcart.Qty;
                string GuidNew = Guid.NewGuid().ToString();
                string cookiesessionval = "";
                if (!string.IsNullOrEmpty(objcart.SessionUniqueId))
                {
                    cookiesessionval = objcart.SessionUniqueId;
                }

                if (objcart.ClientUserId == 0)
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
                    long clientusrid = Convert.ToInt64(objcart.ClientUserId);
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
                    if (objcart.ClientUserId == 0)
                    {
                        var cartlist = _db.tbl_SecondCart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                        CartCount = cartlist.Count();
                    }
                    else
                    {
                        long clientusrid = Convert.ToInt64(objcart.ClientUserId);
                        var cartlist = _db.tbl_SecondCart.Where(o => o.ClientUserId == clientusrid).ToList();
                        CartCount = cartlist.Count();
                    }

                }
                else
                {
                    response.AddError("Item Out of Stock.");
                }
                response.Data = CartCount;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("UpdateSecondCart"), HttpPost]
        public ResponseDataModel<int> UpdateSecondCart(CartVM objcart)
        {
            ResponseDataModel<int> response = new ResponseDataModel<int>();
            int CartCount = 0;
            try
            {
                bool isOutofStock = false;
                long CartId = objcart.CartId;
                var objCrt = _db.tbl_SecondCart.Where(o => o.SecondCartId == CartId).FirstOrDefault();
                if (objCrt != null)
                {
                    long ItemId = objCrt.CartItemId.Value;
                    long UserId = Convert.ToInt64(objCrt.ClientUserId);
                    int TotalStk = ItemStock(ItemId);
                    int TotalSold = SoldItems(ItemId);
                    int InStock = TotalStk - TotalSold;
                    long Qty = objcart.Qty;

                    //if (InStock < Qty)
                   // {
                       // isOutofStock = true;
                    //}
                   // else
                    //{
                        objCrt.CartItemQty = Qty;
                        _db.SaveChanges();
                   // }

                   // if (isOutofStock == true)
                   // {
                      // response.AddError("Item Out of Stock.");
                   // }
                }
                else
                {
                    response.AddError("Cart Item Not Exist");
                }


                response.Data = CartCount;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("RemoveSecondCartItem"), HttpPost]
        public ResponseDataModel<CartVM> RemoveSecondCartItem(CartVM objcart)
        {
            ResponseDataModel<CartVM> response = new ResponseDataModel<CartVM>();
            CartVM objcart1 = new CartVM();
            try
            {
                long CartItemId = objcart.CartId;
                var objCart = _db.tbl_SecondCart.Where(o => o.SecondCartId == CartItemId).FirstOrDefault();
                _db.tbl_SecondCart.Remove(objCart);
                _db.SaveChanges();
                response.Data = objcart1;
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