using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery
{
    public class HappyCustomerVM
    {
        public int HappyCustomerId { get; set; }
       
        [Required, Display(Name = "Finance Year")] 
        public string FinanceYear { get; set; }
         
        [Required, Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        
        [Display(Name = "Customer Image")]
        public HttpPostedFileBase CustomerImageFile { get; set; }
        public bool IsActive { get; set; }
        
        // Additional fields
        public string CustomerImage { get; set; }
        public List<SelectListItem> FinanceYearList { get; set; }
    }
}