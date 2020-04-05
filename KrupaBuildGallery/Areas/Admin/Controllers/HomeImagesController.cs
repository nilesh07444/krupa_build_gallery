using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class HomeImagesController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public string HomeDirectoryPath = "";
        public HomeImagesController()
        {
            _db = new krupagallarydbEntities(); 
            HomeDirectoryPath = ErrorMessage.HomeDirectoryPath;
        }

        public ActionResult Index()
        {
            List<HomeImageVM> lstHomeImages = new List<HomeImageVM>();
            try
            {

                lstHomeImages = (from c in _db.tbl_HomeImages
                                 select new HomeImageVM
                                 {
                                     HomeImageId = c.HomeImageId,
                                     HomeImageName = c.HomeImageName,
                                     HeadingText1 = c.HeadingText1,
                                     HeadingText2 = c.HeadingText2,
                                     IsActive = c.IsActive
                                 }).OrderByDescending(x => x.HomeImageId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstHomeImages);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(HomeImageVM homeImageVM, HttpPostedFileBase HomeImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath(HomeDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (HomeImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HomeImageFile.FileName); 
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HomeImageFile", ErrorMessage.SelectOnlyImage);
                            return View(homeImageVM);
                        }
                        
                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(HomeImageFile.FileName);
                        HomeImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        ModelState.AddModelError("HomeImageFile", ErrorMessage.ImageRequired);
                        return View(homeImageVM);
                    }

                    tbl_HomeImages objHome = new tbl_HomeImages();
                    objHome.HeadingText1 = homeImageVM.HeadingText1;
                    objHome.HeadingText2 = homeImageVM.HeadingText2;
                    objHome.HomeImageName = fileName;
                    objHome.IsActive = true;
                    objHome.CreatedBy = LoggedInUserId;
                    objHome.CreatedDate = DateTime.UtcNow;
                    objHome.UpdatedBy = LoggedInUserId;
                    objHome.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_HomeImages.Add(objHome);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(homeImageVM);
        }

        public ActionResult Edit(int Id)
        {
            HomeImageVM objHome = new HomeImageVM();

            try
            {
                objHome = (from c in _db.tbl_HomeImages
                           where c.HomeImageId == Id
                           select new HomeImageVM
                           {
                               HomeImageId = c.HomeImageId,
                               HomeImageName = c.HomeImageName,
                               HeadingText1 = c.HeadingText1,
                               HeadingText2 = c.HeadingText2,
                               IsActive = c.IsActive
                           }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objHome);
        }

        [HttpPost]
        public ActionResult Edit(HomeImageVM homeImageVM, HttpPostedFileBase HomeImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    tbl_HomeImages objHome = _db.tbl_HomeImages.Where(x => x.HomeImageId == homeImageVM.HomeImageId).FirstOrDefault();

                    string fileName = string.Empty;
                    string path = Server.MapPath(HomeDirectoryPath);
                    if (HomeImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HomeImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HomeImageFile", ErrorMessage.SelectOnlyImage);
                            return View(homeImageVM);
                        }

                        // Save image in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(HomeImageFile.FileName);
                        HomeImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objHome.HomeImageName;
                    }

                    objHome.HomeImageName = fileName;
                    objHome.HeadingText1 = homeImageVM.HeadingText1;
                    objHome.HeadingText2 = homeImageVM.HeadingText2;

                    objHome.UpdatedBy = LoggedInUserId;
                    objHome.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(homeImageVM);
        }

        [HttpPost]
        public string DeleteHomeImage(int HomeImageId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_HomeImages objHome = _db.tbl_HomeImages.Where(x => x.HomeImageId == HomeImageId).FirstOrDefault();

                if (objHome == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_HomeImages.Remove(objHome);
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
                tbl_HomeImages objHome = _db.tbl_HomeImages.Where(x => x.HomeImageId == Id).FirstOrDefault();

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

    }
}

