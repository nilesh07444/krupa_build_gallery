using KrupaBuildGallery.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class ImportExcelController : Controller
    {
        public ActionResult Category()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Category(ImportExcelDataVM excelVM, HttpPostedFileBase ExcelFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    // Check file is selected or not
                    if (ExcelFile == null)
                    {
                        ModelState.AddModelError("ExcelFile", ErrorMessage.SelectExcelFile);
                        return View(excelVM);
                    }

                    // Image file validation
                    string ext = Path.GetExtension(ExcelFile.FileName);
                    if (ext.ToUpper().Trim() != ".XLS" && ext.ToUpper() != ".XLSX")
                    {
                        ModelState.AddModelError("ExcelFile", ErrorMessage.SelectExcelFile);
                        return View(excelVM);
                    }

                    return RedirectToAction("Category");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(excelVM);
        }
        public ActionResult Product()
        {
            return View();
        }
        public ActionResult SubProduct()
        {
            return View();
        }
        public ActionResult ProductItem()
        {
            return View();
        }
    }
}