using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using KrupaBuildGallery.ViewModel;
namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class LoginController : ApiController
    {
        krupagallarydbEntities _db;
        public LoginController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: WebAPI/Login
        [Route("CheckLogin"), HttpPost]
        public ResponseDataModel<ClientUserVM> CheckLogin(LoginVM objLogin)
        {
            ResponseDataModel<ClientUserVM> response = new ResponseDataModel<ClientUserVM>();
            ClientUserVM objclientuser = new ClientUserVM();
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(objLogin.Password); // Encrypt(userLogin.Password);

                var data = _db.tbl_ClientUsers.Where(x => (x.MobileNo == objLogin.MobileNo && x.ClientRoleId == 1)
                && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        response.IsError = true;
                        response.AddError("Your Account is not active. Please contact administrator.");
                    }
                    else
                    {
                        objclientuser.FirstName = data.FirstName;
                        objclientuser.LastName = data.LastName;
                        objclientuser.MobileNo = data.MobileNo;
                        objclientuser.RoleId = data.ClientRoleId;
                        objclientuser.Email = data.Email;
                        objclientuser.ClientUserId = data.ClientUserId;
                        response.Data = objclientuser;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError("Invalid mobilenumber or password."); 
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("CheckLoginDistributor"), HttpPost]
        public ResponseDataModel<ClientUserVM> CheckLoginDistributor(LoginVM objLogin)
        {
            ResponseDataModel<ClientUserVM> response = new ResponseDataModel<ClientUserVM>();
            ClientUserVM objclientuser = new ClientUserVM();
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(objLogin.Password); // Encrypt(userLogin.Password);                
                var data = _db.tbl_ClientUsers.Where(x => (x.Email.ToLower() == objLogin.MobileNo.ToLower() || x.MobileNo.ToLower() == objLogin.MobileNo.ToLower()) && x.ClientRoleId == 2 && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        response.IsError = true;
                        response.AddError("Your Account is not active. Please contact administrator.");
                    }
                    else
                    {
                        objclientuser.FirstName = data.FirstName;
                        objclientuser.LastName = data.LastName;
                        objclientuser.MobileNo = data.MobileNo;
                        objclientuser.RoleId = data.ClientRoleId;
                        objclientuser.Email = data.Email;
                        objclientuser.ClientUserId = data.ClientUserId;
                        response.Data = objclientuser;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError("Invalid mobilenumber/email or password.");
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