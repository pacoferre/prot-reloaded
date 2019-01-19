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
        public AppUserModel Login([FromBody]LoginObject login)
        {
            bool valid;

            HttpContext.Session.Clear();

            valid = AppUser.Login(login.email, login.password, contextProvider);

            if (!valid)
            {
                HttpContext.Session.Clear();
            }
            else
            {
                CookiesHelper.WriteCookie(HttpContext, CookiesHelper.LoginCookieName, login.email, 1);
            }

            return contextProvider.Mapper.Map<AppUserModel>(contextProvider.GetAppUser());
        }

        [HttpGet]
        public AppUserModel CurrentUser()
        {
            if (!contextProvider.UserIsAuthenticated)
            {
                return null;
            }
            return contextProvider.Mapper.Map<AppUserModel>(contextProvider.GetAppUser());
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
