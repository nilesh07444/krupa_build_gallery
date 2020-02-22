using ConstructionDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class DistributorController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public DistributorController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Distributor
        public ActionResult Index()
        {
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
            try
            {

                lstClientUser = (from cu in _db.tbl_ClientUsers
                                 join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                                 where !cu.IsDelete && cu.ClientRoleId == 2
                                 select new ClientUserVM
                                 {
                                     ClientUserId = cu.ClientUserId,
                                     FirstName = cu.FirstName,
                                     LastName = cu.LastName,
                                     UserName = cu.UserName,
                                     Email = cu.Email,
                                     Password = cu.Password,
                                     RoleId = cu.ClientRoleId,
                                     CompanyName = cu.CompanyName,
                                     ProfilePic = cu.ProfilePicture,
                                     MobileNo = cu.MobileNo,
                                     IsActive = cu.IsActive,
                                     City = co.City,
                                     State = co.State,
                                     AddharCardNo = co.Addharcardno,
                                     PanCardNo = co.Pancardno,
                                     GSTNo = co.GSTno
                                 }).OrderBy(x => x.FirstName).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstClientUser);
        }

        public ActionResult RequestList()
        {
            List<DistributorRequestVM> lstDistriRequest = new List<DistributorRequestVM>();
            try
            {

                lstDistriRequest = (from cu in _db.tbl_DistributorRequestDetails                                  
                                 where !cu.IsDelete.Value
                                 select new DistributorRequestVM
                                 {
                                     DistributorRequestId = cu.DistributorRequestId,
                                     FirstName = cu.FirstName,
                                     LastName = cu.LastName,                                     
                                     Email = cu.Email,                                     
                                     CompanyName = cu.CompanyName,                                     
                                     MobileNo = cu.MobileNo,                                     
                                     City = cu.City,
                                     State = cu.State,
                                     AddharCardNo = cu.AddharcardNo,
                                     PanCardNo = cu.PanCardNo,
                                     GSTNo = cu.GSTNo
                                 }).OrderBy(x => x.FirstName).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstDistriRequest);
        }

        public ActionResult Detail(long Id)
        {
            ClientUserVM objClientUserVM = (from cu in _db.tbl_ClientUsers
                             join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                             where !cu.IsDelete && cu.ClientUserId == Id
                             select new ClientUserVM
                             {
                                 ClientUserId = cu.ClientUserId,
                                 FirstName = cu.FirstName,
                                 LastName = cu.LastName,
                                 UserName = cu.UserName,
                                 Email = cu.Email,
                                 Password = cu.Password,
                                 RoleId = cu.ClientRoleId,
                                 CompanyName = cu.CompanyName,
                                 ProfilePic = cu.ProfilePicture,
                                 MobileNo = cu.MobileNo,
                                 IsActive = cu.IsActive,
                                 City = co.City,
                                 State = co.State,
                                 AddharCardNo = co.Addharcardno,
                                 PanCardNo = co.Pancardno,
                                 GSTNo = co.GSTno == "" ?"N/A":co.GSTno
                             }).FirstOrDefault();

            List<OrderVM> lstOrders = new List<OrderVM>();
            lstOrders = (from p in _db.tbl_Orders                        
                        where p.ClientUserId == Id
                        select new OrderVM
                        {
                            OrderId = p.OrderId,                            
                            ClientUserId = p.ClientUserId,
                            OrderAmount = p.OrderAmount,
                            OrderShipCity = p.OrderShipCity,
                            OrderShipState = p.OrderShipState,
                            OrderShipAddress = p.OrderShipAddress,
                            OrderPincode = p.OrderShipPincode,
                            OrderShipClientName = p.OrderShipClientName,
                            OrderShipClientPhone = p.OrderShipClientPhone,
                            OrderStatusId = p.OrderStatusId,
                            OrderAmountDue = p.AmountDue.Value,
                            PaymentType = p.PaymentType,
                            OrderDate = p.CreatedDate
                        }).OrderByDescending(x => x.OrderDate).ToList();
            if (lstOrders != null && lstOrders.Count() > 0)
            {
                lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
            }
            objClientUserVM.OrderList = lstOrders;
            //if (lstOrders != null)
            //{
            //    objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
            //    List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
            //                                       join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
            //                                       where p.OrderId == Id
            //                                       select new OrderItemsVM
            //                                       {
            //                                           OrderId = p.OrderId.Value,
            //                                           OrderItemId = p.OrderDetailId,
            //                                           ProductItemId = p.ProductItemId.Value,
            //                                           ItemName = p.ItemName,
            //                                           Qty = p.Qty.Value,
            //                                           Price = p.Price.Value,
            //                                           Sku = p.Sku,
            //                                           GSTAmt = p.GSTAmt.Value,
            //                                           IGSTAmt = p.IGSTAmt.Value,
            //                                           ItemImg = c.MainImage
            //                                       }).OrderByDescending(x => x.OrderItemId).ToList();
            //    objOrder.OrderItems = lstOrderItms;
            //}
            return View(objClientUserVM);            
        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }

        public ActionResult PaymentDetail(long Id)
        {
            List<PaymentHistoryVM> lstPaymentHist = new List<PaymentHistoryVM>();
            lstPaymentHist = (from p in _db.tbl_PaymentHistory
                        join o in _db.tbl_Orders on p.OrderId equals o.OrderId
                        where p.OrderId == Id
                        select new PaymentHistoryVM
                        {
                            OrderId = p.OrderId,
                            AmountDue = p.AmountDue,
                            OrderTotalAmout = o.OrderAmount,
                            AmountPaid = p.AmountPaid,
                            PaymentDate = p.DateOfPayment,
                            PaymentHistoryId = p.PaymentHistory_Id,
                            Paymentthrough = p.PaymentBy,
                            CurrentAmountDue = o.AmountDue.Value
                        }).OrderBy(x => x.PaymentDate).ToList();
           ViewData["orderobj"] =_db.tbl_Orders.Where(o => o.OrderId == Id).FirstOrDefault();
            return View(lstPaymentHist);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string MakePayment(FormCollection frm)
        {
            decimal AmountDue = 0, AmountPaid = 0;
            long OrderId = 0;
            DateTime PymtRecievedDate = new DateTime();
            if (frm["AmountDue"] != null)
            {
                AmountDue = Convert.ToDecimal(frm["AmountDue"].ToString());
            }
            if (frm["OrderId"] != null)
            {
                OrderId = Convert.ToInt64(frm["OrderId"].ToString());
            }
            if (frm["AmountPaid"] != null)
            {
                AmountPaid = Convert.ToDecimal(frm["AmountPaid"].ToString());
            }
            if (frm["PymtRecievedDate"] != null)
            {
                PymtRecievedDate = Convert.ToDateTime(frm["PymtRecievedDate"].ToString());
            }
            tbl_PaymentHistory objPyment = new tbl_PaymentHistory();
            objPyment.OrderId = OrderId;
            objPyment.PaymentBy = "Cash";
            objPyment.AmountDue = AmountDue;
            objPyment.AmountPaid = AmountPaid;
            objPyment.DateOfPayment = PymtRecievedDate;
            objPyment.CreatedBy = clsAdminSession.UserID;
            objPyment.CreatedDate = DateTime.Now;
            _db.tbl_PaymentHistory.Add(objPyment);
            _db.SaveChanges();

            var objOrder =_db.tbl_Orders.Where(o => o.OrderId == OrderId).FirstOrDefault();
            if(objOrder != null)
            {
                objOrder.AmountDue = AmountDue - AmountPaid;
                long ClientUserId = objOrder.ClientUserId;
                tbl_ClientOtherDetails objtbl_ClientOtherDetails =_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == ClientUserId).FirstOrDefault();
                objtbl_ClientOtherDetails.AmountDue = objtbl_ClientOtherDetails.AmountDue - AmountPaid;
                _db.SaveChanges();
            }
            return "Success";
        }         
    }
}