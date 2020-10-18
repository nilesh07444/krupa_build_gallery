using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class ChtMessage
    {
        public long ChatMeesageId { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string MessageDate { get; set; }                
    }
}