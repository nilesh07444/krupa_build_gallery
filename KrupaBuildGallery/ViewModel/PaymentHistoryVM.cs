using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class PaymentHistoryVM
    {
        public long PaymentHistoryId { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime PaymentDate { get; set; }
        public long OrderId { get; set; }
        public string Paymentthrough { get; set; }
        public decimal OrderTotalAmout { get; set; }
        public decimal CurrentAmountDue { get; set; }
        
    }
}