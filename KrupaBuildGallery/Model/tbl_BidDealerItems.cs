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
    
    public partial class tbl_BidDealerItems
    {
        public long Pk_DealerItem_Id { get; set; }
        public Nullable<long> Fk_ItemId { get; set; }
        public Nullable<long> Fk_PurchaseDealerId { get; set; }
    }
}