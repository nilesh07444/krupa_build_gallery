using ConstructionDiary.Models;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.ViewModel;
using System.Net;
using System.Configuration;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{    
    [CustomAuthorize] 
    public class OrderController : Controller
    {
		private readonly krupagallarydbEntities _db;
        public OrderController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Order
        public ActionResult Index(int Status = -1)
        {
            
            List<OrderVM> lstOrders = new List<OrderVM>();
            try
            {

                lstOrders = (from p in _db.tbl_Orders
                             join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                               where !p.IsDelete && (Status == -1 || p.OrderStatusId == Status)
                             select new OrderVM
                               {
                                   OrderId = p.OrderId,
                                   ClientUserName = c.FirstName+" "+c.LastName,
                                   ClientUserId = p.ClientUserId,
                                   OrderAmount = p.OrderAmount,
                                   OrderShipCity = p.OrderShipCity,
                                   OrderShipState = p.OrderShipState,
                                   OrderShipAddress = p.OrderShipAddress,
                                   OrderPincode = p.OrderShipPincode,
                                   OrderShipClientName = p.OrderShipClientName,
                                   OrderShipClientPhone = p.OrderShipClientPhone,
                                   OrderStatusId =  p.OrderStatusId,       
                                   PaymentType = p.PaymentType,
                                   OrderDate = p.CreatedDate,
                                   ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                                   ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                               }).OrderByDescending(x => x.OrderDate).ToList();

                if(lstOrders != null && lstOrders.Count() > 0)
                {
                    lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
                }
                ViewBag.Status = Status;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstOrders);
        }

        public ActionResult Detail(long Id)
        {
            OrderVM objOrder = new OrderVM();
            objOrder = (from p in _db.tbl_Orders
                         join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                         where p.OrderId == Id
                         select new OrderVM
                         {
                             OrderId = p.OrderId,
                             ClientUserName = c.FirstName + " " + c.LastName,
                             ClientUserId = p.ClientUserId,
                             OrderAmount = p.OrderAmount,
                             OrderShipCity = p.OrderShipCity,
                             OrderShipState = p.OrderShipState,
                             OrderShipAddress = p.OrderShipAddress,
                             OrderPincode = p.OrderShipPincode,
                             OrderShipClientName = p.OrderShipClientName,
                             OrderShipClientPhone = p.OrderShipClientPhone,
                             OrderStatusId = p.OrderStatusId,
                             PaymentType = p.PaymentType,
                             OrderDate = p.CreatedDate,
                             ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                             ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                         }).OrderByDescending(x => x.OrderDate).FirstOrDefault();          
            if(objOrder != null)
            {   
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                            join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                            where p.OrderId == Id
                            select new OrderItemsVM
                            {
                                OrderId = p.OrderId.Value,
                                OrderItemId = p.OrderDetailId,
                                ProductItemId = p.ProductItemId.Value,
                                ItemName = p.ItemName,
                                Qty = p.Qty.Value,
                                Price = p.Price.Value,
                                Sku = p.Sku,
                                GSTAmt = p.GSTAmt.Value,
                                IGSTAmt = p.IGSTAmt.Value,
                                ItemImg = c.MainImage,
                                Discount = p.Discount.HasValue ? p.Discount.Value : 0
                            }).OrderByDescending(x => x.OrderItemId).ToList();
                objOrder.OrderItems = lstOrderItms;
            }
            return View(objOrder);
        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }

        [HttpPost]
        public string ChangeOrderStatus(long OrderId,int Status,string Dispatchtime)
        {
            tbl_Orders objordr =  _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if(objordr != null)
            {
                objordr.OrderStatusId = Status;
                long clientusrid = objordr.ClientUserId;
                _db.SaveChanges();
                if(Status == 2)
                {
                    tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    if (objclntusr != null)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            string msg = "Your order no."+ objordr.OrderId+" has been confirmed. We will dispatch your order within " + Dispatchtime;
                            string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objclntusr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                return "InvalidNumber";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(objclntusr.Email))
                                {
                                    tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                                    string FromEmail = objGensetting.FromEmail;
                                  
                                    string msg1 = "Your order #" + objordr.OrderId + " has been confirmed. We will dispatch your order within " + Dispatchtime;
                                    clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order has been confirmed - Krupa Build Gallery", msg1);
                                }
                            }

                        }
                    }
                }
                else if(Status == 3)
                {
                    tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
                    if (objclntusr != null)
                    {
                        using (WebClient webClient = new WebClient())
                        {                            
                         
                            string msg = "Your order no." + objordr.OrderId + " has been dispatched";
                            string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objclntusr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                return "InvalidNumber";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(objclntusr.Email))
                                {
                                    tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                                    string FromEmail = objGensetting.FromEmail;
                                  
                                    string msg1 = "Your order #" + objordr.OrderId + " has been dispatched";
                                    clsCommon.SendEmail(objclntusr.Email, FromEmail, "Your Order has been dispatched - Krupa Build Gallery", msg1);
                                }
                            }

                        }
                    }
                }              
            }
            
            return "";
        }

        [HttpPost]
        public string SetShipCharge(long OrderId,decimal ShippingCharge)
        {
            tbl_Orders objordr = _db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if(objordr != null)
            {
                objordr.ShippingCharge = ShippingCharge;
            }
            long clientusrid = objordr.ClientUserId;
            tbl_ClientUsers objclntusr = _db.tbl_ClientUsers.Where(o => o.ClientUserId == clientusrid).FirstOrDefault();
            if (objclntusr != null)
            {
                using (WebClient webClient = new WebClient())
                {
                    string msg = "Shipping Charges for Your order no." + objordr.OrderId + " is: Rs " + ShippingCharge + ". Please pay from your order details you can find button to pay.";
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objclntusr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(objclntusr.Email))
                        {
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;

                            string msg1 = "Shipping Charges for Your order no." + objordr.OrderId + " is: Rs " + ShippingCharge + ". Please pay from your order details you can find button to pay.";
                             clsCommon.SendEmail(objclntusr.Email, FromEmail, "Shipping Charge - Krupa Build Gallery", msg1);
                           
                        }
                    }

                }
            }
            _db.SaveChanges();
            return "";
        }

        public ActionResult OrderItemClientRequests(int Status = 0)
        {

            List<OrderItemRequestsVM> lstItemClientRequests = new List<OrderItemRequestsVM>();
            try
            {
                lstItemClientRequests = (from p in _db.tbl_ItemReturnCancelReplace
                             join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                             join ordritems in _db.tbl_OrderItemDetails on p.ItemId equals ordritems.OrderDetailId
                             join Itm in _db.tbl_ProductItems on ordritems.ProductItemId equals Itm.ProductItemId
                             where (Status == 0 && p.IsApproved == null) || (p.ItemStatus == Status && p.IsApproved == true)
                             select new OrderItemRequestsVM
                             {
                                 OrderItemRequestId = p.ItemReturnCancelReplaceId,
                                 OrderId = p.OrderId.Value,                                 
                                 ItemName = Itm.ItemName,
                                 Amount = p.Amount.Value,
                                 Reason = p.Reason,
                                 OrderItemId = p.ItemId.Value,
                                 ItemStatus = p.ItemStatus.Value,                              
                                 DateCreated = p.DateCreated.Value
                             }).OrderByDescending(x => x.DateCreated).ToList();

              
                ViewBag.Status = Status;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstItemClientRequests);
        }

        [HttpPost]
        public string ApproveRejectItemRequest(string requestid,string aprprovereject)
        {
            long ItmRequestId = Convert.ToInt64(requestid);
            string msgsms = "";
            tbl_ItemReturnCancelReplace objReq =_db.tbl_ItemReturnCancelReplace.Where(o => o.ItemReturnCancelReplaceId == ItmRequestId).FirstOrDefault();
            tbl_ClientUsers objClient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == objReq.ClientUserId).FirstOrDefault();
            tbl_OrderItemDetails objOrderItm = _db.tbl_OrderItemDetails.Where(o => o.OrderDetailId == objReq.ItemId).FirstOrDefault();

            if (aprprovereject == "reject")
            {
                objReq.IsApproved = false;
                string mobilenumber = objClient.MobileNo;
                if (objReq.ItemStatus == 6)
                {
                    msgsms = "Item Return Request Rejected for Order No." + objReq.OrderId;
                    objOrderItm.ItemStatus = 4;                    
                }
                else if (objReq.ItemStatus == 7)
                {
                    objOrderItm.ItemStatus = 4;
                    msgsms = "Item Replace Request Rejected for Order No." + objReq.OrderId;                   
                }
                else if (objReq.ItemStatus == 8)
                {
                    objOrderItm.ItemStatus = 4;
                    msgsms = "Item Exchange Request Rejected for Order No." + objReq.OrderId;                   
                }
                _db.SaveChanges();
                SendMessageSMS(mobilenumber, msgsms);

            }
            else
            {
                string mobilenumber = objClient.MobileNo;
                if (objReq.ItemStatus == 6)
                {
                    objReq.IsApproved = true;
                    _db.SaveChanges();
                }
                else if (objReq.ItemStatus == 7)
                {
                    objReq.IsApproved = true;
                    _db.SaveChanges();
                }
                else if (objReq.ItemStatus == 8)
                {
                    objReq.IsApproved = true;
                    objOrderItm.IsDelete = true;
                    decimal amt = objReq.Amount.Value;
                    decimal deprc = Math.Round((objReq.Amount.Value * 3) / 100, 2);
                    decimal amtredund = amt - deprc;
                    tbl_Wallet objWlt = new tbl_Wallet();
                    objWlt.Amount = amtredund;
                    objWlt.CreditDebit = "Credit";
                    objWlt.ItemId = objReq.ItemId;
                    objWlt.OrderId = objReq.OrderId;
                    objWlt.ClientUserId = objReq.ClientUserId;
                    objWlt.WalletDate = DateTime.UtcNow;
                    objWlt.Description = "Amount Refund to Wallet Order #" + objReq.OrderId;
                    _db.tbl_Wallet.Add(objWlt);                   
                    if (objClient != null)
                    {
                        decimal amtwlt = objClient.WalletAmt.HasValue ? objClient.WalletAmt.Value : 0;
                        amtwlt = amtwlt + amtredund;
                        objClient.WalletAmt = amtwlt;
                        _db.SaveChanges();
                    }
                    _db.SaveChanges();
                    msgsms = "You Item is Exchanged for Order No." + objReq.OrderId + " . Amount Rs." + amtredund + " Refunded to your wallet";
                    SendMessageSMS(mobilenumber, msgsms);
                }
            }
             

            return "Success";
        }

        public string SendMessageSMS(string mobile, string msg)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + mobile + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return "success";
                    }

                }
            }
            catch (WebException ex)
            {
                return "fail";
            }
        }
    }
}