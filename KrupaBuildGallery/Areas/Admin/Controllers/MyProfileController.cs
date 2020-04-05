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
    public class MyProfileController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public string AdminUserDirectoryPath = "";

        public MyProfileController()
        {
            _db = new krupagallarydbEntities();
            AdminUserDirectoryPath = ErrorMessage.AdminUserDirectoryPath;
        }

        public ActionResult Index()
        {
            AdminUserVM objAdminUser = new AdminUserVM();
            long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

            try
            {
                objAdminUser = (from a in _db.tbl_AdminUsers
                                join r in _db.tbl_AdminRoles on a.AdminRoleId equals r.AdminRoleId into outerRole
                                from r in outerRole.DefaultIfEmpty()
                                where a.AdminUserId == LoggedInUserId
                                select new AdminUserVM
                                {
                                    AdminUserId = a.AdminUserId,
                                    AdminRoleId = a.AdminRoleId,
                                    FirstName = a.FirstName,
                                    LastName = a.LastName,
                                    RoleName = r.AdminRoleName,
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
                 
            }
            catch (Exception ex)
            { 
            }

            return View(objAdminUser);
        }
    }
}