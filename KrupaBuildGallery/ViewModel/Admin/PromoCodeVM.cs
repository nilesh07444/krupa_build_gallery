using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PromoCodeVM
    {
        public long PromoCodeId { get; set; }

        [Required]
        [Display(Name = "Promo Code *")]
        public string PromoCode { get; set; }

        [Required]
        [Display(Name = "Discount Percentage *")]
        public decimal? DiscountPercentage { get; set; }

        [Required]
        [Display(Name = "Expiry Date *")]
        public string ExpiryDate { get; set; }

        [Required]
        [Display(Name = "Total Max Usage *")]
        public int? TotalMaxUsage { get; set; }

        public bool IsActive { get; set; }

        //
        public DateTime? dtExpiryDate { get; set; }

        public int TotalUsedByUsers { get; set; }

        public List<PromoUsedDetailVM> lstPromoUsedDetail { get; set; }

    }

    public class PromoUsedDetailVM
    {
        public long PromoCodeId { get; set; }
        public long ClientUserId { get; set; }
        public long RoleId { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public long OrderNo { get; set; }
        public decimal? PromoDiscount { get; set; }
    }


}