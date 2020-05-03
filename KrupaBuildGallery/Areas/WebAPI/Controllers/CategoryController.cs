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
                               }).OrderBy(x => x.CategoryName).ToList();
                lstCategory.ForEach(x => x.TotalItems = TotalItemsinCategory(x.CategoryId));
                response.Data = lstCategory;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [Route("GetProductListByCategoryId"), HttpGet]
        public ResponseDataModel<List<ProductVM>> GetProductListByCategoryId(string CategoryId)
        {
            ResponseDataModel<List<ProductVM>> response = new ResponseDataModel<List<ProductVM>>();
            List<ProductVM> lstProducts = new List<ProductVM>();
            try
            {
                long CategoryId64 = Convert.ToInt64(CategoryId);
                lstProducts = (from c in _db.tbl_Products
                               where !c.IsDelete && c.IsActive && c.CategoryId == CategoryId64
                               select new ProductVM
                               {
                                   ProductId = c.Product_Id,
                                   ProductName = c.ProductName,
                                   ProductImage = c.ProductImage,
                                   IsActive = c.IsActive
                               }).OrderBy(x => x.ProductName).ToList();

                response.Data = lstProducts;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }


        [Route("GetSubProductListByProductId"), HttpGet]
        public ResponseDataModel<List<SubProductVM>> GetSubProductListByProductId(string ProductId)
        {
            ResponseDataModel<List<SubProductVM>> response = new ResponseDataModel<List<SubProductVM>>();
            List<SubProductVM> lstSubProducts = new List<SubProductVM>();
            try
            {
                long ProductId64 = Convert.ToInt64(ProductId);
                lstSubProducts = (from c in _db.tbl_SubProducts
                               where !c.IsDelete && c.IsActive && c.ProductId == ProductId64
                                  select new SubProductVM
                                {
                                   ProductId = c.ProductId,
                                   SubProductName = c.SubProductName,
                                   SubProductId = c.SubProductId,
                                   SubProductImage = c.SubProductImage,
                                   IsActive = c.IsActive
                               }).OrderBy(x => x.ProductName).ToList();

                response.Data = lstSubProducts;

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        public int TotalItemsinCategory(long CatId)
        {
            return _db.tbl_ProductItems.Where(o => o.CategoryId == CatId && o.IsDelete == false && o.IsActive == true).ToList().Count;
        }

    }
}