using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class OrderItemsVM
    {
        public long OrderItemId { get; set; }
        public long ProductItemId { get; set; }
        public long OrderId { get; set; }
        public string ItemName { get; set; }
        public long Qty { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public decimal GSTAmt { get; set; }
        public decimal IGSTAmt { get; set; }
        public string ItemImg { get; set; }
        public decimal GST_Per { get; set; }
        public string HSNCode { get;set; }
        public decimal Discount { get; set; }
        public int ItemStatus { get; set; }
        public string ItemStatustxt { get; set; }
        public bool IsReturnable { get; set; }
        public string VariantQtytxt { get; set; }
        public decimal FinalAmt { get; set; }
        public decimal ShipingChargeOf1Item { get; set; }
        public string DeliveryPersonName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime modifieddate { get; set; }
        public decimal MRPPrice { get; set; }
        public string InvoiceYear { get; set; }
        public long InvoiceNo { get; set; }
        public long ClientUserId { get; set; }
    }
}