using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.ViewModel.Admin
{
    public class ComboSubItemVM
    {
        public long ComboOfferId { get; set; }     
        public bool IsActive { get; set; }     
        // sub fields
        public List<SelectListItem> Sub_CategoryList { get; set; }
        public List<SelectListItem> Sub_ProductList { get; set; }
        public List<SelectListItem> Sub_SubProductList { get; set; }
        public List<SelectListItem> Sub_ProductItemList { get; set; }
        public List<VariantItemVM> Sub_ProductVariantList { get; set; }
        public string Sub_CategoryName { get; set; }
        public string Sub_ProductName { get; set; }
        public string Sub_SubProductName { get; set; }
        public string Sub_ProductItemName { get; set; }
        public long CategoryId { get; set; }
        public long SubProductId { get; set; }
        public long ProductId { get; set; }
        public long ProductItemId { get; set; }
        public long VarintId { get; set; }
        public long Qty { get; set; }
        public decimal ActualPrice { get; set; }
        public string VarintNm { get; set; }
    }
}