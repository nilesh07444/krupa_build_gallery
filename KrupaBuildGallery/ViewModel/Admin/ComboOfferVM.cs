using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class ComboOfferVM
    {
        public long ComboOfferId { get; set; }
        [Required, Display(Name = "Offer Title")] 
        public string OfferTitle { get; set; } 
        [Required(ErrorMessage = "Required"), Display(Name = "Category")]
        public long Main_CategoryId { get; set; } 

        [Required(ErrorMessage = "Required"), Display(Name = "Product")]
        public long Main_ProductId { get; set; }

        [Display(Name = "Sub Product")]
        public long? Main_SubProductId { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        [Display(Name = "Product Item")]
        public long Main_ProductItemId { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required")]
        [Display(Name = "Qty")]
        public decimal Main_Qty { get; set; }
       
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Offer Price must be greater than 0")]
        [Display(Name = "Combo Offer Price")]
        public decimal ComboOfferPrice { get; set; }
        
        [Required,Display(Name = "Offer Start Date")] 
        public string OfferStartDate { get; set; }

        [Required, Display(Name = "Offer End Date")] 
        public string OfferEndDate { get; set; }
        [Display(Name = "Offer Image")]
        public HttpPostedFileBase OfferImageFile { get; set; }
        public bool IsActive { get; set; }

        public long MainVariantId { get; set; }

        [Required]
        [Display(Name = "Offer Description *")]
        public string OfferDescription { get; set; }
        // Additional Fields
        // main fields
        public List<SelectListItem> Main_CategoryList { get; set; }
        public List<SelectListItem> Main_ProductList { get; set; }
        public List<SelectListItem> Main_SubProductList { get; set; }
        public List<SelectListItem> Main_ProductItemList { get; set; }
        public List<SelectListItem> Main_VariantList { get; set; }
        public string Main_CategoryName { get; set; }
        public string Main_ProductName { get; set; }
        public string Main_SubProductName { get; set; }
        public string Main_ProductItemName { get; set; }

        // sub fields
        public List<SelectListItem> Sub_CategoryList { get; set; }
        public List<SelectListItem> Sub_ProductList { get; set; }
        public List<SelectListItem> Sub_SubProductList { get; set; }
        public List<SelectListItem> Sub_ProductItemList { get; set; }
        public string Sub_CategoryName { get; set; }
        public string Sub_ProductName { get; set; }
        public string Sub_SubProductName { get; set; }
        public string Sub_ProductItemName { get; set; }

        public DateTime? dtOfferStartDate { get; set; }
        public DateTime? dtOfferEndDate { get; set; }
        public string OfferImage { get; set; }
        public decimal TotlOriginalOfferPrice { get; set; }

        [Display(Name = "Cash On Delivery Use")]
        public bool IsCashonDelieveryuse { get; set; }

        public string Sku { get; set; }
        public string MainItemVariantName { get; set; }
        public decimal ShippingRate { get; set; }
        public decimal MainItemMRPPrice { get; set; }
    }
}