using ConstructionDiary.Models;
using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ContactUsMessageController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public ContactUsMessageController()
        {
            _db = new krupagallarydbEntities();
        }

        [AdminPermission(RolePermissionEnum.View)]
        public ActionResult Index()
        {
            List<ContactUsMessageVM> lstContactUsMessageVM = new List<ContactUsMessageVM>();
            try
            {

                lstContactUsMessageVM = (from cu in _db.tbl_ContactFormData                                    
                                         where !cu.IsDelete
                                    select new ContactUsMessageVM
                                    {
                                        ContactUsMsgId = cu.ContactForm_Id,
                                        Name = cu.Name,
                                        MobileNumber = cu.PhoneNumber,
                                        Email = cu.Email,
                                        FromWhere = cu.FromWhere,
                                        Message = cu.Message,
                                        MessageDate = cu.MessageDate
                                    }).OrderByDescending(x => x.MessageDate).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstContactUsMessageVM);
        }

        [HttpPost]
        public string DeleteContactUs(long Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_ContactFormData objtbl_ContactFormData = _db.tbl_ContactFormData.Where(x => x.ContactForm_Id == Id).FirstOrDefault();

                if (objtbl_ContactFormData != null)
                {
                    _db.tbl_ContactFormData.Remove(objtbl_ContactFormData);
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

        [AdminPermission(RolePermissionEnum.View)]
        public ActionResult View(long Id)
        {
            ContactUsMessageVM lstContactUsMessageVM = new ContactUsMessageVM();

            try
            {

                lstContactUsMessageVM = (from cu in _db.tbl_ContactFormData
                                         where cu.ContactForm_Id == Id
                                         select new ContactUsMessageVM
                                         {
                                             ContactUsMsgId = cu.ContactForm_Id,
                                             Name = cu.Name,
                                             MobileNumber = cu.PhoneNumber,
                                             Email = cu.Email,
                                             FromWhere = cu.FromWhere,
                                             Message = cu.Message,
                                             MessageDate = cu.MessageDate
                                         }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstContactUsMessageVM);
        }

    }
}