using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class AvailablePincodeVM
    {
        public int AvailablePincodeId { get; set; }
        [Required]
        [Display(Name = "Pincode *")]
        public string AvailablePincode { get; set; }
    }
}