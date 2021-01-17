using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PurchaseGSTVM
    {
        public DateTime PurchaseDate { get; set; }
        public long DealerId { get; set; }
        public string DealerCode { get; set; }
        public string BillNumber { get; set; }
        public string GSTNumber { get; set; }
        public string FirmName { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal TotalIGST { get; set; }
        public decimal TotalCGST { get; set; }
        public decimal TotalSGST { get; set; }
        public GSTVM FreeGST { get; set; }
        public GSTVM GST5 { get; set; }
        public GSTVM GST12 { get; set; }
        public GSTVM GST18 { get; set; }
        public GSTVM GST28 { get; set; }
    }
}