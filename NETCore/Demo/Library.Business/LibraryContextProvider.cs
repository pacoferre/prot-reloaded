using AutoMapper;
using Demo.Library.Business.Data;
using Microsoft.AspNetCore.Http;
using PROTR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business
{
    public class LibraryContextProvider : ContextProvider
    {
        public LibraryContextProvider(IHttpContextAccessor contextAccessor, IMapper mapper,
                LibraryBusinessProvider businessProvider, LibraryContext dbContext, DbDialect dbDialect)
            : base(contextAccessor, mapper, businessProvider, dbContext, dbDialect)
        {

        }
    }
}
