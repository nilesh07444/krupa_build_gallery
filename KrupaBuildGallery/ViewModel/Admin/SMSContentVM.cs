using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class SMSContentVM
    {
        public int SMSContentId { get; set; }
        [Required]
        [Display(Name = "SMS Title *")]
        public string SMSTitle { get; set; }
        [Required]
        [Display(Name = "SMS Description *")]
        public string SMSDescription { get; set; }
        [Required]
        [Display(Name = "Sequence Number *")]
        public int? SeqNo { get; set; }
    }
}