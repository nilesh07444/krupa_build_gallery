using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.ViewModel
{
    public class ProductVM
    {
        public long ProductId { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        [Required]
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
        public string ProductImage { get; set; }
    }
}