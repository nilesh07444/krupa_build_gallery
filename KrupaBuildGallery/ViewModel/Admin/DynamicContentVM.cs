using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class DynamicContentVM
    {
        public int DynamicContentId { get; set; }
        [Required]
        [Display(Name = "Content Type *")]
        public int DynamicContentType { get; set; }
        [Required]
        [Display(Name = "Content Title *")]
        public string ContentTitle { get; set; }
        [Required]
        [Display(Name = "Content Description *")]
        public string ContentDescription { get; set; }
        [Required]
        [Display(Name = "Sequence Number *")]
        public int? SeqNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        //
        public List<SelectListItem> DynamicContentTypeList { get; set; }

    }
}