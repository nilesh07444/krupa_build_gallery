using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class ContactUsMessageVM
    {
        public long ContactUsMsgId { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string FromWhere { get; set; }
        public DateTime MessageDate { get; set; }             
        public string ClientUserId { get; set; }
        
    }
}