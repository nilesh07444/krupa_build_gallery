using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class GeneralSettingController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public GeneralSettingController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/GeneralSetting
        public ActionResult Index()
        { 
            tbl_GeneralSetting objGenSetting = _db.tbl_GeneralSetting.FirstOrDefault();
            return View(objGenSetting);
        }

        [HttpPost]
        public string SaveGeneralSetting(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string intialpoints = frm["txtpointinitial"];
                string shippingmsg = frm["txtshippingmsg"];
                tbl_GeneralSetting objGenSetting = _db.tbl_GeneralSetting.FirstOrDefault();
                objGenSetting.InitialPointCustomer = Convert.ToDecimal(intialpoints);
                objGenSetting.ShippingMessage = shippingmsg;
                _db.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }
    }
}