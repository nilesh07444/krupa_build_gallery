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

        [Route("GetBids"), HttpPost]
        public ResponseDataModel<List<BidVM>> GetBids(GeneralVM objGen)
        {
            ResponseDataModel<List<BidVM>> response = new ResponseDataModel<List<BidVM>>();
            string strmsg = "";
            try
            {                
                long DealerId = Convert.ToInt64(objGen.DealerId);
                string StartDate = objGen.startdate;
                string EndDate = objGen.enddate;
                DateTime dtStart = DateTime.MinValue;
                DateTime dtEnd = DateTime.MaxValue;
                long StatuID = Convert.ToInt64(objGen.StatusId);
                if (!string.IsNullOrEmpty(StartDate))
                {
                    dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
                }
                dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);
                List<BidVM> lstBids = new List<BidVM>();
                lstBids = (from cu in _db.tbl_Bids
                           join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                           join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                           where cu.IsDeleted == 0 && cu.BidDate >= dtStart && cu.BidDate <= dtEnd 
                           select new BidVM
                           {
                               BidId = cu.Pk_Bid_id,
                               ItemId = cu.ItemId.Value,
                               ItemName = itm.ItemName,
                               Qty = cu.Qty.Value,
                               Unittype = unityp.UnitTypeName,
                               BidStatus = cu.BidStatus.Value,
                               BidDate = cu.BidDate.Value
                           }).OrderByDescending(x => x.BidDate).ToList();
                List<tbl_BidDealers> lstDelrBid = _db.tbl_BidDealers.Where(o => o.FK_DealerId == DealerId).ToList();
                if (lstBids != null && lstBids.Count() > 0)
                {                   
                    List<long> BidIdList = lstDelrBid.Select(x => x.Fk_BidId.Value).ToList();
                    List<BidVM> lstBidsN = (from cu in lstBids
                                            join dlitm in _db.tbl_BidDealerItems on cu.ItemId equals dlitm.Fk_ItemId
                                            where !BidIdList.Contains(cu.BidId)
                               select new BidVM
                               {
                                   BidId = cu.BidId,
                                   ItemId = cu.ItemId,
                                   ItemName = cu.ItemName,
                                   Qty = cu.Qty,
                                   Unittype = cu.Unittype,
                                   BidStatus = cu.BidStatus,
                                   BidDate = cu.BidDate,
                                   DelearBidId = 0
                               }).OrderByDescending(x => x.BidDate).ToList();

                    List<BidVM> lstBidsN1 = (from cu in lstBids
                                            where BidIdList.Contains(cu.BidId)
                                            select new BidVM
                                            {
                                                BidId = cu.BidId,
                                                ItemId = cu.ItemId,
                                                ItemName = cu.ItemName,
                                                Qty = cu.Qty,
                                                Unittype = cu.Unittype,
                                                BidStatus = cu.BidStatus,
                                                BidDate = cu.BidDate,
                                                DelearBidId = 1                                                
                                            }).OrderByDescending(x => x.BidDate).ToList();

                    lstBids = lstBidsN.Union(lstBidsN1).ToList();
                    lstBids = lstBids.Where(x => (x.BidStatus == 3 && DealerId == 1) || (x.BidStatus != 3)).ToList();
                }

                if(lstBids != null && lstBids.Count() > 0)
                {
                    foreach(BidVM objdl in lstBids)
                    {
                        var obj = lstDelrBid.Where(x => x.Fk_BidId == objdl.BidId && x.FK_DealerId == DealerId).FirstOrDefault();
                        if(obj != null)
                        {
                            objdl.BidStatus = obj.BidStatus.Value;
                            objdl.DelearBidId = obj.Pk_BidDealers;
                        }
                        else
                        {
                            if(objdl.BidStatus != 3)
                            {
                                objdl.BidStatus = -1;
                            }                            
                        }
                    }
                }

                if(StatuID != -2)
                {
                    lstBids = lstBids.Where(o => o.BidStatus == StatuID).ToList();
                }
               
                response.Data = lstBids;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("SaveBidDealers"), HttpPost]
        public ResponseDataModel<string> SaveBidDealers(BidDealerVM objBidDVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            string strmsg = "";
            try
            {
                tbl_BidDealers obj = _db.tbl_BidDealers.Where(o => o.Fk_BidId == objBidDVM.BidId && o.FK_DealerId == objBidDVM.DealerId).FirstOrDefault();
                if(obj == null)
                {
                    obj = new tbl_BidDealers();
                    obj.BidSendDate = DateTime.Now;
                    obj.BidStatus = 0;
                }
                obj.Fk_BidId = objBidDVM.BidId;
                obj.FK_DealerId = objBidDVM.DealerId;
                obj.Price = objBidDVM.Price;
                obj.MinimumQtyToBuy = objBidDVM.MinimumQtytoBuy;
                obj.TermsAndCondions = objBidDVM.TermsCondition;
                obj.PaymentTerms = objBidDVM.PaymentTerms;
                obj.PaymentType = objBidDVM.PaymentType;
                obj.PickupCity = objBidDVM.PickupCity;
                obj.PickupCityPincode = objBidDVM.PickupCityPincode;
                obj.BidValidDays = objBidDVM.BidValidDays;                
                obj.BidModifiedDate = DateTime.Now;
                
                if(obj == null || obj.Pk_BidDealers == 0)
                {
                    _db.tbl_BidDealers.Add(obj);
                }                
                _db.SaveChanges();


                List<tbl_BidDealers> lstBiddle = _db.tbl_BidDealers.Where(o => o.Fk_BidId == objBidDVM.BidId).ToList();
                if (lstBiddle != null && lstBiddle.Count() == 1)
                {
                    tbl_Bids objBd = _db.tbl_Bids.Where(o => o.Pk_Bid_id == objBidDVM.BidId).FirstOrDefault();
                    if (objBd != null)
                    {
                        objBd.BidStatus = 0;
                    }
                    _db.SaveChanges();                   
                }
                response.Data = "Success";
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetDealerBid"), HttpPost]
        public ResponseDataModel<BidDealerVM> GetDealerBid(GeneralVM objGen)
        {
            ResponseDataModel<BidDealerVM> response = new ResponseDataModel<BidDealerVM>();
            string strmsg = "";
            try
            {
                long DealerId = Convert.ToInt64(objGen.DealerId);
                long BidId = Convert.ToInt64(objGen.BidId);
                BidDealerVM objBidDealerVM = (from cu in _db.tbl_BidDealers
                                              join dl in _db.tbl_PurchaseDealers on cu.FK_DealerId equals dl.Pk_Dealer_Id
                                              where cu.Fk_BidId == BidId && cu.FK_DealerId == DealerId
                                              select new BidDealerVM
                                              {
                                                  BidDealerId = cu.Pk_BidDealers,
                                                  DealerId = dl.Pk_Dealer_Id,
                                                  FirmName = dl.FirmName,
                                                  BusinessCode = dl.BussinessCode,
                                                  BidValidDays = cu.BidValidDays.Value,
                                                  FirmMobile = dl.FirmContactNo,
                                                  Price = cu.Price.Value,
                                                  BidSentDate = cu.BidSendDate.Value,
                                                  MinimumQtytoBuy = cu.MinimumQtyToBuy.Value,
                                                  BidStatus = cu.BidStatus.Value,
                                                  TermsCondition = cu.TermsAndCondions,
                                                  PaymentTerms = cu.PaymentTerms,
                                                  PaymentType = cu.PaymentType,
                                                  PickupCity = cu.PickupCity,
                                                  PickupCityPincode = cu.PickupCityPincode,
                                                  Remarks = cu.Remarks,
                                                  RejectReason = cu.RejectReason,
                                                  BidId = cu.Fk_BidId.Value
                                              }).FirstOrDefault();
                if(objBidDealerVM == null)
                {
                    objBidDealerVM = new BidDealerVM();
                }
                response.Data = objBidDealerVM;
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("BidderLogin"), HttpPost]
        public ResponseDataModel<PurchaseDealerVM> BidderLogin(PurchaseDealerVM objDlr)
        {
            ResponseDataModel<PurchaseDealerVM> response = new ResponseDataModel<PurchaseDealerVM>();
            PurchaseDealerVM objBidder = new PurchaseDealerVM();
            try
            {
                //string EncyptedPassword = clsCommon.EncryptString(objLogin.Password); // Encrypt(userLogin.Password);

                var data = _db.tbl_PurchaseDealers.Where(x => x.BussinessCode == objDlr.BussinessCode && x.Password == objDlr.Password
                                        && !x.IsDelete.Value).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive.Value)
                    {
                        response.IsError = true;
                        response.AddError("Your Account is not active. Please contact administrator.");
                    }
                    else
                    {
                        objBidder.FirmName = data.FirmName;
                        objBidder.FirmContactNo = data.FirmContactNo;
                        objBidder.Pk_Dealer_Id = data.Pk_Dealer_Id;
                        objBidder.BussinessCode = data.BussinessCode;
                        
                        response.Data = objBidder;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError("Invalid BussinessCode or Password");
                }

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