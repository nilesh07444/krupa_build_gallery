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
    
    public partial class tbl_Bids
    {
        public long Pk_Bid_id { get; set; }
        public Nullable<long> ItemId { get; set; }
        public Nullable<long> Qty { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> BidStatus { get; set; }
        public Nullable<long> IsDeleted { get; set; }
        public Nullable<System.DateTime> BidDate { get; set; }
        public Nullable<int> BidNo { get; set; }
        public string BidYear { get; set; }
    }
}