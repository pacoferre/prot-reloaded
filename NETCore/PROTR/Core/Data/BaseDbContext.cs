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
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace PROTR.Core.Data
{
    public class ProtDbContext : DbContext
    {
        public ProtDbContext(DbContextOptions options) : base(options)
        {
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            return await Database.GetDbConnection().QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        public async Task<IEnumerable<T>> ParametrizedQueryAsync<T>(string sql, object parameters = null)
        {
            return await Database.GetDbConnection().QueryAsync<T>(sql, parameters);
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            return await Database.GetDbConnection().ExecuteAsync(sql, parameters);
        }

        public List<ColumnDefinition> GetDefinitions(string objectName)
        {
            List<ColumnDefinition> list = new List<ColumnDefinition>();

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetTypes().FirstOrDefault()?.Namespace?.Contains("Business") ?? false)
                .SelectMany(a => a.GetTypes())
                .Where(t => t.Name.EndsWith(objectName + "Model"))
                .FirstOrDefault();
            var entityType = Model.FindEntityType(type.ToString());
            var props = entityType.GetProperties().AsList();
            var keys = entityType.GetKeys().AsList();

            foreach(var oProp in type.GetProperties())
            {
                var prop = props.First(_ => _.Name == oProp.Name);
                var col = new ColumnDefinition
                {
                    ColumnName = prop.Name,
                    DataType = prop.ClrType,
                    IsNullable = prop.IsNullable,
                    IsComputed = prop.GetValueGeneratorFactory() != null,
                    IsPrimaryKey = prop.IsPrimaryKey(),
                    MaxLength = prop.GetMaxLength() ?? int.MaxValue
                };

                list.Add(col);
            }

            return list;
        }

        public async Task BeginTransactionAsync()
        {
            await Database.BeginTransactionAsync();
        }

        public void CommitTransaction()
        {
            Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Database.RollbackTransaction();
        }

        public async Task ReadBusinessObject(BusinessBase obj)
        {
            var set = GetType()
                .GetMethod("Set")
                .MakeGenericMethod(obj.ModelType)
                .Invoke(this, null);
            var keys = obj.Keys;
            var task = (Task) set
                .GetType()
                .GetMethod("FindAsync", new[] { typeof(object[]) })
                .Invoke(set, new[] { keys });

            await task.ConfigureAwait(false);

            var found = task.GetType().GetProperty("Result").GetValue(task);

            if (found != null)
            {
                obj.FromModelObject(found);
            }
        }

        public async Task StoreBusinessObject(BusinessBase obj)
        {
            var def = obj.Decorator;

            if (obj.IsNew)
            {
                if (def.PrimaryKeyIsOneGuid)
                {
                    obj[def.PrimaryKeys[0]] = Lib.GenerateComb();
                }

                if (def.PrimaryKeyIsOneInt || def.PrimaryKeyIsOneLong)
                {
                    var result = await (Task<object>) typeof(DapperEFCoreExts)
                        .GetMethod("InsertAsync")
                        .MakeGenericMethod(obj.ModelType)
                        .Invoke(this, new object[] { obj.ToModelObject,
                            true, obj.Decorator.PrimaryKeyFieldName });

                    if (result == null)
                    {
                        throw new Exception("Error inserting new " + obj.Description);
                    }
                }
                else
                {
                    var result = await (Task<object>) typeof(DapperEFCoreExts)
                        .GetMethod("InsertAsync")
                        .MakeGenericMethod(obj.ModelType)
                        .Invoke(this, new object[] { obj.ToModelObject,
                            false });

                    if ((int) result != 1)
                    {
                        throw new Exception("Error inserting new " + obj.Description);
                    }
                }
                obj.IsNew = false;
                obj.IsModified = false;
            }
            else if (obj.IsDeleting)
            {
                var result = await (Task<int>) typeof(DapperEFCoreExts)
                    .GetMethod("DeleteAsync")
                    .MakeGenericMethod(obj.ModelType)
                    .Invoke(this, new object[] { obj.ToModelObject });

                if (result != 1)
                {
                    throw new Exception("Error deleting " + obj.Description);
                }
            }
            else
            {
                var result = await (Task<int>) UpdateAsyncMethod
                    .Value
                    .MakeGenericMethod(obj.ModelType)
                    .Invoke(null, new object[] { this, obj.ToModelObject, Type.Missing });

                if (result != 1)
                {
                    throw new Exception("Error updating " + obj.Description);
                }
            }
        }

        private static Lazy<MethodInfo> UpdateAsyncMethod = new Lazy<MethodInfo>(() =>
        {
            var methodsInfo = typeof(DapperEFCoreExts).GetMethods()
                    .Where(x => x.Name == "UpdateAsync")
                    .Select(x => new { M = x, P = x.GetParameters() })
                    .Where(x => x.P.Length == 3 && x.P[0].ParameterType == typeof(DbContext) && x.P[1].ParameterType == typeof(object));
            return methodsInfo.FirstOrDefault()?.M;
        });

        public async Task ReadBusinessCollection(BusinessCollectionBase col)
        {
            string sql = col.SQLQuery;
            object param = col.SQLParameters;

            var result = await (Task<IEnumerable<object>>)GetType()
                .GetMethod("QueryAsync")
                .MakeGenericMethod(col.ActiveObject.ModelType)
                .Invoke(this, new object[] { sql, param });

            col.Clear();

            foreach (var item in result)
            {
                BusinessBase obj = col.CreateNew();

                obj.FromModelObject(item);
                obj.IsModified = false;
                obj.IsNew = false;

                col.Add(obj);
            }
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
