using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel.Admin
{
    public class RatingReviewVM
    {
        public long RatingReviewId { get; set; }
        public string ItemName { get; set; }
        public string ClientName { get; set; }
        public decimal Ratings { get; set; }
        public long OrderId { get; set; }       
        public string Review { get; set; }
        public DateTime RatingDate { get; set; }        
        public string VariantTxt { get; set; }
        public string MobileNo { get; set; }
    }
}