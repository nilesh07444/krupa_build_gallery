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
    public class FreeOfferController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public FreeOfferController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/FreeOffer
        public ActionResult Index()
        {
            List<FreeOfferVM> lstOfferVm = new List<FreeOfferVM>();
            try
            {
                lstOfferVm = (from i in _db.tbl_FreeOffers                              
                              where i.IsDeleted == false
                              select new FreeOfferVM
                              {
                                  FreeOfferId = i.FreeOfferId,
                                  OrderAmountFrom = i.OrderAmountFrom.Value,
                                  OrderAmountTo = i.OrderAmountTo.Value,
                                  OfferStartDate = i.OfferStartDate.Value,
                                  OfferEndDate = i.OfferEndDate.Value                                  
                              }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstOfferVm);
        }

        public ActionResult Add()
        {
            FreeOfferVM objOffer = new FreeOfferVM();

            return View(objOffer);
        }

        [HttpPost]
        public string DeleteOffer(long FreeOfferId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_FreeOffers objtbloffer = _db.tbl_FreeOffers.Where(x => x.FreeOfferId == FreeOfferId && x.IsDeleted == false).FirstOrDefault();

                if (objtbloffer == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objtbloffer.IsDeleted = true;
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