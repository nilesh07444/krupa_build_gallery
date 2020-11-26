using ConstructionDiary.Models;
using KrupaBuildGallery.Filters;
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
    public class ItemTextController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public ItemTextController()
        {
            _db = new krupagallarydbEntities();
        }

        [AdminPermission(RolePermissionEnum.View)]
        public ActionResult Index()
        {

            List<ItemTextVM> lstItemText = new List<ItemTextVM>();
            try
            {

                lstItemText = (from i in _db.tbl_Itemtext_master                               
                               select new ItemTextVM
                               {
                                   ItemTextId = i.Item_Text_Id,
                                   ItemText = i.ItemText,                                  
                               }).OrderBy(x => x.ItemText).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstItemText);
        }

        [AdminPermission(RolePermissionEnum.Add)]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(ItemTextVM itemtextVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existItemtext = _db.tbl_Itemtext_master.Where(x => x.ItemText.ToLower() == itemtextVM.ItemText.ToLower()).FirstOrDefault();
                    if (existItemtext != null)
                    {
                        ModelState.AddModelError("ItemText", ErrorMessage.ItemTextExists);
                        return View(itemtextVM);
                    }

                    tbl_Itemtext_master objitemtext = new tbl_Itemtext_master();
                    objitemtext.ItemText = itemtextVM.ItemText;
                    objitemtext.CreatedBy = LoggedInUserId;
                    objitemtext.CreatedDate = DateTime.UtcNow;
                    _db.tbl_Itemtext_master.Add(objitemtext);
                    _db.SaveChanges();

                    return RedirectToAction("Add");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(itemtextVM);
        }

        [AdminPermission(RolePermissionEnum.Edit)]
        public ActionResult Edit(int Id)
        {
            ItemTextVM objItemTextVM = new ItemTextVM();

            try
            {
                objItemTextVM = (from c in _db.tbl_Itemtext_master
                                 where c.Item_Text_Id == Id
                                 select new ItemTextVM
                                 {
                                     ItemTextId = c.Item_Text_Id,
                                     ItemText = c.ItemText
                                 }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objItemTextVM);
        }

        [HttpPost]
        public ActionResult Edit(ItemTextVM itemtextVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Check for exist
                    var existItemtext = _db.tbl_Itemtext_master.Where(x => x.Item_Text_Id != itemtextVM.ItemTextId && x.ItemText.ToLower() == itemtextVM.ItemText.ToLower()).FirstOrDefault();
                    if (existItemtext != null)
                    {
                        ModelState.AddModelError("ItemText", ErrorMessage.ItemTextExists);
                        return View(itemtextVM);
                    }

                    tbl_Itemtext_master objtbl_Itemtext_master = _db.tbl_Itemtext_master.Where(x => x.Item_Text_Id == itemtextVM.ItemTextId).FirstOrDefault();

                    objtbl_Itemtext_master.ItemText = itemtextVM.ItemText;
                 
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(itemtextVM);
        }

        [HttpPost]
        public string DeleteItemText(long Itemtextid)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Itemtext_master objtbl_Itemtext_master = _db.tbl_Itemtext_master.Where(x => x.Item_Text_Id == Itemtextid).FirstOrDefault();

                if (objtbl_Itemtext_master == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    _db.tbl_Itemtext_master.Remove(objtbl_Itemtext_master);
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