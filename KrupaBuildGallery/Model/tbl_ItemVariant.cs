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
    
    public partial class tbl_ItemVariant
    {
        public long VariantItemId { get; set; }
        public Nullable<long> ProductItemId { get; set; }
        public Nullable<decimal> PricePecentage { get; set; }
        public string UnitQty { get; set; }
        public Nullable<decimal> CustomerPrice { get; set; }
        public Nullable<decimal> DistributorPrice { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
