using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class BidItemVM
    {
        public long Pk_PurchaseBidItemId { get; set; }
        [Required]
        [Display(Name = "Item Name *")]
        public string ItemName { get; set; }

        public List<SelectListItem> UnitList { get; set; }

        [Required]
        public long UnitType { get; set; }
        public bool IsDeleted { get; set; }
        public string UnitName { get; set; }
    }   

}