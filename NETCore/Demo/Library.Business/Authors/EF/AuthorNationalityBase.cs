using AutoMapper;
using PROTR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Authors.EF
{
    public class AuthorNationalityBase : BusinessBase
    {
        public class AuthorNationalityProfile : Profile
        {
            public AuthorNationalityProfile()
            {
                CreateMap<AuthorNationalityModel, AuthorNationality>();
                CreateMap<AuthorNationality, AuthorNationalityModel>();
            }
        }

        public AuthorNationalityBase(ContextProvider contextProvider) : base(contextProvider)
        {
            ModelType = typeof(AuthorNationalityModel);
        }

        public string Name 
        {
            get
            {
                return this[0].ToString();
            }
            set
            {
                this[0] = value;
            }
        }

    }
}
