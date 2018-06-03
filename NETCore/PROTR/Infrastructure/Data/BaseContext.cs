using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PROTR.Core.Security.EF;
using System;
using System.Collections.Generic;
using System.Text;

namespace PROTR.Infrastructure.Data
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AppUser>(ConfigureAppUser);
        }

        private void ConfigureAppUser(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUser");

            builder.HasKey(user => user.IdAppUser);

            builder.Property(user => user.IdAppUser)
                .HasColumnName("idAppUser")
                .ForSqlServerUseSequenceHiLo("appuser_hilo")
                .IsRequired();

            builder.Property(user => user.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(30);

            builder.Property(user => user.Surname)
                .HasColumnName("surname")
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(user => user.Su)
                .HasColumnName("su")
                .IsRequired(true)
                .HasDefaultValue(false);

            builder.Property(user => user.Email)
                .HasColumnName("email")
                .IsRequired(true)
                .HasMaxLength(200);

            builder.Property(user => user.Password)
                .HasColumnName("password")
                .IsRequired(true)
                .HasMaxLength(400);

            builder.Property(user => user.Deactivated)
                .HasColumnName("deactivated")
                .IsRequired(true)
                .HasDefaultValue(false);
        }
    }
}
