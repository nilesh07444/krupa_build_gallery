using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class BidItemController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public BidItemController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {

            List<BidItemVM> lstItemText = new List<BidItemVM>();
            try
            {

                lstItemText = (from i in _db.tbl_PurchaseBidItems
                               join j in _db.tbl_BidItemUnitTypes on i.UnitType.Value equals j.BidItemUnitTypeId
                               select new BidItemVM
                               {
                                   Pk_PurchaseBidItemId = i.Pk_PurchaseBidItemId,
                                   ItemName = i.ItemName,
                                   UnitName = j.UnitTypeName
                               }).OrderBy(x => x.UnitName).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            ViewBag.SuccessMsg = "";
            if(TempData["SuccessAdd"] != null)
            {
                ViewBag.SuccessMsg = "Item Add Successfully";
            }

            return View(lstItemText);
        }

      
        public ActionResult Add()
        {
            BidItemVM objBidItm = new BidItemVM();
            objBidItm.UnitList = GetBidUnitItems();
            ViewBag.SuccessMsg = "";
            if (TempData["SuccessAdd"] != null)
            {
                ViewBag.SuccessMsg = "Item Add Successfully";
            }
            return View(objBidItm);
        }

        [HttpPost]
        public ActionResult Add(BidItemVM objBidItemVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existItemtext = _db.tbl_PurchaseBidItems.Where(x => x.ItemName.ToLower() == objBidItemVM.ItemName.ToLower() && x.IsDeleted == false).FirstOrDefault();
                    if (existItemtext != null)
                    {
                        ModelState.AddModelError("ItemName", ErrorMessage.ItemNameExists);
                        objBidItemVM.UnitList = GetBidUnitItems();
                        return View(objBidItemVM);
                    }

                    tbl_PurchaseBidItems objBidItm = new tbl_PurchaseBidItems();
                    objBidItm.ItemName = objBidItemVM.ItemName;
                    objBidItm.CreatedBy = LoggedInUserId;
                    objBidItm.CreatedDate = DateTime.UtcNow;
                    objBidItm.IsDeleted = false;
                    objBidItm.UnitType = objBidItemVM.UnitType;
                    _db.tbl_PurchaseBidItems.Add(objBidItm);
                    _db.SaveChanges();
                    TempData["SuccessAdd"] = "Success";
                    return RedirectToAction("Add");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            objBidItemVM.UnitList = GetBidUnitItems();
            return View(objBidItemVM);
        }

        public ActionResult Edit(int Id)
        {
            BidItemVM objItemTextVM = new BidItemVM();

            try
            {
                objItemTextVM = (from c in _db.tbl_PurchaseBidItems
                                 where c.Pk_PurchaseBidItemId == Id
                                 select new BidItemVM
                                 {
                                     Pk_PurchaseBidItemId = c.Pk_PurchaseBidItemId,
                                     ItemName = c.ItemName,
                                     UnitType = c.UnitType.Value
                                 }).FirstOrDefault();
                objItemTextVM.UnitList = GetBidUnitItems();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objItemTextVM);
        }

        [HttpPost]
        public ActionResult Edit(BidItemVM objBidItm)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existItemtext = _db.tbl_PurchaseBidItems.Where(x => x.Pk_PurchaseBidItemId != objBidItm.Pk_PurchaseBidItemId && x.ItemName.ToLower() == objBidItm.ItemName.ToLower()).FirstOrDefault();
                    if (existItemtext != null)
                    {
                        ModelState.AddModelError("ItemName", ErrorMessage.ItemNameExists);
                        objBidItm.UnitList = GetBidUnitItems();
                        return View(objBidItm);
                    }

                    tbl_PurchaseBidItems objBidItemVM = _db.tbl_PurchaseBidItems.Where(x => x.Pk_PurchaseBidItemId == objBidItm.Pk_PurchaseBidItemId).FirstOrDefault();

                    objBidItemVM.ItemName = objBidItm.ItemName;
                    objBidItemVM.UnitType = objBidItm.UnitType;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objBidItm);
        }

        [HttpPost]
        public string DeleteBidItem(long ItemId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_PurchaseBidItems objtbl_PurchaseBidItems = _db.tbl_PurchaseBidItems.Where(x => x.Pk_PurchaseBidItemId == ItemId).FirstOrDefault();

                if (objtbl_PurchaseBidItems == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objtbl_PurchaseBidItems.IsDeleted = true;
                    objtbl_PurchaseBidItems.ModifiedBy = LoggedInUserId;
                    objtbl_PurchaseBidItems.ModifiedDate = DateTime.UtcNow;
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

        public List<SelectListItem> GetBidUnitItems()
        {
            var lstUnts = _db.tbl_BidItemUnitTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.UnitTypeName).ToList();
            List<SelectListItem> lstselc = new List<SelectListItem>();
            if (lstUnts != null && lstUnts.Count() > 0)
            {
                foreach (var objj in lstUnts)
                {
                    SelectListItem obb = new SelectListItem();
                    obb.Value = objj.BidItemUnitTypeId.ToString();
                    obb.Text = objj.UnitTypeName;
                    lstselc.Add(obb);
                }
            }

            return lstselc;
        }
    }
}