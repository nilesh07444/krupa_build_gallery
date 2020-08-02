using KrupaBuildGallery.ViewModel.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class ComboOfferClientVM
    {
        public List<ComboSubItemVM> SubItems { get; set; }
        public ComboOfferVM Combo { get; set; }
        
    }
}