using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{    
    public class HomeImageVM
    {
        public int HomeImageId { get; set; }
        public string HomeImageName { get; set; }
        public string HeadingText1 { get; set; }
        public string HeadingText2 { get; set; }
        public bool IsActive { get; set; }
    }
}