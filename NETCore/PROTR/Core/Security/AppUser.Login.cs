using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PROTR.Core.Security.EF;

namespace PROTR.Core.Security
{
    public partial class AppUser
    {
        internal static bool UseAppUserNoDB = false;

        public static byte[] SALT;

        public static async Task<bool> LoginWindowsWithoutDomain(ContextProvider contextProvider)
        {
            string login = contextProvider.GetLoginWithOutDomain();
            int idAppUser = (await contextProvider
                .QueryUser(_ => _.Email == login && !_.Deactivated))?.IdAppUser ?? 0;
            AppUser usu = (AppUser)contextProvider.BusinessProvider.CreateObject(contextProvider, "AppUser");
            bool valid = false;

            if (idAppUser > 0)
            {
                valid = true;

                await usu.ReadFromDB(idAppUser);
                await contextProvider.SetAppUser(usu);

                await usu.PostLogin(login, "", usu, valid);
            }
            else
            {
                await usu.PostLogin(login, "", null, valid);
            }

            return valid;
        }

        public static async Task<bool> Login(ContextProvider contextProvider, string email, string password)
        {
            AppUser theUser = (AppUser)contextProvider.BusinessProvider.CreateObject(contextProvider, "AppUser");

            return await theUser.LoginInternal(email, password);
        }

        protected async Task<bool> LoginInternal(string email, string password)
        {
            var userData = (await contextProvider.QueryUser(_ => _.Email == email && !_.Deactivated));
            AppUser usu = null;
            bool valid = false;

            if (userData != null)
            {
                string enc = PasswordDerivedString(userData.IdAppUser.NoNullString(), password);

                if (enc == userData.Password)
                {
                    usu = (AppUser)businessProvider.CreateObject(contextProvider, "AppUser");

                    await usu.ReadFromDB(userData.IdAppUser);
                }

                // Hack to 
                if (usu == null)
                {
                    if (userData.Password == password)
                    {
                        usu = (AppUser)businessProvider.CreateObject(contextProvider, "AppUser");

                        await usu.ReadFromDB(userData.IdAppUser);

                        usu.Password = password;

                        await usu.StoreToDB();
                    }
                    else
                    {
                        usu = null;
                    }
                }

                if (usu != null && usu.Deactivated.NoNullBool())
                {
                    usu = null;
                }
            }

            if (usu != null)
            {
                await contextProvider.SetAppUser(usu);
                valid = true;
            }

            await PostLogin(email, password, usu, valid);

            return valid;
        }

        public virtual Task PostLogin(string email, string password, AppUser user, bool valid)
        {
            return Task.CompletedTask;
        }

        public virtual async Task<bool> ChangePassword(string actual, string newPassword)
        {
            await ReadFromDB();

            string enc = PasswordDerivedString(this[0].NoNullString(), actual);

            if (enc == CheckPassword && CheckStrength(newPassword))
            {
                Password = newPassword;

                await StoreToDB();
            }
            else
            {
                return false;
            }

            return true;
        }

        public virtual async Task<bool> ChangePasswordBlind(string newPassword)
        {
            await ReadFromDB();

            if (CheckStrength(newPassword))
            {
                Password = newPassword;

                await StoreToDB();
            }
            else
            {
                return false;
            }

            return true;
        }

        protected virtual bool CheckStrength(string password)
        {
            return true;
        }

        public static string PasswordDerivedString(string idAppUser, string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: idAppUser + "_fg784gyb4yskj_" + password,
                salt: SALT,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 200,
                numBytesRequested: 256 / 8));
        }
    }
}
