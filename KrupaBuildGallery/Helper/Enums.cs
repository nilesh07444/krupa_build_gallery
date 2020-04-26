﻿using System;
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
        ItemText = 15
    }

    public enum Modules
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
        ItemText = 15
    }

    public enum ModulePermission
    {
        None = 0,
        View = 1,
        Add = 2,
        Edit = 3,
        Full = 4
    }

}