//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tbl_AdminRolePermissions
    {
        public int AdminRolePermissionId { get; set; }
        public int AdminRoleId { get; set; }
        public int AdminRoleModuleId { get; set; }
        public int Permission { get; set; }
    
        public virtual tbl_AdminRoles tbl_AdminRoles { get; set; }
        public virtual tbl_AdminRoleModules tbl_AdminRoleModules { get; set; }
    }
}