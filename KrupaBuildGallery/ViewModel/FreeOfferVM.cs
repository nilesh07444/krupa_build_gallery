using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class FreeOfferVM
    {
        public long FreeOfferId { get; set; }            
        public DateTime OfferStartDate { get; set; }
        public DateTime OfferEndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Order Amount From must be greater than 0")]
        [Display(Name = "Order Amount From")]
        public decimal OrderAmountFrom { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Order Amount To must be greater than 0")]
        [Display(Name = "Order Amount To")]
        public decimal OrderAmountTo { get; set; }

        [Required, Display(Name = "Offer Start Date")]
        public string OfferStartDateStr { get; set; }

        [Required, Display(Name = "Offer End Date")]
        public string OfferEndDateStr { get; set; }        
    
        public bool IsActive { get; set; }
    }
}