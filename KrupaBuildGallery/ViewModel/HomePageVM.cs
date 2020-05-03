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

        public List<ProductItemVM> NewArrivalProducts { get; set; }

        public List<HomeImageVM> HomePageSlider { get; set; }


    }
}