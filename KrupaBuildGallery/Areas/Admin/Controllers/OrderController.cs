using ConstructionDiary.Models;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.ViewModel;

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
        public ActionResult Index()
        {
            
            List<OrderVM> lstOrders = new List<OrderVM>();
            try
            {

                lstOrders = (from p in _db.tbl_Orders
                             join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                               where !p.IsDelete
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
                                   OrderDate = p.CreatedDate                                   
                               }).OrderByDescending(x => x.OrderDate).ToList();

                if(lstOrders != null && lstOrders.Count() > 0)
                {
                    lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
                }
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
                             OrderDate = p.CreatedDate
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
                                ItemImg = c.MainImage                                                                                                
                            }).OrderByDescending(x => x.OrderItemId).ToList();
                objOrder.OrderItems = lstOrderItms;
            }
            return View(objOrder);
        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }
    }
}