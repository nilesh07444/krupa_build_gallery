using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class OrderItemRequestsVM
    {
        public long OrderItemRequestId { get; set; }
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public int ItemStatus { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime DateCreated { get; set; }
        public long ClientUserId { get; set; }
        public DateTime DateModified { get; set; }
        public long ModifiedBy { get; set; }
        public string ItemName { get; set; }
        
    }
}