using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class ImportExcelDataVM
    {
        [Display(Name = "Select Excel")]
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}