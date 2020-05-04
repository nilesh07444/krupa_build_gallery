using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class OrderController : ApiController
    {
        krupagallarydbEntities _db;
        public OrderController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("GetOrderList"), HttpPost]
        public ResponseDataModel<List<OrderVM>> GetOrderList(GeneralVM objGen)
        {
            ResponseDataModel<List<OrderVM>> response = new ResponseDataModel<List<OrderVM>>();
            List<OrderVM> lstOrders = new List<OrderVM>();
            try
            {
                long UserId = Convert.ToInt64(objGen.ClientUserId);
                long RoleId = Convert.ToInt64(objGen.RoleId);                
                lstOrders = (from p in _db.tbl_Orders
                             join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                             where !p.IsDelete && p.ClientUserId == UserId
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
                                 OrderAmountDue = p.AmountDue.HasValue ? p.AmountDue.Value : 0,
                                 ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                                 ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                             }).OrderByDescending(x => x.OrderDate).ToList();

                if (lstOrders != null && lstOrders.Count() > 0)
                {
                    lstOrders.ForEach(x => {x.OrderStatus = GetOrderStatus(x.OrderStatusId);x.OrderDateString = CommonMethod.ConvertFromUTC(x.OrderDate); });
                }

                response.Data = lstOrders;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }

        [Route("GetOrderDetails"), HttpPost]
        public ResponseDataModel<OrderVM> GetOrderDetails(GeneralVM objGen)
        {
            ResponseDataModel<OrderVM> response = new ResponseDataModel<OrderVM>();
            OrderVM objOrder = new OrderVM();
            try
            {
                long Id = Convert.ToInt64(objGen.OrderId);                
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
                                ClientEmail = c.Email,
                                ClientMobileNo = c.MobileNo,
                                OrderAmountDue = p.AmountDue.HasValue ? p.AmountDue.Value : 0,
                                ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                                ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2
                            }).OrderByDescending(x => x.OrderDate).FirstOrDefault();
                if (objOrder != null)
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

                objOrder.PaymentHistory = lstPaymentHist;                
                response.Data = objOrder;

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