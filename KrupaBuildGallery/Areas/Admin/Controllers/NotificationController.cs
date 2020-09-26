using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class NotificationController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public NotificationController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<NotificationVM> lstNotifications = (from n in _db.tbl_Notification
                                                     select new NotificationVM
                                                     {
                                                         NotificationId = n.NotificationId,
                                                         NotificationTitle = n.NotificationTitle,
                                                         NotificationDescription = n.NotificationDescription,
                                                         NotificationImage = n.NotificationImage,
                                                         IsActive = n.IsActive,
                                                         IsDelete = n.IsDelete,
                                                         CreatedDate = n.CreatedDate
                                                     }).Where(x => !x.IsDelete).OrderByDescending(x => x.NotificationId).ToList();
            return View(lstNotifications);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(NotificationVM notificationVM, HttpPostedFileBase NotificationImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath("~/Images/NotificationMedia/");
                    if (NotificationImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(NotificationImageFile.FileName);
                        NotificationImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = notificationVM.NotificationImage;
                    }

                    tbl_Notification objNotification = new tbl_Notification();
                    objNotification.NotificationTitle = notificationVM.NotificationTitle;
                    objNotification.NotificationDescription = notificationVM.NotificationDescription;
                    objNotification.NotificationImage = fileName;

                    objNotification.IsActive = true;
                    objNotification.IsDelete = false;
                    objNotification.CreatedBy = LoggedInUserId;
                    objNotification.CreatedDate = DateTime.UtcNow;
                    _db.tbl_Notification.Add(objNotification);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(notificationVM);
        }

        public ActionResult Edit(int Id)
        {
            NotificationVM objNotification = new NotificationVM();

            try
            {
                objNotification = (from c in _db.tbl_Notification
                                   where c.NotificationId == Id
                                   select new NotificationVM
                                   {
                                       NotificationId = c.NotificationId,
                                       NotificationTitle = c.NotificationTitle,
                                       NotificationDescription = c.NotificationDescription,
                                       NotificationImage = c.NotificationImage,
                                       IsActive = c.IsActive
                                   }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objNotification);
        }

        [HttpPost]
        public ActionResult Edit(NotificationVM notificationVM, HttpPostedFileBase NotificationImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath("~/Images/NotificationMedia/");
                    if (NotificationImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(NotificationImageFile.FileName);
                        NotificationImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = notificationVM.NotificationImage;
                    }

                    tbl_Notification objNotification = _db.tbl_Notification.Where(x => x.NotificationId == notificationVM.NotificationId).FirstOrDefault();
                    objNotification.NotificationTitle = notificationVM.NotificationTitle;
                    objNotification.NotificationDescription = notificationVM.NotificationDescription;
                    objNotification.NotificationImage = fileName;

                    objNotification.ModifiedBy = LoggedInUserId;
                    objNotification.ModifiedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(notificationVM);
        }

        [HttpPost]
        public string DeleteNotification(long NotificationId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Notification objNotification = _db.tbl_Notification.Where(x => x.NotificationId == NotificationId && !x.IsDelete).FirstOrDefault();

                if (objNotification == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objNotification.IsDelete = true;
                    objNotification.ModifiedBy = LoggedInUserId;
                    objNotification.ModifiedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Notification objNotification = _db.tbl_Notification.Where(x => x.NotificationId == Id).FirstOrDefault();

                if (objNotification != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objNotification.IsActive = true;
                    }
                    else
                    {
                        objNotification.IsActive = false;
                    }

                    objNotification.ModifiedBy = LoggedInUserId;
                    objNotification.ModifiedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult View(int Id)
        {
            NotificationVM objNotification = new NotificationVM();

            try
            {
                objNotification = (from c in _db.tbl_Notification
                                   join uC in _db.tbl_AdminUsers on c.CreatedBy equals uC.AdminUserId into outerCreated
                                   from uC in outerCreated.DefaultIfEmpty()

                                   join uM in _db.tbl_AdminUsers on c.ModifiedBy equals uM.AdminUserId into outerModified
                                   from uM in outerModified.DefaultIfEmpty()

                                   where c.NotificationId == Id
                                   select new NotificationVM
                                   {
                                       NotificationId = c.NotificationId,
                                       NotificationTitle = c.NotificationTitle,
                                       NotificationImage = c.NotificationImage,
                                       IsActive = c.IsActive,

                                       CreatedDate = c.CreatedDate,
                                       ModifiedDate = c.ModifiedDate,
                                       strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                       strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                                   }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objNotification);
        }

    }
}