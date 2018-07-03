using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuiGigAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "UploadProfileImage",
                "UploadProfileImage",
                  defaults: new { controller = "FileUpload", action = "UploadProfileImage", id = UrlParameter.Optional },
               namespaces: new[] { "QuiGigAPI.Controllers" }
            );

            routes.MapRoute(
                "UploadPortfolioImage",
                "UploadPortfolioImage",
                  defaults: new { controller = "FileUpload", action = "UploadPortfolioImage", id = UrlParameter.Optional },
               namespaces: new[] { "QuiGigAPI.Controllers" }
            );

            routes.MapRoute(
               "google-login",
               "google-login",
                 defaults: new { controller = "SocialLogin", action = "SocialLogin", id = UrlParameter.Optional },
              namespaces: new[] { "QuiGigAPI.Controllers" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
