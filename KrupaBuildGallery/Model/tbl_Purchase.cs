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
    
    public partial class tbl_Purchase
    {
        public long PurchaseId { get; set; }
        public string BillNo { get; set; }
        public string BillYear { get; set; }
        public string DealerPartyCode { get; set; }
        public Nullable<long> DealerId { get; set; }
        public Nullable<decimal> TotalBillAmt { get; set; }
        public Nullable<decimal> PlusMinus1 { get; set; }
        public Nullable<decimal> Insurance { get; set; }
        public Nullable<decimal> TDS { get; set; }
        public Nullable<decimal> PlusMinus2 { get; set; }
        public Nullable<decimal> FinalBillAmount { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<decimal> PaymentPaid { get; set; }
        public Nullable<long> AutoBillId { get; set; }
        public string Remarks { get; set; }
    }
}
