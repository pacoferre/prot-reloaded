using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace PROTR.Core.Security
{
    public partial class AppUser
    {
        internal static bool UseAppUserNoDB = false;

        public static byte[] SALT;

        public static bool LoginWindowsWithoutDomain(ContextProvider contextProvider)
        {
            string login = contextProvider.GetLoginWithOutDomain();
            int idAppUser = contextProvider.DbContext.QueryFirstOrDefault<int>(@"SELECT idAppUser FROM AppUser
WHERE (email = @email) AND (deactivated = 0)", new { email = login });
            AppUser usu = (AppUser)contextProvider.BusinessProvider.CreateObject(contextProvider, "AppUser");
            bool valid = false;

            if (idAppUser > 0)
            {
                valid = true;

                usu.ReadFromDB(idAppUser);
                contextProvider.SetAppUser(usu);

                usu.PostLogin(login, "", usu, valid);
            }
            else
            {
                usu.PostLogin(login, "", null, valid);
            }

            return valid;
        }

        public static bool Login(string email, string password, ContextProvider contextProvider)
        {
            AppUser theUser = (AppUser)contextProvider.BusinessProvider.CreateObject(contextProvider, "AppUser");

            return theUser.LoginInternal(email, password);
        }

        protected bool LoginInternal(string email, string password)
        {
            dynamic userData = contextProvider
                .DbContext
                .QueryFirstOrDefault<dynamic>("Select idAppUser, password From AppUser Where email = @Email", new { Email = email });
            AppUser usu = null;
            bool valid = false;

            if (userData != null)
            {
                string enc = PasswordDerivedString(((int)userData.idAppUser).NoNullString(), password);

                if (enc == userData.password)
                {
                    usu = (AppUser)businessProvider.CreateObject(contextProvider, "AppUser");

                    usu.ReadFromDB((int)userData.idAppUser);
                }

                // Hack to 
                if (usu == null)
                {
                    if (userData.password == password)
                    {
                        usu = (AppUser)businessProvider.CreateObject(contextProvider, "AppUser");

                        usu.ReadFromDB((int)userData.idAppUser);

                        usu["password"] = password;

                        usu.StoreToDB();
                    }
                    else
                    {
                        usu = null;
                    }
                }

                if (usu != null && usu["deactivated"].NoNullBool())
                {
                    usu = null;
                }
            }

            if (usu != null)
            {
                contextProvider.SetAppUser(usu);
                valid = true;
            }

            PostLogin(email, password, usu, valid);

            return valid;
        }

        public virtual void PostLogin(string email, string password, AppUser user, bool valid)
        {

        }

        public virtual bool ChangePassword(string actual, string newPassword)
        {
            ReadFromDB();

            string enc = PasswordDerivedString(this[0].NoNullString(), actual);

            if (enc == this["checkpassword"].NoNullString() && CheckStrength(newPassword))
            {
                this["password"] = newPassword;

                StoreToDB();
            }
            else
            {
                return false;
            }

            return true;
        }

        public virtual bool ChangePasswordBlind(string newPassword)
        {
            ReadFromDB();

            if (CheckStrength(newPassword))
            {
                this["password"] = newPassword;

                StoreToDB();
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
