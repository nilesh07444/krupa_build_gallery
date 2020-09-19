using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class AvailablePincodeController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public AvailablePincodeController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<AvailablePincodeVM> lstPincode = new List<AvailablePincodeVM>();
            try
            {

                lstPincode = (from c in _db.tbl_AvailablePincode
                              select new AvailablePincodeVM
                              {
                                  AvailablePincodeId = c.AvailablePincodeId,
                                  AvailablePincode = c.AvailablePincode
                              }).OrderByDescending(x => x.AvailablePincodeId).ToList();

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
        public ActionResult Add(AvailablePincodeVM pincodeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existPincode = _db.tbl_AvailablePincode.Where(x => x.AvailablePincode.ToLower() == pincodeVM.AvailablePincode.ToLower()).FirstOrDefault();
                    if (existPincode != null)
                    {
                        ModelState.AddModelError("AvailablePincode", ErrorMessage.PincodeExists);
                        return View(pincodeVM);
                    }

                    tbl_AvailablePincode objPincode = new tbl_AvailablePincode();
                    objPincode.AvailablePincode = pincodeVM.AvailablePincode;

                    objPincode.CreatedBy = LoggedInUserId;
                    objPincode.CreatedDate = DateTime.UtcNow;
                    objPincode.UpdatedBy = LoggedInUserId;
                    objPincode.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_AvailablePincode.Add(objPincode);

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

        public ActionResult Edit(int Id)
        {
            AvailablePincodeVM objPincode = new AvailablePincodeVM();

            try
            {
                objPincode = (from c in _db.tbl_AvailablePincode
                               where c.AvailablePincodeId == Id
                               select new AvailablePincodeVM
                               {
                                   AvailablePincodeId = c.AvailablePincodeId,
                                   AvailablePincode = c.AvailablePincode,
                               }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objPincode);
        }

        [HttpPost]
        public ActionResult Edit(AvailablePincodeVM pincodeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existCategory = _db.tbl_AvailablePincode.Where(x => x.AvailablePincodeId != pincodeVM.AvailablePincodeId && x.AvailablePincode.ToLower() == pincodeVM.AvailablePincode.ToLower()).FirstOrDefault();
                    if (existCategory != null)
                    {
                        ModelState.AddModelError("AvailablePincode", ErrorMessage.PincodeExists);
                        return View(pincodeVM);
                    }

                    tbl_AvailablePincode objPincode = _db.tbl_AvailablePincode.Where(x => x.AvailablePincodeId == pincodeVM.AvailablePincodeId).FirstOrDefault();
                    objPincode.AvailablePincode = pincodeVM.AvailablePincode;

                    objPincode.UpdatedBy = LoggedInUserId;
                    objPincode.UpdatedDate = DateTime.UtcNow;
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
        public string DeletePincode(long AvailablePincodeId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_AvailablePincode objPincode = _db.tbl_AvailablePincode.Where(x => x.AvailablePincodeId == AvailablePincodeId).FirstOrDefault();

                if (objPincode == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    _db.tbl_AvailablePincode.Remove(objPincode);

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