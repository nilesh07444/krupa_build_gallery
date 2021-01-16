using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PurchaseVM
    {
        public long PurchaseId { get; set; }
        public string BillNo { get; set; }
        public string BillYear { get; set; }
        public string DealerPartyCode { get; set; }
        public long DealerId { get; set; }
        public decimal TotalBillAmt { get; set; }
        public decimal PlusMinus1 { get; set; }
        public decimal Insurance { get; set; }
        public decimal TDS { get; set; }
        public decimal PlusMinus2 { get; set; }
        public decimal FinalBillAmount { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<decimal> PaymentPaid { get; set; }
        public Nullable<long> AutoBillId { get; set; }
        public string Remarks { get; set; }        
        public decimal OutStandingAmt { get; set; }
        public decimal TotalAmtPaidWithoutBill { get; set; }
        public List<PurchaseItemVM> lstPurchaseItems { get; set; }
        public List<PurchasePaymentVM> lstPayments { get; set; }
        public List<tbl_PurchasePayment> lstPurchasePayments { get; set; }
    }
}