using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PROTR.Core;
using PROTR.Core.REST;
using PROTR.Core.Security;
using PROTR.Core.Security.EF;
using PROTR.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PROTR.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        protected IMemoryCache memoryCache;
        protected ContextProvider contextProvider;

        public AuthenticationController(IMemoryCache memoryCache, ContextProvider contextProvider)
        {
            this.memoryCache = memoryCache;
            this.contextProvider = contextProvider;
        }

        public class LoginObject
        {
            public string email;
            public string password;
        }

        [HttpPost]
        public async Task<AppUserModel> Login([FromBody]LoginObject login)
        {
            bool valid;

            HttpContext.Session.Clear();

            valid = await AppUser.Login(contextProvider, login.email, login.password);

            if (!valid)
            {
                HttpContext.Session.Clear();
            }
            else
            {
                CookiesHelper.WriteCookie(HttpContext, CookiesHelper.LoginCookieName, login.email, 1);
            }

            return (AppUserModel)(await contextProvider.GetAppUser()).ToModelObject;
        }

        [HttpGet]
        public async Task<AppUserModel> CurrentUser()
        {
            if (!contextProvider.UserIsAuthenticated)
            {
                return null;
            }
            return (AppUserModel)(await contextProvider.GetAppUser()).ToModelObject;
        }

        [HttpGet]
        public bool Logout()
        {
            if (contextProvider.UserIsAuthenticated)
            {
                HttpContext.Session.Clear();
            }

            return true;
        }

        /*
          loginUrl = '/Authentication/Login'; post
          logoutUrl = '/Authentication/Logout'; get
          currentUserUrl = '/Authentication/CurrentUser'; get
        */

    }
}
