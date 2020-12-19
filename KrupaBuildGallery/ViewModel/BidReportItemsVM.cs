using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class BidReportItemsVM
    {
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public List<BidVM> lstBids { get; set; }
    }
}