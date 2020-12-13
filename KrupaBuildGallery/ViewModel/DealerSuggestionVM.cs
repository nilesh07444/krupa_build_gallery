using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class DealerSuggestionVM
    {
        public long SuggetionId { get; set; }
        public long DealerId { get; set; }
        public string Suggestion { get; set; }
        public string PicFile { get; set; }
    }
}