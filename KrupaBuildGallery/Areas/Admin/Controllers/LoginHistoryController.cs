using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;


namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class LoginHistoryController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public LoginHistoryController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index(int UserId = -1,string StartDate = "",string EndDate = "")
        {
            List<LoginHistoryVM> lstHistory = new List<LoginHistoryVM>();
            try
            {
                DateTime dtStart = DateTime.MinValue;
                DateTime dtEnd = DateTime.MaxValue;
                if(!string.IsNullOrEmpty(StartDate))
                {
                    DateTime dt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dtStart = dt;
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime dt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dtEnd = dt;                   
                }

                List<AdminUserVM> lstAdminusr = (from ad in _db.tbl_AdminUsers                                                 
                                                 select new AdminUserVM
                                                 {
                                                   AdminUserId = ad.AdminUserId,
                                                   FirstName = ad.FirstName,
                                                   LastName = ad.LastName
                                                 }).OrderByDescending(x => x.FirstName).ToList();

                lstHistory = (from c in _db.tbl_LoginHistory
                              join u in _db.tbl_AdminUsers on c.UserId equals u.AdminUserId
                              where (UserId == -1 || c.UserId == UserId) && c.DateAction >= dtStart && c.DateAction <= dtEnd
                              select new LoginHistoryVM
                              {
                                  LoginHistoryId = c.LoginHistoryId,
                                  UserId = c.UserId,
                                  DateAction = c.DateAction,
                                  AdminUserFullName = u.FirstName + " " + u.LastName
                              }).OrderByDescending(x => x.LoginHistoryId).ToList();

                ViewData["lstAdminusr"] = lstAdminusr;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
                ViewBag.UserId = UserId;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstHistory);
        }
         
    }
}