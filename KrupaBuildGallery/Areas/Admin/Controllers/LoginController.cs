using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Newtonsoft.Json;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {

        private readonly krupagallarydbEntities _db;
        public LoginController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {



            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginVM userLogin)
        {
            try
            {
                string EncyptedPassword = userLogin.Password; // Encrypt(userLogin.Password);

                var data = _db.tbl_AdminUsers.Where(x => x.MobileNo == userLogin.MobileNo && x.Password == EncyptedPassword && !x.IsDeleted).FirstOrDefault();

                if (data != null)
                {
                    if (data.AdminRoleId == (int)AdminRoles.Agent || data.AdminRoleId == (int)AdminRoles.DeliveryUser || data.AdminRoleId == (int)AdminRoles.ChannelPartner)
                    {
                        TempData["LoginError"] = "You are not authorize to access Admin panel";
                        return View();
                    }

                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        return View();
                    }

                    var roleData = _db.tbl_AdminRoles.Where(x => x.AdminRoleId == data.AdminRoleId).FirstOrDefault();

                    if (!roleData.IsActive)
                    {
                        TempData["LoginError"] = "Your Role is not active. Please contact administrator.";
                        return View();
                    }

                    if (roleData.IsDelete)
                    {
                        TempData["LoginError"] = "Your Role is deleted. Please contact administrator.";
                        return View();
                    }

                    clsAdminSession.SessionID = Session.SessionID;
                    clsAdminSession.UserID = data.AdminUserId;
                    clsAdminSession.RoleID = data.AdminRoleId;
                    clsAdminSession.RoleName = roleData.AdminRoleName;
                    clsAdminSession.UserName = data.FirstName + " " + data.LastName;
                    clsAdminSession.ImagePath = data.ProfilePicture;
                    clsAdminSession.MobileNumber = data.MobileNo;
                    // Get Role Permissions
                    if (data.AdminRoleId != 1)
                    {
                        List<RoleModuleVM> lstPermissions = (from m in _db.tbl_AdminRoleModules
                                                             join p in _db.tbl_AdminRolePermissions.Where(x => x.AdminRoleId == data.AdminRoleId) on m.AdminRoleModuleId equals p.AdminRoleModuleId into outerPerm
                                                             from p in outerPerm
                                                             select new RoleModuleVM
                                                             {
                                                                 AdminRoleModuleId = m.AdminRoleModuleId,
                                                                 SelectedValue = (p != null ? p.Permission : 0)
                                                             }).ToList();

                        UserPermissionVM objUserPermission = new UserPermissionVM();
                        objUserPermission.Role = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Role).First().SelectedValue;
                        objUserPermission.Category = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Category).First().SelectedValue;
                        objUserPermission.Product = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Product).First().SelectedValue;
                        objUserPermission.SubProduct = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.SubProduct).First().SelectedValue;
                        objUserPermission.ProductItem = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ProductItem).First().SelectedValue;
                        objUserPermission.Stock = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Stock).First().SelectedValue;
                        objUserPermission.Order = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Order).First().SelectedValue;
                        objUserPermission.Offer = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Role).First().SelectedValue;
                        objUserPermission.Customers = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Customers).First().SelectedValue;
                        objUserPermission.Distibutors = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Distibutors).First().SelectedValue;
                        objUserPermission.DistibutorRequest = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.DistibutorRequest).First().SelectedValue;
                        objUserPermission.ContactRequest = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ContactRequest).First().SelectedValue;
                        objUserPermission.Setting = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Setting).First().SelectedValue;
                        objUserPermission.ManagePageContent = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ManagePageContent).First().SelectedValue;
                        objUserPermission.ItemText = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ItemText).First().SelectedValue;

                        string jsonPermissionValues = JsonConvert.SerializeObject(objUserPermission, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                        clsAdminSession.UserPermission = jsonPermissionValues;

                    }

                    // Add Login history entry
                    #region LoginHistoryEntry
                    tbl_LoginHistory objLogin = new tbl_LoginHistory();
                    objLogin.UserId = data.AdminUserId;
                    objLogin.Type = "Login";
                    objLogin.DateAction = DateTime.UtcNow;
                    string VisitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (VisitorsIPAddr != null || VisitorsIPAddr != String.Empty)
                    {
                        VisitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];
                    }
                    objLogin.IPAddress = VisitorsIPAddr;
                    _db.tbl_LoginHistory.Add(objLogin);
                    _db.SaveChanges();
                    #endregion LoginHistoryEntry

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    TempData["LoginError"] = "Invalid Mobile or Password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View();
        }
          
        public ActionResult Signout()
        {
            tbl_LoginHistory objLogin = new tbl_LoginHistory();
            objLogin.UserId = clsAdminSession.UserID;
            objLogin.Type = "LogOut";
            objLogin.DateAction = DateTime.Now;
            string VisitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (VisitorsIPAddr != null || VisitorsIPAddr != String.Empty)
            {
                VisitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];
            }
            objLogin.IPAddress = VisitorsIPAddr;
            _db.tbl_LoginHistory.Add(objLogin);

            clsAdminSession.SessionID = "";
            clsAdminSession.UserID = 0;

            return RedirectToAction("Index");
        }

    }
}