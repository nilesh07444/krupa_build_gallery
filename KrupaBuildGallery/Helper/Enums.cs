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

    public enum AdminRoles
    {
        SuperAdmin = 1,
        Agent = 2,
        DeliveryUser = 3,
        ChannelPartner = 4
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
        ItemText = 15,
        BidItem = 16
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

    public enum ItemTypes
    {
        PackedItem = 1,
        UnPackedItem = 2
    }

    public enum HomeImageFor
    {
        Website = 1,
        MobileApp = 2
    }

    public enum DynamicContents
    {
        FAQ = 1,
        PrivacyPolicy = 2,
        TermsCondition = 3,
        ReturnPolicy = 4
    }

    public enum RolePermissionEnum
    {
        DoNotCheck = -1,
        None = 0,
        View = 1,
        Add = 2,
        Edit = 3,
        Full = 4
    }

}