using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class BidTermsVM
    {
        public long Pk_DelearTerms { get; set; }
        public long DealerId { get; set; }
        public string Terms { get; set; }
        public int TermsType { get; set; }

    }
}