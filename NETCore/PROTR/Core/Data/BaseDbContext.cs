using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PROTR.Core.Security.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using PROTR.Core.Data.Dapper.Extensions;
using System.Linq;

namespace PROTR.Core.Data
{
    public class ProtDbContext : DbContext
    {
        private IDbConnection conn = null;
        private IDbTransaction trans = null;
        private bool inTransaction = false;

        public ProtDbContext(DbContextOptions options) : base(options)
        {
        }

        public T QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            return Database.GetDbConnection().QueryFirstOrDefault<T>(sql, parameters);
        }

        public IEnumerable<T> ParametrizedQuery<T>(string sql, object parameters = null)
        {
            return Database.GetDbConnection().Query<T>(sql, parameters);
        }

        public int Execute(string sql, object parameters = null)
        {
            return Database.GetDbConnection().Execute(sql, parameters);
        }

        public List<ColumnDefinition> GetDefinitions(string objectName)
        {
            List<ColumnDefinition> list = new List<ColumnDefinition>();

            var entityType = Model.FindEntityType(objectName);
            var props = entityType.GetProperties().AsList();
            var keys = entityType.GetKeys().AsList();

            props.ForEach(prop =>
            {
                var col = new ColumnDefinition();

                col.ColumnName = prop.Name;
                col.DataType = prop.ClrType;
                col.IsNullable = prop.IsNullable;
                col.IsComputed = prop.GetValueGeneratorFactory() != null;
                col.IsPrimaryKey = prop.IsPrimaryKey();
                col.MaxLength = prop.GetMaxLength() ?? int.MaxValue;

                list.Add(col);
            });

            return list;
        }

        public void BeginTransaction()
        {
            Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Database.RollbackTransaction();
        }

        public void ReadBusinessObject(BusinessBase obj)
        {
            var set = GetType()
                .GetMethod("Set")
                .MakeGenericMethod(obj.ModelType)
                .Invoke(this, null);
            var keys = obj.Keys;
            var found = set
                .GetType()
                .GetMethod("Find")
                .Invoke(set, keys);

            if (found != null)
            {
                obj.ContextProvider.Mapper.Map(found, obj, obj.ModelType, obj.GetType());
            }
        }

        public void StoreBusinessObject(BusinessBase obj)
        {
            var def = obj.Decorator;

            if (obj.IsNew)
            {
                if (def.primaryKeyIsOneGuid)
                {
                    obj[def.PrimaryKeys[0]] = Lib.GenerateComb();
                }

                if (def.primaryKeyIsOneInt || def.primaryKeyIsOneLong)
                {
                    var result = this.Insert<AppUserModel>(obj.ContextProvider.Mapper.Map(obj, obj.GetType(), obj.ModelType),
                        true, obj.Decorator.ListProperties[obj.Decorator.PrimaryKeys[0]].FieldName);

                    if (result == null)
                    {
                        throw new Exception("Error inserting new " + obj.Description);
                    }

                    obj.IsNew = false;

                }
                else
                {
                    var result = this.Insert<AppUserModel>(obj.ContextProvider.Mapper.Map(obj, obj.GetType(), obj.ModelType),
                        false);
                    obj.IsNew = false;
                }
            }
            else if (obj.IsDeleting)
            {
                int result = conn.Execute(def.DeleteQuery, def.GetPrimaryKeyParameters(obj), trans);

                if (result != 1)
                {
                    throw new Exception("Error deleting " + obj.Description);
                }
            }
            else
            {
                int result = conn.Execute(def.UpdateQuery, def.GetUpdateParameters(obj), trans);

                if (result != 1)
                {
                    throw new Exception("Error updating " + obj.Description);
                }
            }

        }

        public void ReadBusinessCollection(BusinessCollectionBase col)
        {

        }
    }

    public class BaseDbContext<T> : ProtDbContext where T : AppUserModel
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<T> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<T>(ConfigureAppUser);
        }

        protected virtual void ConfigureAppUser(EntityTypeBuilder<T> builder)
        {
            builder.ToTable("AppUser");

            builder.HasKey(user => user.IdAppUser);

            builder.Property(user => user.IdAppUser)
                .HasColumnName("idAppUser");

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
