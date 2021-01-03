using KrupaBuildGallery.Model;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class PurchaseBidController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public PurchaseBidController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/PurchaseBid
        public ActionResult Index(int Status = -2)
        {
            List<BidVM> lstBids= new List<BidVM>();
            try
            {
                lstBids = (from cu in _db.tbl_Bids
                           join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                           join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                           where cu.IsDeleted == 0 && (Status == -2 || cu.BidStatus == Status)
                           select new BidVM
                           {
                               BidId = cu.Pk_Bid_id,
                               ItemId = cu.ItemId.Value,
                               ItemName = itm.ItemName,
                               Qty = cu.Qty.Value,
                               Unittype = unityp.UnitTypeName,
                               BidStatus = cu.BidStatus.Value,
                               BidDate = cu.BidDate.Value,
                               BidNum = cu.BidNo.Value,
                               BidYear = cu.BidYear                               
                           }).OrderByDescending(x => x.BidDate).ToList();
                if (lstBids != null && lstBids.Count() > 0)
                {
                    lstBids.ForEach(x => { x.TotalBids = TotalBids(x.BidId); x.Status = GetGenBidStatus(x.BidStatus); });
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            ViewBag.Status = Status;
            return View(lstBids);
        }
        public int TotalBids(long BidId)
        {
            int? TotalBid = _db.tbl_BidDealers.Where(o => o.Fk_BidId == BidId).ToList().Count;
          
            return Convert.ToInt32(TotalBid);
        }
        public string GetGenBidStatus(long StatusId)
        {
            string Status = "Open";

            if (StatusId == 0)
            {
                Status = "Pending";
            }
            else if (StatusId == 1)
            {
                Status = "Accepted";
            }
            else if(StatusId == 2)
            {
                Status = "Rejected";
            }
            else if (StatusId == 3)
            {
                Status = "Closed";
            }           
            return Status;
        }

        [HttpPost]
        public string DeleteBid(long BidId)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Bids objBid = _db.tbl_Bids.Where(x => x.Pk_Bid_id == BidId).FirstOrDefault();

                if (objBid == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objBid.IsDeleted = 1;
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

        public ActionResult Add()
        {
            BidVM objBidVM = new BidVM();

            objBidVM.BidItemList = GetBidItemList();

            return View(objBidVM);           
        }

        [HttpPost]
        public ActionResult Add(BidVM objBidVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    int year = DateTime.UtcNow.Year;
                    int toyear = year + 1;
                    if (DateTime.UtcNow.Month <= 3)
                    {
                        year = year - 1;
                        toyear = year;
                    }
                    DateTime dtfincialyear = new DateTime(year, 4, 1);
                    DateTime dtendyear = new DateTime(toyear, 3, 31);
                    var objBidstemp = _db.tbl_Bids.Where(o => o.BidDate >= dtfincialyear && o.BidDate <= dtendyear).OrderByDescending(o => o.BidDate).FirstOrDefault();
                    long Bidno = 1;
                    if (objBidstemp != null)
                    {
                        if (objBidstemp.BidNo == null)
                        {
                            objBidstemp.BidNo = 1;
                        }
                        Bidno = objBidstemp.BidNo.Value + 1;
                    }


                    tbl_Bids objBidItm = new tbl_Bids();
                    objBidItm.ItemId = objBidVM.ItemId;
                    objBidItm.BidDate = DateTime.Now;
                    objBidItm.IsDeleted = 0;
                    objBidItm.BidStatus = -1;
                    objBidItm.BidNo = Convert.ToInt32(Bidno);
                    objBidItm.BidYear = year + "-" + toyear;
                    objBidItm.Qty = objBidVM.Qty;
                    objBidItm.Remarks = objBidVM.Remarks;
                    _db.tbl_Bids.Add(objBidItm);
                    _db.SaveChanges();

                    var Itm = _db.tbl_PurchaseBidItems.Where(o => o.Pk_PurchaseBidItemId == objBidVM.ItemId).FirstOrDefault();
                    List<PurchaseDealerVM> lstPurchaseDealerVM = (from cu in _db.tbl_BidDealerItems
                                                             join dl in _db.tbl_PurchaseDealers on cu.Fk_PurchaseDealerId equals dl.Pk_Dealer_Id
                                                             where cu.Fk_ItemId == objBidVM.ItemId
                                                             select new PurchaseDealerVM
                                                             {
                                                                OwnerContactNo = dl.OwnerContactNo
                                                             }).ToList();

                    List<string> lstMobileNos = new List<string>();
                    if(lstPurchaseDealerVM != null && lstPurchaseDealerVM.Count() > 0)
                    {
                        lstMobileNos = lstPurchaseDealerVM.Select(x => x.OwnerContactNo).ToList();
                    }

                    var test = lstMobileNos.ToArray();
                    for (int i = 0; i < lstMobileNos.Count; i += 15)
                    {
                        int length = Math.Min(100, lstMobileNos.Count - i);
                        string[] carray = new string[length];
                        Array.Copy(test, i, carray, 0, length);
                        using (WebClient webClient = new WebClient())
                        {
                            string arrystrmobile = string.Join(",", carray);
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(111566, 999999);
                            string msg = "New Bid for Item:" + Itm.ItemName + "\n";
                            msg += "Shopping & Saving\n";
                            //int SmsId = (int)SMSType.DistributorReqAccepted;
                            //clsCommon objcm = new clsCommon();
                            //string msg = objcm.GetSmsContent(SmsId);
                            // msg = msg.Replace("{{MobileNo}}", objReq.MobileNo + "").Replace("{{Password}}", Password);
                            msg = HttpUtility.UrlEncode(msg);
                            //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";                            
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", arrystrmobile).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                /// return "InvalidNumber";
                            }
                            else
                            {
                                //  return num.ToString();
                            }

                        }
                    }
                    return RedirectToAction("Index");
                }
                    
                   

                    /*      string notibody = "";
                          var obj = _db.tbl_PurchaseBidItems.Where(o => o.Pk_PurchaseBidItemId == objBidVM.ItemId).FirstOrDefault();
                          notibody = "New Bid for Item - " + obj.ItemName + " Qty:" + objBidItm.Qty;
                          WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                          tRequest.Method = "post";
                          //serverKey - Key from Firebase cloud messaging server  
                          tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAav-0MMY:APA91bHt6Bann_b6RoXLP3XxTI-eE5d2Uimxc231h756R8mwxjrJyqnaE959-EYhOsEtRLus1C2mZG_NyY5VACFZAKRkn0S6PSB-1QDBg3EaITICkDutSJRYaoG1Wd23JUmEwwlJcY94"));
                          //Sender Id - From firebase project setting  
                          tRequest.Headers.Add(string.Format("Sender: id={0}", "459556532422"));
                          tRequest.ContentType = "application/json";
                          var payload = new
                          {
                              to = "/topics/ShoppingSaving3",
                              priority = "high",
                              content_available = true,
                              //notification = new
                              //{                           
                              //    body = notificationVM.NotificationDescription,
                              //    title = notificationVM.NotificationTitle,
                              //    image= imgurl,
                              //    click_action = "OPEN_ACTIVITY_1",
                              //    badge = 1,
                              //    sound = "default"
                              //},
                              data = new
                              {
                                  body = notibody,
                                  title = "New Shopping Saving Bid",
                                  notificationdetailid = objBidItm.Pk_Bid_id,
                                  imageurl = "",
                                  badge = 1,
                                  sound = "default"
                              }

                          };

                          string postbody = JsonConvert.SerializeObject(payload).ToString();
                          Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                          tRequest.ContentLength = byteArray.Length;
                          using (Stream dataStream = tRequest.GetRequestStream())
                          {
                              dataStream.Write(byteArray, 0, byteArray.Length);
                              using (WebResponse tResponse = tRequest.GetResponse())
                              {
                                  using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                  {
                                      if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                          {
                                              String sResponseFromServer = tReader.ReadToEnd();
                                              //result.Response = sResponseFromServer;
                                          }
                                  }
                              }
                          }*/

                  
                
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            objBidVM.BidItemList = GetBidItemList();
            return View(objBidVM);
        }

        private List<SelectListItem> GetBidItemList()
        {
            var ItemList = _db.tbl_PurchaseBidItems.Where(x => x.IsDeleted == false)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.Pk_PurchaseBidItemId).Trim(), Text = o.ItemName })
                         .OrderBy(x => x.Text).ToList();

            return ItemList;
        }

        [HttpPost]
        public string CloseBid(long BidId)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Bids objBid = _db.tbl_Bids.Where(x => x.Pk_Bid_id == BidId).FirstOrDefault();

                if (objBid == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objBid.BidStatus = 3;
                    _db.SaveChanges();

                    List<tbl_BidDealers> lstDeler = _db.tbl_BidDealers.Where(o => o.Fk_BidId == BidId).ToList();
                    if(lstDeler != null && lstDeler.Count() > 0)
                    {
                        foreach(tbl_BidDealers objdel in lstDeler)
                        {
                            if(objdel.BidStatus != 1)
                            {
                                objdel.BidStatus = 2;
                            }                            
                        }
                    }
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

        public ActionResult Detail(long Id)
        {
            BidVM objBid = (from cu in _db.tbl_Bids
                       join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                       join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                       where cu.Pk_Bid_id == Id
                       select new BidVM
                       {
                           BidId = cu.Pk_Bid_id,
                           ItemId = cu.ItemId.Value,
                           ItemName = itm.ItemName,
                           Qty = cu.Qty.Value,
                           Unittype = unityp.UnitTypeName,
                           BidStatus = cu.BidStatus.Value,
                           BidDate = cu.BidDate.Value
                       }).OrderByDescending(x => x.BidDate).FirstOrDefault();

            List<BidDealerVM> lstBidDealerVM = (from cu in _db.tbl_BidDealers
                                                     join dl in _db.tbl_PurchaseDealers on cu.FK_DealerId equals dl.Pk_Dealer_Id
                                                     where cu.Fk_BidId == Id
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
                                                         BidStatus = cu.BidStatus.Value
                                                     }).OrderByDescending(x => x.BidSentDate).ToList();
            if (lstBidDealerVM != null && lstBidDealerVM.Count() > 0)
            {
                lstBidDealerVM.ForEach(x =>  x.Status = GetGenBidStatus(x.BidStatus));
            }
            ViewData["Bids"] = lstBidDealerVM;
            ViewData["objBid"] = objBid;
            return View();
        }

        public ActionResult DealerBidDetails(long Id)
        {
            BidDealerVM objBidDealerVM = (from cu in _db.tbl_BidDealers
                                                join dl in _db.tbl_PurchaseDealers on cu.FK_DealerId equals dl.Pk_Dealer_Id
                                                where cu.Pk_BidDealers == Id
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

            objBidDealerVM.Status = GetGenBidStatus(objBidDealerVM.BidStatus);
         

            BidVM objBid = (from cu in _db.tbl_Bids
                            join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                            join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                            where cu.Pk_Bid_id == objBidDealerVM.BidId
                            select new BidVM
                            {
                                BidId = cu.Pk_Bid_id,
                                ItemId = cu.ItemId.Value,
                                ItemName = itm.ItemName,
                                Qty = cu.Qty.Value,
                                Unittype = unityp.UnitTypeName,
                                BidStatus = cu.BidStatus.Value,
                                BidDate = cu.BidDate.Value
                            }).OrderByDescending(x => x.BidDate).FirstOrDefault();

           
            ViewData["Bids"] = objBidDealerVM;
            ViewData["objBid"] = objBid;
            return View();
        }

        [HttpPost]
        public string AcceptRejectBid(long BidId, string IsApprove,string Reason = "")
        {
            string ReturnMessage = "";

            try
            {
                var objBid = _db.tbl_BidDealers.Where(o => o.Pk_BidDealers == BidId).FirstOrDefault();
                if (objBid != null)
                {
                    var objDeler = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == objBid.FK_DealerId).FirstOrDefault();
                    BidVM objBids = (from cu in _db.tbl_Bids
                                     join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                                     join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                                     where cu.Pk_Bid_id == objBid.Fk_BidId
                                     select new BidVM
                                     {
                                         BidId = cu.Pk_Bid_id,
                                         ItemId = cu.ItemId.Value,
                                         ItemName = itm.ItemName,
                                         Qty = cu.Qty.Value,
                                         Unittype = unityp.UnitTypeName,
                                         BidStatus = cu.BidStatus.Value,
                                         BidDate = cu.BidDate.Value,
                                         BidNum = cu.BidNo.Value,
                                         BidYear = cu.BidYear
                                     }).OrderByDescending(x => x.BidDate).FirstOrDefault();
                    string BidNumber = "BD/" + objBids.BidYear + "/" + objBids.BidNum;
                    if (IsApprove == "false")
                    {                       
                        objBid.BidStatus = 2; //   0 For Pending  1 For Accept 2 For Reject
                        objBid.RejectReason = Reason;
                        _db.SaveChanges();
                        try
                        {
                           
                            string ToEmail = objDeler.Email;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;
                            string Subject = "Your Bid " + BidNumber + " for Item - " + objBids.ItemName + " Rejected - Shoping Saving";
                            string bodyhtml = "Following is the reason<br/>";
                            bodyhtml += "===============================<br/>";
                            bodyhtml += Reason;
                            clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                        }
                        catch (Exception e)
                        {
                            string ErrorMessage = e.Message.ToString();
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(111566, 999999);
                            string msg = "Your Bid " + BidNumber + " for Item - " + objBids.ItemName + " Rejected - Shoping Saving\n";
                            msg += "Following Is The Reason:\n";
                            msg += Reason;
                            //int SmsId = (int)SMSType.DistributorReqRejected;
                            //clsCommon objcm = new clsCommon();
                            //string msg = objcm.GetSmsContent(SmsId);
                            //msg = msg.Replace("{{Reason}}", Reason);
                            msg = HttpUtility.UrlEncode(msg);
                            //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", objDeler.OwnerContactNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                /// return "InvalidNumber";
                            }
                            else
                            {
                                //  return num.ToString();
                            }

                        }

                    }
                    else
                    {
                        objBid.BidStatus = 1; //   0 For Pending  1 For Accept 2 For Reject
                        _db.SaveChanges();

                        try
                        {
                            string ToEmail = objDeler.Email;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;
                            string Subject = "Your Bid " + BidNumber + " for Item:" + objBids.ItemName +" Accepted - Shopping & Saving";
                            string bodyhtml = "Your Bid " + BidNumber + " for Item:" + objBids.ItemName + " Accepted<br/>";
                            bodyhtml += "We will contact you asap:<br/>";                      
                            bodyhtml += "Shopping & Saving <br/>";
                            clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                        }
                        catch (Exception e)
                        {
                            string ErrorMessage = e.Message.ToString();
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(111566, 999999);
                            string msg = "Your Bid " + objBids.BidNumber + " for Item:" + objBids.ItemName + " Accepted \n";
                            msg += "We will contact you asap:\n";
                            msg += "Shopping & Saving\n";                            
                            //int SmsId = (int)SMSType.DistributorReqAccepted;
                            //clsCommon objcm = new clsCommon();
                            //string msg = objcm.GetSmsContent(SmsId);
                            // msg = msg.Replace("{{MobileNo}}", objReq.MobileNo + "").Replace("{{Password}}", Password);
                            msg = HttpUtility.UrlEncode(msg);
                            //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";                            
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", objDeler.OwnerContactNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                /// return "InvalidNumber";
                            }
                            else
                            {
                                //  return num.ToString();
                            }

                        }


                    }
                }


                List<tbl_BidDealers> lstBiddle = _db.tbl_BidDealers.Where(o => o.Fk_BidId == objBid.Fk_BidId).ToList();
                if(lstBiddle != null && lstBiddle.Count() > 0)
                {
                    var lst = lstBiddle.Where(o => o.BidStatus.Value == 1).ToList();
                    if(lst != null && lst.Count() > 0)
                    {
                        tbl_Bids objBd = _db.tbl_Bids.Where(o => o.Pk_Bid_id == objBid.Fk_BidId).FirstOrDefault();
                        if(objBd != null)
                        {
                            objBd.BidStatus = 1;
                        }
                    }
                    else
                    {
                        var lst2 = lstBiddle.Where(o => o.BidStatus.Value == 2).ToList();
                        if (lst2 != null && lst2.Count() > 0)
                        {
                            tbl_Bids objBd = _db.tbl_Bids.Where(o => o.Pk_Bid_id == objBid.Fk_BidId).FirstOrDefault();
                            if (objBd != null)
                            {
                                objBd.BidStatus = 2;
                            }
                        }
                    }
                }
                _db.SaveChanges();
                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string AcceptRejectBidMultiple(string BidIds, string IsApprove, string Reason = "")
        {
            string ReturnMessage = "";

            try
            {
                string[] arryBidIds = BidIds.Trim('^').Split('^');

                if(arryBidIds != null && arryBidIds.Count() > 0)
                {
                    foreach(string strbd in arryBidIds)
                    {
                        if(!string.IsNullOrEmpty(strbd))
                        {
                            long BidId = Convert.ToInt64(strbd);
                            var objBid = _db.tbl_BidDealers.Where(o => o.Pk_BidDealers == BidId).FirstOrDefault();
                            if (objBid != null)
                            {
                                var objDeler = _db.tbl_PurchaseDealers.Where(o => o.Pk_Dealer_Id == objBid.FK_DealerId).FirstOrDefault();
                                BidVM objBids = (from cu in _db.tbl_Bids
                                                 join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                                                 join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                                                 where cu.Pk_Bid_id == objBid.Fk_BidId
                                                 select new BidVM
                                                 {
                                                     BidId = cu.Pk_Bid_id,
                                                     ItemId = cu.ItemId.Value,
                                                     ItemName = itm.ItemName,
                                                     Qty = cu.Qty.Value,
                                                     Unittype = unityp.UnitTypeName,
                                                     BidStatus = cu.BidStatus.Value,
                                                     BidDate = cu.BidDate.Value,
                                                     BidNum = cu.BidNo.Value,
                                                     BidYear = cu.BidYear
                                                 }).OrderByDescending(x => x.BidDate).FirstOrDefault();
                                string BidNumber = "BD/" + objBids.BidYear + "/" + objBids.BidNum;
                                if (IsApprove == "false")
                                {
                                    objBid.BidStatus = 2; //   0 For Pending  1 For Accept 2 For Reject
                                    objBid.RejectReason = Reason;
                                    _db.SaveChanges();
                                    try
                                    {

                                        string ToEmail = objDeler.Email;
                                        tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                                        string FromEmail = objGensetting.FromEmail;
                                        string Subject = "Your Bid "+ BidNumber + " for Item - " + objBids.ItemName + " Rejected - Shoping Saving";
                                        string bodyhtml = "Following is the reason<br/>";
                                        bodyhtml += "===============================<br/>";
                                        bodyhtml += Reason;
                                        clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                                    }
                                    catch (Exception e)
                                    {
                                        string ErrorMessage = e.Message.ToString();
                                    }

                                    using (WebClient webClient = new WebClient())
                                    {
                                        WebClient client = new WebClient();
                                        Random random = new Random();
                                        int num = random.Next(111566, 999999);
                                        string msg = "Your Bid " + BidNumber + " for Item - " + objBids.ItemName + " Rejected - Shoping Saving\n";
                                        msg += "Following Is The Reason:\n";
                                        msg += Reason;
                                        //int SmsId = (int)SMSType.DistributorReqRejected;
                                        //clsCommon objcm = new clsCommon();
                                        //string msg = objcm.GetSmsContent(SmsId);
                                        //msg = msg.Replace("{{Reason}}", Reason);
                                        msg = HttpUtility.UrlEncode(msg);
                                        //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                                        string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", objDeler.OwnerContactNo).Replace("--MSG--", msg);
                                        var json = webClient.DownloadString(url);
                                        if (json.Contains("invalidnumber"))
                                        {
                                            /// return "InvalidNumber";
                                        }
                                        else
                                        {
                                            //  return num.ToString();
                                        }

                                    }

                                }
                                else
                                {
                                    objBid.BidStatus = 1; //   0 For Pending  1 For Accept 2 For Reject
                                    _db.SaveChanges();

                                    try
                                    {
                                        string ToEmail = objDeler.Email;
                                        tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                                        string FromEmail = objGensetting.FromEmail;
                                        string Subject = "Your Bid " + BidNumber + " for Item:" + objBids.ItemName + " Accepted - Shopping & Saving";
                                        string bodyhtml = "Your Bid " + BidNumber + " for Item:" + objBids.ItemName + " Accepted<br/>";
                                        bodyhtml += "We will contact you asap:<br/>";
                                        bodyhtml += "Shopping & Saving <br/>";
                                        clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                                    }
                                    catch (Exception e)
                                    {
                                        string ErrorMessage = e.Message.ToString();
                                    }

                                    using (WebClient webClient = new WebClient())
                                    {
                                        WebClient client = new WebClient();
                                        Random random = new Random();
                                        int num = random.Next(111566, 999999);
                                        string msg = "Your Bid for Item:" + objBids.ItemName + " Accepted \n";
                                        msg += "We will contact you asap:\n";
                                        msg += "Shopping & Saving\n";
                                        //int SmsId = (int)SMSType.DistributorReqAccepted;
                                        //clsCommon objcm = new clsCommon();
                                        //string msg = objcm.GetSmsContent(SmsId);
                                        // msg = msg.Replace("{{MobileNo}}", objReq.MobileNo + "").Replace("{{Password}}", Password);
                                        msg = HttpUtility.UrlEncode(msg);
                                        //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";                            
                                        string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", objDeler.OwnerContactNo).Replace("--MSG--", msg);
                                        var json = webClient.DownloadString(url);
                                        if (json.Contains("invalidnumber"))
                                        {
                                            /// return "InvalidNumber";
                                        }
                                        else
                                        {
                                            //  return num.ToString();
                                        }

                                    }


                                }

                                List<tbl_BidDealers> lstBiddle = _db.tbl_BidDealers.Where(o => o.Fk_BidId == objBid.Fk_BidId).ToList();
                                if (lstBiddle != null && lstBiddle.Count() > 0)
                                {
                                    var lst = lstBiddle.Where(o => o.BidStatus.Value == 1).ToList();
                                    if (lst != null && lst.Count() > 0)
                                    {
                                        tbl_Bids objBd = _db.tbl_Bids.Where(o => o.Pk_Bid_id == objBid.Fk_BidId).FirstOrDefault();
                                        if (objBd != null)
                                        {
                                            objBd.BidStatus = 1;
                                        }
                                    }
                                    else
                                    {
                                        var lst2 = lstBiddle.Where(o => o.BidStatus.Value == 2).ToList();
                                        if (lst2 != null && lst2.Count() > 0)
                                        {
                                            tbl_Bids objBd = _db.tbl_Bids.Where(o => o.Pk_Bid_id == objBid.Fk_BidId).FirstOrDefault();
                                            if (objBd != null)
                                            {
                                                objBd.BidStatus = 2;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        _db.SaveChanges();
                    }
                }           


                _db.SaveChanges();
                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult BidReportItemWise()
        {
            ViewData["ItemList"] = GetBidItemList(); 
            return View("~/Areas/Admin/Views/PurchaseBid/BidReportByItem.cshtml");
        }

        public void ExportBidReportItemWise(long ItemId, string StartDate, string EndDate)
        {
            ExcelPackage excel = new ExcelPackage();
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);
            var objPrdItm = _db.tbl_ProductItems.Where(o => o.ProductItemId == ItemId).FirstOrDefault();
            List<long> lstItems = new List<long>();
            List<BidReportItemsVM> lstReports = new List<BidReportItemsVM>();
            if (ItemId == -1)
            {
                lstItems = _db.tbl_PurchaseBidItems.OrderBy(x => x.ItemName).ToList().Select(x => x.Pk_PurchaseBidItemId).ToList();
            }
            else
            {
                lstItems.Add(ItemId);
            }

            foreach (long ItmId in lstItems)
            {
                BidReportItemsVM objItm = new BidReportItemsVM();
                objItm.ItemId = ItemId;
                var objBidItm = _db.tbl_PurchaseBidItems.Where(o => o.Pk_PurchaseBidItemId == ItmId).FirstOrDefault();
                if (objBidItm != null)
                {
                    objItm.ItemName = objBidItm.ItemName;
                }
                List<BidVM> lstBdVM = new List<BidVM>();
                List<tbl_Bids> lstBids = _db.tbl_Bids.Where(o => o.BidDate >= dtStart && o.BidDate <= dtEnd && o.ItemId == ItmId).OrderBy(x => x.BidDate).ToList();
                if (lstBids != null && lstBids.Count() > 0)
                {
                    foreach (var objBd in lstBids)
                    {
                        BidVM objBid = (from cu in _db.tbl_Bids
                                        join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                                        join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                                        where cu.Pk_Bid_id == objBd.Pk_Bid_id
                                        select new BidVM
                                        {
                                            BidId = cu.Pk_Bid_id,
                                            ItemId = cu.ItemId.Value,
                                            ItemName = itm.ItemName,
                                            Qty = cu.Qty.Value,
                                            BidNum = cu.BidNo.Value,
                                            BidYear = cu.BidYear,
                                            Unittype = unityp.UnitTypeName,
                                            BidStatus = cu.BidStatus.Value,
                                            BidDate = cu.BidDate.Value
                                        }).OrderByDescending(x => x.BidDate).FirstOrDefault();
                        objBid.Status = GetGenBidStatus(objBid.BidStatus);
                        List<BidDealerVM> lstBidDealerVM = (from cu in _db.tbl_BidDealers
                                                            join dl in _db.tbl_PurchaseDealers on cu.FK_DealerId equals dl.Pk_Dealer_Id
                                                            where cu.Fk_BidId == objBd.Pk_Bid_id
                                                            select new BidDealerVM
                                                            {
                                                                BidDealerId = cu.Pk_BidDealers,
                                                                DealerId = dl.Pk_Dealer_Id,
                                                                FirmName = dl.FirmName,
                                                                BusinessCode = dl.BussinessCode,
                                                                BidValidDays = cu.BidValidDays.Value,
                                                                FirmMobile = dl.FirmContactNo,
                                                                Price = cu.Price.Value,
                                                                OwnerContactNo = dl.OwnerContactNo,
                                                                BidSentDate = cu.BidSendDate.Value,
                                                                MinimumQtytoBuy = cu.MinimumQtyToBuy.Value,
                                                                BidStatus = cu.BidStatus.Value
                                                            }).OrderByDescending(x => x.BidSentDate).ToList();
                        if (lstBidDealerVM != null && lstBidDealerVM.Count() > 0)
                        {
                            lstBidDealerVM.ForEach(x => x.Status = GetGenBidStatus(x.BidStatus));
                        }
                        objBid.lstBidDealer = lstBidDealerVM;
                        lstBdVM.Add(objBid);
                    }
                }
                objItm.lstBids = lstBdVM;
                lstReports.Add(objItm);
            }
            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "Item Name", "Bid Number", "Bid Date", "Qty", "BusinessCode", "Firm Name", "Contact Number", "Bid Receive Date", "Bid Price","Bid Status", "Bid Valid Days" };
            var workSheet = excel.Workbook.Worksheets.Add("Bid Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Bid Report: " + StartDate + " to " + EndDate;
            for (var col = 1; col < arrycolmns.Length + 1; col++)
            {
                workSheet.Cells[2, col].Style.Font.Bold = true;
                workSheet.Cells[2, col].Style.Font.Size = 12;
                workSheet.Cells[2, col].Value = arrycolmns[col - 1];
                workSheet.Cells[2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[2, col].AutoFitColumns(30, 70);
                workSheet.Cells[2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.WrapText = true;
            }
            int row1 = 1;
            foreach (var obj in lstReports)
            {
                decimal TotalDateWise = 0;
                decimal TotalDateWiseQty = 0;
                workSheet.Cells[row1 + 2, 1].Style.Font.Bold = true;
                workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                workSheet.Cells[row1 + 2, 1].Value = obj.ItemName;
                workSheet.Cells[row1 + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Cells[row1 + 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[row1 + 2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[row1 + 2, 1].Style.WrapText = true;
                workSheet.Cells[row1 + 2, 1].AutoFitColumns(30, 70);
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                workSheet.Cells[row1 + 2, 1, row1 + 2, arrycolmns.Length - 1].Merge = true;

                row1 = row1 + 1;                
                if (obj.lstBids != null && obj.lstBids.Count() > 0)
                {
                    foreach (var ordrr in obj.lstBids)
                    {
                        string BidNo = "BD/" + ordrr.BidYear + "/" + ordrr.BidNum;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 2].Value = BidNo;
                        workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 3].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 3].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 3].Value = ordrr.BidDate.ToString("dd/MM/yyyy");
                        workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3, row1 + 2, arrycolmns.Length].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.WrapText = true;                     
                        workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 4].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 4].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 4].Value = ordrr.Qty +" "+ordrr.Unittype;
                        workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 4, row1 + 2, arrycolmns.Length].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4, row1 + 2, arrycolmns.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4, row1 + 2, arrycolmns.Length].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4, row1 + 2, arrycolmns.Length].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 4, row1 + 2, arrycolmns.Length].Merge = true;
                        workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);
                        row1 = row1 + 1;                        
                        if (ordrr.lstBidDealer != null && ordrr.lstBidDealer.Count() > 0)
                        {
                            foreach (var objItem in ordrr.lstBidDealer)
                            {
                               
                                for (var col = 5; col < arrycolmns.Length + 1; col++)
                                {
                                    if (arrycolmns[col - 1] == "BusinessCode")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.BusinessCode;
                                    }
                                    else if (arrycolmns[col - 1] == "Firm Name")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.FirmName;
                                    }
                                    else if (arrycolmns[col - 1] == "Contact Number")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.OwnerContactNo;
                                    }
                                    else if (arrycolmns[col - 1] == "Bid Receive Date")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.BidSentDate.ToString("dd/MM/yyyy");
                                    }
                                    else if (arrycolmns[col - 1] == "Bid Price")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Price;
                                    }
                                    else if (arrycolmns[col - 1] == "Bid Status")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.Status;
                                    }
                                    else if (arrycolmns[col - 1] == "Bid Valid Days")
                                    {
                                        workSheet.Cells[row1 + 2, col].Value = objItem.BidValidDays;
                                    }
                                   
                                    workSheet.Cells[row1 + 2, col].Style.Font.Bold = false;
                                    workSheet.Cells[row1 + 2, col].Style.Font.Size = 12;
                                    workSheet.Cells[row1 + 2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    workSheet.Cells[row1 + 2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    workSheet.Cells[row1 + 2, col].Style.WrapText = true;
                                    workSheet.Cells[row1 + 2, col].AutoFitColumns(30, 70);
                                }
                                row1 = row1 + 1;
                            }
                        }
                       
                        row1 = row1 + 1;
                    }
                    row1 = row1 + 1;                   
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Bid Report Item.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult GetBidReportItemWise(long ItemId, string StartDate, string EndDate)
        {
            DateTime dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            DateTime dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);
            var objPrdItm = _db.tbl_ProductItems.Where(o => o.ProductItemId == ItemId).FirstOrDefault();
            List<long> lstItems = new List<long>();
            List<BidReportItemsVM> lstReports = new List<BidReportItemsVM>();
            if(ItemId == -1)
            {
                lstItems = _db.tbl_PurchaseBidItems.OrderBy(x => x.ItemName).ToList().Select(x => x.Pk_PurchaseBidItemId).ToList();
            }
            else
            {
                lstItems.Add(ItemId);
            }

            foreach(long ItmId in lstItems)
            {
                BidReportItemsVM objItm = new BidReportItemsVM();
                objItm.ItemId = ItemId;
                var objBidItm = _db.tbl_PurchaseBidItems.Where(o => o.Pk_PurchaseBidItemId == ItmId).FirstOrDefault();
                if(objBidItm != null)
                {
                    objItm.ItemName = objBidItm.ItemName;
                }
                List<BidVM> lstBdVM=  new List<BidVM>();              
                List<tbl_Bids> lstBids = _db.tbl_Bids.Where(o => o.BidDate >= dtStart && o.BidDate <= dtEnd && o.ItemId == ItmId).OrderBy(x => x.BidDate).ToList();
                if(lstBids != null && lstBids.Count() > 0)
                {
                    foreach(var objBd in lstBids)
                    {
                        BidVM objBid = (from cu in _db.tbl_Bids
                                        join itm in _db.tbl_PurchaseBidItems on cu.ItemId equals itm.Pk_PurchaseBidItemId
                                        join unityp in _db.tbl_BidItemUnitTypes on itm.UnitType equals unityp.BidItemUnitTypeId
                                        where cu.Pk_Bid_id == objBd.Pk_Bid_id
                                        select new BidVM
                                        {
                                            BidId = cu.Pk_Bid_id,
                                            ItemId = cu.ItemId.Value,
                                            ItemName = itm.ItemName,
                                            Qty = cu.Qty.Value,
                                            Unittype = unityp.UnitTypeName,
                                            BidStatus = cu.BidStatus.Value,
                                            BidDate = cu.BidDate.Value
                                        }).OrderByDescending(x => x.BidDate).FirstOrDefault();
                        objBid.Status = GetGenBidStatus(objBid.BidStatus);
                        List<BidDealerVM> lstBidDealerVM = (from cu in _db.tbl_BidDealers
                                                            join dl in _db.tbl_PurchaseDealers on cu.FK_DealerId equals dl.Pk_Dealer_Id
                                                            where cu.Fk_BidId == objBd.Pk_Bid_id
                                                            select new BidDealerVM
                                                            {
                                                                BidDealerId = cu.Pk_BidDealers,
                                                                DealerId = dl.Pk_Dealer_Id,
                                                                FirmName = dl.FirmName,
                                                                BusinessCode = dl.BussinessCode,
                                                                BidValidDays = cu.BidValidDays.Value,
                                                                FirmMobile = dl.FirmContactNo,
                                                                Price = cu.Price.Value,
                                                                OwnerContactNo = dl.OwnerContactNo,
                                                                BidSentDate = cu.BidSendDate.Value,
                                                                MinimumQtytoBuy = cu.MinimumQtyToBuy.Value,
                                                                BidStatus = cu.BidStatus.Value
                                                            }).OrderByDescending(x => x.BidSentDate).ToList();
                        if (lstBidDealerVM != null && lstBidDealerVM.Count() > 0)
                        {
                            lstBidDealerVM.ForEach(x => x.Status = GetGenBidStatus(x.BidStatus));
                        }
                        objBid.lstBidDealer = lstBidDealerVM;
                        lstBdVM.Add(objBid);
                    }
                }
                objItm.lstBids = lstBdVM;
                lstReports.Add(objItm);
            }
            return PartialView("~/Areas/Admin/Views/PurchaseBid/_BidReportItemwise.cshtml", lstReports);
        }
    }
}