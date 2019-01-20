using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Authors.EF
{
    public class AuthorModel
    {
        public int IdAuthor { get; set; }
        public int IdAuthorNationality { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public AuthorNationalityModel AuthorNationality { get; set; }
    }
}
