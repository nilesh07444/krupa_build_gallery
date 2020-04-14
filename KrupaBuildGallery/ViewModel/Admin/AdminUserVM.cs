using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class AdminUserVM
    {
        public long AdminUserId { get; set; }
        [Required]
        [Display(Name = "Role")]
        public int AdminRoleId { get; set; }
        [Required]
        [MaxLength(100), Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100), Display(Name = "Last Name")]
        public string LastName { get; set; }
        [MaxLength(100), Display(Name = "Email Id")]
        public string Email { get; set; }
        [Required]
        [MinLength(5), MaxLength(20), Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [MaxLength(10), MinLength(10), Display(Name = "Mobile No")]
        public string MobileNo { get; set; }
        [MaxLength(10), MinLength(10), Display(Name = "Alternate Mobile No")]
        public string AlternateMobile { get; set; }
        [MaxLength(200), Display(Name = "Address")]
        public string Address { get; set; }
        [MaxLength(100), Display(Name = "City")]
        public string City { get; set; }
        [MaxLength(50), Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Date Of Birth")]
        public string Dob { get; set; }
        [Display(Name = "Date Of Joining")]
        public string DateOfJoin { get; set; }
        [MaxLength(10), Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        [MaxLength(20), Display(Name = "Aadhar Card No")]
        public string AdharCardNo { get; set; }
        public string ProfilePicture { get; set; }
        [Display(Name = "Working Time")]
        public string WorkingTime { get; set; }
        [MaxLength(50), Display(Name = "Card Expiry Date")]
        public string DateOfIdCardExpiry { get; set; }
        [MaxLength(200), Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfilePictureFile { get; set; }
         
        // Addional fields
        public List<SelectListItem> RoleList { get; set; }
        public DateTime? dtDateOfIdCardExpiry { get; set; }
        public DateTime? dtDateOfJoin { get; set; }
        public DateTime? dtDob { get; set; }
        public string RoleName { get; set; }
         
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}