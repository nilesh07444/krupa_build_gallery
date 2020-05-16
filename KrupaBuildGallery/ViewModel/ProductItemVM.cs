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
        [Display(Name = "Product Name *")]
        public long ProductId { get; set; }
        [Display(Name = "Sub Product Name")]
        public long? SubProductId { get; set; }
        [Required]
        [Display(Name = "Item Name *")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "Item Description *")]
        public string ItemDescription { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MRP Price must be greater than 0")]
        [Display(Name = "MRP Price *")]
        public decimal MRPPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Customer Price must be greater than 0")]
        [Display(Name = "Customer Price *")]
        public decimal CustomerPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Distributor Price must be greater than 0")]
        [Display(Name = "Distributor Price *")]
        public decimal DistributorPrice { get; set; }
        [Required]
        [Display(Name = "GST Per *")]
        public decimal GST_Per { get; set; }
        [Display(Name = "IGST Per *")]
        public decimal IGST_Per { get; set; } 
        public string MainImage { get; set; }
        [Required]
        [Display(Name = "Notification Text *")]
        public string Notification { get; set; }
        [Display(Name = "Sku")]
        public string Sku { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Item Main Image *")]
        public HttpPostedFileBase ItemMainImageFile { get; set; }

        [Display(Name = "Item Gallery Images *")]
        public HttpPostedFileBase[] ItemGalleryImageFile { get; set; }

        [Display(Name = "Popular Product Item")]
        public bool IsPopularProduct { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public List<SelectListItem> SubProductList { get; set; }
        public List<SelectListItem> GST { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string SubProductName { get; set; }
        public bool IsWishListItem { get; set; } 
        public List<string> OtherImages { get; set; }
        public int InStock { get; set; }        
        public int Sold { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quatity must be greater than 0")]
        [Display(Name = "Initial Qty *")]
        public int InitialQty { get; set; }
        [Required]
        [Display(Name = "Shipping Charge *")]
        public decimal ShippingCharge { get; set; }
        public string Tags { get; set; }
        public List<SelectListItem> GodownList { get; set; }
        [Display(Name = "Godown *")]
        public long GodownId { get; set; }

        [Display(Name = "HSN Code")]
        public string HSNCode { get; set; }

        // Additional fields
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string GodownName { get; set; }
        public List<SelectListItem> GalleryImagesList { get; set; }

        [Display(Name = "Returnable Item")]
        public bool IsReturnableItem { get; set; }

        [Display(Name = "Advance Payment Percentage")]
        public decimal PayAdvancePer { get; set; }

        [Display(Name = "Item Type")]
        public int ItemType { get; set; }

        [Display(Name = "Cash On Delivery Use")]
        public bool IsCashonDelieveryuse { get; set; }

    }
}