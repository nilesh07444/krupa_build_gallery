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
            List<tbl_ExtraAmount> extramtlst = _db.tbl_ExtraAmount.ToList();
            ViewData["extramtlst"] = extramtlst;
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

        [HttpPost]
        public string SaveExtraAmtSetting(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string nummmbers = Convert.ToString(frm["hdntrnumbr"]);
                List<tbl_ExtraAmount> extramtlst = _db.tbl_ExtraAmount.ToList();
                foreach(var objj in extramtlst)
                {
                    _db.tbl_ExtraAmount.Remove(objj);
                    _db.SaveChanges();
                }

                if (!string.IsNullOrEmpty(nummmbers))
                {
                    string[] arrynums = nummmbers.Split(',');
                    for(var jk = 0;jk<arrynums.Length;jk++)
                    {
                        tbl_ExtraAmount objextrammt = new tbl_ExtraAmount();
                        objextrammt.AmountFrom = Convert.ToDecimal(frm["txtAmtFrom_" + arrynums[jk]]);
                        objextrammt.AmountTo = Convert.ToDecimal(frm["txtAmtTo_" + arrynums[jk]]);
                        objextrammt.ExtraAmount = Convert.ToDecimal(frm["txtExtrAmt_" + arrynums[jk]]);
                        objextrammt.CreatedBy = clsAdminSession.UserID;
                        objextrammt.ModifiedBy = clsAdminSession.UserID;
                        objextrammt.CreatedDate = DateTime.UtcNow;
                        _db.tbl_ExtraAmount.Add(objextrammt);
                        _db.SaveChanges();
                    }
                }
               
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