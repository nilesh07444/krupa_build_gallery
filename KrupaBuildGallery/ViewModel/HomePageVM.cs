using KrupaBuildGallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class HomePageVM
    {
        public List<ProductItemVM> PopularProducts { get; set; }

        public List<ProductItemVM> OfferProducts { get; set; }

        public List<ProductItemVM> UnPackedItems { get; set; }

        public List<HomeImageVM> HomePageSlider { get; set; }

        public List<CategoryVM> Categories { get; set; }

        public List<AdvertiseImageVM> lstAds { get; set; }
        public string BannerImage { get; set; }
        public WebsiteStatisticsVM webstats { get; set; }

        public List<ComboOfferVM> lstComboOffers { get; set; }

    }
}