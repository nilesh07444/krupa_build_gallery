using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class ReferenceController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public ReferenceController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {
            List<ReferenceVM> lstReference = new List<ReferenceVM>();
            try
            {

                lstReference = (from c in _db.tbl_ReferenceMaster
                                where c.IsDeleted == false
                                select new ReferenceVM
                                {
                                    ReferenceId = c.ReferenceId,
                                    Reference = c.Reference
                                }).OrderByDescending(x => x.ReferenceId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstReference);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(ReferenceVM referenceVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existReference = _db.tbl_ReferenceMaster.Where(x => x.Reference.ToLower() == referenceVM.Reference.ToLower()).FirstOrDefault();
                    if (existReference != null)
                    {
                        ModelState.AddModelError("Reference", ErrorMessage.PincodeExists);
                        return View(referenceVM);
                    }

                    tbl_ReferenceMaster objReference = new tbl_ReferenceMaster();
                    objReference.Reference = referenceVM.Reference;
                    objReference.IsDeleted = false;
                    objReference.CreatedBy = LoggedInUserId;
                    objReference.CreatedDate = DateTime.UtcNow;
                    objReference.UpdatedBy = LoggedInUserId;
                    objReference.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_ReferenceMaster.Add(objReference);

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(referenceVM);
        }

        public ActionResult Edit(int Id)
        {
            ReferenceVM objReference = new ReferenceVM();

            try
            {
                objReference = (from c in _db.tbl_ReferenceMaster
                                where c.ReferenceId == Id
                                select new ReferenceVM
                                {
                                    ReferenceId = c.ReferenceId,
                                    Reference = c.Reference,
                                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objReference);
        }

        [HttpPost]
        public ActionResult Edit(ReferenceVM referenceVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existReference = _db.tbl_ReferenceMaster.Where(x => x.ReferenceId != referenceVM.ReferenceId && x.Reference.ToLower() == referenceVM.Reference.ToLower()).FirstOrDefault();
                    if (existReference != null)
                    {
                        ModelState.AddModelError("Reference", ErrorMessage.PincodeExists);
                        return View(referenceVM);
                    }

                    tbl_ReferenceMaster objReference = _db.tbl_ReferenceMaster.Where(x => x.ReferenceId == referenceVM.ReferenceId).FirstOrDefault();
                    objReference.Reference = referenceVM.Reference;

                    objReference.UpdatedBy = LoggedInUserId;
                    objReference.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(referenceVM);
        }

        [HttpPost]
        public string DeleteReference(long ReferenceId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_ReferenceMaster objReference = _db.tbl_ReferenceMaster.Where(x => x.ReferenceId == ReferenceId).FirstOrDefault();

                if (objReference == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    _db.tbl_ReferenceMaster.Remove(objReference);

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