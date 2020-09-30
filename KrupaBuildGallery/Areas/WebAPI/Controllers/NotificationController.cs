using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using KrupaBuildGallery.ViewModel;
using System.Net;
using System.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class NotificationController : ApiController
    {
        krupagallarydbEntities _db;
        public NotificationController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("GetNotificationById"), HttpPost]
        public ResponseDataModel<NotificationVM> GetNotificationById(GeneralVM objGen)
        {
            ResponseDataModel<NotificationVM> response = new ResponseDataModel<NotificationVM>();
            NotificationVM objNotif = new NotificationVM();
            try
            {
                long ClientUserId = Convert.ToInt64(objGen.ClientUserId);
                int NotificationId = Convert.ToInt32(objGen.NotificationId);
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
                response.Data = objNotif;

                tbl_NotificationUserRead objread = _db.tbl_NotificationUserRead.Where(o => o.NotificationId == NotificationId && o.ClientUserId == ClientUserId).FirstOrDefault();
                if(objread == null)
                {
                    objread = new tbl_NotificationUserRead();
                    objread.NotificationId = NotificationId;
                    objread.ClientUserId = ClientUserId;
                    objread.CreatedDate = DateTime.UtcNow;
                    _db.tbl_NotificationUserRead.Add(objread);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetNotifications"), HttpPost]
        public ResponseDataModel<List<NotificationVM>> GetNotifications(GeneralVM objGen)
        {
            ResponseDataModel<List<NotificationVM>> response = new ResponseDataModel<List<NotificationVM>>();
            List<NotificationVM> lstnotifications = new List<NotificationVM>();
            try
            {
                int ClientUserId = Convert.ToInt32(objGen.ClientUserId);
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
                response.Data = lstnotifications;

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