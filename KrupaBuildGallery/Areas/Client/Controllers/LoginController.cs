using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class LoginController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public LoginController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckLogin(FormCollection frm)
        {
            string username = frm["email"];
            string password = frm["password"];
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);

                var data = _db.tbl_ClientUsers.Where(x => (x.UserName == username || x.Email == username)
                && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        return RedirectToAction("Index", "Login",new { area = "Client" });
                    }

                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = data.ClientUserId;
                    clsClientSession.RoleID = Convert.ToInt32(data.ClientRoleId);
                    clsClientSession.FirstName = data.FirstName;
                    clsClientSession.LastName = data.LastName;
                    clsClientSession.ImagePath = data.ProfilePicture;
                    clsClientSession.Email = data.Email;
                    return RedirectToAction("Index", "HomePage", new { area = "Client" });
                }
                else
                {
                    TempData["LoginError"] = "Invalid username or password";
                    return RedirectToAction("Index", "Login",new { area = "Client" });
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["LoginError"] = ErrorMessage;
            }

            return RedirectToAction("Index", "Login", new { area = "Client" });

        }

        public ActionResult Signout()
        {
            clsClientSession.SessionID = "";
            clsClientSession.UserID = 0;
            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "HomePage", new { area = "Client" });
        }
    }
}