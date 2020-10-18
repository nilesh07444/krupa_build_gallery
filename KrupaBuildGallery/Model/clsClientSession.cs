﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KrupaBuildGallery.ViewModel;

namespace KrupaBuildGallery.Model
{
    public class clsClientSession
    {

        public static long UserID
        {
            get
            {
                return HttpContext.Current.Session["ClientUserID"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["ClientUserID"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["ClientUserID"] = value;
            }
        }
        public static int RoleID
        {
            get
            {
                return HttpContext.Current.Session["ClientRoleID"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["ClientRoleID"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["ClientRoleID"] = value;
            }
        }
        public static String SessionID
        {
            get
            {
                return HttpContext.Current.Session["ClientSessionID"] != null ? Convert.ToString(HttpContext.Current.Session["ClientSessionID"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientSessionID"] = value;
            }
        }
        public static String RoleName
        {
            get
            {
                return HttpContext.Current.Session["ClientRoleName"] != null ? Convert.ToString(HttpContext.Current.Session["ClientRoleName"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientRoleName"] = value;
            }
        }

        public static String Email
        {
            get
            {
                return HttpContext.Current.Session["ClientEmail"] != null ? Convert.ToString(HttpContext.Current.Session["ClientEmail"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientEmail"] = value;
            }
        }

        public static String FirmName
        {
            get
            {
                return HttpContext.Current.Session["ClientFirmName"] != null ? Convert.ToString(HttpContext.Current.Session["ClientFirmName"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientFirmName"] = value;
            }
        }
        public static string ImagePath
        {
            get
            {
                return HttpContext.Current.Session["ClientImagePath"] != null ? Convert.ToString(HttpContext.Current.Session["ClientImagePath"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientImagePath"] = value;
            }
        }
        public static string MobileNumber
        {
            get
            {
                return HttpContext.Current.Session["ClientMobileNumber"] != null ? Convert.ToString(HttpContext.Current.Session["ClientMobileNumber"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientMobileNumber"] = value;
            }
        }


        public static string UserName
        {
            get
            {
                return HttpContext.Current.Session["ClientUserName"] != null ? Convert.ToString(HttpContext.Current.Session["ClientUserName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientUserName"] = value;
            }
        }

        public static string FirstName
        {
            get
            {
                return HttpContext.Current.Session["ClientFirstName"] != null ? Convert.ToString(HttpContext.Current.Session["ClientFirstName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientFirstName"] = value;
            }
        }

        public static string LastName
        {
            get
            {
                return HttpContext.Current.Session["ClientLastName"] != null ? Convert.ToString(HttpContext.Current.Session["ClientLastName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ClientLastName"] = value;
            }
        }

        public static string Prefix
        {
            get
            {
                return HttpContext.Current.Session["Prefix"] != null ? Convert.ToString(HttpContext.Current.Session["Prefix"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["Prefix"] = value;
            }
        }
    }

    public class ChatMessageModel
    {      
        public List<ChtMessage> ChatMessages { get; set; }
        public bool IsOnline { get; set; }
        public long LastChatMessageId { get; set; }
    }

    public class OnlineUserDetails
    {
        public long UserID { get; set; }
        public List<string> ConnectionID { get; set; }
        public string Name { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsOnline { get; set; }
        public int UnReadMessageCount { get; set; }
        public DateTime LastUpdationTime { get; set; }
    }
}