using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class InvoiceItemVM
    {
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public decimal BasicAmount { get; set; } 
        public decimal ItemAmount { get; set; }
        public decimal GSTPer { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal Discount { get; set; }        
        public long ItemId { get; set; }
        public decimal beforetaxamount { get; set; }
        public decimal AdvncePayAMt { get; set; }
    }
}