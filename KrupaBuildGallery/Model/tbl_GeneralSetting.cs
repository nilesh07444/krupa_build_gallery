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
    
    public partial class tbl_GeneralSetting
    {
        public long GeneralSettingId { get; set; }
        public Nullable<decimal> InitialPointCustomer { get; set; }
        public string ShippingMessage { get; set; }
        public string SMTPHost { get; set; }
        public string SMTPPort { get; set; }
        public Nullable<bool> EnableSSL { get; set; }
        public string SMTPEmail { get; set; }
        public string SMTPPwd { get; set; }
        public string AdminSMSNumber { get; set; }
        public string AdminEmail { get; set; }
        public string FromEmail { get; set; }
        public string AdvertiseBannerImage { get; set; }
        public Nullable<decimal> ReturnPerInGodhra { get; set; }
        public Nullable<decimal> ReturnPerOutGodhra { get; set; }
        public Nullable<decimal> ExchangePer { get; set; }
        public Nullable<bool> IsShowBannerImage { get; set; }
        public Nullable<decimal> CashLimitPerOrder { get; set; }
        public Nullable<decimal> CashLimitPerYear { get; set; }
        public string RazorPayKey { get; set; }
        public string RazorPaySecret { get; set; }
        public Nullable<int> ReferenceReferralDiscountPoints { get; set; }
        public string FlashMessage { get; set; }
    }
}
