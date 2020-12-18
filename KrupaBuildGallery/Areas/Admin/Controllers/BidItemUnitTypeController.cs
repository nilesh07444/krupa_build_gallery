using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class BidItemUnitTypeController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public BidItemUnitTypeController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {

            List<BidItemUnitTypeVM> lstItemText = new List<BidItemUnitTypeVM>();
            try
            {
                lstItemText = (from i in _db.tbl_BidItemUnitTypes
                               select new BidItemUnitTypeVM
                               {
                                   BidItemUnitTypeId = i.BidItemUnitTypeId,
                                   UnitTypeName = i.UnitTypeName,
                                   IsDeleted = i.IsDeleted
                               }).Where(x => x.IsDeleted == false).OrderByDescending(x => x.BidItemUnitTypeId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            ViewBag.SuccessMsg = "";
            if (TempData["SuccessAdd"] != null)
            {
                ViewBag.SuccessMsg = "Unit Added Successfully";
            }

            return View(lstItemText);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(BidItemUnitTypeVM objBidItemUnitTypeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existUnit = _db.tbl_BidItemUnitTypes.Where(x => x.UnitTypeName.ToLower() == objBidItemUnitTypeVM.UnitTypeName.ToLower() && x.IsDeleted == false).FirstOrDefault();
                    if (existUnit != null)
                    {
                        ModelState.AddModelError("UnitTypeName", ErrorMessage.ItemNameExists);
                        return View(objBidItemUnitTypeVM);
                    }

                    tbl_BidItemUnitTypes objUnit = new tbl_BidItemUnitTypes();
                    objUnit.UnitTypeName = objBidItemUnitTypeVM.UnitTypeName;
                    objUnit.IsDeleted = false;
                    _db.tbl_BidItemUnitTypes.Add(objUnit);
                    _db.SaveChanges();
                    TempData["SuccessAdd"] = "Success";
                    return RedirectToAction("Add");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objBidItemUnitTypeVM);
        }

        public ActionResult Edit(int Id)
        {
            BidItemUnitTypeVM objUnitVM = new BidItemUnitTypeVM();

            try
            {
                objUnitVM = (from c in _db.tbl_BidItemUnitTypes
                             where c.BidItemUnitTypeId == Id
                             select new BidItemUnitTypeVM
                             {
                                 BidItemUnitTypeId = c.BidItemUnitTypeId,
                                 UnitTypeName = c.UnitTypeName,
                             }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objUnitVM);
        }

        [HttpPost]
        public ActionResult Edit(BidItemUnitTypeVM objUnit)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existUnit = _db.tbl_BidItemUnitTypes.Where(x => x.BidItemUnitTypeId != objUnit.BidItemUnitTypeId && x.UnitTypeName.ToLower() == objUnit.UnitTypeName.ToLower()).FirstOrDefault();
                    if (existUnit != null)
                    {
                        ModelState.AddModelError("UnitTypeName", ErrorMessage.UnitExists);
                        return View(objUnit);
                    }

                    tbl_BidItemUnitTypes objBidItemUnitTypeVM = _db.tbl_BidItemUnitTypes.Where(x => x.BidItemUnitTypeId == objUnit.BidItemUnitTypeId).FirstOrDefault();
                    objBidItemUnitTypeVM.UnitTypeName = objUnit.UnitTypeName;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objUnit);
        }

        [HttpPost]
        public string DeleteBidUnit(long Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_BidItemUnitTypes obj = _db.tbl_BidItemUnitTypes.Where(x => x.BidItemUnitTypeId == Id).FirstOrDefault();

                if (obj == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    obj.IsDeleted = true;
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