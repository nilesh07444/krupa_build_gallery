using KrupaBuildGallery;
using KrupaBuildGallery.Filters;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Services;
using System.Web.Mvc;
using System.Web.Routing;

namespace ConstructionDiary.Models
{
    public class CustomAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int userId = filterContext.HttpContext.Session["UserID"] != null ? Int32.Parse(filterContext.HttpContext.Session["UserID"].ToString()) : 0;
            if (userId == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            Exception = "error"
                        }
                    };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        { "controller", "Login" },
                        { "action", "Index" }
                    });
                }
                return;

            }

            // Check Page Access

            int roleId = filterContext.HttpContext.Session["RoleID"] != null ? Int32.Parse(filterContext.HttpContext.Session["RoleID"].ToString()) : 0;
            if (roleId == 1)
            {
                // This is super admin user, so have full access.
                return;
            }

            int permissionMode = 0;

            dynamic permissionAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AdminPermissionAttribute), true).FirstOrDefault();
            var controllerName = (string)filterContext.RouteData.Values["controller"];

            if (permissionAttributes != null && permissionAttributes.Permission != null)
            {
                if (permissionAttributes.Permission != null)
                {
                    permissionMode = (int)permissionAttributes.Permission;

                    if (permissionMode > (int)RolePermissionEnum.DoNotCheck)
                    {
                        bool IsDenied = CommonMethod.IsPageAccessDeniedToUser(controllerName, permissionMode);

                        if (IsDenied)
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                                { "controller", "AccessDenied" },
                                { "action", "Index" }
                            });
                        }
                    }

                }
            }

            base.OnActionExecuting(filterContext);
        }

    }
}