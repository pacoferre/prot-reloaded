using Demo.Library.Business.Authors;
using Demo.Library.Business.Security;
using PROTR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business
{
    public class LibraryBusinessProvider : BusinessBaseProvider
    {
        public override void RegisterBusinessCreators()
        {
            base.RegisterBusinessCreators();

            ObjectToDBTable.Add("LibraryAppUser", "AppUser");

            creators["AppUser"] = (contextProvider) => new LibraryAppUser(contextProvider);

            decorators.Add("Author", () => new AuthorDecorator(this));
        }
    }
}
