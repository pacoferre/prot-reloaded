using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PROTR.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Infrastructure.Data
{
    public class LibraryContext : BaseContext
    {
        public LibraryContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // PROTR tables
            base.OnModelCreating(builder);

            // Local tables
        }
    }
}
