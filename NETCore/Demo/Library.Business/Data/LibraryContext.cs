﻿using Demo.Library.Business.Authors.EF;
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

        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorNationality> AuthorNationalities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // PROTR tables
            base.OnModelCreating(builder);

            // Local tables
            builder.Entity<Author>(ConfigureAuthor);
            builder.Entity<AuthorNationality>(ConfigureAuthorNationality);
        }

        protected override void ConfigureAppUser(EntityTypeBuilder<LibraryAppUserModel> builder)
        {
            base.ConfigureAppUser(builder);

            // Rest of fields.
        }

        private void ConfigureAuthor(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Author");

            builder.HasKey(obj => obj.IdAuthor);

            builder.Property(obj => obj.IdAuthor)
                .HasColumnName("idAuthor");

            builder.Property(obj => obj.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(30);

            builder.Property(obj => obj.Surname)
                .HasColumnName("surname")
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(obj => obj.IdAuthorNationality)
                .HasColumnName("idAuthorNationality")
                .IsRequired(true)
                .HasMaxLength(50);

            builder
                .HasOne(one => one.AuthorNationality)
                .WithMany(many => many.Authors)
                .HasForeignKey(one => one.IdAuthorNationality);
        }

        private void ConfigureAuthorNationality(EntityTypeBuilder<AuthorNationality> builder)
        {
            builder.ToTable("AuthorNationality");

            builder.HasKey(obj => obj.IdAuthorNationality);

            builder.Property(obj => obj.IdAuthorNationality)
                .HasColumnName("idAuthorNationality");

            builder.Property(obj => obj.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(70);
        }
    }
}

