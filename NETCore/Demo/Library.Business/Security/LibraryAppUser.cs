using Demo.Library.Business.Security.EF;
using PROTR.Core;
using PROTR.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Security
{
    public class LibraryAppUser : AppUser
    {
        public LibraryAppUser(ContextProvider contextProvider) : base(contextProvider)
        {
            ModelType = typeof(LibraryAppUserModel);
        }
    }
}
