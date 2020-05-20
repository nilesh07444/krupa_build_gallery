﻿using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class CheckoutGenVM
    {   
        public tbl_ClientOtherDetails ClientOtherDetails { get; set; }
        public string shippingmsg { get; set; }
        public List<CartVM> CartList { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal ShippingChargeTotal { get; set; }
        public decimal TotalOrder { get; set; }
        public List<InvoiceItemVM> InvItems { get; set; }
        public decimal CreditRemaining { get; set; }

    }
}