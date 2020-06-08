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
            List<tbl_ImportExcel> lstExcl = new List<tbl_ImportExcel>();
            lstExcl = _db.tbl_ImportExcel.Where(o => o.ImportType == 1).ToList();
            ViewData["lstExcl"] = lstExcl;
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
                            DeleteAllImportError(1);
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int srno = 0;
                                try
                                {
                                    srno = Convert.ToInt32(dr["SRNo"].ToString());
                                    string CategoryNm = dr["CategoryName"].ToString();
                                    string CategoryImageName = "DefaultImg.png";
                                    if (string.IsNullOrEmpty(CategoryNm))
                                    {
                                        ImportExcelDataVM objimport = new ImportExcelDataVM();
                                        objimport.SrNo = srno;
                                        objimport.ErrorMsg = "Category Name is Blank";
                                        lstImportExcl.Add(objimport);
                                        tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                        objtblex.SrNo = srno.ToString();
                                        objtblex.ErroMsg = objimport.ErrorMsg;
                                        objtblex.ImportedDate = DateTime.UtcNow;
                                        objtblex.ImportedBy = clsAdminSession.UserID;
                                        objtblex.ImportType = 1;
                                        _db.tbl_ImportExcel.Add(objtblex);
                                        _db.SaveChanges();
                                        continue;
                                    }
                                    else
                                    {
                                        var existCategory = _db.tbl_Categories.Where(x => x.CategoryName.ToLower() == CategoryNm.ToLower() && !x.IsDelete).FirstOrDefault();
                                        if (existCategory != null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "Category Name is already exist";
                                            lstImportExcl.Add(objimport);
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            objtblex.ImportType = 1;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
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
                                        objCategory.IsImported = true;
                                        _db.tbl_Categories.Add(objCategory);
                                        _db.SaveChanges();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ImportExcelDataVM objimport = new ImportExcelDataVM();
                                    objimport.SrNo = srno;
                                    objimport.ErrorMsg = ex.Message;
                                    lstImportExcl.Add(objimport);
                                    tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                    objtblex.SrNo = srno.ToString();
                                    objtblex.ErroMsg = objimport.ErrorMsg;
                                    objtblex.ImportedDate = DateTime.UtcNow;
                                    objtblex.ImportedBy = clsAdminSession.UserID;
                                    _db.tbl_ImportExcel.Add(objtblex);
                                    _db.SaveChanges();
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
                throw ex;
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
            List<tbl_ImportExcel> lstExcl = new List<tbl_ImportExcel>();
            lstExcl = _db.tbl_ImportExcel.Where(o => o.ImportType == 2).ToList();
            ViewData["lstExcl"] = lstExcl;
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
                            DeleteAllImportError(2);
                            List<tbl_Categories> lstCats = _db.tbl_Categories.Where(o => o.IsDelete == false).ToList();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int srno = 0;
                                try
                                {
                                    srno = Convert.ToInt32(dr["SRNo"].ToString());
                                    string CategoryNm = dr["CategoryName"].ToString();
                                    string ProductNm = dr["ProductName"].ToString();
                                    string ProductImageName = "DefaultImg.png";
                                    if (string.IsNullOrEmpty(CategoryNm) || string.IsNullOrEmpty(ProductNm))
                                    {
                                        ImportExcelDataVM objimport = new ImportExcelDataVM();
                                        objimport.SrNo = srno;
                                        objimport.ErrorMsg = "Category Name Or Product Name is Blank";
                                        lstImportExcl.Add(objimport);
                                        tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                        objtblex.SrNo = srno.ToString();
                                        objtblex.ErroMsg = objimport.ErrorMsg;
                                        objtblex.ImportedDate = DateTime.UtcNow;
                                        objtblex.ImportedBy = clsAdminSession.UserID;
                                        objtblex.ImportType = 2;
                                        _db.tbl_ImportExcel.Add(objtblex);
                                        _db.SaveChanges();
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
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            objtblex.ImportType = 2;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
                                            continue;
                                        }

                                        var objCatExist = lstCats.Where(o => o.CategoryName.ToLower() == CategoryNm.ToLower()).FirstOrDefault();
                                        if (objCatExist == null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "Category Name Not exist";
                                            lstImportExcl.Add(objimport);
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            objtblex.ImportType = 2;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
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
                                        objProducts.IsImported = true;
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
                                    tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                    objtblex.SrNo = srno.ToString();
                                    objtblex.ErroMsg = objimport.ErrorMsg;
                                    objtblex.ImportedDate = DateTime.UtcNow;
                                    objtblex.ImportType = 2;
                                    objtblex.ImportedBy = clsAdminSession.UserID;
                                    _db.tbl_ImportExcel.Add(objtblex);
                                    _db.SaveChanges();
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
            List<tbl_ImportExcel> lstExcl = new List<tbl_ImportExcel>();
            lstExcl = _db.tbl_ImportExcel.Where(o => o.ImportType == 3).ToList();
            ViewData["lstExcl"] = lstExcl;
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
                            DeleteAllImportError(3);
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

                                    string SubProductImageName = "DefaultImg.png";
                                    if (string.IsNullOrEmpty(CategoryNm) || string.IsNullOrEmpty(ProductNm) || string.IsNullOrEmpty(SubProductName))
                                    {
                                        ImportExcelDataVM objimport = new ImportExcelDataVM();
                                        objimport.SrNo = srno;
                                        objimport.ErrorMsg = "Category Name Or Product Name Or Sub ProductName is Blank";
                                        lstImportExcl.Add(objimport);
                                        tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                        objtblex.SrNo = srno.ToString();
                                        objtblex.ErroMsg = objimport.ErrorMsg;
                                        objtblex.ImportedDate = DateTime.UtcNow;
                                        objtblex.ImportType = 3;
                                        objtblex.ImportedBy = clsAdminSession.UserID;
                                        _db.tbl_ImportExcel.Add(objtblex);
                                        _db.SaveChanges();
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
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportType = 3;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
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
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportType = 3;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
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
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportType = 3;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
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
                                        objSubCategory.IsImported = true;

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
                                    tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                    objtblex.SrNo = srno.ToString();
                                    objtblex.ErroMsg = objimport.ErrorMsg;
                                    objtblex.ImportType = 3;
                                    objtblex.ImportedDate = DateTime.UtcNow;
                                    objtblex.ImportedBy = clsAdminSession.UserID;
                                    _db.tbl_ImportExcel.Add(objtblex);
                                    _db.SaveChanges();
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
            ViewBag.Message = "";
            if (TempData["Result"] != null)
            {
                ViewBag.Message = TempData["Result"];
            }
            List<tbl_ImportExcel> lstExcl = new List<tbl_ImportExcel>();
            lstExcl = _db.tbl_ImportExcel.Where(o => o.ImportType == 4).ToList();
            ViewData["lstExcl"] = lstExcl;
            return View();
        }

        [HttpPost]
        public ActionResult ProductItem(ImportExcelDataVM excelVM, HttpPostedFileBase ExcelFile)
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
                        if (ds.Tables[0].Columns.Contains("ProductName") && ds.Tables[0].Columns.Contains("CategoryName") && ds.Tables[0].Columns.Contains("ItemName"))
                        {
                            int cnt = 0;
                            DeleteAllImportError(4);
                            List<tbl_Categories> lstCats = _db.tbl_Categories.Where(o => o.IsDelete == false).ToList();
                            List<tbl_Products> lstProd = _db.tbl_Products.Where(o => o.IsDelete == false).ToList();
                            List<tbl_SubProducts> lstSubProd = _db.tbl_SubProducts.Where(o => o.IsDelete == false).ToList();
                            List<tbl_Units> lstUnitss = _db.tbl_Units.ToList();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int srno = 0;
                                try
                                {
                                    srno = Convert.ToInt32(dr["SRNo"].ToString());
                                    string CategoryNm = dr["CategoryName"].ToString();
                                    string ProductNm = dr["ProductName"].ToString();
                                    string SubProductName = dr["SubProductName"].ToString();
                                    string ItemName = dr["ItemName"].ToString();
                                    string Description = dr["ItemDescription"].ToString();
                                    string MRPPrice = dr["MRPPrice"].ToString();
                                    string DistributorPrice = dr["DistributorPrice"].ToString();
                                    string CustomerPrice = dr["CustomerPrice"].ToString();
                                    string GSTPercentage = dr["GSTPercentage"].ToString();
                                    string MainImageName = "DefaultImg.png";
                                    string NotificationText = dr["NotificationText"].ToString();
                                    string Sku = dr["Sku"].ToString();
                                    string HSNCode = dr["HSNCode"].ToString();
                                    string IsPopularItem = dr["IsPopularItem"].ToString();
                                    string ShippingCharge = dr["ShippingCharge"].ToString();
                                    string GodownName = dr["GodownName"].ToString();
                                    string PackedUnPacked = dr["PackedUnPacked"].ToString();
                                    string IsReturnable = dr["IsReturnable"].ToString();
                                    string AdvancePaymentPercentage = dr["AdvancePaymentPercentage"].ToString();
                                    string CashOnDelivery = dr["CashOnDelivery"].ToString();
                                    string MinimumStock = dr["MinimumStock"].ToString();
                                    string UnitType = dr["UnitType"].ToString();
                                    string IntialStock = dr["IntialStock"].ToString();
                                    string G50ML50S8x4Pecentage = dr["G50ML50S8x4Pecentage"].ToString();
                                    string G100ML100S7x4Pecentage = dr["G100ML100S7x4Pecentage"].ToString();
                                    string G250ML250S7x3Pecentage = dr["G250ML250S7x3Pecentage"].ToString();
                                    string G500ML500S6x4Pecentage = dr["G500ML500S6x4Pecentage"].ToString();
                                    string KG1L1S6x3Pecentage = dr["KG1L1S6x3Pecentage"].ToString();
                                    string KG2L2Pecentage = dr["KG2L2Pecentage"].ToString();
                                    string KG5L5Pecentage = dr["KG5L5Pecentage"].ToString();
                                    string ItemTag = dr["ItemTag"].ToString();

                                    if (string.IsNullOrEmpty(CategoryNm) || string.IsNullOrEmpty(ProductNm) || string.IsNullOrEmpty(ItemName))
                                    {
                                        ImportExcelDataVM objimport = new ImportExcelDataVM();
                                        objimport.SrNo = srno;
                                        objimport.ErrorMsg = "Category Name Or Product Name Or Item Name is Blank";
                                        lstImportExcl.Add(objimport);
                                        tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                        objtblex.SrNo = srno.ToString();
                                        objtblex.ErroMsg = objimport.ErrorMsg;
                                        objtblex.ImportedDate = DateTime.UtcNow;
                                        objtblex.ImportedBy = clsAdminSession.UserID;
                                        objtblex.ImportType = 4;
                                        _db.tbl_ImportExcel.Add(objtblex);
                                        _db.SaveChanges();
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
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            objtblex.ImportType = 4;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
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
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            objtblex.ImportType = 4;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
                                            continue;
                                        }
                                        long ProdId = objProduExist.Product_Id;
                                        var existProductItem = _db.tbl_ProductItems.Where(x => x.ItemName.ToLower() == ItemName.ToLower()
                        && x.CategoryId == CatId && x.ProductId == ProdId
                        && !x.IsDelete).FirstOrDefault();

                                        if (existProductItem != null)
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "Item already exist";
                                            lstImportExcl.Add(objimport);
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            objtblex.ImportType = 4;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
                                            continue;
                                        }

                                        long SubProductId = 0;
                                        if (!string.IsNullOrEmpty(SubProductName))
                                        {
                                            var existSubProduct = _db.tbl_SubProducts.Where(x => x.SubProductName.ToLower() == SubProductName.ToLower()
               && x.CategoryId == CatId && x.ProductId == ProdId
               && !x.IsDelete).FirstOrDefault();

                                            if (existSubProduct == null)
                                            {
                                                ImportExcelDataVM objimport = new ImportExcelDataVM();
                                                objimport.SrNo = srno;
                                                objimport.ErrorMsg = "Sub Product Name Not exist with this Category and Product";
                                                lstImportExcl.Add(objimport);
                                                tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                                objtblex.SrNo = srno.ToString();
                                                objtblex.ErroMsg = objimport.ErrorMsg;
                                                objtblex.ImportedDate = DateTime.UtcNow;
                                                objtblex.ImportType = 4;
                                                objtblex.ImportedBy = clsAdminSession.UserID;
                                                _db.tbl_ImportExcel.Add(objtblex);
                                                _db.SaveChanges();
                                                continue;
                                            }
                                            SubProductId = existSubProduct.SubProductId;
                                        }

                                        var objItmTyp = lstUnitss.Where(o => o.UnitName.ToLower() == UnitType.ToLower()).FirstOrDefault();
                                        if (objItmTyp == null)
                                        {
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = "Unit Type Not Exists";
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportType = 4;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
                                            continue;
                                        }

                                        var objGown = _db.tbl_Godown.Where(o => o.GodownName.ToLower() == GodownName && o.IsDeleted == false).FirstOrDefault();
                                        if (objGown == null)
                                        {
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = "Godown is Not Exist";
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportType = 4;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
                                            continue;
                                        }
                                        long GodownId = objGown.GodownId;
                                        string fileName = string.Empty;

                                        tbl_ProductItems objProductItem = new tbl_ProductItems();
                                        objProductItem.CategoryId = CatId;
                                        objProductItem.ProductId = ProdId;
                                        objProductItem.SubProductId = SubProductId;
                                        objProductItem.ItemName = ItemName;
                                        objProductItem.ItemDescription = Description;
                                        objProductItem.Sku = Sku;
                                        if (string.IsNullOrEmpty(MRPPrice) || string.IsNullOrEmpty(CustomerPrice) || string.IsNullOrEmpty(DistributorPrice))
                                        {
                                            ImportExcelDataVM objimport = new ImportExcelDataVM();
                                            objimport.SrNo = srno;
                                            objimport.ErrorMsg = "MRP Price or Customer Price or Distributor Value is Blank";
                                            lstImportExcl.Add(objimport);
                                            tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                            objtblex.SrNo = srno.ToString();
                                            objtblex.ErroMsg = objimport.ErrorMsg;
                                            objtblex.ImportType = 4;
                                            objtblex.ImportedDate = DateTime.UtcNow;
                                            objtblex.ImportedBy = clsAdminSession.UserID;
                                            _db.tbl_ImportExcel.Add(objtblex);
                                            _db.SaveChanges();
                                            continue;
                                        }
                                        else
                                        {
                                            if (!IsValidPriceFormat(MRPPrice))
                                            {
                                                tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                                objtblex.SrNo = srno.ToString();
                                                objtblex.ErroMsg = "MRP price is not in correct format";
                                                objtblex.ImportedDate = DateTime.UtcNow;
                                                objtblex.ImportType = 4;
                                                objtblex.ImportedBy = clsAdminSession.UserID;
                                                _db.tbl_ImportExcel.Add(objtblex);
                                                _db.SaveChanges();
                                                continue;
                                            }

                                            if (!IsValidPriceFormat(DistributorPrice))
                                            {
                                                tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                                objtblex.SrNo = srno.ToString();
                                                objtblex.ErroMsg = "DistributorPrice is not in correct format";
                                                objtblex.ImportedDate = DateTime.UtcNow;
                                                objtblex.ImportType = 4;
                                                objtblex.ImportedBy = clsAdminSession.UserID;
                                                _db.tbl_ImportExcel.Add(objtblex);
                                                _db.SaveChanges();
                                                continue;
                                            }

                                            if (!IsValidPriceFormat(CustomerPrice))
                                            {
                                                tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                                objtblex.SrNo = srno.ToString();
                                                objtblex.ErroMsg = "CustomerPrice is not in correct format";
                                                objtblex.ImportedDate = DateTime.UtcNow;
                                                objtblex.ImportType = 4;
                                                objtblex.ImportedBy = clsAdminSession.UserID;
                                                _db.tbl_ImportExcel.Add(objtblex);
                                                _db.SaveChanges();
                                                continue;
                                            }
                                        }
                                        //bool result = int.TryParse(s, out i); //i now = 108  
                                        objProductItem.MRPPrice = Convert.ToDecimal(MRPPrice);
                                        objProductItem.CustomerPrice = Convert.ToDecimal(CustomerPrice);
                                        objProductItem.DistributorPrice = Convert.ToDecimal(DistributorPrice);
                                        decimal GSTPer = 0;
                                        if (!string.IsNullOrEmpty(GSTPercentage))
                                        {
                                            if (GSTPercentage == "0" || GSTPercentage == "5" || GSTPercentage == "12" || GSTPercentage == "18" || GSTPercentage == "28")
                                            {
                                                GSTPer = Convert.ToDecimal(GSTPercentage);
                                            }
                                            else
                                            {
                                                tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                                objtblex.SrNo = srno.ToString();
                                                objtblex.ErroMsg = "GST Percentage Not Valid";
                                                objtblex.ImportedDate = DateTime.UtcNow;
                                                objtblex.ImportType = 4;
                                                objtblex.ImportedBy = clsAdminSession.UserID;
                                                _db.tbl_ImportExcel.Add(objtblex);
                                                _db.SaveChanges();
                                                continue;
                                            }
                                        }
                                        objProductItem.GST_Per = GSTPer;
                                        objProductItem.IGST_Per = Convert.ToDecimal(GSTPercentage);
                                        objProductItem.Notification = NotificationText;
                                        objProductItem.MainImage = MainImageName;
                                        bool IsPopulrItm = false;
                                        if (!string.IsNullOrEmpty(IsPopularItem))
                                        {
                                            if (IsPopularItem.ToLower().Trim() == "yes")
                                            {
                                                IsPopulrItm = true;
                                            }
                                        }
                                        bool IsRetunble = false;
                                        if (!string.IsNullOrEmpty(IsReturnable))
                                        {
                                            if (IsReturnable.ToLower().Trim() == "yes")
                                            {
                                                IsRetunble = true;
                                            }
                                        }
                                        bool IsCashonDelv = false;
                                        if (!string.IsNullOrEmpty(CashOnDelivery))
                                        {
                                            if (CashOnDelivery.ToLower().Trim() == "yes")
                                            {
                                                IsCashonDelv = true;
                                            }
                                        }
                                        decimal advncepay = 0;
                                        if (!string.IsNullOrEmpty(AdvancePaymentPercentage))
                                        {
                                            advncepay = Convert.ToDecimal(AdvancePaymentPercentage);
                                        }
                                        objProductItem.IsPopularProduct = IsPopulrItm;
                                        decimal ShipingChrg = 0;
                                        if (!string.IsNullOrEmpty(ShippingCharge))
                                        {
                                            ShipingChrg = Convert.ToDecimal(ShippingCharge);
                                        }

                                        int itmtyp = 1;
                                        if (!string.IsNullOrEmpty(PackedUnPacked))
                                        {
                                            if (PackedUnPacked.ToLower().Trim() == "packed")
                                            {
                                                itmtyp = 1;
                                            }
                                            else
                                            {
                                                itmtyp = 2;
                                            }
                                        }
                                        int MiniStk = 0;
                                        if (!string.IsNullOrEmpty(MinimumStock))
                                        {
                                            MiniStk = Convert.ToInt32(MinimumStock);
                                        }

                                        objProductItem.ShippingCharge = ShipingChrg;
                                        objProductItem.IsActive = true;
                                        objProductItem.IsDelete = false;
                                        objProductItem.CreatedBy = clsAdminSession.UserID;
                                        objProductItem.CreatedDate = DateTime.UtcNow;
                                        objProductItem.UpdatedBy = clsAdminSession.UserID;
                                        objProductItem.HSNCode = HSNCode;
                                        objProductItem.UpdatedDate = DateTime.UtcNow;
                                        objProductItem.GodownId = GodownId;
                                        objProductItem.IsReturnable = IsRetunble;
                                        objProductItem.PayAdvancePer = advncepay;
                                        objProductItem.ItemType = itmtyp;
                                        objProductItem.IsCashonDeliveryUse = IsCashonDelv;
                                        objProductItem.MinimumStock = MiniStk;
                                        objProductItem.Tags = ItemTag;
                                        objProductItem.UnitType = Convert.ToInt32(objItmTyp.UnitId);
                                        objProductItem.IsImported = true;
                                        _db.tbl_ProductItems.Add(objProductItem);
                                        _db.SaveChanges();

                                        string[] kgs = { "50 Grams", "100 Grams", "250 Grams", "500 Grams", "1 Kg", "2 Kg", "5 Kg" };
                                        string[] kgsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };
                                        string[] ltrs = { "50 ml", "100 ml", "250 ml", "500 ml", "1 Ltr", "2 Ltr", "5 Ltr" };
                                        string[] ltrsQty = { "0.05", "0.10", "0.25", "0.50", "1", "2", "5" };

                                        string[] sheets = { "8x4", "7x4", "7x3", "6x4", "6x3" };
                                        string[] sheetsqty = { "32", "28", "21", "24", "18" };
                                        var objUnt = _db.tbl_Units.Where(o => o.UnitId == objProductItem.UnitType).FirstOrDefault();
                                        List<string> lstPerVarint = new List<string>();
                                        lstPerVarint.Add(G50ML50S8x4Pecentage);
                                        lstPerVarint.Add(G100ML100S7x4Pecentage);
                                        lstPerVarint.Add(G250ML250S7x3Pecentage);
                                        lstPerVarint.Add(G500ML500S6x4Pecentage);
                                        lstPerVarint.Add(KG1L1S6x3Pecentage);
                                        lstPerVarint.Add(KG2L2Pecentage);
                                        lstPerVarint.Add(KG5L5Pecentage);
                                        if (objUnt != null)
                                        {
                                            if (objUnt.UnitName.ToLower().Contains("killo") || objUnt.UnitName.ToLower().Contains("litr"))
                                            {
                                                for (int kk = 1; kk <= kgs.Length; kk++)
                                                {
                                                    tbl_ItemVariant objtbl_ItemVariant = new tbl_ItemVariant();
                                                    objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                                    objtbl_ItemVariant.IsActive = true;

                                                    if (lstPerVarint[kk - 1] != null)
                                                    {
                                                        decimal perc = 0;
                                                        if (!string.IsNullOrEmpty(lstPerVarint[kk - 1]))
                                                        {
                                                            perc = Convert.ToDecimal(lstPerVarint[kk - 1].ToString());
                                                        }
                                                        objtbl_ItemVariant.IsActive = false;
                                                        if (perc > 0)
                                                        {
                                                            objtbl_ItemVariant.IsActive = true;
                                                        }
                                                        int k = kk - 1;
                                                        objtbl_ItemVariant.PricePecentage = perc;
                                                        if (objUnt.UnitName.ToLower().Contains("killo"))
                                                        {
                                                            objtbl_ItemVariant.UnitQty = kgs[k];
                                                            decimal qtt = Convert.ToDecimal(kgsQty[k].ToString());
                                                            if (qtt >= 1)
                                                            {
                                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * perc) / 100, 2);
                                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * perc) / 100, 2);
                                                            }
                                                            else
                                                            {
                                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * perc) / 100, 2);
                                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * perc) / 100, 2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            objtbl_ItemVariant.UnitQty = ltrs[k];
                                                            decimal qtt = Convert.ToDecimal(ltrsQty[k].ToString());
                                                            if (qtt >= 1)
                                                            {
                                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * qtt * perc) / 100, 2);
                                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * qtt * perc) / 100, 2);
                                                            }
                                                            else
                                                            {
                                                                objtbl_ItemVariant.CustomerPrice = Math.Round((objProductItem.CustomerPrice * perc) / 100, 2);
                                                                objtbl_ItemVariant.DistributorPrice = Math.Round((objProductItem.DistributorPrice * perc) / 100, 2);
                                                            }
                                                        }
                                                    }
                                                    objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                                    _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                                }
                                                _db.SaveChanges();
                                            }
                                            else if (objUnt.UnitName.ToLower().Contains("sheet"))
                                            {
                                                for (int kk = 1; kk <= sheets.Length; kk++)
                                                {
                                                    int k = kk - 1;
                                                    tbl_ItemVariant objtbl_ItemVariant = new tbl_ItemVariant();
                                                    objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                                    objtbl_ItemVariant.IsActive = true;

                                                    decimal sqft = Convert.ToDecimal(sheetsqty[k]);
                                                    objtbl_ItemVariant.UnitQty = sheets[k];
                                                    objtbl_ItemVariant.CustomerPrice = Math.Round(sqft * objProductItem.CustomerPrice, 2);
                                                    objtbl_ItemVariant.DistributorPrice = Math.Round(sqft * objProductItem.DistributorPrice, 2);
                                                    objtbl_ItemVariant.PricePecentage = 100;
                                                    objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                                    _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                                }
                                                _db.SaveChanges();
                                            }
                                            else
                                            {
                                                tbl_ItemVariant objtbl_ItemVariant = new tbl_ItemVariant();
                                                objtbl_ItemVariant.ProductItemId = objProductItem.ProductItemId;
                                                objtbl_ItemVariant.IsActive = true;
                                                objtbl_ItemVariant.UnitQty = objUnt.UnitName;
                                                objtbl_ItemVariant.CustomerPrice = Math.Round(objProductItem.CustomerPrice, 2);
                                                objtbl_ItemVariant.DistributorPrice = Math.Round(objProductItem.DistributorPrice, 2);
                                                objtbl_ItemVariant.PricePecentage = 100;
                                                objtbl_ItemVariant.CreatedDate = DateTime.UtcNow;
                                                _db.tbl_ItemVariant.Add(objtbl_ItemVariant);
                                                _db.SaveChanges();
                                            }
                                        }

                                        tbl_ItemStocks objItemStock = new tbl_ItemStocks();
                                        objItemStock.CategoryId = objProductItem.CategoryId;
                                        objItemStock.ProductId = objProductItem.ProductId;
                                        objItemStock.SubProductId = objProductItem.SubProductId;
                                        objItemStock.ProductItemId = objProductItem.ProductItemId;
                                        objItemStock.Qty = Convert.ToInt64(IntialStock);

                                        objItemStock.IsActive = true;
                                        objItemStock.IsDelete = false;
                                        objItemStock.CreatedBy = clsAdminSession.UserID;
                                        objItemStock.CreatedDate = DateTime.UtcNow;
                                        objItemStock.UpdatedBy = clsAdminSession.UserID;
                                        objItemStock.UpdatedDate = DateTime.UtcNow;
                                        _db.tbl_ItemStocks.Add(objItemStock);
                                        _db.SaveChanges();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ImportExcelDataVM objimport = new ImportExcelDataVM();
                                    objimport.SrNo = srno;
                                    objimport.ErrorMsg = ex.Message;
                                    lstImportExcl.Add(objimport);
                                    tbl_ImportExcel objtblex = new tbl_ImportExcel();
                                    objtblex.SrNo = srno.ToString();
                                    objtblex.ErroMsg = objimport.ErrorMsg;
                                    objtblex.ImportType = 4;
                                    objtblex.ImportedDate = DateTime.UtcNow;
                                    objtblex.ImportedBy = clsAdminSession.UserID;
                                    _db.tbl_ImportExcel.Add(objtblex);
                                    _db.SaveChanges();
                                }

                            }
                            TempData["Result"] = "Items Imported";
                        }
                        else
                        {
                            TempData["Result"] = "Please select Excel file with SubProducts Import Format Data";
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    return RedirectToAction("ProductItem");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return View(excelVM);
        }
        public void DeleteAllImportError(int type)
        {
            try
            {
                List<tbl_ImportExcel> lstErrors = _db.tbl_ImportExcel.Where(o => o.ImportType == type).ToList();
                if (lstErrors != null && lstErrors.Count() > 0)
                {
                    foreach (var objErr in lstErrors)
                    {
                        _db.tbl_ImportExcel.Remove(objErr);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {

            }

        }

        public bool IsValidPriceFormat(string prc)
        {
            decimal amts = 0;
            bool result = decimal.TryParse(prc, out amts);

            decimal amts2 = 0;
            bool result2 = decimal.TryParse(prc, out amts2);
            if (result == true || result2 == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public ActionResult DisplayImportExcelErrorLog()
        //{
        //    return 
        //}
    }
}