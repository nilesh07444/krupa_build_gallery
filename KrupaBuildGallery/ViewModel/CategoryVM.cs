using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class CategoryVM
    {
        public long CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string CategoryImage { get; set; }
    }
}