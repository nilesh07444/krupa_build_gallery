﻿using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class ForgotPasswordController : ApiController
    {
        krupagallarydbEntities _db;
        public ForgotPasswordController()
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
                        string msg = "Your forgot password OTP code is " + num;
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

        [Route("ResetPassword"), HttpPost]
        public ResponseDataModel<OtpVM> ResetPassword(PwdVM objPwdVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            try
            {
                string mobilenumber = objPwdVM.MobileNumber;
                int usertype = Convert.ToInt32(objPwdVM.UserType);
                string NewPassword = objPwdVM.NewPassword;
                tbl_ClientUsers objUser = _db.tbl_ClientUsers.Where(x => x.ClientRoleId == usertype && x.MobileNo == mobilenumber).FirstOrDefault();
                if (objUser != null)
                {
                    objUser.Password = clsCommon.EncryptString(NewPassword);
                    _db.SaveChanges();
                    response.IsError = false;                    
                }
                else
                {
                    response.AddError("User Not found");
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