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
    public class PublicController : OSController
    {
        public PublicController()
        {
        }
    }
}
