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
                        objclientuser.CartCount = 0;
                        long ClientUsrId = data.ClientUserId;                        
                        UpdatCarts(objLogin.SessionUniqueId, objclientuser.ClientUserId);
                        var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == ClientUsrId).ToList();
                        if (cartlist != null && cartlist.Count() > 0)
                        {
                            objclientuser.SessionUniqueId = cartlist.FirstOrDefault().CartSessionId;
                            objclientuser.CartCount = cartlist.Count();
                        }
                        else
                        {
                            objclientuser.SessionUniqueId = "cust" + objclientuser.ClientUserId;
                        }
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
                        objclientuser.CartCount = 0;
                        long ClientUsrId = data.ClientUserId;                     
                        UpdatCarts(objLogin.SessionUniqueId, objclientuser.ClientUserId);
                        var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == ClientUsrId).ToList();
                        if (cartlist != null && cartlist.Count() > 0)
                        {
                            objclientuser.SessionUniqueId = cartlist.FirstOrDefault().CartSessionId;
                            objclientuser.CartCount = cartlist.Count();
                        }
                        else
                        {
                            objclientuser.SessionUniqueId = "cust" + objclientuser.ClientUserId;
                        }
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
                            string msg1 = "Your Otp Code For Login Is " + num;
                            try
                            {
                                clsCommon.SendEmail(objClientUsr.Email, FromEmail, "OTP Code for Login - Shopping & Saving", msg1);
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
        public void UpdatCarts(string SessionIdUniq,long ClientUserId)
        {

            string GuidNew = "cust" + ClientUserId; 
            string cookiesessionval = "";
            if (!string.IsNullOrEmpty(SessionIdUniq))
            {
                cookiesessionval = SessionIdUniq;
            }
            
            if (ClientUserId > 0)
            {
                if (string.IsNullOrEmpty(cookiesessionval))
                {
                    cookiesessionval = GuidNew;
                }
                long clientusrid = Convert.ToInt64(ClientUserId);
                var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                if (cartlist != null && cartlist.Count() > 0)
                {
                    string sessioncrtid = cartlist.FirstOrDefault().CartSessionId;
                    var cartlistsessions = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval && o.ClientUserId == 0).ToList();
                    if (cartlistsessions != null && cartlistsessions.Count() > 0)
                    {
                        foreach (var obj in cartlistsessions)
                        {
                            var objcrtsession = cartlist.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                            if (objcrtsession != null)
                            {
                                objcrtsession.CartItemQty = objcrtsession.CartItemQty + obj.CartItemQty;
                                _db.tbl_Cart.Remove(obj);
                            }
                            else
                            {
                                var crtobj1 = new tbl_Cart();
                                crtobj1.CartItemId = obj.CartItemId;
                                crtobj1.CartItemQty = obj.CartItemQty;
                                crtobj1.CartSessionId = sessioncrtid;
                                crtobj1.ClientUserId = clientusrid;
                                crtobj1.CreatedDate = DateTime.Now;
                                _db.tbl_Cart.Add(crtobj1);
                                _db.tbl_Cart.Remove(obj);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var cartlistsessions = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                    foreach (var obj in cartlistsessions)
                    {
                        var objcrtsession = cartlist.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                        var crtobj1 = new tbl_Cart();
                        crtobj1.CartItemId = obj.CartItemId;
                        crtobj1.CartItemQty = obj.CartItemQty;
                        crtobj1.CartSessionId = GuidNew;
                        crtobj1.ClientUserId = clientusrid;
                        crtobj1.CreatedDate = DateTime.Now;
                        _db.tbl_Cart.Add(crtobj1);
                        _db.tbl_Cart.Remove(obj);
                        _db.SaveChanges();
                    }
                }

            }
        }
        

    }
}