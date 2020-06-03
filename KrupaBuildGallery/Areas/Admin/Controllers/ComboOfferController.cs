using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class ComboOfferController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public ComboOfferController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<ComboOfferVM> lstOfferVm = new List<ComboOfferVM>();

            try
            {

            }
            catch (Exception ex)
            {
            }

            return View(lstOfferVm);
        }

        public ActionResult Add()
        {
            ComboOfferVM objOffer = new ComboOfferVM();

            objOffer.Main_CategoryList = GetCategoryList();
            objOffer.Main_ProductList = new List<SelectListItem>();
            objOffer.Main_SubProductList = new List<SelectListItem>();
            objOffer.Main_ProductItemList = new List<SelectListItem>();

            objOffer.Sub_CategoryList = GetCategoryList();
            objOffer.Sub_ProductList = new List<SelectListItem>();
            objOffer.Sub_SubProductList = new List<SelectListItem>();
            objOffer.Sub_ProductItemList = new List<SelectListItem>();

            return View(objOffer);
        }

        [HttpPost]
        public ActionResult Add(ComboOfferVM objOffer)
        {
            return View(objOffer);
        }

        public ActionResult Edit(int Id)
        {
            ComboOfferVM objOffer = new ComboOfferVM();

            try
            {


                objOffer.Main_CategoryList = GetCategoryList();
                objOffer.Main_ProductList = new List<SelectListItem>();
                objOffer.Main_SubProductList = new List<SelectListItem>();
                objOffer.Main_ProductItemList = new List<SelectListItem>();

                objOffer.Sub_CategoryList = GetCategoryList();
                objOffer.Sub_ProductList = new List<SelectListItem>();
                objOffer.Sub_SubProductList = new List<SelectListItem>();
                objOffer.Sub_ProductItemList = new List<SelectListItem>();
            }
            catch (Exception ex)
            {
            }

            return View(objOffer);
        }

        [HttpPost]
        public ActionResult Edit(ComboOfferVM objOffer)
        {
            return View(objOffer);
        }

        private List<SelectListItem> GetCategoryList()
        {
            var CategoryList = _db.tbl_Categories.Where(x => x.IsActive && !x.IsDelete)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.CategoryId).Trim(), Text = o.CategoryName })
                         .OrderBy(x => x.Text).ToList();

            return CategoryList;
        }

        [HttpPost]
        public string DeleteOffer(long ComboOfferId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Offers objtbloffer = _db.tbl_Offers.Where(x => x.OfferId == ComboOfferId && x.IsActive && !x.IsDelete).FirstOrDefault();

                if (objtbloffer == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objtbloffer.IsDelete = true;
                    objtbloffer.UpdatedBy = LoggedInUserId;
                    objtbloffer.UpdatedDate = DateTime.UtcNow;

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
                tbl_Offers objtbl_Offers = _db.tbl_Offers.Where(x => x.OfferId == Id).FirstOrDefault();

                if (objtbl_Offers != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_Offers.IsActive = true;
                    }
                    else
                    {
                        objtbl_Offers.IsActive = false;
                    }

                    objtbl_Offers.UpdatedBy = LoggedInUserId;
                    objtbl_Offers.UpdatedDate = DateTime.UtcNow;

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