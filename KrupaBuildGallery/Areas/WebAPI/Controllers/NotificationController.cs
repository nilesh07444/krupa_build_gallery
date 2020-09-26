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


        //[Route("GetNotifications"), HttpPost]
        //public ResponseDataModel<List<NotificationVM>> GetNotifications(GeneralVM objGen)
        //{
        //    ResponseDataModel<List<NotificationVM>> response = new ResponseDataModel<List<NotificationVM>>();
        //    List<NotificationVM> lstWishItems = new List<NotificationVM>();
        //    try
        //    {
        //        long ClientUserId = Convert.ToInt64(objGen.ClientUserId);                
        //        response.Data = lstWishItems;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.AddError(ex.Message.ToString());
        //        return response;
        //    }

        //    return response;

        //}
    }
}