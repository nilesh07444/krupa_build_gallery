using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class RoleModuleVM
    {
        public int AdminRoleModuleId { get; set; }
        public string ModuleName { get; set; }
        public int DisplayOrder { get; set; }
        public bool None { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Full { get; set; }
        public int SelectedValue { get; set; }

        public int AdminRolePermissionId { get; set; }
    }
}