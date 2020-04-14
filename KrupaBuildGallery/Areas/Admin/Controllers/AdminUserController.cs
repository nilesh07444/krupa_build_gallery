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
    public class AdminUserController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public string AdminUserDirectoryPath = "";

        public AdminUserController()
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
                                 where !a.IsDeleted
                                 select new AdminUserVM
                                 {
                                     AdminUserId = a.AdminUserId,
                                     AdminRoleId = a.AdminRoleId,
                                     RoleName = r.AdminRoleName,
                                     FirstName = a.FirstName,
                                     LastName = a.LastName,
                                     Email = a.Email,
                                     MobileNo = a.MobileNo,
                                     ProfilePicture = a.ProfilePicture,
                                     IsActive = a.IsActive
                                 }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstAdminUsers);
        }

        public ActionResult Add()
        {
            AdminUserVM objAdminUser = new AdminUserVM();

            objAdminUser.RoleList = GetActiveRoleList();

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
                        userVM.RoleList = GetActiveRoleList();
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

                    objAdminUser.AdminRoleId = userVM.AdminRoleId;
                    objAdminUser.FirstName = userVM.FirstName;
                    objAdminUser.LastName = userVM.LastName;
                    objAdminUser.Email = userVM.Email;
                    objAdminUser.MobileNo = userVM.MobileNo;
                    objAdminUser.Password = userVM.Password;
                    objAdminUser.AlternateMobile = userVM.AlternateMobile;
                    objAdminUser.Address = userVM.Address;
                    objAdminUser.City = userVM.City;
                    objAdminUser.Designation = userVM.Designation;
                    objAdminUser.BloodGroup = userVM.BloodGroup;
                    objAdminUser.AdharCardNo = userVM.AdharCardNo;
                    objAdminUser.WorkingTime = userVM.WorkingTime;
                    objAdminUser.Remarks = userVM.Remarks;
                    objAdminUser.ProfilePicture = fileName;

                    if (!string.IsNullOrEmpty(userVM.Dob))
                    {
                        DateTime exp_Dob = DateTime.ParseExact(userVM.Dob, "dd/MM/yyyy", null);
                        objAdminUser.Dob = exp_Dob;
                    }

                    if (!string.IsNullOrEmpty(userVM.DateOfJoin))
                    {
                        DateTime exp_Join = DateTime.ParseExact(userVM.DateOfJoin, "dd/MM/yyyy", null);
                        objAdminUser.DateOfJoin = exp_Join;
                    }

                    if (!string.IsNullOrEmpty(userVM.DateOfIdCardExpiry))
                    {
                        DateTime exp_CardExpiry = DateTime.ParseExact(userVM.DateOfIdCardExpiry, "dd/MM/yyyy", null);
                        objAdminUser.DateOfIdCardExpiry = exp_CardExpiry;
                    }

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

            userVM.RoleList = GetActiveRoleList();

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
                                    Designation = a.Designation,
                                    dtDateOfIdCardExpiry = a.DateOfIdCardExpiry,
                                    dtDob = a.Dob,
                                    dtDateOfJoin = a.DateOfJoin,
                                    BloodGroup = a.BloodGroup,
                                    WorkingTime = a.WorkingTime,
                                    AdharCardNo = a.AdharCardNo,
                                    Remarks = a.Remarks,
                                    ProfilePicture = a.ProfilePicture,
                                    IsActive = a.IsActive
                                }).FirstOrDefault();

                if (objAdminUser.dtDob != null)
                {
                    objAdminUser.Dob = Convert.ToDateTime(objAdminUser.dtDob).ToString("dd/MM/yyyy");
                }

                if (objAdminUser.dtDateOfJoin != null)
                {
                    objAdminUser.DateOfJoin = Convert.ToDateTime(objAdminUser.dtDateOfJoin).ToString("dd/MM/yyyy");
                }

                if (objAdminUser.dtDateOfIdCardExpiry != null)
                {
                    objAdminUser.DateOfIdCardExpiry = Convert.ToDateTime(objAdminUser.dtDateOfIdCardExpiry).ToString("dd/MM/yyyy");
                }

                if (objAdminUser.AdminRoleId == 1)
                {

                    objAdminUser.RoleList = GetActiveRoleListWithAdminRole();
                }
                else
                {
                    objAdminUser.RoleList = GetActiveRoleList();
                }

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
                     
                    objAdminUser.AdminRoleId = userVM.AdminRoleId;
                    objAdminUser.FirstName = userVM.FirstName;
                    objAdminUser.LastName = userVM.LastName;
                    objAdminUser.Email = userVM.Email;
                    objAdminUser.MobileNo = userVM.MobileNo;
                    objAdminUser.Password = userVM.Password;
                    objAdminUser.AlternateMobile = userVM.AlternateMobile;
                    objAdminUser.Address = userVM.Address;
                    objAdminUser.City = userVM.City;
                    objAdminUser.Designation = userVM.Designation;
                    objAdminUser.BloodGroup = userVM.BloodGroup;
                    objAdminUser.AdharCardNo = userVM.AdharCardNo;
                    objAdminUser.WorkingTime = userVM.WorkingTime;
                    objAdminUser.Remarks = userVM.Remarks;
                    objAdminUser.ProfilePicture = fileName;

                    if (!string.IsNullOrEmpty(userVM.Dob))
                    {
                        DateTime exp_Dob = DateTime.ParseExact(userVM.Dob, "dd/MM/yyyy", null);
                        objAdminUser.Dob = exp_Dob;
                    }
                    else
                    {
                        objAdminUser.Dob = null;
                    }

                    if (!string.IsNullOrEmpty(userVM.DateOfJoin))
                    {
                        DateTime exp_Join = DateTime.ParseExact(userVM.DateOfJoin, "dd/MM/yyyy", null);
                        objAdminUser.DateOfJoin = exp_Join;
                    }
                    else
                    {
                        objAdminUser.DateOfJoin = null;
                    }

                    if (!string.IsNullOrEmpty(userVM.DateOfIdCardExpiry))
                    {
                        DateTime exp_CardExpiry = DateTime.ParseExact(userVM.DateOfIdCardExpiry, "dd/MM/yyyy", null);
                        objAdminUser.DateOfIdCardExpiry = exp_CardExpiry;
                    }
                    else
                    {
                        objAdminUser.DateOfIdCardExpiry = null;
                    }

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

            userVM.RoleList = GetActiveRoleList();

            return View(userVM);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public string ChangePassword(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string CurrentPassword = frm["currentpwd"];
                string NewPassword = frm["newpwd"];

                long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                tbl_AdminUsers objUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == LoggedInUserId).FirstOrDefault();

                if (objUser != null)
                {
                    string EncryptedCurrentPassword = CurrentPassword; // CoreHelper.Encrypt(CurrentPassword);
                    if (objUser.Password == EncryptedCurrentPassword)
                    {
                        objUser.Password = NewPassword; //CoreHelper.Encrypt(NewPassword);
                        _db.SaveChanges();

                        ReturnMessage = "SUCCESS";
                        Session.Clear();
                    }
                    else
                    {
                        ReturnMessage = "CP_NOT_MATCHED";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }

        public string SendSMSOfCreateUser(AdminUserVM userVM)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();

                    string msg = "Hello " + userVM.FirstName + "\n\n";
                    msg += "You are member of Krupa Build Gallery" + "\n\n";

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
                                    Designation = a.Designation,
                                    dtDateOfIdCardExpiry = a.DateOfIdCardExpiry,
                                    dtDob = a.Dob,
                                    dtDateOfJoin = a.DateOfJoin,
                                    BloodGroup = a.BloodGroup,
                                    WorkingTime = a.WorkingTime,
                                    AdharCardNo = a.AdharCardNo,
                                    Remarks = a.Remarks,
                                    ProfilePicture = a.ProfilePicture,
                                    IsActive = a.IsActive,

                                    CreatedDate = a.CreatedDate,
                                    UpdatedDate = a.UpdatedDate,
                                    strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                    strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                                }).FirstOrDefault();

                if (objAdminUser.dtDob != null)
                {
                    objAdminUser.Dob = Convert.ToDateTime(objAdminUser.dtDob).ToString("dd/MM/yyyy");
                }

                if (objAdminUser.dtDateOfJoin != null)
                {
                    objAdminUser.DateOfJoin = Convert.ToDateTime(objAdminUser.dtDateOfJoin).ToString("dd/MM/yyyy");
                }

                if (objAdminUser.dtDateOfIdCardExpiry != null)
                {
                    objAdminUser.DateOfIdCardExpiry = Convert.ToDateTime(objAdminUser.dtDateOfIdCardExpiry).ToString("dd/MM/yyyy");
                }
                 
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objAdminUser);
        }

        private List<SelectListItem> GetActiveRoleList()
        {
            var RoleList = _db.tbl_AdminRoles.Where(x => x.IsActive && !x.IsDelete && x.AdminRoleId != 1)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminRoleId).Trim(), Text = o.AdminRoleName })
                         .OrderBy(x => x.Text).ToList();

            return RoleList;
        }

        private List<SelectListItem> GetActiveRoleListWithAdminRole()
        {
            var RoleList = _db.tbl_AdminRoles.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminRoleId).Trim(), Text = o.AdminRoleName })
                         .OrderBy(x => x.Text).ToList();

            return RoleList;
        }

    }
}