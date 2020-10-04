using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class PincodeCityStateController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public PincodeCityStateController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index(string state)
        {
            List<PincodeCityStateVM> lstPincode = new List<PincodeCityStateVM>();
            try
            {
                if (state == null)
                {
                    state = "Gujarat";
                }

                lstPincode = (from c in _db.tbl_PincodeCityState
                              select new PincodeCityStateVM
                              {
                                  Id = c.Id,
                                  Pincode = c.Pincode,
                                  City = c.City,
                                  State = c.State
                              }).Where(x => x.State.ToLower() == state.ToLower()).OrderByDescending(x => x.Id).ToList();

                List<string> lstStates = _db.tbl_PincodeCityState.Select(x => x.State).Distinct().OrderBy(x => x).ToList();
                ViewData["lstStates"]  = lstStates;

                ViewBag.SelectedState = state;

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstPincode);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(PincodeCityStateVM pincodeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existPincode = _db.tbl_PincodeCityState.Where(x => x.Pincode == pincodeVM.Pincode).FirstOrDefault();
                    if (existPincode != null)
                    {
                        ModelState.AddModelError("Pincode", ErrorMessage.PincodeExists);
                        return View(pincodeVM);
                    }

                    tbl_PincodeCityState objPincode = new tbl_PincodeCityState();
                    objPincode.Pincode = pincodeVM.Pincode;
                    objPincode.City = pincodeVM.City;
                    objPincode.State = pincodeVM.State;
                    _db.tbl_PincodeCityState.Add(objPincode);

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(pincodeVM);
        }

        [HttpPost]
        public string DeletePincode(int Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_PincodeCityState objPincode = _db.tbl_PincodeCityState.Where(x => x.Id == Id).FirstOrDefault();

                if (objPincode == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_PincodeCityState.Remove(objPincode);
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