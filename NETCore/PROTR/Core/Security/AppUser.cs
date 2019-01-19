using PROTR.Core.Security.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core.Security
{
    public class AppUserDecorator : BusinessBaseDecorator
    {
        public AppUserDecorator(BusinessBaseProvider provider) : base(provider)
        {
        }

        protected override void SetCustomProperties()
        {
            Properties["password"].NoChecking = true;

            Singular = "User";
            Plural = "Users";

            Properties["email"].IsOnlyOnNew = true;
            Properties["email"].Type = PropertyInputType.email;
            Properties["password"].Type = PropertyInputType.password;
        }
    }

    public partial class AppUser : BusinessBase
    {
        public AppUser(ContextProvider contextProvider) : base(contextProvider)
        {
            ModelType = typeof(AppUserModel);
        }

        public AppUser(BusinessBaseProvider provider, bool noDB) : base(provider, noDB)
        {
            ModelType = typeof(AppUserModel);
        }

        public int IdAppUser
        {
            get
            {
                return this[0].NoNullInt();
            }
            set
            {
                this[0] = value;
            }
        }

        public string Name
        {
            get
            {
                return this[1].ToString();
            }
            set
            {
                this[1] = value;
            }
        }

        public string Surname
        {
            get
            {
                return this[2].ToString();
            }
            set
            {
                this[2] = value;
            }
        }

        public bool Su
        {
            get
            {
                return this[3].NoNullBool();
            }
            set
            {
                this[3] = value;
            }
        }

        public string Email
        {
            get
            {
                return this[4].ToString();
            }
            set
            {
                this[4] = value;
            }
        }

        public string Password
        {
            get
            {
                return this[5].ToString();
            }
            set
            {
                this[5] = value;
            }
        }

        public bool Deactivated
        {
            get
            {
                return this[4].NoNullBool();
            }
            set
            {
                this[4] = value;
            }
        }

        public override string Description
        {
            get
            {
                return Name + " " + Surname;
            }
        }

        protected string newPassword = null;

        public override bool Validate()
        {
            bool isValid = base.Validate();

            if (isValid)
            {
                if (IsNew && newPassword.NoNullString() == "")
                {
                    LastErrorMessage = "Password must be set";
                    isValid = false;
                }
            }

            return isValid;
        }

        public override object this[string property]
        {
            get
            {
                if (property == "password")
                {
                    return newPassword??"";
                }
                if (property == "checkpassword")
                {
                    return base["password"];
                }
                return base[property];
            }
            set
            {
                if (property == "password")
                {
                    if (value.NoNullString() != "")
                    {
                        newPassword = value.ToString();
                        base[property] = "NWRE";
                    }
                }
                else
                {
                    base[property] = value;
                }
            }
        }

        protected override void AfterStoreToDB(bool wasNew, bool wasModified, bool wasDeleting)
        {
            if (newPassword != null)
            {
                string enc = PasswordDerivedString(this["idAppUser"].NoNullString(), newPassword.ToString());

                contextProvider.DbContext.Execute("update AppUser set password = @password Where idAppUser = @id", new { password = enc, id = this["idAppUser"].NoNullString() });

                newPassword = null;

                ReadFromDB();
            }
        }

        public string ShortDateFormat
        {
            get
            {
                return "dd/MM/yyyy"; // Pending translation.
            }
        }

        public IFormatProvider Culture
        {
            get
            {
                return null;
            }
        }
    }
}
