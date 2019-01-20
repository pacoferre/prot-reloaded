using System;
using System.Collections.Generic;
using System.Text;
using Demo.Library.Business.Authors.EF;
using PROTR.Core;

namespace Demo.Library.Business.Authors
{
    public class Author : AuthorBase
    {
        public Author(ContextProvider contextProvider) : base(contextProvider)
        {
        }
    }
}
