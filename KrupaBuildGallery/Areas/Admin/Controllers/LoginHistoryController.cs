using System;
using System.Collections.Generic;
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
        public ActionResult Index()
        {
            List<LoginHistoryVM> lstHistory = new List<LoginHistoryVM>();
            try
            {

                lstHistory = (from c in _db.tbl_LoginHistory
                              join u in _db.tbl_AdminUsers on c.UserId equals u.AdminUserId
                              select new LoginHistoryVM
                              {
                                  LoginHistoryId = c.LoginHistoryId,
                                  UserId = c.UserId,
                                  DateAction = c.DateAction,
                                  AdminUserFullName = u.FirstName + " " + u.LastName
                              }).OrderByDescending(x => x.LoginHistoryId).ToList();


            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstHistory);
        }
         
    }
}