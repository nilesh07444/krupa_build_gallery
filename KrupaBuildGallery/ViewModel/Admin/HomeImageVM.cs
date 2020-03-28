using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{    
    public class HomeImageVM
    {
        public int HomeImageId { get; set; }
        public string HomeImageName { get; set; }
        [Display(Name = "Heading Text 1")]
        public string HeadingText1 { get; set; }
        [Display(Name = "Heading Text 2")]
        public string HeadingText2 { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Home Image")]
        public HttpPostedFileBase HomeImageFile { get; set; }
    }
}