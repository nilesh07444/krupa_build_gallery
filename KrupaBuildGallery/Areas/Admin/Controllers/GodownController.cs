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
    public class GodownController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public GodownController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {
            List<GodownVM> lstGodown = new List<GodownVM>();
            try
            {

                lstGodown = (from c in _db.tbl_Godown
                             where !c.IsDeleted
                             select new GodownVM
                             {
                                 GodownId = c.GodownId,
                                 GodownName = c.GodownName,
                                 IsActive = c.IsActive
                             }).OrderByDescending(x => x.GodownId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstGodown);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(GodownVM godownVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    var dataExist = _db.tbl_Godown.Where(x => x.GodownName.ToLower() == godownVM.GodownName.ToLower() && !x.IsDeleted).FirstOrDefault();
                    if (dataExist != null)
                    {
                        ModelState.AddModelError("GodownName", ErrorMessage.GodownNameExists);
                        return View(godownVM);
                    }

                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    tbl_Godown objGodown = new tbl_Godown();
                    objGodown.GodownName = godownVM.GodownName;
                    objGodown.IsActive = true;
                    objGodown.IsDeleted = false;
                    objGodown.CreatedBy = LoggedInUserId;
                    objGodown.CreatedDate = DateTime.UtcNow;
                    objGodown.UpdatedBy = LoggedInUserId;
                    objGodown.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_Godown.Add(objGodown);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(godownVM);
        }

        public ActionResult Edit(int Id)
        {
            GodownVM objGodown = new GodownVM();

            objGodown = (from c in _db.tbl_Godown
                         where c.GodownId == Id
                         select new GodownVM
                         {
                             GodownId = c.GodownId,
                             GodownName = c.GodownName,
                             IsActive = c.IsActive
                         }).FirstOrDefault();

            return View(objGodown);
        }

        [HttpPost]
        public ActionResult Edit(GodownVM godownVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    var dataExist = _db.tbl_Godown.Where(x => x.GodownName.ToLower() == godownVM.GodownName.ToLower() && !x.IsDeleted && x.GodownId != godownVM.GodownId).FirstOrDefault();
                    if (dataExist != null)
                    {
                        ModelState.AddModelError("GodownName", ErrorMessage.GodownNameExists);
                        return View(godownVM);
                    }

                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    tbl_Godown objGodown = _db.tbl_Godown.Where(x => x.GodownId == godownVM.GodownId).FirstOrDefault();
                    objGodown.GodownName = godownVM.GodownName;
                    objGodown.UpdatedBy = LoggedInUserId;
                    objGodown.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(godownVM);
        }

        [HttpPost]
        public string DeleteGodown(int GodownId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Godown objGodown = _db.tbl_Godown.Where(x => x.GodownId == GodownId).FirstOrDefault();

                if (objGodown == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    objGodown.IsDeleted = true;
                    objGodown.UpdatedBy = LoggedInUserId;
                    objGodown.UpdatedDate = DateTime.UtcNow;
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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Godown objHome = _db.tbl_Godown.Where(x => x.GodownId == Id).FirstOrDefault();

                if (objHome != null)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objHome.IsActive = true;
                    }
                    else
                    {
                        objHome.IsActive = false;
                    }

                    objHome.UpdatedBy = LoggedInUserId;
                    objHome.UpdatedDate = DateTime.UtcNow;

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