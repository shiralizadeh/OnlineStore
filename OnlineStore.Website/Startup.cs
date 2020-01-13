using Microsoft.Owin;
using OnlineStore.Identity;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineStore.Website.Startup))]
namespace OnlineStore.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            StartupIdentity.Configure(app);
        }
    }
}
