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
                string ReferralCode = objRegisterVM.ReferallCode;
                bool ReferlValid = true;
                string ReferenceReferralCode = "";
                long ReferenceReferralClientUserId = 0;
                long ReferenceReferralDiscountPoints = 0;
                if (!string.IsNullOrEmpty(ReferralCode))
                {
                    var objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();

               
                    if (!string.IsNullOrWhiteSpace(ReferralCode))
                    {
                        tbl_ClientUsers referralCodeUser = _db.tbl_ClientUsers.Where(x => x.OwnReferralCode.ToLower() == ReferralCode.ToLower()).FirstOrDefault();
                        if (referralCodeUser == null)
                        {
                            ReferlValid = false;
                        }
                        else
                        {
                            ReferenceReferralClientUserId = referralCodeUser.ClientUserId;
                            ReferenceReferralCode = referralCodeUser.OwnReferralCode;
                            ReferenceReferralDiscountPoints = objGensetting.ReferenceReferralDiscountPoints != null ? objGensetting.ReferenceReferralDiscountPoints.Value : 0;
                        }
                    }
                }
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == email.ToLower() && o.ClientRoleId == 1).FirstOrDefault();
                if (objClientUsr != null)
                {
                    response.IsError = true;
                    response.AddError("Your Account is already exist.Please go to Login or Contact to support");                  
                }
                else if(ReferlValid == false)
                {
                    response.IsError = true;
                    response.AddError("Referral Code not found..");
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
                    objClientUsr.Prefix = objRegisterVM.Prefix;
                    objClientUsr.Reference = objRegisterVM.Reference;
                    string randomReferralCode = CommonMethod.GetRandomReferralCode(8);
                    objClientUsr.OwnReferralCode = randomReferralCode;
                    objClientUsr.ReferenceReferralClientUserId = ReferenceReferralClientUserId;
                    objClientUsr.ReferenceReferralCode = ReferenceReferralCode;
                    objClientUsr.ReferencePointReceived = (int)ReferenceReferralDiscountPoints;
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

                    // Give referral discount points to referral code user
                    if (!string.IsNullOrWhiteSpace(ReferralCode))
                    {
                        if (ReferenceReferralDiscountPoints > 0)
                        {
                            tbl_PointDetails objPoint = new tbl_PointDetails();
                            objPoint.ClientUserId = ReferenceReferralClientUserId;
                            objPoint.ExpiryDate = DateTime.UtcNow.AddMonths(6);
                            objPoint.CreatedBy = objclientuser.ClientUserId;
                            objPoint.CreatedDate = DateTime.UtcNow;
                            objPoint.UsedPoints = 0;
                            objPoint.IsReferralPoints = true;
                            objPoint.Points = objGensetting.ReferenceReferralDiscountPoints;
                            _db.tbl_PointDetails.Add(objPoint);
                            _db.SaveChanges();
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

        [Route("SendOTP"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTP(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.MobileNo.ToLower() == MobileNum.ToLower() && o.ClientRoleId == 1).FirstOrDefault();
                bool IsValidReferl = true;
                if (!string.IsNullOrWhiteSpace(objOtpVM.ReferalCode))
                {
                    bool IsReferalCodeFound = _db.tbl_ClientUsers.Where(x => x.OwnReferralCode.ToLower() == objOtpVM.ReferalCode.ToLower()).Any();
                    if (!IsReferalCodeFound)
                    {
                        IsValidReferl = false;
                    }
                }
                if (objClientUsr != null)
                {
                    response.IsError = true;
                    response.AddError("Your Account is already exist with this mobile number.Please go to Login or Contact to support");
                }
                else if(IsValidReferl == false)
                {
                    response.IsError = true;
                    response.AddError("Referral Code is Invalid. Please enter valid referral code");
                }
                else
                {
                    using (WebClient webClient = new WebClient())
                    {
                        WebClient client = new WebClient();
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        //string msg = "Registration's OTP Code Is " + num + "\n Thanks \n Shopping & Saving";
                        int SmsId = (int)SMSType.RegistrationOtp;
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


        [Route("GetReferences"), HttpPost]
        public ResponseDataModel<List<string>> GetReferences()
        {
            ResponseDataModel<List<string>> response = new ResponseDataModel<List<string>>();
            List<string> lstRefer = new List<string>();
            try
            {
                List<tbl_ReferenceMaster> lstref = _db.tbl_ReferenceMaster.Where(o => o.IsDeleted == false).OrderBy(x => x.Reference).ToList();
                if(lstref != null && lstref.Count() > 0)
                {
                    lstRefer = lstref.Select(x => x.Reference).ToList();
                }
                response.Data = lstRefer;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SendDistributorRequest"), HttpPost]
        public ResponseDataModel<string> SendDistributorRequest(DistributorRequestVM objDistributorRequestVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            ClientUserVM objclientuser = new ClientUserVM();
            string strmsg = "";
            try
            {
                string email = objDistributorRequestVM.Email;
                string firstnm = objDistributorRequestVM.FirstName;
                string lastnm = objDistributorRequestVM.LastName;
                string mobileno = objDistributorRequestVM.MobileNo;
                string businessname = objDistributorRequestVM.CompanyName;
                string addharno = objDistributorRequestVM.AddharCardNo;
                string city = objDistributorRequestVM.City;
                string state = objDistributorRequestVM.State;
                string gstno = objDistributorRequestVM.GSTNo;
                string prefix = objDistributorRequestVM.Prefix;
                string dob = objDistributorRequestVM.DateOfBirthString;
                string alternatemobileno = objDistributorRequestVM.AlternateMobile;
                string shopname = objDistributorRequestVM.ShopName;
                string pancardno = objDistributorRequestVM.PanCardNo;

                string photo = objDistributorRequestVM.ProfilePhoto;
                string pancardphotoname = objDistributorRequestVM.PancardPhoto;
                string gstphotoname = objDistributorRequestVM.GSTPhoto;
                string addharphoto = objDistributorRequestVM.AddharPhoto;
                string shopphotoname = objDistributorRequestVM.ShopPhoto;
                string cancellationchequephotoname = objDistributorRequestVM.CancellationChequePhoto;

                tbl_DistributorRequestDetails objRequest = _db.tbl_DistributorRequestDetails.Where(o => (o.Email.ToLower() == email.ToLower() || o.MobileNo.ToLower() == mobileno.ToLower()) && o.IsDelete == false && o.Status == 0).FirstOrDefault();
                if (objRequest != null)
                {
                    response.IsError = true;
                    response.AddError("You have already sent a request with this email or mobile number.");
                }
                else
                {
                    tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => (o.Email.ToLower() == email.ToLower() || o.MobileNo.ToLower() == mobileno.ToLower()) && o.ClientRoleId == 2 && o.IsDelete == false).FirstOrDefault();
                    if (objClientUsr != null)
                    {
                        response.IsError = true;
                        response.AddError("Email or Mobile is already exist. Please try with another email or mobile");                                              
                    }
                    else
                    {                      

                        objRequest = new tbl_DistributorRequestDetails();

                        objRequest.CreatedDate = DateTime.Now;
                        objRequest.FirstName = firstnm;
                        objRequest.LastName = lastnm;
                        objRequest.Email = email;
                        objRequest.MobileNo = mobileno;
                        objRequest.CompanyName = businessname;
                        objRequest.City = city;
                        objRequest.State = state;
                        objRequest.AddharcardNo = addharno;
                        objRequest.GSTNo = gstno;
                        objRequest.AlternateMobileNo = alternatemobileno;
                        objRequest.PanCardNo = pancardno;
                        objRequest.PanCardPhoto = pancardphotoname;
                        objRequest.ProfilePhoto = photo;
                        objRequest.Prefix = prefix;
                        objRequest.ShopName = shopname;
                        objRequest.ShopPhoto = shopphotoname;
                        objRequest.AddharPhoto = addharphoto;
                        objRequest.GSTPhoto = gstphotoname;
                        DateTime dt = DateTime.ParseExact(dob, "dd/MM/yyyy", null);
                        objRequest.Dob = dt;
                        objRequest.IsDelete = false;
                        objRequest.Status = 0;
                        objRequest.Reason = "";
                        objRequest.CancellationChequePhoto = cancellationchequephotoname;
                        _db.tbl_DistributorRequestDetails.Add(objRequest);
                        _db.SaveChanges();
                        tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                        string AdminEmail = objGensetting.AdminEmail;

                        _db.SaveChanges();
                        string FromEmail = objGensetting.FromEmail;
                        if (!string.IsNullOrEmpty(objRequest.Email))
                        {
                            FromEmail = objRequest.Email;
                        }
                        string dobstr = objRequest.Dob.Value.ToString("dd-MMM-yyyy");
                        string Subject = "New Distributor Request - Shopping & Saving";
                        string bodyhtml = "Following Are The Details:<br/>";
                        bodyhtml += "FirstName: " + objRequest.FirstName + "<br/>";
                        bodyhtml += "LastName: " + objRequest.LastName + "<br/>";
                        bodyhtml += "Date of Birth: " + dobstr + "<br/>";
                        bodyhtml += "MobileNo: " + objRequest.MobileNo + "<br/>";
                        bodyhtml += "Alternate MobileNo: " + objRequest.AlternateMobileNo + "<br/>";
                        bodyhtml += "Email: " + objRequest.Email + "<br/>";
                        bodyhtml += "CompanyName: " + objRequest.CompanyName + "<br/>";
                        bodyhtml += "City: " + objRequest.City + "<br/>";
                        bodyhtml += "State: " + objRequest.State + "<br/>";
                        bodyhtml += "Addhar Card No: " + objRequest.AddharcardNo + "<br/>";
                        bodyhtml += "Pan Card No: " + objRequest.PanCardNo + "<br/>";
                        bodyhtml += "GST No: " + objRequest.GSTNo + "<br/>";

                        clsCommon.SendEmail(AdminEmail, FromEmail, Subject, bodyhtml);
                        strmsg = "Request receive Successfully.We will contact you asap.";
                    }
                }

                response.Data = strmsg;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SendOTPDistributorRequest"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTPDistributorRequest(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                string Email = objOtpVM.Email;
                tbl_DistributorRequestDetails objRequest = _db.tbl_DistributorRequestDetails.Where(o => (o.Email.ToLower() == Email.ToLower() || o.MobileNo.ToLower() == MobileNum.ToLower()) && o.IsDelete == false && o.Status == 0).FirstOrDefault();
                if (objRequest != null)
                {
                    response.IsError = true;
                    response.AddError("You have already sent a request with this email or mobile number.");
                }
                else
                {
                    tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => (o.Email.ToLower() == Email.ToLower() || o.MobileNo.ToLower() == MobileNum.ToLower()) && o.ClientRoleId == 2 && o.IsDelete == false).FirstOrDefault();
                    if (objClientUsr != null)
                    {
                        response.IsError = true;
                        response.AddError("Email or Mobile is already exist. Please try with another email or mobile");
                    }
                    else
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            Random random = new Random();
                            int num = random.Next(555555, 999999);
                            //string msg = "Your distributor request OTP code is " + num;
                            int SmsId = (int)SMSType.DistributorReqOtp;
                            clsCommon objcm = new clsCommon();
                            string msg = objcm.GetSmsContent(SmsId);
                            msg = msg.Replace("{{OTP}}", num + "");
                            msg = HttpUtility.UrlEncode(msg);
                            //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", MobileNum).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                response.IsError = true;
                                response.AddError("Invalid Mobile Number");
                            }
                            else
                            {
                                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                                string FromEmail = objGensetting.FromEmail;
                                Random random1 = new Random();
                                int num1 = random1.Next(111566, 499999);
                                string msg1 = "Your Distributor Request OTP Code Is " + num1;
                                clsCommon.SendEmail(Email, FromEmail, "OTP Code For Distributor Request - Shopping & Saving", msg1);
                                objOtp.Otp = num.ToString();
                                objOtp.OtpEmail = num1.ToString();
                                response.Data = objOtp;
                            }

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