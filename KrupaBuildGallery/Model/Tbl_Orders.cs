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
    
    public partial class Tbl_Orders
    {
        public long Pk_Order_Id { get; set; }
        public Nullable<long> Fk_ClientUserId { get; set; }
        public Nullable<decimal> OrderAmount { get; set; }
        public string OrderShipCity { get; set; }
        public string OrderShipState { get; set; }
        public string OrderShipAddress { get; set; }
        public string OrderShipPincode { get; set; }
        public string OrderShipClientName { get; set; }
        public string OrderShipClientPhone { get; set; }
        public Nullable<long> OrderStatusId { get; set; }
        public string PaymentType { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
