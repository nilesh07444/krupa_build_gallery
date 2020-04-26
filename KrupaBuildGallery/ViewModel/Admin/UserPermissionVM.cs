using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class UserPermissionVM
    {
        public int Role { get; set; }
        public int Category { get; set; }
        public int Product { get; set; }
        public int SubProduct { get; set; }
        public int ProductItem { get; set; }
        public int Stock { get; set; }
        public int Order { get; set; }
        public int Offer { get; set; }
        public int Customers { get; set; }
        public int Distibutors { get; set; }
        public int DistibutorRequest { get; set; }
        public int ContactRequest { get; set; }
        public int Setting { get; set; }
        public int ManagePageContent { get; set; }
        public int ItemText { get; set; }
    }
}