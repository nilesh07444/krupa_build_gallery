//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KrupaBuildGallery.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_ProductItems
    {
        public long ProductItemId { get; set; }
        public long CategoryId { get; set; }
        public long ProductId { get; set; }
        public Nullable<long> SubProductId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal MRPPrice { get; set; }
        public decimal DistributorPrice { get; set; }
        public decimal CustomerPrice { get; set; }
        public decimal GST_Per { get; set; }
        public decimal IGST_Per { get; set; }
        public Nullable<decimal> Cess { get; set; }
        public string MainImage { get; set; }
        public string Notification { get; set; }
        public string Sku { get; set; }
        public bool IsPopularProduct { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
