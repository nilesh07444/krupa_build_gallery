using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class DistributorRequestVM
    {
        public long DistributorRequestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string MobileNo { get; set; }        
        public string City { get; set; }
        public string State { get; set; }
        public string AddharCardNo { get; set; }
        public string PanCardNo { get; set; }
        public string GSTNo { get; set; }
    }  
}