using AutoMapper;
using Demo.Library.Business.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PROTR.Core;
using PROTR.Core.Security.EF;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Library.Business
{
    public class LibraryContextProvider : ContextProvider
    {
        public LibraryContextProvider(IHttpContextAccessor contextAccessor, IMapper mapper,
                LibraryBusinessProvider businessProvider, LibraryContext dbContext, DbDialect dbDialect)
            : base(contextAccessor, mapper, businessProvider, dbContext, dbDialect)
        {

        }

        public override async Task<AppUserModel> QueryUser(Expression<Func<AppUserModel, bool>> predicate)
        {
            return await ((LibraryContext) DbContext).Users.FirstOrDefaultAsync(predicate);
        }
    }
}
