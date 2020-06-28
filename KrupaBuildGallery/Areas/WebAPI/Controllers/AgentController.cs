using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
                                ProfilePicture = p.ProfilePicture,
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                Email = p.Email,
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


        [Route("AddStaff"), HttpPost]
        public ResponseDataModel<string> AddStaff(AdminUserVM objadminusr)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                long AgentId = Convert.ToInt64(objadminusr.strCreatedBy);
                string FirstName = objadminusr.FirstName;
                string LastName = objadminusr.LastName;
                string Email = objadminusr.Email;
                string MobileNo = objadminusr.MobileNo;
                string Address = objadminusr.Address;
                string City = objadminusr.City;
                string Password = objadminusr.Password;
                tbl_AdminUsers objUsr = new tbl_AdminUsers();
                var objtbladmn = _db.tbl_AdminUsers.Where(o => o.MobileNo == MobileNo && o.IsDeleted == false).FirstOrDefault();
                if (objtbladmn != null)
                {
                    response.AddError("Mobile number already exist");
                }
                else
                {
                    objUsr.FirstName = FirstName;
                    objUsr.LastName = LastName;
                    objUsr.Email = Email;
                    objUsr.MobileNo = MobileNo;
                    objUsr.Password = Password;
                    objUsr.AdminRoleId = 3;
                    objUsr.ParentAgentId = AgentId;
                    objUsr.CreatedBy = AgentId;
                    objUsr.IsActive = true;
                    objUsr.IsDeleted = false;
                    objUsr.CreatedDate = DateTime.UtcNow;
                    objUsr.Address = Address;
                    objUsr.City = City;
                    _db.tbl_AdminUsers.Add(objUsr);
                    _db.SaveChanges();
                    AdminUserVM objadmin = new AdminUserVM();
                    objadmin.FirstName = FirstName +" "+LastName ;
                    objadmin.MobileNo = MobileNo;
                    objadmin.Password = Password;
                    SendSMSOfCreateUser(objadmin);
                    response.Data = "Success";
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }


        [Route("EditStaff"), HttpPost]
        public ResponseDataModel<string> EditStaff(AdminUserVM objadminusr)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                long AgentId = Convert.ToInt64(objadminusr.strCreatedBy);
                long adminuserid = objadminusr.AdminUserId;
                string FirstName = objadminusr.FirstName;
                string LastName = objadminusr.LastName;
                string Email = objadminusr.Email;
                string MobileNo = objadminusr.MobileNo;
                string Address = objadminusr.Address;
                string City = objadminusr.City;
                string Password = objadminusr.Password;
                var objtbladmn = _db.tbl_AdminUsers.Where(o => o.MobileNo == MobileNo && o.IsDeleted == false && o.AdminUserId != adminuserid).FirstOrDefault();
                if (objtbladmn != null)
                {
                    response.AddError("Mobile number already exist");
                }
                else
                {
                    tbl_AdminUsers objUsr = _db.tbl_AdminUsers.Where(o => o.AdminUserId == adminuserid).FirstOrDefault();
                    objUsr.FirstName = FirstName;
                    objUsr.LastName = LastName;
                    objUsr.Email = Email;
                    objUsr.MobileNo = MobileNo;
                    objUsr.Password = Password;
                    objUsr.AdminRoleId = 3;
                    objUsr.ParentAgentId = AgentId;
                    objUsr.UpdatedBy = AgentId;
                    objUsr.UpdatedDate = DateTime.UtcNow;
                    objUsr.Address = Address;
                    objUsr.IsActive = true;
                    objUsr.City = City;
                    _db.SaveChanges();
                    response.Data = "Success";
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetAdminUserDetails"), HttpPost]
        public ResponseDataModel<AdminUserVM> GetAdminUserDetails(GeneralVM objGeneral)
        {
            ResponseDataModel<AdminUserVM> response = new ResponseDataModel<AdminUserVM>();
            AdminUserVM objUsr = new AdminUserVM();
            try
            {
                long AgntUserId = Convert.ToInt64(objGeneral.ClientUserId);

                objUsr = (from p in _db.tbl_AdminUsers
                          where p.AdminUserId == AgntUserId
                          select new AdminUserVM
                          {
                              AdminUserId = p.AdminUserId,
                              ProfilePicture = p.ProfilePicture,
                              FirstName = p.FirstName,
                              LastName = p.LastName,
                              Email = p.Email == null ? "" : p.Email,
                              MobileNo = p.MobileNo,
                              Address = p.Address,
                              Password = p.Password,
                              City = p.City,
                              WorkingTime = p.WorkingTime
                          }).FirstOrDefault();

                response.Data = objUsr;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetCashAmountAvailable"), HttpPost]
        public ResponseDataModel<string> CashAmountAvailable(GeneralVM objGeneral)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string stringcashavailable = "0";
            try
            {
                long AgntUserId = Convert.ToInt64(objGeneral.ClientUserId);
                decimal TotalAmout = (from p in _db.tbl_OrderItemDelivery
                                      join c in _db.tbl_Orders on p.OrderId equals c.OrderId
                                      where p.DelieveryPersonId == AgntUserId && p.Status == 4 && c.IsCashOnDelivery == true
                                      select new GeneralVM
                                      {
                                          AmountDecmal = p.AmountToReceived.HasValue ? p.AmountToReceived.Value : 0
                                      }).ToList().Sum(x => x.AmountDecmal);
                var lstdl = _db.tbl_CashDeliveryAmount.Where(o => o.ReceivedBy == AgntUserId).ToList();
                    decimal receiveamt = 0;
                if (lstdl != null && lstdl.Count() > 0)
                {
                    receiveamt = _db.tbl_CashDeliveryAmount.Where(o => o.ReceivedBy == AgntUserId && o.IsAccept == true).ToList().Sum(o => o.Amount.HasValue ? o.Amount.Value : 0);
                }
                decimal paidamt = 0;
                var paidamts = _db.tbl_CashDeliveryAmount.Where(o => o.SentBy == AgntUserId).ToList();
                if(paidamts != null && paidamts.Count() > 0)
                {
                    paidamt = paidamts.Sum(o => o.Amount.HasValue ? o.Amount.Value : 0);
                }
                
                decimal remaining = (TotalAmout + receiveamt) - paidamt;
                response.Data = remaining.ToString();

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SentCashOrderAmount"), HttpPost]
        public ResponseDataModel<string> SentCashOrderAmount(GeneralVM objGeneral)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string stringcashavailable = "0";
            try
            {
                long AgntUserId = Convert.ToInt64(objGeneral.ClientUserId);
                decimal Amount = Convert.ToDecimal(objGeneral.Amount);
                long recevierid = 0;
                tbl_AdminUsers objAdminusr = _db.tbl_AdminUsers.Where(o => o.AdminUserId == AgntUserId).FirstOrDefault();
                if(objAdminusr != null)
                {
                    if(objAdminusr.ParentAgentId != null && objAdminusr.ParentAgentId != 0)
                    {
                        recevierid = objAdminusr.ParentAgentId.Value;
                    }
                    else if(objAdminusr.ParentAgentId == null || objAdminusr.ParentAgentId == 0)
                    {
                        tbl_AdminUsers objsupAdminusr = _db.tbl_AdminUsers.Where(o => o.AdminRoleId == 1).FirstOrDefault();
                        recevierid = objsupAdminusr.AdminUserId;
                    }
                }

                tbl_CashDeliveryAmount objcashdel = new tbl_CashDeliveryAmount();
                objcashdel.Amount = Amount;
                objcashdel.CreatedDate = DateTime.UtcNow;
                objcashdel.SentBy = AgntUserId;
                objcashdel.ReceivedBy = recevierid;
                objcashdel.IsAccept = false;
                _db.tbl_CashDeliveryAmount.Add(objcashdel);
                _db.SaveChanges();

                response.Data = "Success";

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("UpdateAcceptCashAmount"), HttpPost]
        public ResponseDataModel<string> UpdateAcceptCashAmount(GeneralVM objGeneral)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string stringcashavailable = "0";
            try
            {
                long Id = Convert.ToInt64(objGeneral.Id);
                tbl_CashDeliveryAmount objcashdel = _db.tbl_CashDeliveryAmount.Where(o => o.tbl_CashOrderAmount_Id == Id).FirstOrDefault();
                objcashdel.IsAccept = true;                
                _db.SaveChanges();

                response.Data = "Success";

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetCashOrderRecivedAmountList"), HttpPost]
        public ResponseDataModel<List<CashOrderAmountVM>> GetCashOrderRecivedAmountList(GeneralVM objGeneral)
        {
            ResponseDataModel<List<CashOrderAmountVM>> response = new ResponseDataModel<List<CashOrderAmountVM>>();
            List<CashOrderAmountVM> lstCashOrderAmountVM = new List<CashOrderAmountVM>();
            try
            {
                long AgntUserId = Convert.ToInt64(objGeneral.ClientUserId);
                List<CashOrderAmountVM> lstcsh = (from p in _db.tbl_CashDeliveryAmount
                                                  join c in _db.tbl_AdminUsers on p.SentBy equals c.AdminUserId
                                                  where p.ReceivedBy == AgntUserId
                                                  select new CashOrderAmountVM
                                                  {
                                                      CashOrderAmountId = p.tbl_CashOrderAmount_Id,
                                                      ReceivedBy = p.ReceivedBy.Value,
                                                      SentBy = p.SentBy.Value,
                                                      dtReceived = p.CreatedDate.Value,
                                                      IsAccept = p.IsAccept.Value,
                                                      Amount = p.Amount.Value,
                                                      SenderName = c.FirstName + " " + c.LastName
                                                  }).ToList().OrderByDescending(x => x.dtReceived).ToList();
               

                response.Data = lstcsh;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        public string SendSMSOfCreateUser(AdminUserVM userVM)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();

                    string msg = "Hello " + userVM.FirstName + "\n\n";
                    msg += "You are member of Shopping & Saving." + "\n\n";

                    msg += "Below are login details:" + "\n";
                    msg += "Mobile No:" + userVM.MobileNo + "\n";
                    msg += "Password:" + userVM.Password + "\n\n";

                    msg += "Regards," + "\n";
                    msg += "Krupa Build Gallery";

                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + userVM.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";

                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return "sucess";
                    }

                }
            }
            catch (WebException ex)
            {
                return ex.Message.ToString();
            }
        }

    }
}