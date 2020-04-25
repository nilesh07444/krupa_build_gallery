using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using KrupaBuildGallery.ViewModel;
using System.Net;
using System.Configuration;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class LoginController : ApiController
    {
        krupagallarydbEntities _db;
        public LoginController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: WebAPI/Login
        [Route("CheckLogin"), HttpPost]
        public ResponseDataModel<ClientUserVM> CheckLogin(LoginVM objLogin)
        {
            ResponseDataModel<ClientUserVM> response = new ResponseDataModel<ClientUserVM>();
            ClientUserVM objclientuser = new ClientUserVM();
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(objLogin.Password); // Encrypt(userLogin.Password);

                var data = _db.tbl_ClientUsers.Where(x => (x.MobileNo == objLogin.MobileNo && x.ClientRoleId == 1)
                && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        response.IsError = true;
                        response.AddError("Your Account is not active. Please contact administrator.");
                    }
                    else
                    {
                        objclientuser.FirstName = data.FirstName;
                        objclientuser.LastName = data.LastName;
                        objclientuser.MobileNo = data.MobileNo;
                        objclientuser.RoleId = data.ClientRoleId;
                        objclientuser.Email = data.Email;
                        objclientuser.ClientUserId = data.ClientUserId;
                        response.Data = objclientuser;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError("Invalid mobilenumber or password."); 
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("CheckLoginDistributor"), HttpPost]
        public ResponseDataModel<ClientUserVM> CheckLoginDistributor(LoginVM objLogin)
        {
            ResponseDataModel<ClientUserVM> response = new ResponseDataModel<ClientUserVM>();
            ClientUserVM objclientuser = new ClientUserVM();
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(objLogin.Password); // Encrypt(userLogin.Password);                
                var data = _db.tbl_ClientUsers.Where(x => (x.Email.ToLower() == objLogin.MobileNo.ToLower() || x.MobileNo.ToLower() == objLogin.MobileNo.ToLower()) && x.ClientRoleId == 2 && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        response.IsError = true;
                        response.AddError("Your Account is not active. Please contact administrator.");
                    }
                    else
                    {
                        objclientuser.FirstName = data.FirstName;
                        objclientuser.LastName = data.LastName;
                        objclientuser.MobileNo = data.MobileNo;
                        objclientuser.RoleId = data.ClientRoleId;
                        objclientuser.Email = data.Email;
                        objclientuser.ClientUserId = data.ClientUserId;
                        response.Data = objclientuser;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError("Invalid mobilenumber/email or password.");
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SendOTP"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTP(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => (o.Email.ToLower() == MobileNum || o.MobileNo.ToLower() == MobileNum.ToLower()) && o.ClientRoleId == 2 && o.IsDelete == false && o.IsActive == true).FirstOrDefault();
                if (objClientUsr == null)
                {
                    response.AddError("Your Account is not exist.Please Contact to support");
                }
                else
                {
                    using (WebClient webClient = new WebClient())
                    {
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        string msg = "Your Otp code for Login is " + num;
                        string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objClientUsr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                        var json = webClient.DownloadString(url);
                        if (json.Contains("invalidnumber"))
                        {
                            response.AddError("Invalid Mobile Number");
                        }
                        else
                        {
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;
                            string msg1 = "Your Otp code for Login is " + num;
                            try
                            {
                                clsCommon.SendEmail(objClientUsr.Email, FromEmail, "OTP Code for Login - Krupa Build Gallery", msg1);
                            }
                            catch (Exception e)
                            {
                                string ErrorMessage = e.Message.ToString();
                            }
                            objOtp.Otp = num.ToString();
                            response.Data = objOtp;
                        }

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

    }
}