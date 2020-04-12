using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class ClientUserVM
    {
        public long ClientUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }
        public string CompanyName { get; set; }
        public string ProfilePic { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AddharCardNo { get; set; }        
        public string PanCardNo { get; set; }
        public string GSTNo { get; set; }                  
        public List<OrderVM> OrderList { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipPostalCode { get; set; }
        public List<PointVM> PointsList { get; set; }

    }
}