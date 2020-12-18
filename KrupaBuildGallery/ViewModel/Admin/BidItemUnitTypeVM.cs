using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class BidItemUnitTypeVM
    {
        public long BidItemUnitTypeId { get; set; }
        [Required]
        [Display(Name = "Unit Name *")]
        public string UnitTypeName { get; set; }
        public bool IsDeleted { get; set; }
    }
}