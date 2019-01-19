using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PROTR.Core.Data;
using PROTR.Core.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PROTR.Core
{
    public class ContextProvider : IDisposable
    {
        internal static bool UseAppUserNoDB = false;
        internal static string CurrentUserIDSessionKey = "USER_ID";
        public static IConfiguration Configuration { get; set; }
        private HttpContext HttpContext { get; }
        public IMapper Mapper { get; }
        public BusinessBaseProvider BusinessProvider { get; }
        public ProtDbContext DbContext { get; }
        public DbDialect DbDialect { get; }
        public int? IDAppUser { get; private set; } = 0;

        public ContextProvider(IHttpContextAccessor contextAccessor, IMapper mapper,
                BusinessBaseProvider businessProvider, ProtDbContext dbContext, DbDialect dbDialect)
        {
            HttpContext = contextAccessor.HttpContext;
            Mapper = mapper;
            BusinessProvider = businessProvider;
            DbContext = dbContext;
            DbDialect = dbDialect;
            IDAppUser = HttpContext.Session.GetInt32(CurrentUserIDSessionKey);
        }

        public ConcurrentDictionary<string, BusinessBase> BusinessItems { get; } = new ConcurrentDictionary<string, BusinessBase>();

        public void Dispose()
        {
        }

        public bool UserIsAuthenticated
        {
            get
            {
                return IDAppUser.HasValue;
            }
        }

        public void SetAppUser(AppUser user)
        {
            // New user logon.
            HttpContext?.Session.Clear();

            HttpContext?.Session.SetInt32(CurrentUserIDSessionKey, (int)user[0]);

            IDAppUser = (int)user[0];

            BusinessProvider.StoreObject(user, UseAppUserNoDB ? "AppUserNoDB" : "AppUser");
        }

        private static object currentUserLock = new object();

        public AppUser GetAppUser()
        {
            if (IDAppUser > 0)
            {
                lock (currentUserLock)
                {
                    return (AppUser)BusinessProvider.RetreiveObject(this,
                        UseAppUserNoDB ? "AppUserNoDB" : "AppUser", IDAppUser.ToString());
                }
            }

            return null;
        }

        public void StoreAppUser(AppUser user, BusinessBaseProvider provider)
        {
            provider.StoreObject(user,
                UseAppUserNoDB ? "AppUserNoDB" : "AppUser");
        }

        public string GetLogin()
        {
            string login = HttpContext?.User?.Identity?.Name ?? "";

            return login;
        }

        public string GetLoginWithOutDomain()
        {
            string login = HttpContext?.User?.Identity?.Name ?? "";

            if (login.Contains("\\"))
            {
                login = login.Split('\\')[1];
            }
            if (login.Contains("@"))
            {
                login = login.Split('@')[0];
            }

            return login;
        }

    }
}
