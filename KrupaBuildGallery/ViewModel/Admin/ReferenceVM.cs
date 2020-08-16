using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class ReferenceVM
    {
        public int ReferenceId { get; set; }

        [Required]
        [Display(Name = "Reference *")]
        public string Reference { get; set; } 
    }
}