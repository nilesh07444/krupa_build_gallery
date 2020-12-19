using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class BidDealerVM
    {
        public string BusinessCode { get; set; }
        public string FirmName { get; set; }
        public long DealerId { get; set; }
        public string FirmMobile { get; set; }
        public decimal Price { get; set; }
        public long MinimumQtytoBuy { get; set; }
        public string TermsCondition { get; set; }
        public string PaymentTerms { get; set; }
        public string PaymentType { get; set; }
        public string PickupCity { get; set; }
        public string PickupCityPincode { get; set; }
        public int BidValidDays { get; set; }
        public DateTime BidSentDate { get; set; }
        public DateTime BidModifiedDate { get; set; }
        public int BidStatus { get; set; }
        public string Remarks { get; set; }
        public string ItemName { get; set; }
        public long BidDealerId { get; set; }
        public string Status { get; set; }
        public long BidId { get; set; }
        public string RejectReason { get; set; }
        public DateTime StatusDate { get; set; }   
        public string OwnerContactNo { get; set; }
    }
}