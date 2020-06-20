using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class AgentController : Controller
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


    }
}