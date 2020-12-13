using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class BidVM
    {
        public long BidId { get; set; }
        public long ItemId { get; set; }
        public int Qty { get; set; }
        public string Unittype { get; set; }
        public string ItemName { get; set; }
        public DateTime BidDate { get; set; }
        public string BidDateStr { get; set; }
        public string Remarks { get; set; }
        public long BidStatus { get; set; }
    }
}