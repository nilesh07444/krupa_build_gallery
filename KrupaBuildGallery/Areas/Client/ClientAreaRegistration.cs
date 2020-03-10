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
                "Client_HomePage",
                "home",
                new { controller = "HomePage", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_AboutUs",
                "aboutus",
                new { controller = "AboutUs", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_ContactUs",
                "contactus",
                new { controller = "ContactUs", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Login",
                "login",
                new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Register",
                "register",
                new { controller = "Register", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_DistributorRequest",
                "distributorrequest",
                new { controller = "DistributorRequest", action = "Index", id = UrlParameter.Optional }
            );
             
            context.MapRoute(
                "Client_Cart",
                "cart",
                new { controller = "Cart", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Checkout",
                "checkout",
                new { controller = "Checkout", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Faq",
                "faq",
                new { controller = "Faq", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Wishlist",
                "wishlist",
                new { controller = "Wishlist", action = "Index", id = UrlParameter.Optional }
            );
             
            context.MapRoute(
                "Client_Privacy",
                "privacy",
                new { controller = "PrivacyPolicy", action = "Index", id = UrlParameter.Optional }
            );

            // Product Urls 

            context.MapRoute(
                "Client_Products_ByFilter",
                "products/{action}/{id}",
                new { Controller = "Products", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Orders",
                "orders/{action}/{id}",
                new { controller = "Orders", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}