using KrupaBuildGallery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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
                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                string AdminEmail = objGensetting.AdminEmail;

                _db.SaveChanges();
                string FromEmail = objGensetting.FromEmail;

                //if (!string.IsNullOrEmpty(objContactform.Email))
                //{
                //    FromEmail = objContactform.Email;
                //}

                string Subject = "Message From Shopping & Saving";
                string bodyhtml = "Following are the message details:<br/>";
                bodyhtml += "Name: " + objContactform.Name +"<br/>";
                bodyhtml += "Email: " + objContactform.Email + "<br/>";
                bodyhtml += "PhoneNumber: " + objContactform.PhoneNumber + "<br/>";
                bodyhtml += "Message: " + objContactform.Message + "<br/>";
                clsCommon.SendEmail(AdminEmail, FromEmail, Subject, bodyhtml);
                TempData["Message"] = "Message Sent Successfully...";
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["Message"] = ErrorMessage;
            }

            return RedirectToAction("Index", "ContactUs", new { area = "Client" });

        }
        public string SendOTP(string MobileNumber)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();
                    Random random = new Random();
                    int num = random.Next(111566,999999);
                    string msg = "Your contact form request OTP code is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message="+ msg+"&sendername=KRUPAB&smstype=TRANS&numbers="+ MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if(json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return num.ToString();
                    }
                    
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}