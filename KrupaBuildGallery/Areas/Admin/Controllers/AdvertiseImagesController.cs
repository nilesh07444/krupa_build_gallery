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
    public class AdvertiseImagesController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public string AdvertiseDirectoryPath = "";
        public AdvertiseImagesController()
        {
            _db = new krupagallarydbEntities();
            AdvertiseDirectoryPath = ErrorMessage.AdvertiseDirectoryPath;
        }

        public ActionResult Index()
        {
            List<AdvertiseImageVM> lstHomeImages = new List<AdvertiseImageVM>();
            try
            {

                lstHomeImages = (from c in _db.tbl_AdvertiseImages
                                 select new AdvertiseImageVM
                                 {
                                     AdvertiseImageId = c.AdvertiseImageId,
                                     ImageUrl = c.AdvertiseImage, 
                                     IsActive = c.IsActive,
                                     SliderType = c.SliderType,
                                     ImageFor = c.ImageFor
                                 }).OrderByDescending(x => x.AdvertiseImageId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstHomeImages);
        }

        public ActionResult Add()
        {
            AdvertiseImageVM AdvertiseImageVM = new AdvertiseImageVM();
            AdvertiseImageVM.SliderTypeList = GetSliderTypeList();
            AdvertiseImageVM.AdvertiseImageForList = GetAdvertiseImageFor();
            return View(AdvertiseImageVM);
        }

        [HttpPost]
        public ActionResult Add(AdvertiseImageVM AdvertiseImageVM, HttpPostedFileBase AdvertiseImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath(AdvertiseDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (AdvertiseImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(AdvertiseImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            AdvertiseImageVM.SliderTypeList = GetSliderTypeList();

                            ModelState.AddModelError("AdvertiseImageFile", ErrorMessage.SelectOnlyImage);
                            return View(AdvertiseImageVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(AdvertiseImageFile.FileName);
                        AdvertiseImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        AdvertiseImageVM.SliderTypeList = GetSliderTypeList();

                        ModelState.AddModelError("AdvertiseImageFile", ErrorMessage.ImageRequired);
                        return View(AdvertiseImageVM);
                    }

                    tbl_AdvertiseImages objAdvertise = new tbl_AdvertiseImages();
                    objAdvertise.ImageFor = AdvertiseImageVM.ImageFor;
                    objAdvertise.AdvertiseImage = fileName;
                    objAdvertise.IsActive = true;
                    objAdvertise.SliderType = AdvertiseImageVM.SliderType;
                    objAdvertise.CreatedBy = LoggedInUserId;
                    objAdvertise.CreatedDate = DateTime.UtcNow;
                    objAdvertise.UpdatedBy = LoggedInUserId;
                    objAdvertise.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_AdvertiseImages.Add(objAdvertise);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(AdvertiseImageVM);
        }

        public ActionResult Edit(int Id)
        {
            AdvertiseImageVM objAdvertise = new AdvertiseImageVM();

            try
            {
                objAdvertise = (from c in _db.tbl_AdvertiseImages
                           where c.AdvertiseImageId == Id
                           select new AdvertiseImageVM
                           {
                               AdvertiseImageId = c.AdvertiseImageId,
                               ImageUrl = c.AdvertiseImage,
                               IsActive = c.IsActive ,
                               SliderType = c.SliderType,
                               ImageFor = c.ImageFor
                           }).FirstOrDefault();

                objAdvertise.SliderTypeList = GetSliderTypeList();
                objAdvertise.AdvertiseImageForList = GetAdvertiseImageFor();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objAdvertise);
        }

        [HttpPost]
        public ActionResult Edit(AdvertiseImageVM AdvertiseImageVM, HttpPostedFileBase AdvertiseImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    tbl_AdvertiseImages objAdvertise = _db.tbl_AdvertiseImages.Where(x => x.AdvertiseImageId == AdvertiseImageVM.AdvertiseImageId).FirstOrDefault();

                    string fileName = string.Empty;
                    string path = Server.MapPath(AdvertiseDirectoryPath);
                    if (AdvertiseImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(AdvertiseImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            AdvertiseImageVM.SliderTypeList = GetSliderTypeList();
                            AdvertiseImageVM.AdvertiseImageForList = GetAdvertiseImageFor();

                            ModelState.AddModelError("AdvertiseImageFile", ErrorMessage.SelectOnlyImage);
                            return View(AdvertiseImageVM);
                        }

                        // Save image in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(AdvertiseImageFile.FileName);
                        AdvertiseImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objAdvertise.AdvertiseImage;
                    }

                    objAdvertise.AdvertiseImage = fileName;
                    objAdvertise.ImageFor = AdvertiseImageVM.ImageFor;
                    objAdvertise.SliderType = AdvertiseImageVM.SliderType;
                    objAdvertise.UpdatedBy = LoggedInUserId;
                    objAdvertise.UpdatedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(AdvertiseImageVM);
        }

        [HttpPost]
        public string DeleteAdvertiseImage(int AdvertiseImageId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_AdvertiseImages objAdvertise = _db.tbl_AdvertiseImages.Where(x => x.AdvertiseImageId == AdvertiseImageId).FirstOrDefault();

                if (objAdvertise == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_AdvertiseImages.Remove(objAdvertise);
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
                tbl_AdvertiseImages objAdvertise = _db.tbl_AdvertiseImages.Where(x => x.AdvertiseImageId == Id).FirstOrDefault();

                if (objAdvertise != null)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objAdvertise.IsActive = true;
                    }
                    else
                    {
                        objAdvertise.IsActive = false;
                    }

                    objAdvertise.UpdatedBy = LoggedInUserId;
                    objAdvertise.UpdatedDate = DateTime.UtcNow;

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

        private List<SelectListItem> GetSliderTypeList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "2", Text = "Slider 2" });
            lst.Add(new SelectListItem { Value = "3", Text = "Slider 3" });

            return lst;
        }

        private List<SelectListItem> GetAdvertiseImageFor()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "1", Text = "Website" });
            lst.Add(new SelectListItem { Value = "2", Text = "Mobile App" });

            return lst;
        }

    }
}