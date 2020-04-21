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
        [Display(Name = "Product Name *")]
        public long ProductId { get; set; }
        [Required]
        [Display(Name = "Category Name *")]
        public long CategoryId { get; set; }
        [Required]
        [Display(Name = "Sub Product Name *")]
        public string SubProductName { get; set; }
        public bool IsActive { get; set; }
        public string SubProductImage { get; set; } 
        public string CategoryName { get; set; }
        [Display(Name = "Sub Product Image")]
        public HttpPostedFileBase SubProductImageFile { get; set; }
        public string ProductName { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> ProductList { get; set; }

        // Additional fields
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}