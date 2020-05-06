using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class MyProfileController : ApiController
    {
        krupagallarydbEntities _db;
        public MyProfileController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("SendOTP"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTP(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.MobileNo.ToLower() == MobileNum.ToLower()).FirstOrDefault();

                if (objClientUsr == null)
                {
                    response.IsError = true;
                    response.AddError("Account is not exist with this mobile number.Please go to Login or Contact to support");
                }
                else
                {
                    using (WebClient webClient = new WebClient())
                    {
                        WebClient client = new WebClient();
                        Random random = new Random();
                        int num = random.Next(310450, 789899);
                        string msg = "Your change password OTP code is " + num;
                        string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNum + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                        var json = webClient.DownloadString(url);
                        if (json.Contains("invalidnumber"))
                        {
                            response.IsError = true;
                            response.AddError("Invalid Mobile Number");
                        }
                        else
                        {
                            objOtp.Otp = num.ToString();
                        }
                        response.Data = objOtp;
                    }
                }
             
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("ChangePassword"), HttpPost]
        public ResponseDataModel<OtpVM> ChangePassword(PwdVM objPwdVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            try
            {
                string CurrentPassword = objPwdVM.OldPassword;
                string NewPassword = objPwdVM.NewPassword;

                long LoggedInUserId = Int64.Parse(objPwdVM.ClientUserId);
                tbl_ClientUsers objUser = _db.tbl_ClientUsers.Where(x => x.ClientUserId == LoggedInUserId).FirstOrDefault();

                if (objUser != null)
                {
                    string EncryptedCurrentPassword = clsCommon.EncryptString(CurrentPassword);
                    if (objUser.Password == EncryptedCurrentPassword)
                    {
                        objUser.Password = clsCommon.EncryptString(NewPassword);
                        _db.SaveChanges();                        
                    }
                    else
                    {
                        response.AddError("Current Password not match");
                    }
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SendContactUsMessage"), HttpPost]
        public ResponseDataModel<string> SendContactUsMessage(ContactUsMessageVM objContact)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                
                tbl_ContactFormData objContactform = new tbl_ContactFormData();
                objContactform.Name = objContact.Name;
                objContactform.Email = objContact.Email;
                objContactform.PhoneNumber = objContact.MobileNumber;
                objContactform.Message = objContact.Message;
                objContactform.MessageDate = DateTime.UtcNow;
                objContactform.IsDelete = false;
                objContactform.FromWhere = "Mobile";
                objContactform.ClientUserId = Convert.ToInt64(objContact.ClientUserId);
                _db.tbl_ContactFormData.Add(objContactform);
                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                string AdminEmail = objGensetting.AdminEmail;

                _db.SaveChanges();
                string FromEmail = objGensetting.FromEmail;

                //if (!string.IsNullOrEmpty(objContactform.Email))
                //{
                //    FromEmail = objContactform.Email;
                //}

                string Subject = "Message From Krupa Build Gallery";
                string bodyhtml = "Following are the message details:<br/>";
                bodyhtml += "Name: " + objContactform.Name + "<br/>";
                bodyhtml += "Email: " + objContactform.Email + "<br/>";
                bodyhtml += "PhoneNumber: " + objContactform.PhoneNumber + "<br/>";
                bodyhtml += "Message: " + objContactform.Message + "<br/>";
                clsCommon.SendEmail(AdminEmail, FromEmail, Subject, bodyhtml);
                response.Data = "Message Sent Successfully...";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }
    }
}