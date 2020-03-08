﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery.Model
{
    public class clsClientSession
    {

        public static long UserID
        {
            get
            {
                return HttpContext.Current.Session["UserID"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["UserID"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["UserID"] = value;
            }
        }
        public static int RoleID
        {
            get
            {
                return HttpContext.Current.Session["RoleID"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["RoleID"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["RoleID"] = value;
            }
        }
        public static String SessionID
        {
            get
            {
                return HttpContext.Current.Session["SessionID"] != null ? Convert.ToString(HttpContext.Current.Session["SessionID"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionID"] = value;
            }
        }
        public static String RoleName
        {
            get
            {
                return HttpContext.Current.Session["RoleName"] != null ? Convert.ToString(HttpContext.Current.Session["RoleName"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["RoleName"] = value;
            }
        }

        public static String Email
        {
            get
            {
                return HttpContext.Current.Session["Email"] != null ? Convert.ToString(HttpContext.Current.Session["Email"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["Email"] = value;
            }
        }

        public static String FirmName
        {
            get
            {
                return HttpContext.Current.Session["FirmName"] != null ? Convert.ToString(HttpContext.Current.Session["FirmName"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["FirmName"] = value;
            }
        }
        public static string ImagePath
        {
            get
            {
                return HttpContext.Current.Session["ImagePath"] != null ? Convert.ToString(HttpContext.Current.Session["ImagePath"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ImagePath"] = value;
            }
        }
        public static string UserName
        {
            get
            {
                return HttpContext.Current.Session["UserName"] != null ? Convert.ToString(HttpContext.Current.Session["UserName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["UserName"] = value;
            }
        }

        public static string FirstName
        {
            get
            {
                return HttpContext.Current.Session["FirstName"] != null ? Convert.ToString(HttpContext.Current.Session["FirstName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["FirstName"] = value;
            }
        }

        public static string LastName
        {
            get
            {
                return HttpContext.Current.Session["LastName"] != null ? Convert.ToString(HttpContext.Current.Session["LastName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["LastName"] = value;
            }
        }
    }
 
}