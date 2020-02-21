using ConstructionDiary.Models;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class CustomerController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public CustomerController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Customer
        public ActionResult Index()
        {
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
            try
            {

                lstClientUser = (from cu in _db.tbl_ClientUsers
                             join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                             where !cu.IsDelete && cu.ClientRoleId == 1
                             select new ClientUserVM
                             {
                                 ClientUserId = cu.ClientUserId,
                                 FirstName = cu.FirstName,
                                 LastName = cu.LastName,
                                 UserName = cu.UserName,
                                 Email = cu.Email,
                                 Password = cu.Password,
                                 RoleId = cu.ClientRoleId,
                                 CompanyName = cu.CompanyName,
                                 ProfilePic = cu.ProfilePicture,
                                 MobileNo = cu.MobileNo,
                                 IsActive = cu.IsActive,
                                 City = co.City,
                                 State = co.State,
                                 AddharCardNo = co.Addharcardno,
                                 PanCardNo = co.Pancardno,
                                 GSTNo = co.GSTno
                             }).OrderBy(x => x.FirstName).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstClientUser);
        }
    }
}