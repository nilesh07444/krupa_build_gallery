using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using KrupaBuildGallery.ViewModel;
using System.Net;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class RegisterController : ApiController
    {
        krupagallarydbEntities _db;
        public RegisterController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: WebAPI/Login
        [Route("CreateAccount"), HttpPost]
        public ResponseDataModel<ClientUserVM> CreateAccount(RegisterVM objRegisterVM)
        {
            ResponseDataModel<ClientUserVM> response = new ResponseDataModel<ClientUserVM>();
            ClientUserVM objclientuser = new ClientUserVM();
            try
            {
                string email = objRegisterVM.Email;
                string firstnm = objRegisterVM.FirstName;
                string lastnm = objRegisterVM.LastName;
                string mobileno = objRegisterVM.MobileNo;
                string password = objRegisterVM.Password;
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == email.ToLower() && o.ClientRoleId == 1).FirstOrDefault();
                if (objClientUsr != null)
                {
                    response.IsError = true;
                    response.AddError("Your Account is already exist.Please go to Login or Contact to support");                  
                }
                else
                {
                    string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);
                    objClientUsr = new tbl_ClientUsers();
                    objClientUsr.Email = email;
                    objClientUsr.ClientRoleId = 1;
                    objClientUsr.CreatedBy = 0;
                    objClientUsr.CreatedDate = DateTime.UtcNow;
                    objClientUsr.FirstName = firstnm;
                    objClientUsr.LastName = lastnm;
                    objClientUsr.MobileNo = mobileno;
                    objClientUsr.IsActive = true;
                    objClientUsr.IsDelete = false;
                    objClientUsr.ProfilePicture = "";
                    objClientUsr.UserName = firstnm + lastnm;
                    objClientUsr.Password = EncyptedPassword;
                    _db.tbl_ClientUsers.Add(objClientUsr);
                    _db.SaveChanges();

                    tbl_ClientOtherDetails objtbl_ClientOtherDetails = new tbl_ClientOtherDetails();
                    objtbl_ClientOtherDetails.ClientUserId = objClientUsr.ClientUserId;
                    objtbl_ClientOtherDetails.IsActive = true;
                    objtbl_ClientOtherDetails.IsDelete = false;
                    objtbl_ClientOtherDetails.CreatedDate = DateTime.Now;
                    objtbl_ClientOtherDetails.CreatedBy = objClientUsr.ClientUserId;
                    _db.tbl_ClientOtherDetails.Add(objtbl_ClientOtherDetails);
                    _db.SaveChanges();

                    objclientuser.ClientUserId = objClientUsr.ClientUserId;
                    objclientuser.RoleId = Convert.ToInt32(objClientUsr.ClientRoleId);
                    objclientuser.FirstName = objClientUsr.FirstName;
                    objclientuser.LastName = objClientUsr.LastName;
                    objclientuser.MobileNo = objClientUsr.MobileNo;
                    objclientuser.Email = objClientUsr.Email;
               
                    var objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    if (objGensetting != null)
                    {
                        tbl_PointDetails objPoint = new tbl_PointDetails();
                        objPoint.ClientUserId = objclientuser.ClientUserId;
                        objPoint.ExpiryDate = DateTime.UtcNow.AddMonths(6);
                        objPoint.CreatedBy = objclientuser.ClientUserId;
                        objPoint.CreatedDate = DateTime.UtcNow;
                        objPoint.UsedPoints = 0;
                        objPoint.Points = objGensetting.InitialPointCustomer;
                        _db.tbl_PointDetails.Add(objPoint);
                        _db.SaveChanges();
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

        [Route("SendOTP"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTP(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.MobileNo.ToLower() == MobileNum.ToLower() && o.ClientRoleId == 1).FirstOrDefault();

                if (objClientUsr != null)
                {
                    response.IsError = true;
                    response.AddError("Your Account is already exist with this mobile number.Please go to Login or Contact to support");
                }
                else
                {
                    using (WebClient webClient = new WebClient())
                    {
                        WebClient client = new WebClient();
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        string msg = "Registration's OTP code is " + num + "\n Thanks \n Krupa Build Gallery";
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
    }
}