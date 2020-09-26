using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    [CustomClientAuthorize]
    public class MyAddressController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public MyAddressController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            long userid = clsClientSession.UserID;
            List<ShippingAddressVM> lstShippingAddress = (from s in _db.tbl_ShippingAddresses
                                                          select new ShippingAddressVM
                                                          {
                                                              ShippingAddressId = s.ShippingAddressId,
                                                              ClientUserId = s.ClientUserId,
                                                              AddressTitle = s.AddressTitle,
                                                              ShipFirstName = s.ShipFirstName,
                                                              ShipLastName = s.ShipLastName,
                                                              ShipPhoneNumber = s.ShipPhoneNumber,
                                                              ShipEmail = s.ShipEmail,
                                                              ShipAddress = s.ShipAddress,
                                                              ShipCity = s.ShipCity,
                                                              ShipState = s.ShipState,
                                                              ShipPostalCode = s.ShipPostalCode,
                                                              GSTNo = s.GSTNo
                                                          }).Where(x => x.ClientUserId == userid).ToList();
            return View(lstShippingAddress);
        }
    }
}