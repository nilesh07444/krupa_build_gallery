using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class RegisterController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public RegisterController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Register
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateAccount(FormCollection frm)
        {        
            try
            {
                string email = frm["email"].ToString();
                string firstnm = frm["fname"].ToString();
                string lastnm = frm["lname"].ToString();
                string mobileno = frm["mobileno"].ToString();
                string password = frm["password"].ToString();
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == email.ToLower()).FirstOrDefault();
                if(objClientUsr != null)
                {
                    TempData["RegisterError"] = "Your Account is already exist.Please go to Login or Contact to support";
                    TempData["email"] = email;
                    TempData["firstnm"] = firstnm;
                    TempData["lastnm"] = lastnm;
                    TempData["mobileno"] = mobileno;
                    return RedirectToAction("Index", "Register", new { area = "Client" });
                }
                else
                {
                    string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);
                    objClientUsr = new tbl_ClientUsers();
                    objClientUsr.Email = email;
                    objClientUsr.ClientRoleId = 1;
                    objClientUsr.CreatedBy = 0;
                    objClientUsr.CreatedDate = DateTime.Now;
                    objClientUsr.FirstName = firstnm;
                    objClientUsr.LastName = lastnm;
                    objClientUsr.MobileNo = mobileno;
                    objClientUsr.IsActive = true;
                    objClientUsr.IsDelete = false;
                    objClientUsr.ProfilePicture = "";
                    objClientUsr.UserName = firstnm + lastnm;
                    objClientUsr.Password = EncyptedPassword;
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
                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = objClientUsr.ClientUserId;
                    clsClientSession.RoleID = Convert.ToInt32(objClientUsr.ClientRoleId);
                    clsClientSession.FirstName = objClientUsr.FirstName;
                    clsClientSession.LastName = objClientUsr.LastName;
                    clsClientSession.ImagePath = objClientUsr.ProfilePicture;
                    clsClientSession.Email = objClientUsr.Email;
                    return RedirectToAction("Index", "HomePage", new { area = "Client" });
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["LoginError"] = ErrorMessage;
            }

            return RedirectToAction("Index", "Login", new { area = "Client" });

        }
    }
}