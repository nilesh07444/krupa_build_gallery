using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class FeedbackVM
    {
        public long FeedbackId { get; set; }
        public string AboutService { get; set; }
        public string AboutDeliveryBoyService { get; set; }
        public string AboutDeliveryBoyBehaviour { get; set; }
        public string OurQuality { get; set; }
        public string Suggestion { get; set; }
        public DateTime FeedbackDate { get; set; }
        public long ClientUserId { get; set; }
        public DateTime FeedbackOfMonth { get; set; }
    }
}