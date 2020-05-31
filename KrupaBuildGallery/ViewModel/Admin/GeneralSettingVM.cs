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

        [Display(Name = "Advertise Banner Image")] 
        public HttpPostedFileBase AdvertiseBannerImageFile { get; set; }

        //
        public string AdvertiseBannerImage { get; set; }

    }
}