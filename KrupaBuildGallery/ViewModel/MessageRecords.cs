using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class MessageRecords
    {
        public List<tbl_ChatMessages> Messages { get; set; }
        public int TotalMessages { get; set; }
        public long LastChatMessageId { get; set; }
    }
}