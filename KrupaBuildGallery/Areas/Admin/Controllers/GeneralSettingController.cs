using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
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
            //tbl_GeneralSetting objGenSetting = _db.tbl_GeneralSetting.FirstOrDefault();
            List<tbl_ExtraAmount> extramtlst = _db.tbl_ExtraAmount.ToList();
            ViewData["extramtlst"] = extramtlst;

            GeneralSettingVM objGenSetting = (from s in _db.tbl_GeneralSetting
                                    select new GeneralSettingVM
                                    {
                                        GeneralSettingId = s.GeneralSettingId,
                                        InitialPointCustomer = s.InitialPointCustomer,
                                        ShippingMessage = s.ShippingMessage,
                                        SMTPHost = s.SMTPHost,
                                        SMTPPort = s.SMTPPort,
                                        EnableSSL = s.EnableSSL,
                                        SMTPEmail = s.SMTPEmail,
                                        SMTPPwd = s.SMTPPwd,
                                        AdminSMSNumber = s.AdminSMSNumber,
                                        AdminEmail = s.AdminEmail,
                                        FromEmail = s.FromEmail,
                                        AdvertiseBannerImage = s.AdvertiseBannerImage,
                                        ReturnPerInGodhra = s.ReturnPerInGodhra,
                                        ReturnPerOutGodhra = s.ReturnPerOutGodhra,
                                        ExchangePer = s.ExchangePer,
                                        CashLimitPerOrder = s.CashLimitPerOrder,
                                        CashLimitPerYear = s.CashLimitPerYear,
                                        RazorPayKey = s.RazorPayKey,
                                        RazorPaySecret = s.RazorPaySecret
                                    }).FirstOrDefault();
             
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
                if (frm["rdbSSL"].ToString() == "Yes")
                {
                    EnableSSL = true;
                }

                decimal txtReturnPerInGodhra = string.IsNullOrEmpty(frm["txtReturnPerInGodhra"]) ? 0 : Convert.ToDecimal(frm["txtReturnPerInGodhra"]);
                decimal txtReturnPerOutGodhra = string.IsNullOrEmpty(frm["txtReturnPerOutGodhra"]) ? 0 : Convert.ToDecimal(frm["txtReturnPerOutGodhra"]);
                decimal txtExchangePer = string.IsNullOrEmpty(frm["txtExchangePer"]) ? 0 : Convert.ToDecimal(frm["txtExchangePer"]);
                decimal txtCashLimitPerOrder = string.IsNullOrEmpty(frm["txtCashLimitPerOrder"]) ? 0 : Convert.ToDecimal(frm["txtCashLimitPerOrder"]);
                decimal txtCashLimitPerYear = string.IsNullOrEmpty(frm["txtCashLimitPerYear"]) ? 0 : Convert.ToDecimal(frm["txtCashLimitPerYear"]);

                string RazorPaykey = frm["txtRazorPayKey"]; 
                string RazorPayScrete = frm["txtRazorPaySecret"];
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
                objGenSetting.RazorPayKey = RazorPaykey;
                objGenSetting.RazorPaySecret = RazorPayScrete;
                objGenSetting.ReturnPerInGodhra = txtReturnPerInGodhra;
                objGenSetting.ReturnPerOutGodhra = txtReturnPerOutGodhra;
                objGenSetting.ExchangePer = txtExchangePer;
                objGenSetting.CashLimitPerOrder = txtCashLimitPerOrder;
                objGenSetting.CashLimitPerYear = txtCashLimitPerYear;


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
                foreach (var objj in extramtlst)
                {
                    _db.tbl_ExtraAmount.Remove(objj);
                    _db.SaveChanges();
                }

                if (!string.IsNullOrEmpty(nummmbers))
                {
                    string[] arrynums = nummmbers.Split(',');
                    for (var jk = 0; jk < arrynums.Length; jk++)
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

        [HttpPost]
        public ActionResult UploadAdvertiseBanner(GeneralSettingVM settingVM, HttpPostedFileBase AdvertiseBannerImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                      
                    string fileName = string.Empty;
                    string path = Server.MapPath(ErrorMessage.AdvertiseDirectoryPath);
                    if (AdvertiseBannerImageFile != null)
                    {
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(AdvertiseBannerImageFile.FileName);
                        AdvertiseBannerImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = settingVM.AdvertiseBannerImage;
                    }
                     
                    tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    objSetting.AdvertiseBannerImage = fileName;
                      
                    _db.SaveChanges();
                        
                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(settingVM);
        }


    }
}