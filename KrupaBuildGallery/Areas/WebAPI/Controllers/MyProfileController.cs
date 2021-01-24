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
                long clientroleid = 0;
                if (!string.IsNullOrEmpty(objOtpVM.UserType))
                {
                    clientroleid = Convert.ToInt64(objOtpVM.UserType);
                }
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => (clientroleid == 0 || o.ClientRoleId == clientroleid) && o.MobileNo.ToLower() == MobileNum.ToLower() && o.IsDelete == false && o.IsActive == true).FirstOrDefault();                
                if (objClientUsr == null)
                {
                    response.IsError = true;
                    response.AddError("Account is not exist or active with this mobile number.Please go to Login or Contact to support");
                }
                else
                {
                    bool iserrr = false;
                    if(clientroleid > 0 && !string.IsNullOrEmpty(objOtpVM.Password))
                    {
                        string Password = objClientUsr.Password;
                        string currpwd = objOtpVM.Password;
                        string EncryptedCurrentPassword = clsCommon.EncryptString(currpwd);
                        if (Password != EncryptedCurrentPassword)
                        {
                            response.IsError = true;
                            response.AddError("Current Password not match");
                            iserrr = true;
                        }                       
                    }

                    if(iserrr == false)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(310450, 789899);
                            // string msg = "Your change password OTP code is " + num;
                            int SmsId = (int)SMSType.ChangePwdOtp;
                            clsCommon objcm = new clsCommon();
                            string msg = objcm.GetSmsContent(SmsId);
                            msg = msg.Replace("{{OTP}}", num + "");
                            msg = HttpUtility.UrlEncode(msg);
                            //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNum + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", MobileNum).Replace("--MSG--", msg);
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

                string Subject = "Message From Shopping & Saving";
                string bodyhtml = "Following Are The Message Details:<br/>";
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

        [Route("SendOTPAdmin"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTPAdmin(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                tbl_AdminUsers objAdminUsr = _db.tbl_AdminUsers.Where(o => o.MobileNo.ToLower() == MobileNum.ToLower()).FirstOrDefault();

                if (objAdminUsr == null)
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
                        //string msg = "Your change password OTP code is " + num;
                        int SmsId = (int)SMSType.ChangePwdOtp;
                        clsCommon objcm = new clsCommon();
                        string msg = objcm.GetSmsContent(SmsId);
                        msg = msg.Replace("{{OTP}}", num + "");
                        msg = HttpUtility.UrlEncode(msg);
                        //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNum + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                        string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", MobileNum).Replace("--MSG--", msg);
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

        [Route("ChangePasswordAdmin"), HttpPost]
        public ResponseDataModel<OtpVM> ChangePasswordAdmin(PwdVM objPwdVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            try
            {
                string CurrentPassword = objPwdVM.OldPassword;
                string NewPassword = objPwdVM.NewPassword;

                long LoggedInUserId = Int64.Parse(objPwdVM.ClientUserId);
                tbl_AdminUsers objUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == LoggedInUserId).FirstOrDefault();

                if (objUser != null)
                {
                    //string EncryptedCurrentPassword = clsCommon.EncryptString(CurrentPassword);
                    if (objUser.Password == CurrentPassword)
                    {
                        objUser.Password = NewPassword;// clsCommon.EncryptString(NewPassword);
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

        [Route("ProfileInfo"), HttpPost]
        public ResponseDataModel<GeneralVM> ProfileInfo(GeneralVM objGeneralVM)
        {
            ResponseDataModel<GeneralVM> response = new ResponseDataModel<GeneralVM>();
            GeneralVM objGenVm = new GeneralVM();
            try
            {
                long userid = Convert.ToInt64(objGeneralVM.ClientUserId);
                objGenVm.ClientUserId = Convert.ToString(userid);
                if (userid > 0)
                {
                    tbl_ClientUsers objClientUser = _db.tbl_ClientUsers.Where(o => o.ClientUserId == userid).FirstOrDefault();
                    decimal waltamt = objClientUser.WalletAmt.HasValue ? objClientUser.WalletAmt.Value : 0;
                    DateTime dtNow = DateTime.UtcNow;
                    List<tbl_PointDetails> lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == userid && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();
                    decimal pointreamining = 0;
                    if (lstpoints != null && lstpoints.Count() > 0)
                    {
                        pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
                    }
                    objGenVm.TotalWalletAmt = Convert.ToString(waltamt);
                    objGenVm.TotalPoints = Convert.ToString(pointreamining);
                    string refrelcod = "";
                    if(!string.IsNullOrEmpty(objClientUser.OwnReferralCode))
                    {
                        refrelcod = objClientUser.OwnReferralCode;
                    }
                    objGenVm.ReferalCode = refrelcod;

                }
                response.Data = objGenVm;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SaveProfile"), HttpPost]
        public ResponseDataModel<string> SaveProfile(GeneralVM objGeneralVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            response.Data = "";
            try
            {
                long userid = Convert.ToInt64(objGeneralVM.ClientUserId);
                long roleid = Convert.ToInt64(objGeneralVM.RoleId);
                if(!string.IsNullOrEmpty(objGeneralVM.Email))
                {
                   var objclnt = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == objGeneralVM.Email.ToLower() && o.ClientRoleId == roleid && o.ClientUserId != userid && o.IsDelete == false).FirstOrDefault();
                   if(objclnt != null)
                    {
                        response.AddError("Email Already Exist");
                        response.Data = "Email Already Exist";
                    }
                    else
                    {
                        tbl_ClientUsers objclint = _db.tbl_ClientUsers.Where(o => o.ClientUserId == userid).FirstOrDefault();
                        objclint.FirstName = objGeneralVM.FirstName;
                        objclint.LastName = objGeneralVM.LastName;
                        objclint.Email = objGeneralVM.Email;
                        objclint.Prefix = objGeneralVM.Prefix;
                        _db.SaveChanges();
                    }
                }
                else
                {
                    tbl_ClientUsers objclint = _db.tbl_ClientUsers.Where(o => o.ClientUserId == userid).FirstOrDefault();
                    objclint.FirstName = objGeneralVM.FirstName;
                    objclint.LastName = objGeneralVM.LastName;
                    objclint.Email = objGeneralVM.Email;
                    objclint.Prefix = objGeneralVM.Prefix;
                    _db.SaveChanges();
                }
                response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }


        [Route("GetShippingAddresses"), HttpPost]
        public ResponseDataModel<List<tbl_ShippingAddresses>> GetShippingAddresses(GeneralVM objGeneralVM)
        {
            ResponseDataModel<List<tbl_ShippingAddresses>> response = new ResponseDataModel<List<tbl_ShippingAddresses>>();
            GeneralVM objGenVm = new GeneralVM();
            try
            {
                long userid = Convert.ToInt64(objGeneralVM.ClientUserId);
                objGenVm.ClientUserId = Convert.ToString(userid);
                List<tbl_ShippingAddresses> lstShippingAddress = _db.tbl_ShippingAddresses.Where(o => o.ClientUserId == userid && o.IsDeleted == false).ToList();
                response.Data = lstShippingAddress;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }


        [Route("SaveShippingAddress"), HttpPost]
        public ResponseDataModel<string> SaveShippingAddress(ShipAddressVM objShipAddress)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            GeneralVM objGenVm = new GeneralVM();
            bool IsValid = true;
            try
            {
                long userid = Convert.ToInt64(objShipAddress.ClientUserId);
                tbl_ShippingAddresses objShip = new tbl_ShippingAddresses();
                if (objShipAddress.ShippingAddressId == 0)
                {
                    var objshipexist = _db.tbl_ShippingAddresses.Where(o => o.AddressTitle.ToLower() == objShipAddress.AddressTitle.ToLower() && o.ClientUserId == objShipAddress.ClientUserId && o.IsDeleted == false).FirstOrDefault();
                    if(objshipexist != null)
                    {
                        response.AddError("Shipping Title Already Exist");
                        IsValid = false;
                    }
                    objShip.CreatedDate = DateTime.UtcNow;
                }
                else
                {
                    long shipadreid = Convert.ToInt64(objShipAddress.ShippingAddressId);
                    var objshipexist = _db.tbl_ShippingAddresses.Where(o => o.AddressTitle.ToLower() == objShipAddress.AddressTitle.ToLower() && o.ShippingAddressId != shipadreid && o.ClientUserId == objShipAddress.ClientUserId && o.IsDeleted == false).FirstOrDefault();
                    if (objshipexist != null)
                    {
                        response.AddError("Shipping Title Already Exist");
                        IsValid = false;
                    }

                    objShip = _db.tbl_ShippingAddresses.Where(o => o.ShippingAddressId == shipadreid).FirstOrDefault();
                }
                if(IsValid)
                {
                    objShip.ShipAddress = objShipAddress.ShipAddress;
                    objShip.ShipCity = objShipAddress.ShipCity;
                    objShip.ShipEmail = objShipAddress.ShipEmail;
                    objShip.ShipPhoneNumber = objShipAddress.ShipPhoneNumber;
                    objShip.ShipFirstName = objShipAddress.ShipFirstName;
                    objShip.ShipLastName = objShipAddress.ShipLastName;
                    objShip.ShipState = objShipAddress.ShipState;
                    objShip.ShipPostalCode = objShipAddress.ShipPostalCode;
                    objShip.AddressTitle = objShipAddress.AddressTitle;
                    objShip.ClientUserId = objShipAddress.ClientUserId;
                    objShip.IsDeleted = false;
                    objShip.GSTNo = objShipAddress.GSTNo;
                    if (objShipAddress.ShippingAddressId == 0)
                    {
                        _db.tbl_ShippingAddresses.Add(objShip);
                    }
                    _db.SaveChanges();                  
                }
                if (IsValid)
                {
                    response.Data = "Success";
                }
                else
                {
                    response.Data = "Already Exist";
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