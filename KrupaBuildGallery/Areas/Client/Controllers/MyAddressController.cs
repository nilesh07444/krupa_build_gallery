using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Newtonsoft.Json;
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
                                                          where s.ClientUserId == userid && s.IsDeleted == false
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
                                                          }).ToList();

            List<string> lstStates = _db.tbl_PincodeCityState.Select(x => x.State).Distinct().OrderBy(x => x).ToList();
            ViewData["lstStates"] = lstStates;

            return View(lstShippingAddress);
        }

        [HttpPost]
        public string SaveShippingAddress(string ShippingDetail)
        {
            string message = string.Empty;
            try
            {
                ShipAddressVM objShipAddress = JsonConvert.DeserializeObject<ShipAddressVM>(ShippingDetail);

                long userid = clsClientSession.UserID;
                tbl_ShippingAddresses objShip = new tbl_ShippingAddresses();

                if (objShipAddress.ShippingAddressId == 0)
                {
                    var objshipexist = _db.tbl_ShippingAddresses.Where(o => o.AddressTitle.ToLower() == objShipAddress.AddressTitle.ToLower() && o.ClientUserId == userid && o.IsDeleted == false).FirstOrDefault();
                    if (objshipexist != null)
                    {
                        message = "Title_Exists";
                        return message;
                    }
                    objShip.CreatedDate = DateTime.UtcNow;
                    objShip.IsDeleted = false;
                    _db.tbl_ShippingAddresses.Add(objShip);
                }
                else
                {
                    long shipadreid = objShipAddress.ShippingAddressId;
                    var objshipexist = _db.tbl_ShippingAddresses.Where(o => o.AddressTitle.ToLower() == objShipAddress.AddressTitle.ToLower() && o.ShippingAddressId != shipadreid && o.ClientUserId == userid && o.IsDeleted == false).FirstOrDefault();
                    if (objshipexist != null)
                    {
                        message = "Title_Exists";
                        return message;
                    }

                    objShip = _db.tbl_ShippingAddresses.Where(o => o.ShippingAddressId == shipadreid).FirstOrDefault();

                }

                objShip.ShipAddress = objShipAddress.ShipAddress;
                objShip.ShipCity = objShipAddress.ShipCity;
                objShip.ShipEmail = objShipAddress.ShipEmail;
                objShip.ShipPhoneNumber = objShipAddress.ShipPhoneNumber;
                objShip.ShipFirstName = objShipAddress.ShipFirstName;
                objShip.ShipLastName = objShipAddress.ShipLastName;
                objShip.ShipState = objShipAddress.ShipState;
                objShip.ShipPostalCode = objShipAddress.ShipPostalCode;
                objShip.AddressTitle = objShipAddress.AddressTitle;
                objShip.ClientUserId = userid;
                objShip.GSTNo = objShipAddress.GSTNo;

                _db.SaveChanges();

                message = "success";
            }
            catch (Exception ex)
            {
                message = "error";
                throw ex;
            }

            return message;
        }

        [HttpGet]
        public JsonResult GetShippingAddressById(long Id)
        {
            ShipAddressVM objShipping = new ShipAddressVM();
            try
            {
                objShipping = (from s in _db.tbl_ShippingAddresses
                               where s.ShippingAddressId == Id
                               select new ShipAddressVM
                               {
                                   ShippingAddressId = s.ShippingAddressId,
                                   ShipAddress = s.ShipAddress,
                                   ShipCity = s.ShipCity,
                                   ShipEmail = s.ShipEmail,
                                   ShipPhoneNumber = s.ShipPhoneNumber,
                                   ShipFirstName = s.ShipFirstName,
                                   ShipLastName = s.ShipLastName,
                                   ShipState = s.ShipState,
                                   ShipPostalCode = s.ShipPostalCode,
                                   AddressTitle = s.AddressTitle,
                                   GSTNo = s.GSTNo
                               }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return Json(objShipping, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCityStateFromPincode(int pincode)
        {
            var data = _db.tbl_PincodeCityState.Where(x => x.Pincode == pincode).FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string DeleteShippingAddress(long ShippingAddressId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_ShippingAddresses objAddress = _db.tbl_ShippingAddresses.Where(x => x.ShippingAddressId == ShippingAddressId&& x.IsDeleted == false).FirstOrDefault();

                if (objAddress == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    _db.tbl_ShippingAddresses.Remove(objAddress);
                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }


    }
}