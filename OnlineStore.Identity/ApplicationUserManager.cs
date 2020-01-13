using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Identity
{
    public class ApplicationUserManager : UserManager<OSUser>
    {
        public ApplicationUserManager(IUserStore<OSUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<OSUser>(context.Get<IdentityDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<OSUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<OSUser>
            {
                MessageFormat = "Your security code is: {0}"
            });

            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<OSUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });

            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<OSUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }
}
