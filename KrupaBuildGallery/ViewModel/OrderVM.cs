using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.ViewModel
{
    public class OrderVM
    {    
        public long OrderId { get; set; }
        public string ClientUserName { get; set; }        
        public long ClientUserId { get; set; }
        public decimal OrderAmount { get; set; }
        public string OrderShipCity { get; set; }
        public string OrderShipState { get; set; }
        public string OrderShipAddress { get; set; }
        public string OrderPincode { get; set; }
        public string OrderShipClientName { get; set; }
        public string OrderShipClientPhone { get; set; }
        public long OrderStatusId { get; set; }
        public string PaymentType { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemsVM> OrderItems { get; set; }
        public string OrderStatus { get; set; }
        public decimal OrderAmountDue { get; set; }
        public string ClientAddress { get; set; }
        public string ClientMobileNo { get; set; }
        public string ClientEmail { get; set; }
        public string RazorpayOrderId { get; set; }
        public decimal ShipmentCharge { get; set; }
        public int ShippingStatus { get; set; }
        public long InvoiceNo { get; set; }
        public string InvoiceYear { get;set; }
        public string OrderDateString { get; set; }
        public List<PaymentHistoryVM> PaymentHistory { get; set; }
        public decimal WalletAmtUsed { get; set; }
        public decimal CreditUsed { get; set; }
        public decimal OnlineUsed { get; set; }
        public int OrderTypeId { get; set; }
        public bool IsCashOnDelivery { get; set; }
        public long ClientRoleId { get; set; }
        public decimal ExtraAmount { get; set; }
        public bool IsExtraAmountReceived { get; set; }
        public decimal AdvancePay { get; set; }
    }
}