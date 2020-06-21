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
    public class AgentController : ApiController
    {
        private readonly krupagallarydbEntities _db;
        public AgentController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("DeliveryAppLogin"), HttpPost]
        public ResponseDataModel<AdminUserVM> DeliveryAppLogin(LoginVM objLogin)
        {
            ResponseDataModel<AdminUserVM> response = new ResponseDataModel<AdminUserVM>();
            AdminUserVM objadminuser = new AdminUserVM();
            try
            {
                //string EncyptedPassword = clsCommon.EncryptString(objLogin.Password); // Encrypt(userLogin.Password);

                var data = _db.tbl_AdminUsers.Where(x => x.MobileNo == objLogin.MobileNo && x.Password == objLogin.Password
                                        && !x.IsDeleted).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        response.IsError = true;
                        response.AddError("Your Account is not active. Please contact administrator.");
                    }
                    else
                    {
                        objadminuser.FirstName = data.FirstName;
                        objadminuser.LastName = data.LastName;
                        objadminuser.MobileNo = data.MobileNo;
                        objadminuser.AdminRoleId = data.AdminRoleId;
                        objadminuser.Email = data.Email;
                        objadminuser.AdminUserId = data.AdminUserId;
                        response.Data = objadminuser;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError("Invalid MobileNo or Password");
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("DelieveryPersonsOfAgents"), HttpPost]
        public ResponseDataModel<List<AdminUserVM>> DelieveryPersonsOfAgents(GeneralVM objGeneral)
        {
            ResponseDataModel<List<AdminUserVM>> response = new ResponseDataModel<List<AdminUserVM>>();
            List<AdminUserVM> lstusers = new List<AdminUserVM>();
            try
            {
                long AgntUserId = Convert.ToInt64(objGeneral.ClientUserId);
         
                lstusers = (from p in _db.tbl_AdminUsers                            
                             where p.ParentAgentId == AgntUserId
                             select new AdminUserVM
                             {
                                 AdminUserId = p.AdminUserId,
                                 FirstName = p.FirstName,
                                 LastName = p.LastName,
                                 MobileNo = p.MobileNo,
                                 Address = p.Address,
                                 WorkingTime = p.WorkingTime                             
                             }).OrderBy(x => x.FirstName).ToList();

                response.Data = lstusers;            

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
                tbl_AdminUsers objadminusr = _db.tbl_AdminUsers.Where(o => (o.MobileNo.ToLower() == MobileNum) && o.IsDeleted == false && o.IsActive == true).FirstOrDefault();
                if (objadminusr == null)
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
                        string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNum + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                        var json = webClient.DownloadString(url);
                        if (json.Contains("invalidnumber"))
                        {
                            response.AddError("Invalid Mobile Number");
                        }
                        else
                        {                           
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