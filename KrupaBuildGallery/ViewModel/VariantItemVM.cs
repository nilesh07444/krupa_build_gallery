using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class VariantItemVM
    {
        public int VariantItemId { get; set; }
        public decimal PricePercentage { get; set; }
        public string UnitQtyText { get; set; }
        public string UnitQtys { get; set; }
        public bool IsActive { get; set; }
        public decimal MRPPrice { get; set; }
        public decimal CustomerPrice { get; set; }
        public decimal DistributorPrice { get; set; }
        public string VariantImg { get; set; }
        public long MinQty { get; set; }
        public string GST { get; set; }
    }
}