using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class FeedbackController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public FeedbackController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {

            List<FeedbackVM> lstFeedbacks = (from f in _db.tbl_Feedbacks
                                             join u in _db.tbl_ClientUsers on f.ClientUserId equals u.ClientUserId
                                             select new FeedbackVM
                                             {
                                                 FeedbackId = f.FeedbackId,
                                                 FeedbackDate = f.FeedbackDate,
                                                 ClientUserId = f.ClientUserId,
                                                 AboutDeliveryBoyBehaviour = f.AboutDeliveryBoyBehaviour,
                                                 AboutDeliveryBoyService = f.AboutDeliveryBoyService,
                                                 AboutService = f.AboutService,
                                                 FeedbackOfMonth = f.FeedbackOfMonth,
                                                 OurQuality = f.OurQuality,
                                                 Suggestion = f.Suggestion,
                                                 UserName = u.FirstName + " " + u.LastName,
                                                 ClientRoleId = u.ClientRoleId,
                                                 MobileNo = u.MobileNo
                                             }).OrderByDescending(x => x.FeedbackDate).ToList();

            return View(lstFeedbacks);
        }
         
        [HttpPost]
        public string DeleteFeedback(int FeedbackId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Feedbacks objFeedback = _db.tbl_Feedbacks.Where(x => x.FeedbackId == FeedbackId).FirstOrDefault();

                if (objFeedback == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_Feedbacks.Remove(objFeedback);
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

    }
}