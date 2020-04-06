using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class LoginHistoryVM
    {
        public long LoginHistoryId { get; set; }
        public long? UserId { get; set; }
        public DateTime? DateAction { get; set; }
        public string IPAddress { get; set; }
        public string Type { get; set; }
        public string AdminUserFullName { get; set; }
    }
}