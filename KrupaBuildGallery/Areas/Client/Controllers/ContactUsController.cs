using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public ContactUsController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SendContactUsMessage(FormCollection frm)
        {           
            try
            {
                tbl_ContactFormData objContactform = new tbl_ContactFormData();
                objContactform.Name = frm["firstname"].ToString() + " " + frm["lastname"].ToString();
                objContactform.Email = frm["email"].ToString();
                objContactform.PhoneNumber = frm["phonenumber"].ToString();
                objContactform.Message = frm["txtmessage"].ToString();
                objContactform.MessageDate = DateTime.Now;
                objContactform.IsDelete = false;
                objContactform.FromWhere = "Web";
                objContactform.ClientUserId = clsClientSession.UserID;
                _db.tbl_ContactFormData.Add(objContactform);
                _db.SaveChanges();
                TempData["Message"] = "Message Sent Successfully...";
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["Message"] = ErrorMessage;
            }

            return RedirectToAction("Index", "ContactUs", new { area = "Client" });

        }
    }
}