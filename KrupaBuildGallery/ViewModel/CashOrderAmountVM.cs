﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class CashOrderAmountVM
    {
        public long CashOrderAmountId { get; set; }
        public long ReceivedBy { get; set; }
        public long SentBy { get; set; }
        public DateTime dtReceived { get; set;}
        public int IsAccept { get; set; }
        public decimal Amount { get; set; }
        public string SenderName { get; set; }
        public string RecieverName { get; set; }
        public string CashSentDate { get; set; }
    }
}