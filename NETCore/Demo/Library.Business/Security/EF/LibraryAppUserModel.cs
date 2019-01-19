using AutoMapper;
using PROTR.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Security.EF
{
    public class LibraryAppUserModel : PROTR.Core.Security.EF.AppUserModel
    {
        public class LibraryAppUserProfile : Profile
        {
            public LibraryAppUserProfile()
            {
                CreateMap<LibraryAppUserModel, LibraryAppUser>();
                CreateMap<LibraryAppUser, LibraryAppUserModel>();
            }
        }
    }
}
