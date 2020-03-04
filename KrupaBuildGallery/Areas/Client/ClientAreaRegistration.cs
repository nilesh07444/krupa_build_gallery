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
                "client/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_HomePage",
                "home",
                new { controller = "HomePage", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_AboutUs",
                "aboutus",
                new { controller = "AboutUs", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_ContactUs",
                "contactus",
                new { controller = "ContactUs", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Login",
                "login",
                new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Register",
                "register",
                new { controller = "Register", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_DistributorRequest",
                "distributorrequest",
                new { controller = "DistributorRequest", action = "Index", id = UrlParameter.Optional }
            );
             
            context.MapRoute(
                "Admin_Cart",
                "cart",
                new { controller = "Cart", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Checkout",
                "checkout",
                new { controller = "Checkout", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Faq",
                "faq",
                new { controller = "Faq", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Wishlist",
                "wishlist",
                new { controller = "Wishlist", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Orders",
                "orders",
                new { controller = "Orders", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Privacy",
                "privacy",
                new { controller = "PrivacyPolicy", action = "Index", id = UrlParameter.Optional }
            );
              
        }
    }
}