using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PurchaseItemVM
    {
        public long PurchaseItemId { get; set; }
        public long CategoryId { get; set; }
        public long ProductId { get; set; }
        public long SubProductId { get; set; }
        public long ItemId { get; set; }
        public long VariantId { get; set; }
        public long Qty { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal LabourCharge { get; set; }
        public decimal ExtraPlusMinus { get; set; }
        public decimal Total { get; set; }
        public decimal TradeDiscount { get; set; }
        public decimal CashDiscount { get; set; }
        public decimal ExtraPlusMinus2 { get; set; }
        public decimal BillAmount { get; set; }
        public decimal IGST { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal FinalAmount { get; set; }
        public long StockVariantId { get; set; }
        public long StockId { get; set; }
        public long StockReportId { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal CGSTAmount { get; set; }
        public long Fk_PurchaseId { get; set; }
        public string ItemName { get; set; }
        public string VariantName { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string SubProductName { get; set; }

    }
}