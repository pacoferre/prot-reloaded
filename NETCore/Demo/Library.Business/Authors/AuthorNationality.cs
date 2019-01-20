using System;
using System.Collections.Generic;
using System.Text;
using Demo.Library.Business.Authors.EF;
using PROTR.Core;

namespace Demo.Library.Business.Authors
{
    public class AuthorNationality : AuthorNationalityBase
    {
        public AuthorNationality(ContextProvider contextProvider) : base(contextProvider)
        {
        }
    }
}
