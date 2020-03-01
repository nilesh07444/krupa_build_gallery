using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class ProductItemVM
    {
        public long ProductItemId { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public long CategoryId { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public long ProductId { get; set; }
        [Display(Name = "Sub Product Name")]
        public long? SubProductId { get; set; }
        [Required]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "Item Description")]
        public string ItemDescription { get; set; }
        [Required]
        [Display(Name = "MRP Price")]
        public decimal MRPPrice { get; set; }
        [Required]
        [Display(Name = "Customer Price")]
        public decimal CustomerPrice { get; set; }
        [Required]
        [Display(Name = "Distributor Price")]
        public decimal DistributorPrice { get; set; }
        [Required]
        [Display(Name = "GST Per")]
        public decimal GST_Per { get; set; }
        [Display(Name = "IGST Per")]
        public decimal IGST_Per { get; set; } 
        public string MainImage { get; set; }
        [Display(Name = "Notification Text")]
        public string Notification { get; set; }
        [Display(Name = "Sku")]
        public string Sku { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Item Main Image")]
        public HttpPostedFileBase ItemMainImageFile { get; set; }

        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public List<SelectListItem> SubProductList { get; set; }

        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string SubProductName { get; set; }

    }
}