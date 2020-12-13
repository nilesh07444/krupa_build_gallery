using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PurchaseDealerVM
    {
        public long Pk_Dealer_Id { get; set; }
        public string FirmName { get; set; }
        public string FirmAddress { get; set; }
        public string FirmCity { get; set; }
        public string FirmGSTNo { get; set; }
        public string VisitingCardPhoto { get; set; }
        public string FirmContactNo { get; set; }
        public string AlternateNo { get; set; }
        public string Email { get; set; }
        public string BusinessDetails { get; set; }
        public string BankAcNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankBranch { get; set; }
        public string BankAcNumber2 { get; set; }
        public string IFSCCode2 { get; set; }
        public string BankBranch2 { get; set; }
        public string OwnerName { get; set; }
        public string OwnerContactNo { get; set; }
        public string Remark { get; set; }
        public string BussinessCode { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public long Status { get; set; }
        public string RejectReason { get; set; }
        public string State { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}