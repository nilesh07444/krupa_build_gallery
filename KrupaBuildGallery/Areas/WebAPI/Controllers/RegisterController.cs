using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class RegisterController : ApiController
    {
        krupagallarydbEntities _db;
        public RegisterController()
        {
            _db = new krupagallarydbEntities();
        }
        //// GET: WebAPI/Login
        //[Route("CreateAccount"), HttpPost]
        //public ResponseDataModel<ClientUserVM> CreateAccount(RegisterVM objRegisterVM)
        //{
        //    ResponseDataModel<ClientUserVM> response = new ResponseDataModel<ClientUserVM>();
        //    ClientUserVM objclientuser = new ClientUserVM();
        //    try
        //    {
        //        tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == email.ToLower()).FirstOrDefault();
        //        if (objClientUsr != null)
        //        {
        //            TempData["RegisterError"] = "Your Account is already exist.Please go to Login or Contact to support";
        //            TempData["email"] = email;
        //            TempData["firstnm"] = firstnm;
        //            TempData["lastnm"] = lastnm;
        //            TempData["mobileno"] = mobileno;
        //            if (string.IsNullOrEmpty(referer))
        //            {
        //                return RedirectToAction("Index", "Register", new { area = "Client", });
        //            }
        //            else
        //            {
        //                return RedirectToAction("Index", "Register", new { area = "Client", referer = referer });
        //            }
        //        }
        //        else
        //        {
        //            string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);
        //            objClientUsr = new tbl_ClientUsers();
        //            objClientUsr.Email = email;
        //            objClientUsr.ClientRoleId = 1;
        //            objClientUsr.CreatedBy = 0;
        //            objClientUsr.CreatedDate = DateTime.Now;
        //            objClientUsr.FirstName = firstnm;
        //            objClientUsr.LastName = lastnm;
        //            objClientUsr.MobileNo = mobileno;
        //            objClientUsr.IsActive = true;
        //            objClientUsr.IsDelete = false;
        //            objClientUsr.ProfilePicture = "";
        //            objClientUsr.UserName = firstnm + lastnm;
        //            objClientUsr.Password = EncyptedPassword;
        //            _db.tbl_ClientUsers.Add(objClientUsr);
        //            _db.SaveChanges();

        //            tbl_ClientOtherDetails objtbl_ClientOtherDetails = new tbl_ClientOtherDetails();
        //            objtbl_ClientOtherDetails.ClientUserId = objClientUsr.ClientUserId;
        //            objtbl_ClientOtherDetails.IsActive = true;
        //            objtbl_ClientOtherDetails.IsDelete = false;
        //            objtbl_ClientOtherDetails.CreatedDate = DateTime.Now;
        //            objtbl_ClientOtherDetails.CreatedBy = objClientUsr.ClientUserId;
        //            _db.tbl_ClientOtherDetails.Add(objtbl_ClientOtherDetails);
        //            _db.SaveChanges();
        //            clsClientSession.SessionID = Session.SessionID;
        //            clsClientSession.UserID = objClientUsr.ClientUserId;
        //            clsClientSession.RoleID = Convert.ToInt32(objClientUsr.ClientRoleId);
        //            clsClientSession.FirstName = objClientUsr.FirstName;
        //            clsClientSession.LastName = objClientUsr.LastName;
        //            clsClientSession.ImagePath = objClientUsr.ProfilePicture;
        //            clsClientSession.Email = objClientUsr.Email;

        //            var objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
        //            if (objGensetting != null)
        //            {
        //                tbl_PointDetails objPoint = new tbl_PointDetails();
        //                objPoint.ClientUserId = clsClientSession.UserID;
        //                objPoint.ExpiryDate = DateTime.UtcNow.AddMonths(6);
        //                objPoint.CreatedBy = clsClientSession.UserID;
        //                objPoint.CreatedDate = DateTime.UtcNow;
        //                objPoint.UsedPoints = 0;
        //                objPoint.Points = objGensetting.InitialPointCustomer;
        //                _db.tbl_PointDetails.Add(objPoint);
        //                _db.SaveChanges();
        //            }

        //            UpdatCarts();
        //            if (!string.IsNullOrEmpty(referer))
        //            {
        //                return RedirectToAction("Index", "Checkout", new { area = "Client" });
        //            }
        //            else
        //            {
        //                return RedirectToAction("Index", "HomePage", new { area = "Client" });
        //            }
        //        }

        //        if (data != null)
        //        {
        //            if (!data.IsActive)
        //            {
        //                response.IsError = true;
        //                response.AddError("Your Account is not active. Please contact administrator.");
        //            }
        //            else
        //            {
        //                objclientuser.FirstName = data.FirstName;
        //                objclientuser.LastName = data.LastName;
        //                objclientuser.MobileNo = data.MobileNo;
        //                objclientuser.RoleId = data.ClientRoleId;
        //                objclientuser.Email = data.Email;
        //                objclientuser.ClientUserId = data.ClientUserId;
        //                response.Data = objclientuser;
        //            }
        //        }
        //        else
        //        {
        //            response.IsError = true;
        //            response.AddError("Invalid mobilenumber or password.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response.AddError(ex.Message.ToString());
        //        return response;
        //    }

        //    return response;

        //}

    }
}