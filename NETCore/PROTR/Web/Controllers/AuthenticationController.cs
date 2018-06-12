using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PROTR.Core.REST;
using PROTR.Core.Security;
using PROTR.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PROTR.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        protected IMemoryCache memoryCache;

        public AuthenticationController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public class LoginRequest
        {
            public string email;
            public string password;
        }

        [HttpPost]
        public Core.Security.EF.AppUser Login([FromBody]LoginRequest login)
        {
            bool valid;

            HttpContext.Session.Clear();

            valid = AppUser.Login(login.email, login.password, HttpContext);

            if (!valid)
            {
                HttpContext.Session.Clear();
            }
            else
            {
                CookiesHelper.WriteCookie(HttpContext, CookiesHelper.LoginCookieName, login.email, 1);
            }

            return AppUser.GetAppUser(HttpContext).MapToEF();
        }

        [HttpGet]
        public Core.Security.EF.AppUser CurrentUser()
        {
            if (!AppUser.UserIsAuthenticated(HttpContext))
            {
                return null;
            }
            return AppUser.GetAppUser(this.HttpContext).MapToEF();
        }

        [HttpGet]
        public bool Logout()
        {
            if (AppUser.UserIsAuthenticated(HttpContext))
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
