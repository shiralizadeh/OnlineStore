using System.Web.Mvc;

namespace OnlineStore.Website.Areas.User
{
    public class UserAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "User";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "User_default",
                "User/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "OnlineStore.Website.Areas.User.Controllers" }
            );
        }
    }
}