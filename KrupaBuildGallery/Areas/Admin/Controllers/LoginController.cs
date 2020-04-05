using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;

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

                    // Add Login history entry
                    #region LoginHistoryEntry
                    tbl_LoginHistory objLogin = new tbl_LoginHistory();
                    objLogin.UserId = data.AdminUserId;
                    objLogin.Type = "Login";
                    objLogin.DateAction = DateTime.Now;
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
            return RedirectToAction("Index");
        }

    }
}