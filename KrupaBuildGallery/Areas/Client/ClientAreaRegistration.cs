using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client
{
    public class ClientAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Client";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Client_default",
                "{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            ); 

            //context.MapRoute(
            //    "Admin_aboutus",
            //    "aboutus",
            //    new { controller = "AboutUs", action = "Index", id = UrlParameter.Optional }
            //);

        }
    }
}