using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PaymentReportBillVM
    {
        public long DealerId { get; set; }
        public string SupplierCode { get; set; }
        public List<PurchaseVM> lstPurchases { get; set; }

    }
}