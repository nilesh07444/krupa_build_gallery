using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class CategoryVM
    {
        public long CategoryId { get; set; }
        [Required]
        [Display(Name = "Category Name *")]
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string CategoryImage { get; set; }
        [Display(Name = "Category Image")]
        public HttpPostedFileBase CategoryImageFile { get; set; }

        // Additional fields
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TotalItems { get; set; }

    }
}