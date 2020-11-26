using ConstructionDiary.Models;
using KrupaBuildGallery.Filters;
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
    public class SMSController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public SMSController()
        {
            _db = new krupagallarydbEntities();
        }

        [AdminPermission(RolePermissionEnum.View)]
        public ActionResult Index()
        {
            List<SMSContentVM> lstSMS = (from s in _db.tbl_SMSContent
                                         select new SMSContentVM
                                         {
                                             SMSContentId = s.SMSContentId,
                                             SMSTitle = s.SMSTitle,
                                             SMSDescription = s.SMSDescription,
                                             SeqNo = s.SeqNo
                                         }).OrderBy(x => x.SeqNo).ToList();
            return View(lstSMS);
        }

        [AdminPermission(RolePermissionEnum.Edit)]
        public ActionResult Edit(int Id)
        {
            SMSContentVM objSMS = (from s in _db.tbl_SMSContent
                                   where s.SMSContentId == Id
                                   select new SMSContentVM
                                   {
                                       SMSContentId = s.SMSContentId,
                                       SMSTitle = s.SMSTitle,
                                       SMSDescription = s.SMSDescription,
                                       SeqNo = s.SeqNo
                                   }).FirstOrDefault();
            return View(objSMS);
        }

        [HttpPost]
        public ActionResult Edit(SMSContentVM smsVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    tbl_SMSContent objCategory = _db.tbl_SMSContent.Where(x => x.SMSContentId == smsVM.SMSContentId).FirstOrDefault();
                    objCategory.SMSTitle = smsVM.SMSTitle;
                    objCategory.SMSDescription = smsVM.SMSDescription;
                    objCategory.SeqNo = smsVM.SeqNo;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(smsVM);
        }

    }
}