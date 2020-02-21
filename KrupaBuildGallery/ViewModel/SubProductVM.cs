using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class SubProductVM
    {
        public long SubProductId { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public long ProductId { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public long CategoryId { get; set; }
        [Required]
        [Display(Name = "Sub Product Name")]
        public string SubProductName { get; set; }
        public bool IsActive { get; set; }
        public string ProductImage { get; set; } 
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
    }
}