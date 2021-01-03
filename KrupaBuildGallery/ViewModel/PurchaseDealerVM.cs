using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class PurchaseDealerVM
    {
        public long Pk_Dealer_Id { get; set; }
        [Required]
        [Display(Name = "Firm Name *")]
        public string FirmName { get; set; }
        [Required]
        [Display(Name = "Firm Address *")]
        public string FirmAddress { get; set; }
        [Required]
        [Display(Name = "Firm City *")]
        public string FirmCity { get; set; }
        //[Required]
        [Display(Name = "Firm State *")]
        public string State { get; set; }
        [Required]
        [Display(Name = "Firm GST No *")]
        public string FirmGSTNo { get; set; }
        public string VisitingCardPhoto { get; set; }
        [Required]
        [Display(Name = "Firm Contact No *")]
        public string FirmContactNo { get; set; }
        [Display(Name = "Firm Alternate Contact No *")]
        public string AlternateNo { get; set; }
        [Required]
        [Display(Name = "Email *")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Business Details *")]
        public string BusinessDetails { get; set; }

        [Required]
        [Display(Name = "1st Bank Name *")]
        public string BankName { get; set; }
        [Required]
        [Display(Name = "1st Bank A/c Number *")]
        public string BankAcNumber { get; set; }
        [Required]
        [Display(Name = "1st IFSC Code *")]
        public string IFSCCode { get; set; }
        [Required]
        [Display(Name = "1st Bank Branch *")]
        public string BankBranch { get; set; }
         
        [Display(Name = "2nd Bank Name")]
        public string BankName2 { get; set; }

        [Display(Name = "2nd Bank A/c Number")]
        public string BankAcNumber2 { get; set; }

        [Display(Name = "2nd IFSC Code")]
        public string IFSCCode2 { get; set; }

        [Display(Name = "2nd Bank Branch")]
        public string BankBranch2 { get; set; }

        [Required]
        [Display(Name = "Owner Name *")]
        public string OwnerName { get; set; }
        [Required]
        [Display(Name = "Owner Contact No *")]
        public string OwnerContactNo { get; set; }
        public string Remark { get; set; }
        public string BussinessCode { get; set; }
        [Required]
        [Display(Name = "Password *")]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public long Status { get; set; }
        public string RejectReason { get; set; } 
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Visiting Card Photo")]
        public HttpPostedFileBase VisitingCardPhotoFile { get; set; }

        public List<SelectListItem> StateList { get; set; }
    }
}