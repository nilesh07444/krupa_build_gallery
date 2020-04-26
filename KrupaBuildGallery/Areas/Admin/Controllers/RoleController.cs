using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class RoleController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public RoleController()
        {
            _db = new krupagallarydbEntities();
        }
        public ActionResult Index()
        {
            List<RoleVM> lstRoles = new List<RoleVM>();

            try
            {
                lstRoles = (from r in _db.tbl_AdminRoles
                            where !r.IsDelete
                            select new RoleVM
                            {
                                AdminRoleId = r.AdminRoleId,
                                AdminRoleName = r.AdminRoleName,
                                AdminRoleDescription = r.AdminRoleDescription,
                                IsActive = r.IsActive
                            }).OrderByDescending(x => x.AdminRoleId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(lstRoles);
        }

        public ActionResult Add()
        {
            RoleVM objRole = new RoleVM();
            try
            {
                objRole.lstRoleModules = (from m in _db.tbl_AdminRoleModules
                                          select new RoleModuleVM
                                          {
                                              AdminRoleModuleId = m.AdminRoleModuleId,
                                              ModuleName = m.ModuleName,
                                              DisplayOrder = m.DisplayOrder,
                                              None = m.None,
                                              Add = m.Add,
                                              Edit = m.Edit,
                                              Full = m.Full
                                          }).OrderBy(x=>x.ModuleName).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return View(objRole);
        }

        [HttpPost]
        public ActionResult Add(RoleVM roleVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    var dataExist = _db.tbl_AdminRoles.Where(x => x.AdminRoleName.ToLower() == roleVM.AdminRoleName.ToLower() && !x.IsDelete).FirstOrDefault();
                    if (dataExist != null)
                    {
                        ModelState.AddModelError("AdminRoleName", ErrorMessage.RoleNameExists);
                        return View(roleVM);
                    }

                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    // Save Role
                    tbl_AdminRoles objRole = new tbl_AdminRoles();
                    objRole.AdminRoleName = roleVM.AdminRoleName;
                    objRole.AdminRoleDescription = roleVM.AdminRoleDescription;
                    objRole.IsActive = true;
                    objRole.IsDelete = false;
                    objRole.CreatedBy = LoggedInUserId;
                    objRole.CreatedDate = DateTime.UtcNow;
                    objRole.UpdatedBy = LoggedInUserId;
                    objRole.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_AdminRoles.Add(objRole);
                    _db.SaveChanges();

                    // Save Module Permission
                    if (roleVM.lstRoleModules.Count > 0)
                    {
                        foreach (RoleModuleVM module in roleVM.lstRoleModules)
                        {
                            tbl_AdminRolePermissions objPermission = new tbl_AdminRolePermissions();
                            objPermission.AdminRoleId = objRole.AdminRoleId;
                            objPermission.AdminRoleModuleId = module.AdminRoleModuleId;
                            objPermission.Permission = module.SelectedValue;
                            _db.tbl_AdminRolePermissions.Add(objPermission);
                            _db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            // Rebind role modules
            roleVM.lstRoleModules = (from m in _db.tbl_AdminRoleModules
                                     select new RoleModuleVM
                                     {
                                         AdminRoleModuleId = m.AdminRoleModuleId,
                                         ModuleName = m.ModuleName,
                                         DisplayOrder = m.DisplayOrder,
                                         None = m.None,
                                         Add = m.Add,
                                         Edit = m.Edit,
                                         Full = m.Full
                                     }).ToList();

            return View(roleVM);
        }

        public ActionResult Edit(int Id)
        {
            RoleVM objRole = new RoleVM();
            try
            {
                objRole = (from r in _db.tbl_AdminRoles
                           where r.AdminRoleId == Id
                           select new RoleVM
                           {
                               AdminRoleId = r.AdminRoleId,
                               AdminRoleName = r.AdminRoleName,
                               AdminRoleDescription = r.AdminRoleDescription,
                               IsActive = r.IsActive
                           }).FirstOrDefault();

                objRole.lstRoleModules = (from m in _db.tbl_AdminRoleModules
                                          join p in _db.tbl_AdminRolePermissions on m.AdminRoleModuleId equals p.AdminRoleModuleId into outerJoinPerm
                                          from p in outerJoinPerm.DefaultIfEmpty()
                                          where p.AdminRoleId == Id
                                          select new RoleModuleVM
                                          {
                                              AdminRoleModuleId = m.AdminRoleModuleId,
                                              ModuleName = m.ModuleName,
                                              DisplayOrder = m.DisplayOrder,
                                              None = m.None,
                                              Add = m.Add,
                                              Edit = m.Edit,
                                              Full = m.Full,
                                              SelectedValue = p.Permission,
                                              AdminRolePermissionId = p.AdminRolePermissionId
                                          }).OrderBy(x => x.ModuleName).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return View(objRole);
        }

        [HttpPost]
        public ActionResult Edit(RoleVM roleVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    var dataExist = _db.tbl_AdminRoles.Where(x => x.AdminRoleName.Trim().ToLower() == roleVM.AdminRoleName.Trim().ToLower() && !x.IsDelete && x.AdminRoleId != roleVM.AdminRoleId).FirstOrDefault();
                    if (dataExist != null)
                    {
                        ModelState.AddModelError("AdminRoleName", ErrorMessage.RoleNameExists);
                        return View(roleVM);
                    }

                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    // Update Role
                    tbl_AdminRoles objRole = _db.tbl_AdminRoles.Where(x => x.AdminRoleId == roleVM.AdminRoleId).FirstOrDefault();
                    if (objRole != null)
                    {
                        objRole.AdminRoleName = roleVM.AdminRoleName.Trim();
                        objRole.AdminRoleDescription = roleVM.AdminRoleDescription;
                        objRole.UpdatedBy = LoggedInUserId;
                        objRole.UpdatedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                    }

                    // Update Module Permission
                    if (roleVM.lstRoleModules.Count > 0)
                    {
                        foreach (RoleModuleVM module in roleVM.lstRoleModules)
                        {
                            tbl_AdminRolePermissions objPermission = _db.tbl_AdminRolePermissions.Where(x => x.AdminRolePermissionId == module.AdminRolePermissionId).FirstOrDefault();
                            if (objPermission != null)
                            {
                                // Update
                                objPermission.Permission = module.SelectedValue;
                                _db.SaveChanges();
                            }
                            else
                            {
                                // Add
                                tbl_AdminRolePermissions objAddPermission = new tbl_AdminRolePermissions();
                                objAddPermission.AdminRoleId = objRole.AdminRoleId;
                                objAddPermission.AdminRoleModuleId = module.AdminRoleModuleId;
                                objAddPermission.Permission = module.SelectedValue;
                                _db.tbl_AdminRolePermissions.Add(objAddPermission);
                                _db.SaveChanges();
                            }

                        }
                    }

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            // Rebind role modules
            roleVM.lstRoleModules = (from m in _db.tbl_AdminRoleModules
                                     select new RoleModuleVM
                                     {
                                         AdminRoleModuleId = m.AdminRoleModuleId,
                                         ModuleName = m.ModuleName,
                                         DisplayOrder = m.DisplayOrder,
                                         None = m.None,
                                         View = m.View,
                                         Add = m.Add,
                                         Edit = m.Edit,
                                         Full = m.Full
                                     }).ToList();

            return View(roleVM);
        }

        [HttpPost]
        public string DeleteRole(int RoleId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_AdminRoles objRole = _db.tbl_AdminRoles.Where(x => x.AdminRoleId == RoleId).FirstOrDefault();

                if (objRole == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    objRole.IsDelete = true;
                    objRole.UpdatedBy = LoggedInUserId;
                    objRole.UpdatedDate = DateTime.UtcNow;
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
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_AdminRoles objHome = _db.tbl_AdminRoles.Where(x => x.AdminRoleId == Id).FirstOrDefault();

                if (objHome != null)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());
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
         
        public ActionResult View(int Id)
        {
            RoleVM objRole = new RoleVM();
            try
            {
                objRole = (from r in _db.tbl_AdminRoles
                           join uC in _db.tbl_AdminUsers on r.CreatedBy equals uC.AdminUserId into outerCreated
                           from uC in outerCreated.DefaultIfEmpty()
                           join uM in _db.tbl_AdminUsers on r.UpdatedBy equals uM.AdminUserId into outerModified
                           from uM in outerModified.DefaultIfEmpty()
                           where r.AdminRoleId == Id
                           select new RoleVM
                           {
                               AdminRoleId = r.AdminRoleId,
                               AdminRoleName = r.AdminRoleName,
                               AdminRoleDescription = r.AdminRoleDescription,
                               IsActive = r.IsActive,

                               CreatedDate = r.CreatedDate,
                               UpdatedDate = r.UpdatedDate,
                               strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                               strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                           }).FirstOrDefault();

                objRole.lstRoleModules = (from m in _db.tbl_AdminRoleModules
                                          join p in _db.tbl_AdminRolePermissions on m.AdminRoleModuleId equals p.AdminRoleModuleId into outerJoinPerm
                                          from p in outerJoinPerm.DefaultIfEmpty()
                                          where p.AdminRoleId == Id
                                          select new RoleModuleVM
                                          {
                                              AdminRoleModuleId = m.AdminRoleModuleId,
                                              ModuleName = m.ModuleName,
                                              DisplayOrder = m.DisplayOrder,
                                              None = m.None,
                                              Add = m.Add,
                                              Edit = m.Edit,
                                              Full = m.Full,
                                              SelectedValue = p.Permission,
                                              AdminRolePermissionId = p.AdminRolePermissionId
                                          }).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return View(objRole);
        }

    }
}