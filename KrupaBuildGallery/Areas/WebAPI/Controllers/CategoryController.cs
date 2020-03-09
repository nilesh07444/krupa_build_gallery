using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
//using System.Web.Mvc;
using System.Web.Http;
using KrupaBuildGallery.Model;

namespace KrupaBuildGallery.Areas.WebAPI.Controllers
{
    public class CategoryController : ApiController
    {
        krupagallarydbEntities _db;
        public CategoryController()
        {
            _db = new krupagallarydbEntities();
        }

        [Route("GetCategoryList"), HttpGet]
        public ResponseDataModel<List<CategoryVM>> GetCategoryList()
        {
            ResponseDataModel<List<CategoryVM>> response = new ResponseDataModel<List<CategoryVM>>();
            List<CategoryVM> lstCategory = new List<CategoryVM>();
            try
            {

                lstCategory = (from c in _db.tbl_Categories
                               where !c.IsDelete && c.IsActive
                               select new CategoryVM
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   CategoryImage = c.CategoryImage,
                                   IsActive = c.IsActive
                               }).OrderByDescending(x => x.CategoryId).ToList();

                response.Data = lstCategory;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }


    }
}