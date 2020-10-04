using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly krupagallarydbEntities _db; 

        public ForgotPasswordController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/ForgotPassword
        public ActionResult Index(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();
                    Random random = new Random();
                    int num = random.Next(310450, 789899);
                    //string msg = "Your forgot password OTP code is " + num;
                    int SmsId = (int)SMSType.ForgotPwdOtp;
                    clsCommon objcm = new clsCommon();
                    string msg = objcm.GetSmsContent(SmsId);
                    msg = msg.Replace("{{OTP}}", num + "");
                    //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", MobileNumber).Replace("--MSG--", msg);
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
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

        [HttpPost]
        public string ResetPasswordSubmit(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string mobilenumber = frm["hdnmobilenumber"];
                int usertype = Convert.ToInt32(frm["hdntypes"]);
                string NewPassword = frm["newpwd"];
                tbl_ClientUsers objUser = _db.tbl_ClientUsers.Where(x => x.ClientRoleId == usertype && x.MobileNo == mobilenumber).FirstOrDefault();

                if (objUser != null)
                {
                    objUser.Password = clsCommon.EncryptString(NewPassword);
                    _db.SaveChanges();

                    ReturnMessage = "SUCCESS";
                    Session.Clear();
                }
                else
                {
                    ReturnMessage = "UserNotFound";
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }
    }
}