﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class PlaceOrderVM
    {
        public string shipfirstname { get; set; }
        public string shiplastname { get; set; }
        public string shipphone { get; set; }
        public string shipemailaddress { get; set; }
        public string shipaddress { get; set; }
        public string shipcity { get; set; }
        public string shipstate { get; set; }
        public string shippincode { get; set; }
        public string Orderamount { get; set; }
        public string shipamount { get; set; }
        public string razorpay_payment_id { get; set; }
        public string ClientUserId { get; set; }
        public string RoleId { get; set; }

        public string MobileNumber {get;set;}
    }
}