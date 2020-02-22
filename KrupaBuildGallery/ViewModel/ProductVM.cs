using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class ProductVM
    {
        public long ProductId { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public long CategoryId { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
        public string ProductImage { get; set; }

        [Display(Name = "Product Image")]
        public HttpPostedFileBase ProductImageFile { get; set; }
        public string CategoryName { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
    }
}