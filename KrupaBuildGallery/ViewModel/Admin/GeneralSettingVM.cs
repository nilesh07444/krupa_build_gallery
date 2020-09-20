using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class GeneralSettingVM
    {
        public long GeneralSettingId { get; set; }
        public decimal? InitialPointCustomer { get; set; }
        public string ShippingMessage { get; set; }
        public string SMTPHost { get; set; }
        public string SMTPPort { get; set; }
        public bool? EnableSSL { get; set; }
        public string SMTPEmail { get; set; }
        public string SMTPPwd { get; set; }
        public string AdminSMSNumber { get; set; }
        public string AdminEmail { get; set; }
        public string FromEmail { get; set; }

        [Display(Name = "Advertise Banner Image")]
        public HttpPostedFileBase AdvertiseBannerImageFile { get; set; }
        [Display(Name = "Return Per(%) (In Godhra)")]
        public decimal? ReturnPerInGodhra { get; set; }
        [Display(Name = "Return Per(%) (Out of Godhra)")]
        public decimal? ReturnPerOutGodhra { get; set; }
        [Display(Name = "Exchange Per(%)")]
        public decimal? ExchangePer { get; set; }
        public bool? IsShowBannerImage { get; set; }
        [Display(Name = "Cash Limit Per Order")]
        public decimal? CashLimitPerOrder { get; set; }
        [Display(Name = "Cash Order Limit Per Year")]
        public decimal? CashLimitPerYear { get; set; }
        //
        public string AdvertiseBannerImage { get; set; }
        public string RazorPayKey { get; set; }
        public string RazorPaySecret { get; set; }
        public int? ReferenceReferralDiscountPoints { get; set; }
    }
}