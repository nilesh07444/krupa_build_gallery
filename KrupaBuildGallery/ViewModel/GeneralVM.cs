using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class GeneralVM
    {
        public string ClientUserId { get; set; }
        public string ItemId { get; set; }
        public string WishListId { get; set; }
        public string RoleId { get; set; }
        public string CategoryId { get; set; }
        public string SortBy { get; set; }
        public string ProductId { get; set; }
        public string SubProductId { get; set; }
        public string SessionUniqueId { get; set; }
        public string StrCartItems { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string RazorPaymentId { get; set; }
        public string RazorOrderid { get; set; }
        public string CheckoutType { get; set; }
        public string OrderDetailId { get; set; }
        public string StatusId { get; set; }
        public string Reason { get; set; }
        public long AgentId { get; set; }
        public long DelieveryPersonId { get; set; }

        public string TotalPendingDeliveryItems { get; set; }
        public string TotalDeliveredItem { get; set; }
        public decimal AmountDecmal { get; set; }
        public string Id { get; set; }
        public string Ratings { get; set; }
        public string Reviews { get; set; }

        public string TotalWalletAmt { get; set; }
        public string TotalPoints { get; set; }

    }
}