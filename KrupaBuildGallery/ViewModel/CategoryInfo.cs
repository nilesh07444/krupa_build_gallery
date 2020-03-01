using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
     
    public class CategoryInfo
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public IEnumerable<ProductInfo> ProductInfo { get; set; }
    }

    public class ProductInfo
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public IEnumerable<SubProductInfo> SubProductInfo { get; set; }
    }

    public class SubProductInfo
    {
        public long SubProductId { get; set; }
        public string SubProductName { get; set; }
        public string SubProductImage { get; set; }
    }

}