using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;


namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class BidController : ApiController
    {
        krupagallarydbEntities _db;
        public BidController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("SendOTPDealerVerify"), HttpPost]
        public ResponseDataModel<OtpVM> SendOTPDealerVerify(OtpVM objOtpVM)
        {
            ResponseDataModel<OtpVM> response = new ResponseDataModel<OtpVM>();
            OtpVM objOtp = new OtpVM();
            try
            {
                string MobileNum = objOtpVM.MobileNo;
                string Email = objOtpVM.Email;
                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    //string msg = "Your distributor request OTP code is " + num;
                    int SmsId = (int)SMSType.RegistrationOtp;
                    clsCommon objcm = new clsCommon();
                    string msg = objcm.GetSmsContent(SmsId);
                    msg = msg.Replace("{{OTP}}", num + "");
                    msg = HttpUtility.UrlEncode(msg);
                    //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", MobileNum).Replace("--MSG--", msg);
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        response.IsError = true;
                        response.AddError("Invalid Mobile Number");
                    }
                    else
                    {
                        tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                        string FromEmail = objGensetting.FromEmail;
                        Random random1 = new Random();
                        int num1 = random1.Next(111566, 499999);
                        string msg1 = "Your Registration OTP Code Is " + num1;
                        clsCommon.SendEmail(Email, FromEmail, "OTP Code For Registration - Shopping & Saving", msg1);
                        objOtp.Otp = num.ToString();
                        objOtp.OtpEmail = num1.ToString();
                        response.Data = objOtp;
                    }

                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("PurchaseDealerRegistation"), HttpPost]
        public ResponseDataModel<string> PurchaseDealerRegistation(PurchaseDealerVM objPurchaseDealerVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string strmsg = "";
            try
            {
                tbl_PurchaseDealersRequest objRequest = new tbl_PurchaseDealersRequest();
                objRequest.FirmName = objPurchaseDealerVM.FirmName;
                objRequest.FirmAddress = objPurchaseDealerVM.FirmAddress;
                objRequest.FirmCity = objPurchaseDealerVM.FirmCity;
                objRequest.FirmGSTNo = objPurchaseDealerVM.FirmGSTNo;
                objRequest.VisitingCardPhoto = objPurchaseDealerVM.VisitingCardPhoto;
                objRequest.FirmContactNo = objPurchaseDealerVM.FirmContactNo;
                objRequest.AlternateNo = objPurchaseDealerVM.AlternateNo;
                objRequest.Email = objPurchaseDealerVM.Email;
                objRequest.BusinessDetails = objPurchaseDealerVM.BusinessDetails;
                objRequest.BankAcNumber = objPurchaseDealerVM.BankAcNumber;
                objRequest.IFSCCode = objPurchaseDealerVM.IFSCCode;
                objRequest.BankBranch = objPurchaseDealerVM.BankBranch;
                objRequest.BankAcNumber2 = objPurchaseDealerVM.BankAcNumber2;
                objRequest.IFSCCode2 = objPurchaseDealerVM.IFSCCode2;
                objRequest.BankBranch2 = objPurchaseDealerVM.BankBranch2;
                objRequest.OwnerName = objPurchaseDealerVM.OwnerName;
                objRequest.OwnerContactNo = objPurchaseDealerVM.OwnerContactNo;
                objRequest.Remark = objPurchaseDealerVM.Remark;
                objRequest.BussinessCode = "";
                objRequest.IsActive = false;
                objRequest.IsDelete = false;
                objRequest.State = objPurchaseDealerVM.State;
                objRequest.Status = 0;
                objRequest.CreatedDate = DateTime.Now;
                _db.tbl_PurchaseDealersRequest.Add(objRequest);
                _db.SaveChanges();
                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                string AdminEmail = objGensetting.AdminEmail;

                _db.SaveChanges();
                string FromEmail = objGensetting.FromEmail;
                if (!string.IsNullOrEmpty(objRequest.Email))
                {
                    FromEmail = objRequest.Email;
                }
                
                string Subject = "New Purchase Dealer Request - Shopping & Saving";
                string bodyhtml = "Following are few details:<br/>";
                bodyhtml += "FirmName: " + objPurchaseDealerVM.FirmName + "<br/>";
                bodyhtml += "FirmAddress: " + objPurchaseDealerVM.FirmAddress + "<br/>";
                bodyhtml += "FirmCity: " + objPurchaseDealerVM.FirmCity + "<br/>";
                bodyhtml += "FirmContactNo: " + objPurchaseDealerVM.FirmContactNo + "<br/>";
                bodyhtml += "AlternateNo: " + objPurchaseDealerVM.AlternateNo + "<br/>";
                bodyhtml += "Email: " + objPurchaseDealerVM.Email + "<br/>";
                bodyhtml += "OwnerName: " + objPurchaseDealerVM.OwnerName + "<br/>";
                bodyhtml += "OwnerContactNo: " + objPurchaseDealerVM.OwnerContactNo + "<br/>";

                clsCommon.SendEmail(AdminEmail, FromEmail, Subject, bodyhtml);
                strmsg = "Request receive Successfully.We will contact you asap.";


                response.Data = strmsg;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetBidItems"), HttpPost]
        public ResponseDataModel<List<sp_GetBidDealerItems_Result>> GetBidItems(GeneralVM objGen)
        {
            ResponseDataModel<List<sp_GetBidDealerItems_Result>> response = new ResponseDataModel<List<sp_GetBidDealerItems_Result>>();
            string strmsg = "";
            try
            {
                int DealerId = Convert.ToInt32(objGen.DealerId);
                List<sp_GetBidDealerItems_Result>  lstresult = _db.sp_GetBidDealerItems(DealerId).ToList();
                response.Data = lstresult;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetDealerTerms"), HttpPost]
        public ResponseDataModel<List<BidTermsVM>> GetDealerTerms(GeneralVM objGen)
        {
            ResponseDataModel<List<BidTermsVM>> response = new ResponseDataModel<List<BidTermsVM>>();
            string strmsg = "";
            try
            {
                long DealerId = Convert.ToInt64(objGen.DealerId);
                List<BidTermsVM> lstBidTermsVM = (from crt in _db.tbl_DealerTerms                                               
                                                where crt.Fk_Dealer_Id == DealerId 
                               select new BidTermsVM
                               {
                                   DealerId = DealerId,
                                   Terms = crt.Terms,
                                   TermsType = crt.TermsType.Value
                               }).ToList();
              
                response.Data = lstBidTermsVM;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SaveDealerTerms"), HttpPost]
        public ResponseDataModel<string> SaveDealerTerms(BidTermsVM objTerm)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string strmsg = "";
            try
            {
                tbl_DealerTerms objTerms = new tbl_DealerTerms();
                if (objTerm.Pk_DelearTerms >  0)
                {
                    objTerms = _db.tbl_DealerTerms.Where(o => o.Pk_DealerTerms_Id == objTerm.Pk_DelearTerms).FirstOrDefault();
                }
                objTerms.IsDeleted = false;
                objTerms.Terms = objTerm.Terms;
                objTerms.TermsType = objTerm.TermsType;
                objTerms.Fk_Dealer_Id = objTerm.DealerId;
                if (objTerm.Pk_DelearTerms == 0)
                {
                    _db.tbl_DealerTerms.Add(objTerms);
                }
                _db.SaveChanges();

                    response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }


        [Route("SaveDealerSuggestion"), HttpPost]
        public ResponseDataModel<string> SaveDealerSuggestion(DealerSuggestionVM objSugg)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string strmsg = "";
            try
            {
                tbl_DealerSuggestions obj = new tbl_DealerSuggestions();
                obj.Fk_DealerId = objSugg.DealerId;
                obj.Suggestion = objSugg.Suggestion;
                obj.PicFile = objSugg.PicFile;
                _db.tbl_DealerSuggestions.Add(obj);
                _db.SaveChanges();

                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                string AdminEmail = objGensetting.AdminEmail;

                _db.SaveChanges();
                string FromEmail = objGensetting.FromEmail;
                tbl_PurchaseDealers objDeler = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == objSugg.DealerId).FirstOrDefault();
                if (!string.IsNullOrEmpty(objDeler.Email))
                {
                    FromEmail = objDeler.Email;
                }

                string Subject = "New Suggestion From Dealer - Shopping & Saving";
                string bodyhtml = "Following are the details:<br/>";
                bodyhtml += "Dealer FirmName: " + objDeler.FirmName + "<br/>";                
                bodyhtml += "Suggestion: " + objSugg.Suggestion + "<br/>";
                
                clsCommon.SendEmail(AdminEmail, FromEmail, Subject, bodyhtml);
                strmsg = "Suggestion Sent Successfully.";

                response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

       
    }
}