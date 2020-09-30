using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class ShipAddressVM
    {
        public long ShippingAddressId { get; set; }
        public long ClientUserId { get; set; }
        public string AddressTitle { get; set; }
        public string ShipFirstName { get; set; }
        public string ShipLastName { get; set; }
        public string ShipPhoneNumber { get; set; }
        public string ShipEmail { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipPostalCode { get; set; }
        public string GSTNo { get; set; }
    }
}