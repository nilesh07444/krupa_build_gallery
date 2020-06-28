using ConstructionDiary.Models;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Helper;
using System.IO;
using System.Net;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class AgentController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public string AdminUserDirectoryPath = "";

        public AgentController()
        {
            _db = new krupagallarydbEntities();
            AdminUserDirectoryPath = ErrorMessage.AdminUserDirectoryPath;
        }

        public ActionResult Index()
        {
            List<AdminUserVM> lstAdminUsers = new List<AdminUserVM>();

            try
            {
                lstAdminUsers = (from a in _db.tbl_AdminUsers
                                 join r in _db.tbl_AdminRoles on a.AdminRoleId equals r.AdminRoleId
                                 where !a.IsDeleted && a.AdminRoleId == (int)AdminRoles.Agent
                                 select new AdminUserVM
                                 {
                                     AdminUserId = a.AdminUserId,
                                     AdminRoleId = a.AdminRoleId,
                                     RoleName = r.AdminRoleName,
                                     FirstName = a.FirstName,
                                     LastName = a.LastName,
                                     Email = a.Email,
                                     MobileNo = a.MobileNo,
                                     City = a.City,
                                     ProfilePicture = a.ProfilePicture,
                                     IsActive = a.IsActive
                                 }).ToList();

                if (lstAdminUsers.Count > 0)
                {
                    lstAdminUsers.ForEach(user =>
                    {
                        user.RemainingCashAmount = GetRemainingCashAmountAvailable(user.AdminUserId);
                    });
                }

            }
            catch (Exception ex)
            {
            }

            return View(lstAdminUsers);
        }

        public ActionResult Add()
        {
            AdminUserVM objAdminUser = new AdminUserVM(); 
            return View(objAdminUser);
        }

        [HttpPost]
        public ActionResult Add(AdminUserVM userVM, HttpPostedFileBase ProfilePictureFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_AdminUsers duplicateMobile = _db.tbl_AdminUsers.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("MobileNo", ErrorMessage.MobileNoExists); 
                        return View(userVM);
                    }

                    string fileName = string.Empty;
                    string path = Server.MapPath(AdminUserDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (ProfilePictureFile != null)
                    {
                        string ext = Path.GetExtension(ProfilePictureFile.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(ProfilePictureFile.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        ProfilePictureFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = userVM.ProfilePicture;
                    }

                    #endregion Validation

                    #region CreateUser

                    tbl_AdminUsers objAdminUser = new tbl_AdminUsers();

                    objAdminUser.AdminRoleId = (int)AdminRoles.Agent;
                    objAdminUser.FirstName = userVM.FirstName;
                    objAdminUser.LastName = userVM.LastName;
                    objAdminUser.Email = userVM.Email;
                    objAdminUser.MobileNo = userVM.MobileNo;
                    objAdminUser.Password = userVM.Password;
                    objAdminUser.AlternateMobile = userVM.AlternateMobile;
                    objAdminUser.Address = userVM.Address;
                    objAdminUser.City = userVM.City; 
                    objAdminUser.Remarks = userVM.Remarks;
                    objAdminUser.ProfilePicture = fileName;
                     
                    objAdminUser.IsActive = true;
                    objAdminUser.IsDeleted = false;
                    objAdminUser.CreatedDate = DateTime.Now;
                    objAdminUser.CreatedBy = LoggedInUserId;
                    _db.tbl_AdminUsers.Add(objAdminUser);
                    _db.SaveChanges();

                    // Send SMS To User
                    string SmsResponse = SendSMSOfCreateUser(userVM);

                    return RedirectToAction("Index");

                    #endregion CreateUser
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
              
            return View(userVM);
        }

        public ActionResult Edit(long Id)
        {
            AdminUserVM objAdminUser = new AdminUserVM();

            try
            {
                objAdminUser = (from a in _db.tbl_AdminUsers
                                where a.AdminUserId == Id
                                select new AdminUserVM
                                {
                                    AdminUserId = a.AdminUserId,
                                    AdminRoleId = a.AdminRoleId,
                                    FirstName = a.FirstName,
                                    LastName = a.LastName,
                                    Email = a.Email,
                                    MobileNo = a.MobileNo,
                                    Password = a.Password,
                                    AlternateMobile = a.AlternateMobile,
                                    Address = a.Address,
                                    City = a.City, 
                                    Remarks = a.Remarks,
                                    ProfilePicture = a.ProfilePicture,
                                    IsActive = a.IsActive
                                }).FirstOrDefault();
                 

            }
            catch (Exception ex)
            {

            }

            return View(objAdminUser);
        }

        [HttpPost]
        public ActionResult Edit(AdminUserVM userVM, HttpPostedFileBase ProfilePictureFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_AdminUsers duplicateMobile = _db.tbl_AdminUsers.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && x.AdminUserId != userVM.AdminUserId && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("MobileNo", ErrorMessage.MobileNoExists);

                        return View(userVM);
                    }

                    tbl_AdminUsers objAdminUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == userVM.AdminUserId).FirstOrDefault();

                    string fileName = string.Empty;
                    string path = Server.MapPath(AdminUserDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (ProfilePictureFile != null)
                    {
                        string ext = Path.GetExtension(ProfilePictureFile.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(ProfilePictureFile.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        ProfilePictureFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objAdminUser.ProfilePicture;
                    }

                    #endregion Validation

                    #region UpdateUser
                     
                    objAdminUser.FirstName = userVM.FirstName;
                    objAdminUser.LastName = userVM.LastName;
                    objAdminUser.Email = userVM.Email;
                    objAdminUser.MobileNo = userVM.MobileNo;
                    objAdminUser.Password = userVM.Password;
                    objAdminUser.AlternateMobile = userVM.AlternateMobile;
                    objAdminUser.Address = userVM.Address;
                    objAdminUser.City = userVM.City; 
                    objAdminUser.Remarks = userVM.Remarks;
                    objAdminUser.ProfilePicture = fileName;
                      
                    objAdminUser.UpdatedDate = DateTime.Now;
                    objAdminUser.UpdatedBy = LoggedInUserId;

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion UpdateUser
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
             
            return View(userVM);
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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_AdminUsers objAdminUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == Id).FirstOrDefault();

                if (objAdminUser != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objAdminUser.IsActive = true;
                    }
                    else
                    {
                        objAdminUser.IsActive = false;
                    }

                    objAdminUser.UpdatedBy = LoggedInUserId;
                    objAdminUser.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeleteAdminUser(int AdminUserId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_AdminUsers objAdminUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == AdminUserId).FirstOrDefault();

                if (objAdminUser == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objAdminUser.IsDeleted = true;
                    objAdminUser.UpdatedBy = LoggedInUserId;
                    objAdminUser.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();

                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult View(int Id)
        {
            AdminUserVM objAdminUser = new AdminUserVM();

            try
            {
                objAdminUser = (from a in _db.tbl_AdminUsers
                                join uC in _db.tbl_AdminUsers on a.CreatedBy equals uC.AdminUserId into outerCreated
                                from uC in outerCreated.DefaultIfEmpty()

                                join uM in _db.tbl_AdminUsers on a.UpdatedBy equals uM.AdminUserId into outerModified
                                from uM in outerModified.DefaultIfEmpty()

                                where a.AdminUserId == Id
                                select new AdminUserVM
                                {
                                    AdminUserId = a.AdminUserId,
                                    AdminRoleId = a.AdminRoleId,
                                    FirstName = a.FirstName,
                                    LastName = a.LastName,
                                    Email = a.Email,
                                    MobileNo = a.MobileNo,
                                    Password = a.Password,
                                    AlternateMobile = a.AlternateMobile,
                                    Address = a.Address,
                                    City = a.City, 
                                    Remarks = a.Remarks,
                                    ProfilePicture = a.ProfilePicture,
                                    IsActive = a.IsActive,

                                    CreatedDate = a.CreatedDate,
                                    UpdatedDate = a.UpdatedDate,
                                    strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                    strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                                }).FirstOrDefault();
                 

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objAdminUser);
        }

        public ActionResult Users(int Id)
        {
            List<AdminUserVM> lstAdminUsers = new List<AdminUserVM>();

            try
            {
                lstAdminUsers = (from a in _db.tbl_AdminUsers
                                 join r in _db.tbl_AdminRoles on a.AdminRoleId equals r.AdminRoleId
                                 where !a.IsDeleted && a.ParentAgentId == Id
                                 select new AdminUserVM
                                 {
                                     AdminUserId = a.AdminUserId,
                                     AdminRoleId = a.AdminRoleId,
                                     RoleName = r.AdminRoleName,
                                     FirstName = a.FirstName,
                                     LastName = a.LastName,
                                     Email = a.Email,
                                     MobileNo = a.MobileNo,
                                     City = a.City,
                                     ProfilePicture = a.ProfilePicture,
                                     IsActive = a.IsActive
                                 }).ToList();

                if (lstAdminUsers.Count > 0)
                {
                    lstAdminUsers.ForEach(user =>
                    {
                        user.RemainingCashAmount = GetRemainingCashAmountAvailable(user.AdminUserId);
                    });
                }

            }
            catch (Exception ex)
            {
            }

            return View(lstAdminUsers);
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
                    string msg = "Your change password OTP code is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
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

        public decimal GetRemainingCashAmountAvailable(long AgntUserId)
        {
            decimal remainingCashAmt = 0;

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
            var paidamts = _db.tbl_CashDeliveryAmount.Where(o => o.SentBy == AgntUserId && o.IsAccept == true).ToList();
            if (paidamts != null && paidamts.Count() > 0)
            {
                paidamt = paidamts.Sum(o => o.Amount.HasValue ? o.Amount.Value : 0);
            }

            remainingCashAmt = (TotalAmout + receiveamt) - paidamt;

            return remainingCashAmt;
        }

    }
}