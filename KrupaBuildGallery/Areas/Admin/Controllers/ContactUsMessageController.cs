using ConstructionDiary.Models;
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
        // GET: Admin/ContactUsMessage
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
    }
}