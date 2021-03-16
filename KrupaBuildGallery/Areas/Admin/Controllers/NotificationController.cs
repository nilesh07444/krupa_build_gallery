using ConstructionDiary.Models;
using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        [AdminPermission(RolePermissionEnum.View)]
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

        [AdminPermission(RolePermissionEnum.Add)]
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
                    //string imgurl = "http://krupatest-001-site1.ftempurl.com/Images/NotificationMedia/" + fileName;
                    string imgurl = "https://shopping-saving.com/Content/assets/images/kbg/logo.png";//  "http://krupatest-001-site1.ftempurl.com/Content/assets/images/kbg/logo.png";
                    //string imgurl = "http://krupashopping-001-site1.dtempurl.com/Images/NotificationMedia/915d7548-ec35-49bb-b85d-ab7590a70db6-4455523b-4d55-4507-95b9-b18d4e4bc7cf-1.jpg";//  "http://krupatest-001-site1.ftempurl.com/Content/assets/images/kbg/logo.png";
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        imgurl = "http://krupashopping-001-site1.dtempurl.com//Images//NotificationMedia//" + fileName;
                    }
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    //serverKey - Key from Firebase cloud messaging server  
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAav-0MMY:APA91bHt6Bann_b6RoXLP3XxTI-eE5d2Uimxc231h756R8mwxjrJyqnaE959-EYhOsEtRLus1C2mZG_NyY5VACFZAKRkn0S6PSB-1QDBg3EaITICkDutSJRYaoG1Wd23JUmEwwlJcY94"));
                    //Sender Id - From firebase project setting  
                    tRequest.Headers.Add(string.Format("Sender: id={0}", "459556532422"));
                    tRequest.ContentType = "application/json";
                    var payload = new
                    {
                        to = "/topics/ShoppingSavingIOS",
                        priority = "high",
                        content_available = true,
                        //notification = new
                        //{                           
                        //    body = notificationVM.NotificationDescription,
                        //    title = notificationVM.NotificationTitle,
                        //    image= imgurl,
                        //    click_action = "OPEN_ACTIVITY_1",
                        //    badge = 1,
                        //    sound = "default"
                        //},
                        data = new
                        {
                            body = notificationVM.NotificationDescription,
                            title = notificationVM.NotificationTitle,
                            notificationdetailid = objNotification.NotificationId,
                            imageurl = imgurl,
                            badge = 1,
                            sound = "default"
                        }

                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null)
                                {
                                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                        //result.Response = sResponseFromServer;
                                    }
                                }
                                   
                            }
                        }
                    }
                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(notificationVM);
        }

        [AdminPermission(RolePermissionEnum.Edit)]
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

        [AdminPermission(RolePermissionEnum.View)]
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