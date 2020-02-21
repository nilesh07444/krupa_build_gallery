using ConstructionDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class DistributorController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public DistributorController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Distributor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RequestList()
        {
            List<DistributorRequestVM> lstDistriRequest = new List<DistributorRequestVM>();
            try
            {

                lstDistriRequest = (from cu in _db.tbl_DistributorRequestDetails                                  
                                 where !cu.IsDelete.Value
                                 select new DistributorRequestVM
                                 {
                                     DistributorRequestId = cu.DistributorRequestId,
                                     FirstName = cu.FirstName,
                                     LastName = cu.LastName,                                     
                                     Email = cu.Email,                                     
                                     CompanyName = cu.CompanyName,                                     
                                     MobileNo = cu.MobileNo,                                     
                                     City = cu.City,
                                     State = cu.State,
                                     AddharCardNo = cu.AddharcardNo,
                                     PanCardNo = cu.PanCardNo,
                                     GSTNo = cu.GSTNo
                                 }).OrderBy(x => x.FirstName).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstDistriRequest);
        }
    }
}