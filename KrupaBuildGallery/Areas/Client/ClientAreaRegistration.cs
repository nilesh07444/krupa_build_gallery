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
              "Client_search",
              "search",
              new { controller = "search", action = "Index", id = UrlParameter.Optional }
          );

            context.MapRoute(
                "Client_Privacy",
                "privacy",
                new { controller = "PrivacyPolicy", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_CancelAndRefundPolicy",
                "cancelandrefundpolicy",
                new { controller = "CancelAndRefundPolicy", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_TermsCondition",
                "termscondition",
                new { controller = "TermsCondition", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "ClientDistributorLogin",
                "distributorlogin",
                new { controller = "Login", action = "Distributor", id = UrlParameter.Optional }
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

            context.MapRoute(
               "ClientLogout",
               "login/signout",
               new { controller = "login", action = "signout", id = UrlParameter.Optional }
           );
            context.MapRoute(
             "Client_forgotpassword",
             "forgotpassword",
             new { controller = "ForgotPassword", action = "Index", id = UrlParameter.Optional }
         );
            context.MapRoute(
               "Client_MyProfile",
               "myprofile",
               new { controller = "MyProfile", action = "Index", id = UrlParameter.Optional }
           );
            context.MapRoute(
            "Client_changepassword",
            "myprofile/changepassword",
            new { controller = "MyProfile", action = "ChangePassword", id = UrlParameter.Optional }
        );

            context.MapRoute(
               "Client_default",
               "client/{controller}/{action}/{id}",
               new { action = "Index", id = UrlParameter.Optional }
           );
        }
    }
}