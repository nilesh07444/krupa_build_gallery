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
    
    public partial class Tbl_ClientOtherDetails
    {
        public long Pk_ClientOtherDetailsId { get; set; }
        public Nullable<long> ClientUserId { get; set; }
        public string Addharcardno { get; set; }
        public string Pancardno { get; set; }
        public string GSTno { get; set; }
        public string Photo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipAddress { get; set; }
        public Nullable<decimal> CreditAmt { get; set; }
        public Nullable<decimal> AmountDue { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
