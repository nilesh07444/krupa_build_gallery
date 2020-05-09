using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class ItemStockVM
    {
        public long StockId { get; set; }        
        [Required]
        [Display(Name = "Category Name *")]
        public long CategoryId { get; set; }
        [Required]
        [Display(Name = "Product Name *")]
        public long ProductId { get; set; }
        [Display(Name = "Sub Product Name")]
        public long? SubProductId { get; set; }

        [Required]
        [Display(Name = "Product Item *")]
        public long ProductItemId { get; set; }
      
        [Required]
        [Range(1,int.MaxValue, ErrorMessage = "Quatity must be greater than 0")]
        [Display(Name = "Quantity *")]
        public long Quantity { get; set; }
        public bool IsActive { get; set; }
        
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public List<SelectListItem> SubProductList { get; set; }
        public List<SelectListItem> ProductItemList { get; set; }

        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string SubProductName { get; set; }
        public string ProductItemName { get; set; }

        // Additional fields
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ItemType { get; set; }

    }


}