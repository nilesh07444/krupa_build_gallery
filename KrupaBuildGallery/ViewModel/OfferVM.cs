using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class OfferVM
    {
        public long OfferId { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public long CategoryId { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public long ProductId { get; set; }
        [Display(Name = "Sub Product Name")]
        public long? SubProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Product Item")]
        [Display(Name = "Product Item")]
        public long ProductItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Offer Price must be greater than 0")]
        [Display(Name = "Customer Offer Price")]
        public decimal CustomerOfferPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Distributor Price must be greater than 0")]
        [Display(Name = "Distributor Offer Price")]
        public decimal DistributorOfferPrice { get; set; }

        public bool IsActive { get; set; }

        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public List<SelectListItem> SubProductList { get; set; }
        public List<SelectListItem> ProductItemList { get; set; }

        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string SubProductName { get; set; }
        public string ProductItemName { get; set; }

        [Display(Name = "Offer Title")]
        [Required]
        public string OfferTitle { get; set; }
        [Display(Name = "Offer Start Date")]
        [Required]       
        public string OfferStartDate { get; set; }

        [Display(Name = "Offer End Date")]
        [Required]
        public string OfferEndDate { get; set; }

        //
        public DateTime? dtOfferStartDate { get; set; }
        public DateTime? dtOfferEndDate { get; set; }

    }
}