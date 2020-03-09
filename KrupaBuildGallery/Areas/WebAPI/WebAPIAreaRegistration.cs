using System.Web.Mvc;
using System.Web.Http;

namespace KrupaBuildGallery.Areas.WebAPI
{
    public class WebAPIAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WebAPI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "WebAPI_default",
            //    "api/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.Routes.MapHttpRoute("WebAPI_default",
                "api/{controller}/{action}/{id}",
                new { action = "Index", id = RouteParameter.Optional });


        }
    }
}