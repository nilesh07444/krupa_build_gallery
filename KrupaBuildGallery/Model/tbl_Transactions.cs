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
    
    public partial class tbl_Transactions
    {
        public long PK_Transacation_Id { get; set; }
        public Nullable<long> ProductItemId { get; set; }
        public Nullable<long> OrderItemId { get; set; }
        public Nullable<long> OrderId { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<long> CreatedByUserId { get; set; }
        public Nullable<long> CreatedByAdminId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string TypeOfTransaction { get; set; }
    }
}
