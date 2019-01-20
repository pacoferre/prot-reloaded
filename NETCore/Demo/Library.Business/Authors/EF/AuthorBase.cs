using AutoMapper;
using PROTR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Authors.EF
{
    public class AuthorBase : BusinessBase
    {
        public class AuthorProfile : Profile
        {
            public AuthorProfile()
            {
                CreateMap<AuthorModel, Author>();
                CreateMap<Author, AuthorModel>();
            }
        }

        public AuthorBase(ContextProvider contextProvider) : base(contextProvider)
        {
            ModelType = typeof(AuthorModel);
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
        public string Surname 
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

    }
}
