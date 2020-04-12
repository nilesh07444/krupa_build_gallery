using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PointVM
    {
        public long PointId { get; set; }
        public decimal Points { get; set; }
        public decimal UsedPoints { get; set; }
        public DateTime ExpiryDate { get; set; }      
    }
}