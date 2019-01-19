using System;
using System.Collections.Generic;
using System.Text;
using PROTR.Core;

namespace Demo.Library.Business.Authors
{
    public class AuthorDecorator : BusinessBaseDecorator
    {
        public AuthorDecorator(BusinessBaseProvider provider) : base(provider)
        {

        }

        protected override void SetCustomProperties()
        {
            base.SetCustomProperties();

            Singular = "Author";
            Plural = "Authors";

            Properties["idAuthorNationality"].Type = PropertyInputType.select;
        }

        public override FilterBase GetFilter(ContextProvider contextProvider, string filterName)
        {
            return new AuthorFilter(contextProvider, this);
        }
    }
}
