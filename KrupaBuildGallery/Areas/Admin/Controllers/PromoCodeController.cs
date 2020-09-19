using ConstructionDiary.Models;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class PromoCodeController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public PromoCodeController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<PromoCodeVM> lstPromoCode = new List<PromoCodeVM>();

            try
            {
                lstPromoCode = (from m in _db.tbl_PromoCode
                                 where !m.IsDeleted
                                 select new PromoCodeVM
                                 {
                                     PromoCodeId = m.PromoCodeId,
                                     PromoCode = m.PromoCode,
                                     DiscountPercentage = m.DiscountPercentage,
                                     TotalMaxUsage = m.TotalMaxUsage,
                                     dtExpiryDate = m.ExpiryDate,
                                     IsActive = m.IsActive
                                 }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstPromoCode);
        }
         
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(PromoCodeVM PromocodeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region CreatePromo

                    tbl_PromoCode objPromoCode = new tbl_PromoCode();
                    objPromoCode.PromoCode = PromocodeVM.PromoCode;
                    objPromoCode.DiscountPercentage = PromocodeVM.DiscountPercentage;
                    objPromoCode.TotalMaxUsage = PromocodeVM.TotalMaxUsage;

                    if (!string.IsNullOrEmpty(PromocodeVM.ExpiryDate))
                    {
                        DateTime exp_date = DateTime.ParseExact(PromocodeVM.ExpiryDate, "dd/MM/yyyy", null);
                        objPromoCode.ExpiryDate = exp_date;
                    }

                    objPromoCode.IsActive = true;
                    objPromoCode.IsDeleted = false;
                    objPromoCode.CreatedDate = DateTime.UtcNow;
                    objPromoCode.CreatedBy = LoggedInUserId;
                    _db.tbl_PromoCode.Add(objPromoCode);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion CreatePromo
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(PromocodeVM);
        }

        public ActionResult Edit(long Id)
        {
            PromoCodeVM objPromoCode = new PromoCodeVM();

            try
            {
                objPromoCode = (from m in _db.tbl_PromoCode
                                 where m.PromoCodeId == Id
                                 select new PromoCodeVM
                                 {
                                     PromoCodeId = m.PromoCodeId,
                                     PromoCode = m.PromoCode,
                                     DiscountPercentage = m.DiscountPercentage,
                                     TotalMaxUsage = m.TotalMaxUsage,
                                     dtExpiryDate = m.ExpiryDate,
                                     IsActive = m.IsActive
                                 }).FirstOrDefault();

                if (objPromoCode.dtExpiryDate != null)
                {
                    objPromoCode.ExpiryDate = Convert.ToDateTime(objPromoCode.dtExpiryDate).ToString("dd/MM/yyyy");
                }

            }
            catch (Exception ex)
            {
            }

            return View(objPromoCode);
        }

        [HttpPost]
        public ActionResult Edit(PromoCodeVM PromocodeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region UpdatePromo

                    tbl_PromoCode objPromoCode = _db.tbl_PromoCode.Where(x => x.PromoCodeId == PromocodeVM.PromoCodeId).FirstOrDefault();

                    objPromoCode.PromoCode = PromocodeVM.PromoCode;
                    objPromoCode.DiscountPercentage = PromocodeVM.DiscountPercentage;
                    objPromoCode.TotalMaxUsage = PromocodeVM.TotalMaxUsage;

                    if (!string.IsNullOrEmpty(PromocodeVM.ExpiryDate))
                    {
                        DateTime exp_date = DateTime.ParseExact(PromocodeVM.ExpiryDate, "dd/MM/yyyy", null);
                        objPromoCode.ExpiryDate = exp_date;
                    }

                    objPromoCode.UpdatedDate = DateTime.UtcNow;
                    objPromoCode.UpdatedBy = LoggedInUserId;

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion UpdatePromo
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(PromocodeVM);
        }

        [HttpPost]
        public string DeletePromoCode(int PromoCodeId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_PromoCode objPromoCode = _db.tbl_PromoCode.Where(x => x.PromoCodeId == PromoCodeId).FirstOrDefault();

                if (objPromoCode == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    objPromoCode.IsDeleted = true;
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
                tbl_PromoCode objPromoCode = _db.tbl_PromoCode.Where(x => x.PromoCodeId == Id).FirstOrDefault();

                if (objPromoCode != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objPromoCode.IsActive = true;
                    }
                    else
                    {
                        objPromoCode.IsActive = false;
                    }

                    objPromoCode.UpdatedBy = LoggedInUserId;
                    objPromoCode.UpdatedDate = DateTime.UtcNow;

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