using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConstructionDiary.Models;
using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class HappyCustomerController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public string HappyCustomerDirectoryPath = "";
        public HappyCustomerController()
        {
            _db = new krupagallarydbEntities();
            HappyCustomerDirectoryPath = ErrorMessage.HappyCustomerDirectoryPath;
        }

        [AdminPermission(RolePermissionEnum.View)]
        public ActionResult Index()
        {
            List<HappyCustomerVM> lstHappyCustomers = new List<HappyCustomerVM>();
            try
            {

                lstHappyCustomers = (from c in _db.tbl_HappyCustomers
                                 select new HappyCustomerVM
                                 {
                                     HappyCustomerId = c.HappyCustomerId,
                                     FinanceYear = c.FinanceYear,
                                     CustomerName = c.CustomerName,
                                     CustomerImage = c.CustomerImage,
                                     IsActive = c.IsActive
                                 }).OrderByDescending(x => x.HappyCustomerId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstHappyCustomers);
        }

        [AdminPermission(RolePermissionEnum.Add)]
        public ActionResult Add()
        {
            HappyCustomerVM customerVM = new HappyCustomerVM(); 
            customerVM.FinanceYearList = GetFinancialYearList();

            return View(customerVM);
        }

        [HttpPost]
        public ActionResult Add(HappyCustomerVM customerVM, HttpPostedFileBase CustomerImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath(HappyCustomerDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (CustomerImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(CustomerImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            customerVM.FinanceYearList = GetFinancialYearList();

                            ModelState.AddModelError("CustomerImageFile", ErrorMessage.SelectOnlyImage);
                            return View(customerVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(CustomerImageFile.FileName);
                        CustomerImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        customerVM.FinanceYearList = GetFinancialYearList();

                        ModelState.AddModelError("CustomerImageFile", ErrorMessage.ImageRequired);
                        return View(customerVM);
                    }

                    tbl_HappyCustomers objCustomer = new tbl_HappyCustomers();
                    objCustomer.FinanceYear = customerVM.FinanceYear;
                    objCustomer.CustomerName = customerVM.CustomerName;
                    objCustomer.CustomerImage = fileName;
                    objCustomer.IsActive = true;
                    objCustomer.CreatedBy = LoggedInUserId;
                    objCustomer.CreatedDate = DateTime.UtcNow;
                    objCustomer.UpdatedBy = LoggedInUserId;
                    objCustomer.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_HappyCustomers.Add(objCustomer);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            customerVM.FinanceYearList = GetFinancialYearList();

            return View(customerVM);
        }

        [AdminPermission(RolePermissionEnum.Edit)]
        public ActionResult Edit(int Id)
        {
            HappyCustomerVM objCustomer = new HappyCustomerVM();

            try
            {
                objCustomer = (from c in _db.tbl_HappyCustomers
                               where c.HappyCustomerId == Id
                               select new HappyCustomerVM
                               {
                                   HappyCustomerId = c.HappyCustomerId,
                                   FinanceYear = c.FinanceYear,
                                   CustomerName = c.CustomerName,
                                   CustomerImage = c.CustomerImage,
                                   IsActive = c.IsActive
                               }).FirstOrDefault();

                objCustomer.FinanceYearList = GetFinancialYearList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objCustomer);
        }

        [HttpPost]
        public ActionResult Edit(HappyCustomerVM customerVM, HttpPostedFileBase CustomerImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    tbl_HappyCustomers objCustomer = _db.tbl_HappyCustomers.Where(x => x.HappyCustomerId == customerVM.HappyCustomerId).FirstOrDefault();

                    string fileName = string.Empty;
                    string path = Server.MapPath(HappyCustomerDirectoryPath);
                    if (CustomerImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(CustomerImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            customerVM.FinanceYearList = GetFinancialYearList();

                            ModelState.AddModelError("CustomerImageFile", ErrorMessage.SelectOnlyImage);
                            return View(customerVM);
                        }

                        // Save image in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(CustomerImageFile.FileName);
                        CustomerImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objCustomer.CustomerImage;
                    }

                    objCustomer.CustomerImage = fileName;
                    objCustomer.FinanceYear = customerVM.FinanceYear;
                    objCustomer.CustomerName = customerVM.CustomerName;

                    objCustomer.UpdatedBy = LoggedInUserId;
                    objCustomer.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            customerVM.FinanceYearList = GetFinancialYearList();

            return View(customerVM);
        }

        [HttpPost]
        public string DeleteHappyCustomer(int HappyCustomerId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_HappyCustomers objCustomer = _db.tbl_HappyCustomers.Where(x => x.HappyCustomerId == HappyCustomerId).FirstOrDefault();

                if (objCustomer == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_HappyCustomers.Remove(objCustomer);
                    _db.SaveChanges();

                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string ChangeStatus(int Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_HappyCustomers objHome = _db.tbl_HappyCustomers.Where(x => x.HappyCustomerId == Id).FirstOrDefault();

                if (objHome != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objHome.IsActive = true;
                    }
                    else
                    {
                        objHome.IsActive = false;
                    }

                    objHome.UpdatedBy = LoggedInUserId;
                    objHome.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        private List<SelectListItem> GetFinancialYearList()
        {
            int startYear = DateTime.Now.Year - 5;
            int endYear = DateTime.Now.Year;

            List<SelectListItem> lstFinancialYear = new List<SelectListItem>();
            for (int i = endYear; i >= startYear; i--)
            {
                string stryear = i + "-" + (i + 1);

                SelectListItem objSelectListItem = new SelectListItem();
                objSelectListItem.Text = stryear;
                objSelectListItem.Value = stryear;
                lstFinancialYear.Add(objSelectListItem);
            }

            return lstFinancialYear;

        }

    }
}