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
                string txtSmtpHost = frm["txtSmtpHost"];
                string txtSmtpPort = frm["txtSmtpPort"];
                string txtSmtpEmail = frm["txtSmtpEmail"];
                string txtSmtpPwd = frm["txtSmtpPwd"];
                string txtadminemail = frm["txtadminemail"];
                string txtfrommail = frm["txtfrommail"];
                string txtsmsmobil = frm["txtsmsmobil"];
                bool EnableSSL = false;
                if(frm["rdbSSL"].ToString() == "Yes")
                {
                    EnableSSL = true;
                }
                
                tbl_GeneralSetting objGenSetting = _db.tbl_GeneralSetting.FirstOrDefault();
                objGenSetting.InitialPointCustomer = Convert.ToDecimal(intialpoints);
                objGenSetting.ShippingMessage = shippingmsg;
                objGenSetting.SMTPEmail = txtSmtpEmail;
                objGenSetting.SMTPHost = txtSmtpHost;
                objGenSetting.SMTPPort = txtSmtpPort;
                objGenSetting.SMTPPwd = txtSmtpPwd;
                objGenSetting.AdminEmail = txtadminemail;
                objGenSetting.AdminSMSNumber = txtsmsmobil.ToString();
                objGenSetting.FromEmail = txtfrommail;
                objGenSetting.EnableSSL = EnableSSL;
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