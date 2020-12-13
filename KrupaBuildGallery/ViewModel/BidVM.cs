using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class BidVM
    {
        public long BidId { get; set; }

        [Required]
        [Display(Name = "Item Name *")]
        public long ItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        [Display(Name = "Qty *")]
        public long Qty { get; set; }
        public string Unittype { get; set; }
        public string ItemName { get; set; }
        public DateTime BidDate { get; set; }
        public string BidDateStr { get; set; }
        public string Remarks { get; set; }
        public long BidStatus { get; set; }
        public int TotalBids { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> BidItemList { get; set; }        
        public long DelearBidId { get; set; }
        
    }
}