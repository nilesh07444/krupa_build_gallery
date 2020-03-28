using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class GodownVM
    {
        public int GodownId { get; set; }
        [Required]
        [Display(Name = "Godown Name")]
        public string GodownName { get; set; } 
        public bool IsActive { get; set; } 
    }
}