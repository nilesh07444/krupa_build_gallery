using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class RoleVM
    {
        public int AdminRoleId { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string AdminRoleName { get; set; }
        [Display(Name = "Role Description")]
        public string AdminRoleDescription { get; set; }
        public bool IsActive { get; set; } 
        public List<RoleModuleVM> lstRoleModules { get; set; }
    }
}
