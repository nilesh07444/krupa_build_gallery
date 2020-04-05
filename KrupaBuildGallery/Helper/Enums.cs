using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.Helper
{
    public enum ClientRoles
    {
        Customer = 1,
        Distributor = 2
    }

    public enum RoleModules
    {
        Role = 1,
        Category = 2,
        Product = 3,
        SubProduct = 4,
        ProductItem = 5,
        Stock = 6,
        Order = 7,
        Offer = 8,
        Customers = 9,
        Distibutors = 10,
        DistibutorRequest = 11,
        ContactRequest = 12,
        Setting = 13,
        ManagePageContent = 14,
    }
}