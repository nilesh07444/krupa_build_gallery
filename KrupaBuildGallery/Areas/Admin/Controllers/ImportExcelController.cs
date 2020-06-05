using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ImportExcelController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public ImportExcelController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Category()
        {
            ViewBag.Message = "";
            if (TempData["Result"] != null)
            {
                ViewBag.Message = TempData["Result"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Category(ImportExcelDataVM excelVM, HttpPostedFileBase ExcelFile)
        {
            try
            {
                string filePath = string.Empty;
                OleDbConnection con = new OleDbConnection();
                OleDbDataAdapter da;
                DataSet ds = new DataSet();
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

                    string path = Server.MapPath("~/UploadsExcel/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string extension = Path.GetExtension(ExcelFile.FileName);
                    string TempFlName = Path.GetFileNameWithoutExtension(ExcelFile.FileName) + "_" + DateTime.Now.Ticks;
                    filePath = path + TempFlName + extension;
                    ExcelFile.SaveAs(filePath);
                    string conString = string.Empty;

                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES";
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + filePath + ";Extended Properties=Excel 12.0";
                            break;
                    }
                    //string strConnection = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + filePath + ";Extended Properties=Excel 12.0";
                    con = new OleDbConnection(conString);
                    con.Open();

                    DataTable dt = con.GetSchema("Tables");
                    string firstSheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                    da = new OleDbDataAdapter("select * from [" + firstSheetName + "]", con);
                    da.Fill(ds);
                    List<ImportExcelDataVM> lstImportExcl = new List<ImportExcelDataVM>();
                    try
                    {
                        if (ds.Tables[0].Columns.Contains("CategoryName"))
                        {
                            int cnt = 0;

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int srno = 0;
                                try
                                {
                                     srno = Convert.ToInt32(dr["SRNo"].ToString());
                                    string CategoryNm = dr["CategoryName"].ToString();
                                    string CategoryImageName = dr["CategoryImageName"].ToString();
                                    if (string.IsNullOrEmpty(CategoryNm))
                                    {
                                        ImportExcelDataVM objimport = new ImportExcelDataVM();
                                        objimport.SrNo = srno;
                                        objimport.ErrorMsg = "CatrgoryName is Blank";
                                        lstImportExcl.Add(objimport);
                                        continue;
                                    }
                                    else
                                    {
                                        var existCategory = _db.tbl_Categories.Where(x => x.CategoryName.ToLower() == CategoryNm.ToLower() && !x.IsDelete).FirstOrDefault();
                                        if(existCategory != null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "CatrgoryName already exist";
                                            lstImportExcl.Add(objimport);
                                            continue;
                                        }
                                        string fileName = string.Empty;
                                        
                                        tbl_Categories objCategory = new tbl_Categories();
                                        objCategory.CategoryName = CategoryNm;
                                        objCategory.CategoryImage = CategoryImageName;

                                        objCategory.IsActive = true;
                                        objCategory.IsDelete = false;
                                        objCategory.CreatedBy = clsAdminSession.UserID;
                                        objCategory.CreatedDate = DateTime.UtcNow;
                                        _db.tbl_Categories.Add(objCategory);
                                        _db.SaveChanges();
                                    }                                  
                                }
                                catch(Exception ex)
                                {
                                    ImportExcelDataVM objimport = new ImportExcelDataVM();
                                    objimport.SrNo = srno;
                                    objimport.ErrorMsg = ex.Message;
                                    lstImportExcl.Add(objimport);
                                }
                               
                            }
                            TempData["Result"] = "Categories Imported";
                        }                        
                        else
                        {
                            TempData["Result"] = "Please select Excel file with Data";
                        }
                    }
                    catch (Exception e)
                    {
                       
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
            ViewBag.Message = "";
            if (TempData["Result"] != null)
            {
                ViewBag.Message = TempData["Result"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Product(ImportExcelDataVM excelVM, HttpPostedFileBase ExcelFile)
        {
            try
            {
                string filePath = string.Empty;
                OleDbConnection con = new OleDbConnection();
                OleDbDataAdapter da;
                DataSet ds = new DataSet();
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

                    string path = Server.MapPath("~/UploadsExcel/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string extension = Path.GetExtension(ExcelFile.FileName);
                    string TempFlName = Path.GetFileNameWithoutExtension(ExcelFile.FileName) + "_" + DateTime.Now.Ticks;
                    filePath = path + TempFlName + extension;
                    ExcelFile.SaveAs(filePath);
                    string conString = string.Empty;

                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES";
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + filePath + ";Extended Properties=Excel 12.0";
                            break;
                    }
                    //string strConnection = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + filePath + ";Extended Properties=Excel 12.0";
                    con = new OleDbConnection(conString);
                    con.Open();

                    DataTable dt = con.GetSchema("Tables");
                    string firstSheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                    da = new OleDbDataAdapter("select * from [" + firstSheetName + "]", con);
                    da.Fill(ds);
                    List<ImportExcelDataVM> lstImportExcl = new List<ImportExcelDataVM>();
                    try
                    {
                        if (ds.Tables[0].Columns.Contains("ProductName") && ds.Tables[0].Columns.Contains("CategoryName"))
                        {
                            int cnt = 0;
                            List<tbl_Categories> lstCats = _db.tbl_Categories.Where(o => o.IsDelete == false).ToList();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int srno = 0;
                                try
                                {
                                    srno = Convert.ToInt32(dr["SRNo"].ToString());
                                    string CategoryNm = dr["CategoryName"].ToString();
                                    string ProductNm = dr["ProductName"].ToString();
                                    string ProductImageName = dr["ProductImageName"].ToString();
                                    if (string.IsNullOrEmpty(CategoryNm) || string.IsNullOrEmpty(ProductNm))
                                    {
                                        ImportExcelDataVM objimport = new ImportExcelDataVM();
                                        objimport.SrNo = srno;
                                        objimport.ErrorMsg = "CatrgoryName Or ProductName is Blank";
                                        lstImportExcl.Add(objimport);
                                        continue;
                                    }
                                    else
                                    {
                                        var existProduct = _db.tbl_Products.Where(x => x.ProductName.ToLower() == ProductNm.ToLower() && !x.IsDelete).FirstOrDefault();
                                        if (existProduct != null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "ProductName already exist";
                                            lstImportExcl.Add(objimport);
                                            continue;
                                        }

                                        var objCatExist = lstCats.Where(o => o.CategoryName.ToLower() == CategoryNm.ToLower()).FirstOrDefault();
                                        if(objCatExist == null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "Category Name Not exist";
                                            lstImportExcl.Add(objimport);
                                            continue;
                                        }
                                        string fileName = string.Empty;

                                        tbl_Products objProducts = new tbl_Products();
                                        objProducts.ProductName = ProductNm;
                                        objProducts.ProductImage = ProductImageName;
                                         
                                        objProducts.CategoryId = objCatExist.CategoryId;
                                        objProducts.IsActive = true;
                                        objProducts.IsDelete = false;
                                        objProducts.CreatedBy = clsAdminSession.UserID;
                                        objProducts.CreatedDate = DateTime.UtcNow;
                                        objProducts.UpdatedBy = clsAdminSession.UserID;
                                        objProducts.UpdatedDate = DateTime.UtcNow;
                                        _db.tbl_Products.Add(objProducts);
                                        _db.SaveChanges();                                       
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ImportExcelDataVM objimport = new ImportExcelDataVM();
                                    objimport.SrNo = srno;
                                    objimport.ErrorMsg = ex.Message;
                                    lstImportExcl.Add(objimport);
                                }

                            }
                            TempData["Result"] = "Products Imported";
                        }
                        else
                        {
                            TempData["Result"] = "Please select Excel file with Product Import Format Data";
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    return RedirectToAction("Product");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(excelVM);
        }
        public ActionResult SubProduct()
        {
            ViewBag.Message = "";
            if (TempData["Result"] != null)
            {
                ViewBag.Message = TempData["Result"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult SubProduct(ImportExcelDataVM excelVM, HttpPostedFileBase ExcelFile)
        {
            try
            {
                string filePath = string.Empty;
                OleDbConnection con = new OleDbConnection();
                OleDbDataAdapter da;
                DataSet ds = new DataSet();
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

                    string path = Server.MapPath("~/UploadsExcel/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string extension = Path.GetExtension(ExcelFile.FileName);
                    string TempFlName = Path.GetFileNameWithoutExtension(ExcelFile.FileName) + "_" + DateTime.Now.Ticks;
                    filePath = path + TempFlName + extension;
                    ExcelFile.SaveAs(filePath);
                    string conString = string.Empty;

                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES";
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + filePath + ";Extended Properties=Excel 12.0";
                            break;
                    }
                    //string strConnection = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + filePath + ";Extended Properties=Excel 12.0";
                    con = new OleDbConnection(conString);
                    con.Open();

                    DataTable dt = con.GetSchema("Tables");
                    string firstSheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                    da = new OleDbDataAdapter("select * from [" + firstSheetName + "]", con);
                    da.Fill(ds);
                    List<ImportExcelDataVM> lstImportExcl = new List<ImportExcelDataVM>();
                    try
                    {
                        if (ds.Tables[0].Columns.Contains("ProductName") && ds.Tables[0].Columns.Contains("CategoryName") && ds.Tables[0].Columns.Contains("SubProductName"))
                        {
                            int cnt = 0;
                            List<tbl_Categories> lstCats = _db.tbl_Categories.Where(o => o.IsDelete == false).ToList();
                            List<tbl_Products> lstProd = _db.tbl_Products.Where(o => o.IsDelete == false).ToList();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int srno = 0;
                                try
                                {
                                    srno = Convert.ToInt32(dr["SRNo"].ToString());
                                    string CategoryNm = dr["CategoryName"].ToString();
                                    string ProductNm = dr["ProductName"].ToString();
                                    string SubProductName = dr["SubProductName"].ToString();
                                    
                                    string SubProductImageName = dr["SubProductImageName"].ToString();
                                    if (string.IsNullOrEmpty(CategoryNm) || string.IsNullOrEmpty(ProductNm) || string.IsNullOrEmpty(SubProductName))
                                    {
                                        ImportExcelDataVM objimport = new ImportExcelDataVM();
                                        objimport.SrNo = srno;
                                        objimport.ErrorMsg = "CatrgoryName Or ProductName Or SubProductName is Blank";
                                        lstImportExcl.Add(objimport);
                                        continue;
                                    }
                                    else
                                    {
                                        var objCatExist = lstCats.Where(o => o.CategoryName.ToLower() == CategoryNm.ToLower()).FirstOrDefault();
                                        if (objCatExist == null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "Category Name Not exist";
                                            lstImportExcl.Add(objimport);
                                            continue;
                                        }
                                        long CatId = objCatExist.CategoryId;
                                        var objProduExist = lstProd.Where(o => o.ProductName.ToLower() == ProductNm.ToLower() && o.CategoryId == CatId).FirstOrDefault();
                                        if (objProduExist == null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "Product Name Not exist with this Category";
                                            lstImportExcl.Add(objimport);
                                            continue;
                                        }
                                        long ProdId = objProduExist.Product_Id;
                                        var existSubProduct = _db.tbl_SubProducts.Where(x => x.SubProductName.ToLower() == SubProductName.ToLower()
                        && x.CategoryId == CatId && x.ProductId == ProdId
                        && !x.IsDelete).FirstOrDefault();

                                        if (existSubProduct != null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "Sub ProductName already exist";
                                            lstImportExcl.Add(objimport);
                                            continue;
                                        }

                                      
                                        string fileName = string.Empty;

                                        tbl_SubProducts objSubCategory = new tbl_SubProducts();
                                        objSubCategory.CategoryId = CatId;
                                        objSubCategory.ProductId = ProdId;
                                        objSubCategory.SubProductName = SubProductName;
                                        objSubCategory.SubProductImage = SubProductImageName;

                                        objSubCategory.IsActive = true;
                                        objSubCategory.IsDelete = false;
                                        objSubCategory.CreatedBy = clsAdminSession.UserID;
                                        objSubCategory.CreatedDate = DateTime.UtcNow;
                                        objSubCategory.UpdatedBy = clsAdminSession.UserID;
                                        objSubCategory.UpdatedDate = DateTime.UtcNow;

                                        _db.tbl_SubProducts.Add(objSubCategory);
                                        _db.SaveChanges();
                                        
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ImportExcelDataVM objimport = new ImportExcelDataVM();
                                    objimport.SrNo = srno;
                                    objimport.ErrorMsg = ex.Message;
                                    lstImportExcl.Add(objimport);
                                }

                            }
                            TempData["Result"] = "SubProducts Imported";
                        }
                        else
                        {
                            TempData["Result"] = "Please select Excel file with SubProducts Import Format Data";
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    return RedirectToAction("SubProduct");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(excelVM);
        }
        public ActionResult ProductItem()
        {
            return View();
        }
    }
}