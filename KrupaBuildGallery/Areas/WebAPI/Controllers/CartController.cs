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
                long CategoryId = Convert.ToInt64(objGen.CategoryId);
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
                                    where crt.ClientUserId == ClientUserId
                                    select new CartVM
                                    {
                                        CartId = crt.Cart_Id,
                                        ItemName = i.ItemName,
                                        ItemId = i.ProductItemId,
                                        Price = RoleId == 1 ? i.CustomerPrice : i.DistributorPrice,
                                        ItemImage = i.MainImage,
                                        Qty = crt.CartItemQty.Value
                                    }).OrderByDescending(x => x.CartId).ToList();
                    lstCartList.ForEach(x => { x.Price = GetPriceGenral(x.ItemId, x.Price,RoleId); x.StockQty = RemainingStock(x.ItemId); });
                }
                else
                {
                    lstCartList = (from crt in _db.tbl_Cart
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
                    lstCartList.ForEach(x => { x.Price = GetOfferPrice(x.ItemId, x.Price); x.StockQty = RemainingStock(x.ItemId); });
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
                long ItemId = objcart.ItemId;
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
                        var crtobj = cartlist.Where(o => o.CartItemId == ItemId).FirstOrDefault();
                        if (crtobj != null)
                        {
                            crtobj.CartItemQty = crtobj.CartItemQty + Qty;

                            if (InStock < crtobj.CartItemQty)
                            {
                                isOutofStock = true;
                            }
                        }
                        else
                        {
                            crtobj = new tbl_Cart();
                            crtobj.CartItemId = ItemId;
                            crtobj.CartItemQty = Qty;
                            crtobj.CartSessionId = cookiesessionval;
                            crtobj.ClientUserId = 0;
                            crtobj.CreatedDate = DateTime.Now;
                            _db.tbl_Cart.Add(crtobj);
                            if (InStock < crtobj.CartItemQty)
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
                        crtobj.CreatedDate = DateTime.Now;
                        _db.tbl_Cart.Add(crtobj);
                        if (InStock < crtobj.CartItemQty)
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
                        var crtobj = cartlist.Where(o => o.CartItemId == ItemId).FirstOrDefault();
                        if (crtobj != null)
                        {
                            crtobj.CartItemQty = crtobj.CartItemQty + Qty;
                            if (InStock < crtobj.CartItemQty)
                            {
                                isOutofStock = true;
                            }
                        }
                        else
                        {
                            crtobj = new tbl_Cart();
                            crtobj.CartItemId = ItemId;
                            crtobj.CartItemQty = Qty;
                            crtobj.CartSessionId = cartlist.FirstOrDefault().CartSessionId;
                            crtobj.ClientUserId = clientusrid;
                            crtobj.CreatedDate = DateTime.Now;
                            _db.tbl_Cart.Add(crtobj);
                            if (InStock < crtobj.CartItemQty)
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
                        crtobj.CreatedDate = DateTime.Now;
                        _db.tbl_Cart.Add(crtobj);
                        if (InStock < crtobj.CartItemQty)
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
            long? TotalSold = _db.tbl_OrderItemDetails.Where(o => o.ProductItemId == ItemId && o.IsDelete == false).Sum(o => (long?)o.Qty.Value);
            if (TotalSold == null)
            {
                TotalSold = 0;
            }
            return Convert.ToInt32(TotalSold);
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
    }
}