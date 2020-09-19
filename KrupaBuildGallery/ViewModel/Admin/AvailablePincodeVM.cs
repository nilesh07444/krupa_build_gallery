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

    public class PincodeCityStateVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Pincode *")]
        public int? Pincode { get; set; }
        [Required]
        [Display(Name = "City *")]
        public string City { get; set; }
        [Required]
        [Display(Name = "State *")]
        public string State { get; set; }
    }
}