using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class ItemTextVM
    {
        public long ItemTextId { get; set; }
        [Required]
        [Display(Name = "Item Text")]
        public string  ItemText { get; set; }
    }
}