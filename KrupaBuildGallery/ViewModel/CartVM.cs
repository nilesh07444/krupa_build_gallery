using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class CartVM
    {
        public long CartId { get; set; }
        public string ItemName { get;set; }
        public long Qty { get; set; }
        public long ItemId { get; set; }        
        public decimal Price { get; set; }
        public string ItemImage { get; set; }
        public string ItemSku { get; set; }        
        public decimal GSTPer { get; set; }
        public decimal IGSTPer { get; set; }
        public int StockQty { get; set; }
        public decimal ShippingCharge { get; set; }
        public long ClientUserId { get; set; }
        public string SessionUniqueId { get; set; }
        public int CartType { get; set; }
        public decimal AdvncePayPer { get; set; }
        public decimal ItmAdvncePayAmt { get; set; }
        public bool IsCashonDelivery { get; set; }
    }
}