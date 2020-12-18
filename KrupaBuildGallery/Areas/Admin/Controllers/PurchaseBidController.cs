using KrupaBuildGallery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
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
                               BidDate = cu.BidDate.Value
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
                                         BidDate = cu.BidDate.Value
                                     }).OrderByDescending(x => x.BidDate).FirstOrDefault();
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
                            string Subject = "Your Bid for Item - "+ objBids.ItemName + " Rejected - Shoping Saving";
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
                            string msg = "Your Bid for Item - " + objBids.ItemName + " Rejected - Shoping Saving\n";
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
                            string Subject = "Your Bid for Item:"+ objBids.ItemName +" Accepted - Shopping & Saving";
                            string bodyhtml = "Your Bid for Item:" + objBids.ItemName + " Accepted<br/>";
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
    }
}