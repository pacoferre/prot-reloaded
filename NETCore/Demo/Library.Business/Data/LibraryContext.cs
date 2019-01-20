using Demo.Library.Business.Authors.EF;
using Demo.Library.Business.Security.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PROTR.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Library.Business.Data
{
    public class LibraryContext : BaseDbContext<LibraryAppUserModel>
    {
        public LibraryContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AuthorModel> Authors { get; set; }
        public DbSet<AuthorNationalityModel> AuthorNationalities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // PROTR tables
            base.OnModelCreating(builder);

            // Local tables
            builder.Entity<AuthorModel>(ConfigureAuthor);
            builder.Entity<AuthorNationalityModel>(ConfigureAuthorNationality);
        }

        protected override void ConfigureAppUser(EntityTypeBuilder<LibraryAppUserModel> builder)
        {
            base.ConfigureAppUser(builder);

            // Rest of fields.
        }

        private void ConfigureAuthor(EntityTypeBuilder<AuthorModel> builder)
        {
            builder.ToTable("Author");

            builder.HasKey(obj => obj.IdAuthor);

            builder.Property(obj => obj.Name)
                .IsRequired(true)
                .HasMaxLength(30);

            builder.Property(obj => obj.Surname)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(obj => obj.IdAuthorNationality)
                .IsRequired(true)
                .HasMaxLength(50);

            builder
                .HasOne(one => one.AuthorNationality)
                .WithMany(many => many.Authors)
                .HasForeignKey(one => one.IdAuthorNationality);
        }

        private void ConfigureAuthorNationality(EntityTypeBuilder<AuthorNationalityModel> builder)
        {
            builder.ToTable("AuthorNationality");

            builder.HasKey(obj => obj.IdAuthorNationality);

            builder.Property(obj => obj.Name)
                .IsRequired(true)
                .HasMaxLength(70);
        }
    }
}

