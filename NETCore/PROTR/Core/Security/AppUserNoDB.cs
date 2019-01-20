using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PROTR.Core.Security
{
    public class AppUserNoDBDecorator : PROTR.Core.Security.AppUserDecorator
    {
        public AppUserNoDBDecorator(BusinessBaseProvider provider) : base(provider)
        {

        }

        public override void SetProperties(ContextProvider contextProvider, string objectName, int dbNumber)
        {
            this.objectName = objectName;
            this.tableName = provider.GetDBTableFor(objectName);

            SetCustomProperties();
        }

        protected override void SetCustomProperties()
        {
            //idAppUser   int Unchecked
            //email nvarchar(200)   Unchecked
            //password    nvarchar(100)   Unchecked
            //name    nvarchar(50)    Unchecked
            //surname nvarchar(50)    Unchecked
            //su  bit Unchecked
            //deactivated bit Unchecked
            PropertyDefinition temp =
                (Properties["idAppUser"]
                    = new PropertyDefinition("idAppUser", "ID", typeof(Int32), 
                        PropertyInputType.number, true));
            temp.DataType = typeof(int);
            temp.DefaultValue = 1;

            temp = (Properties["email"]
                    = new PropertyDefinition("email", "Email", typeof(string), PropertyInputType.text));
            temp.MaxLength = 200;
            temp.DataType = typeof(string);
            temp.DefaultValue = "";

            temp = (Properties["password"]
                    = new PropertyDefinition("password", "Password", typeof(string), PropertyInputType.password));
            temp.MaxLength = 100;
            temp.DataType = typeof(string);
            temp.DefaultValue = "";

            temp = (Properties["name"]
                    = new PropertyDefinition("name", "Name", typeof(string), PropertyInputType.text));
            temp.MaxLength = 50;
            temp.DataType = typeof(string);
            temp.DefaultValue = "";

            temp = (Properties["surname"]
                    = new PropertyDefinition("surname", "Surname", typeof(string), PropertyInputType.text));
            temp.MaxLength = 50;
            temp.DataType = typeof(string);
            temp.DefaultValue = "";

            temp = (Properties["su"]
                    = new PropertyDefinition("su", "SU", typeof(bool), PropertyInputType.checkbox));
            temp.DataType = typeof(bool);
            temp.DefaultValue = false;

            temp = (Properties["deactivated"]
                    = new PropertyDefinition("deactivated", "Deactivated", typeof(bool), PropertyInputType.checkbox));
            temp.DataType = typeof(bool);
            temp.DefaultValue = false;

            base.SetCustomProperties();

            PostSetCustomProperties();
        }
    }

    public class AppUserNoDB : PROTR.Core.Security.AppUser
    {
        public AppUserNoDB(BusinessBaseProvider provider) : base(provider, true)
        {
        }

        public override Task StoreToDB()
        {
            // Nothing to do.
            //base.StoreToDB();
            return Task.CompletedTask;
        }

        public static async Task LoginWindows(ContextProvider contextProvider, int idAppUser = 1)
        {
            AppUser usu = null;

            UseAppUserNoDB = true;

            usu = (AppUser)contextProvider.BusinessProvider.CreateObject(contextProvider, "AppUserNoDB");

            await usu.SetNew();

            usu["idAppUser"] = idAppUser;

            usu["name"] = "User";
            usu["surname"] = contextProvider.GetLogin();

            usu.IsNew = false;

            await contextProvider.SetAppUser(usu);
        }
    }
}
