using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Authors.EF
{
    public class AuthorNationality
    {
        public int IdAuthorNationality { get; set; }
        public string Name { get; set; }

        public List<Author> Authors { get; set; }
    }
}
