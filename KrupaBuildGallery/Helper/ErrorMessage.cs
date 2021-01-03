using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.Helper
{
    public static class ErrorMessage
    {
        // Admin Portal Messages
        public static string CategoryNameExists = "Category Name is already exist";
        public static string SubProductNameExists = "Sub Product Name is already exist";
        public static string ProductNameExists = "Product Name is already exist";
        public static string ItemNameExists = "Item Name is already exist";
        public static string QtyGreater = "Please enter quantity greater than 0";
        public static string ItemTextExists = "Item Text is already exist";
        public static string ImageRequired = "Image is required";
        public static string GodownNameExists = "Godown name is already exist";
        public static string SelectOnlyImage = "Please select only image file";
        public static string RoleNameExists = "Role name is already exist";
        public static string MobileNoExists = "Mobile No is already exist";
        public static string SelectExcelFile = "Please select excel file";
        public static string PincodeExists = "Pincode is already exist";
        public static string UnitExists = "Unit is already exist";
        public static string EmailExists = "Email is already exist";
        public static string OwnerContactNoExists = "Owner Contact No is already exist";
        public static string FirmContactNoExists = "Firm Contact No is already exist";
        // Client Portal Messages


        // Folder Directory Path
        public static string HomeDirectoryPath = "/Images/HomeMedia/";
        public static string AdminUserDirectoryPath = "/Images/AdminUserMedia/";
        public static string CategoryDirectoryPath = "/Images/CategoryMedia/";
        public static string ProductItemDirectoryPath = "/Images/ProductItemMedia/";
        public static string DefaultImagePath = "/Images/default_image.jpg";
        public static string HappyCustomerDirectoryPath = "/Images/HappyCustomerMedia/";
        public static string AdvertiseDirectoryPath = "/Images/AdvertiseMedia/";
        public static string UsersDocumentsDirectoryPath = "/Images/UsersDocuments/";
    }
}