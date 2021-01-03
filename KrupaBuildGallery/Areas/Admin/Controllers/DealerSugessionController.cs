using ConstructionDiary.Models;
using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Helper;
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
    public class DealerSugessionController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public DealerSugessionController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<DealerSuggestionVM> lstSuggestions = new List<DealerSuggestionVM>();
            try
            {
                lstSuggestions = (from s in _db.tbl_DealerSuggestions
                                  join d in _db.tbl_PurchaseDealers on s.Fk_DealerId equals d.Pk_Dealer_Id
                                  select new DealerSuggestionVM
                                  {
                                      SuggetionId = s.Pk_DealerSuggstion_Id,
                                      DealerId = s.Fk_DealerId,
                                      Suggestion = s.Suggestion,
                                      DealerName = d.OwnerName,
                                      OwnerNumber = d.OwnerContactNo,
                                      PicFile = s.PicFile,
                                      SuggestionDate = s.SuggestionDate
                                  }).OrderByDescending(x => x.SuggestionDate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(lstSuggestions);
        }

        public ActionResult View(int Id)
        {
            DealerSuggestionVM objSuggestions = new DealerSuggestionVM();
            try
            {
                objSuggestions = (from s in _db.tbl_DealerSuggestions
                                  join d in _db.tbl_PurchaseDealers on s.Fk_DealerId equals d.Pk_Dealer_Id
                                  select new DealerSuggestionVM
                                  {
                                      SuggetionId = s.Pk_DealerSuggstion_Id,
                                      DealerId = s.Fk_DealerId,
                                      Suggestion = s.Suggestion,
                                      DealerName = d.OwnerName,
                                      OwnerNumber = d.OwnerContactNo,
                                      PicFile = s.PicFile,
                                      SuggestionDate = s.SuggestionDate
                                  }).Where(x => x.SuggetionId == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(objSuggestions);
        }

    }
}