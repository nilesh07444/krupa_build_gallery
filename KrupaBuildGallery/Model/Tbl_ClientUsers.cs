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
    
    public partial class tbl_ClientUsers
    {
        public long ClientUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public long ClientRoleId { get; set; }
        public string CompanyName { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string AlternateMobileNo { get; set; }
        public string Prefix { get; set; }
        public Nullable<decimal> WalletAmt { get; set; }
        public string Reference { get; set; }
        public string OwnReferralCode { get; set; }
        public string ReferenceReferralCode { get; set; }
        public Nullable<long> ReferenceReferralClientUserId { get; set; }
        public Nullable<int> ReferencePointReceived { get; set; }
    }
}
