using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Authors.EF
{
    public class AuthorNationalityModel
    {
        public int IdAuthorNationality { get; set; }
        public string Name { get; set; }

        public List<AuthorModel> Authors { get; set; }
    }
}
