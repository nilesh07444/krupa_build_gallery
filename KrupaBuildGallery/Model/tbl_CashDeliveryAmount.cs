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
    
    public partial class tbl_CashDeliveryAmount
    {
        public long tbl_CashOrderAmount_Id { get; set; }
        public Nullable<long> ReceivedBy { get; set; }
        public Nullable<long> SentBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsAccept { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}