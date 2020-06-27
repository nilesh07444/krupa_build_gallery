using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public static class CommonMethod
    {
        public static string ConvertFromUTC(DateTime? utcDateTime)
        {
            if (utcDateTime == null)
                return "";

            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
            string dt = dateTimeAsTimeZone.ToString("dd/MM/yyyy hh:mm tt");
            return dt;
        }

        public static DateTime ConvertToUTC(DateTime dateTime, string timeZone)
        {
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, nzTimeZone);
            return utcDateTime;
        }

        public static List<List<CategoryVM>> GetCategoryForMegaMenu()
        {

            krupagallarydbEntities _db = new krupagallarydbEntities();

            //List<CategoryVM> lstCategory = new List<CategoryVM>();            
            //for (int i = 1; i <= 10; i++)
            //{
            //    lstCategory.Add(new CategoryVM { CategoryId = i, CategoryName = "Category " + i });
            //}

            List<CategoryVM> lstCategory = (from c in _db.tbl_Categories
                                            where !c.IsDelete && c.IsActive
                                            select new CategoryVM
                                            {
                                                CategoryId = c.CategoryId,
                                                CategoryName = c.CategoryName
                                            }).OrderBy(x => x.CategoryName).ToList();

            List<List<CategoryVM>> categoryMegaMenuVMs = Split(lstCategory, 10);

            return categoryMegaMenuVMs;
        }

        public static List<List<T>> Split<T>(this List<T> items, int sliceSize = 30)
        {
            List<List<T>> list = new List<List<T>>();
            for (int i = 0; i < items.Count; i += sliceSize)
                list.Add(items.GetRange(i, Math.Min(sliceSize, items.Count - i)));
            return list;
        }

    }
}