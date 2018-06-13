using Demo.Library.Business.Authors;
using PROTR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business
{
    public class LibraryProvider : BusinessBaseProvider
    {
        public override void RegisterBusinessCreators()
        {
            base.RegisterBusinessCreators();

            //creators.Add("Customer", () => new Customer.Customer());

            decorators.Add("Author", () => new AuthorDecorator());
        }
    }
}
