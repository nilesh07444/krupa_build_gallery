using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class NotificationController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public NotificationController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Notification
        public ActionResult Index()
        {
            List<NotificationVM> lstnotifications = new List<NotificationVM>();
            int ClientUserId = Convert.ToInt32(clsClientSession.UserID);
            var objClientUser = _db.tbl_ClientUsers.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
            DateTime dtCretdt = objClientUser.CreatedDate;
            lstnotifications = (from n in _db.sp_GetNotificationList(ClientUserId)
                                where n.CreatedDate >= dtCretdt
                                select new NotificationVM
                                {
                                    NotificationId = n.NotificationId,
                                    NotificationTitle = n.NotificationTitle,
                                    NotificationDescription = n.NotificationDescription,
                                    NotificationImage = n.NotificationImage,
                                    IsRead = n.IsRead > 0 ? true : false,
                                    CreatedDate = n.CreatedDate
                                }).ToList().OrderByDescending(x => x.CreatedDate).ToList();
            if (lstnotifications != null && lstnotifications.Count() > 0)
            {
                lstnotifications.ForEach(x => x.NotificationDate = CommonMethod.ConvertFromUTCOnlyDate(x.CreatedDate));
            }

            return View(lstnotifications);
        }

        public ActionResult NotificationDetails(int Id)
        {
            NotificationVM objNotif = new NotificationVM();

            long ClientUserId = Convert.ToInt64(clsClientSession.UserID);
            int NotificationId = Id;
            objNotif = (from n in _db.tbl_Notification
                        where n.NotificationId == NotificationId
                        select new NotificationVM
                        {
                            NotificationId = n.NotificationId,
                            NotificationTitle = n.NotificationTitle,
                            NotificationDescription = n.NotificationDescription,
                            NotificationImage = n.NotificationImage,
                            CreatedDate = n.CreatedDate
                        }).FirstOrDefault();

            objNotif.NotificationDate = CommonMethod.ConvertFromUTCOnlyDate(objNotif.CreatedDate);
     
            tbl_NotificationUserRead objread = _db.tbl_NotificationUserRead.Where(o => o.NotificationId == NotificationId && o.ClientUserId == ClientUserId).FirstOrDefault();
            if (objread == null)
            {
                objread = new tbl_NotificationUserRead();
                objread.NotificationId = NotificationId;
                objread.ClientUserId = ClientUserId;
                objread.CreatedDate = DateTime.UtcNow;
                _db.tbl_NotificationUserRead.Add(objread);
                _db.SaveChanges();
            }
            ViewData["objNotif"] = objNotif;
            return View();
        }
    }
}