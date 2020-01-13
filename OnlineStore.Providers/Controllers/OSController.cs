using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using OnlineStore.Identity;
using System.Text.RegularExpressions;
using OnlineStore.EntityFramework;

namespace OnlineStore.Providers.Controllers
{
    public class OSController : Controller
    {
        public OSController()
        {
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public string UserID
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                    return User.Identity.GetUserId();
                else
                    return null;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return User.IsInRole("Administrator");
            }
        }

        public bool IsWriter
        {
            get
            {
                return User.IsInRole("Writer");
            }
        }

        public bool IsAccountant
        {
            get
            {
                return User.IsInRole("Accountant");
            }
        }

        public bool IsSeller
        {
            get
            {
                return User.IsInRole("Seller");
            }
        }

        protected ViewResult ClearView(object model, string viewUrl = null)
        {
            ModelState.Clear();

            if (!String.IsNullOrWhiteSpace(viewUrl))
            {
                return base.View(viewUrl, model);
            }

            return base.View(model);
        }

        protected void SetErrors(Exception ex)
        {
            if (ex is DbException)
            {
                ViewBag.Errors = (ex as DbException).Errors;
            }
            else
            {
                var msg = ex.Message;
                if (IsAdmin)
                    msg += Environment.NewLine + ex.StackTrace;

                msg = Regex.Replace(msg, @"\r\n?|\n", "<br />");

                ViewBag.Errors = new List<string>() { msg };
            }

            ViewBag.Success = false;
        }
    }
}
