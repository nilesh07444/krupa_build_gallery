﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KrupaBuildGallery.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class krupagallarydbEntities : DbContext
    {
        public krupagallarydbEntities()
            : base("name=krupagallarydbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tbl_AdminRolePermissions> tbl_AdminRolePermissions { get; set; }
        public DbSet<tbl_Cart> tbl_Cart { get; set; }
        public DbSet<tbl_Categories> tbl_Categories { get; set; }
        public DbSet<tbl_ClientOtherDetails> tbl_ClientOtherDetails { get; set; }
        public DbSet<tbl_ClientRoles> tbl_ClientRoles { get; set; }
        public DbSet<tbl_ClientUsers> tbl_ClientUsers { get; set; }
        public DbSet<tbl_ContactFormData> tbl_ContactFormData { get; set; }
        public DbSet<tbl_DistributorRequestDetails> tbl_DistributorRequestDetails { get; set; }
        public DbSet<tbl_Godown> tbl_Godown { get; set; }
        public DbSet<tbl_GSTMaster> tbl_GSTMaster { get; set; }
        public DbSet<tbl_HomeImages> tbl_HomeImages { get; set; }
        public DbSet<tbl_ItemStocks> tbl_ItemStocks { get; set; }
        public DbSet<tbl_Itemtext_master> tbl_Itemtext_master { get; set; }
        public DbSet<tbl_LoginHistory> tbl_LoginHistory { get; set; }
        public DbSet<tbl_Offers> tbl_Offers { get; set; }
        public DbSet<tbl_OrderItemDetails> tbl_OrderItemDetails { get; set; }
        public DbSet<tbl_Orders> tbl_Orders { get; set; }
        public DbSet<tbl_PaymentHistory> tbl_PaymentHistory { get; set; }
        public DbSet<tbl_ProductItemImages> tbl_ProductItemImages { get; set; }
        public DbSet<tbl_ProductItems> tbl_ProductItems { get; set; }
        public DbSet<tbl_Products> tbl_Products { get; set; }
        public DbSet<tbl_SubProducts> tbl_SubProducts { get; set; }
        public DbSet<tbl_WishList> tbl_WishList { get; set; }
        public DbSet<tbl_AdminRoles> tbl_AdminRoles { get; set; }
        public DbSet<tbl_AdminRoleModules> tbl_AdminRoleModules { get; set; }
        public DbSet<tbl_AdminUsers> tbl_AdminUsers { get; set; }
        public DbSet<tbl_PointDetails> tbl_PointDetails { get; set; }
        public DbSet<tbl_GeneralSetting> tbl_GeneralSetting { get; set; }
    }
}
