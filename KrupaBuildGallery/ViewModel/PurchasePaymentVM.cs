using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PurchasePaymentVM
    {
        public long PurchasePaymentId { get; set; }
        public long PurchaseId { get; set; }
        public long DealerId { get; set; }
        public string BillNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentBy { get; set; }
        public decimal Vatav { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<long> ModfiedBy { get; set; }
        public string Remarks { get; set; }
        public string ChequeBankName { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string DealerCode { get; set; }
    }
}