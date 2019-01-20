using Dapper;
using Microsoft.EntityFrameworkCore;
using PROTR.Core.Data.Dapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PROTR.Core.Data.Dapper.Extensions
{
    public static partial class DapperEFCoreExts
    {
        /// <summary>
        ///  Inserts a row into the DB asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to insert.</typeparam>
        /// <param name="dbSet">The <see cref="DbSet{T}"/>.</param>
        /// <param name="entity">The entity instance.</param>
        /// <param name="returnIdentity">Retrieve identity.</param>
        /// <param name="propertyKey">The name of the identity property.</param>
        /// <returns>The Id of the inserted row or null.</returns>
        public static Task<object> InsertAsync<TEntity>(this DbSet<TEntity> dbSet, object entity, bool returnIdentity = true, string propertyKey = "Id")
            where TEntity : class, new() =>
            (dbSet ?? throw new ArgumentNullException(nameof(dbSet)))
                .GetDbContext().InsertAsync<TEntity>(entity, returnIdentity, propertyKey);

        /// <summary>
        ///  Inserts a row into the DB asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to insert.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entity">The entity instance.</param>
        /// <param name="returnIdentity">Retrieve identity.</param>
        /// <param name="propertyKey">The name of the identity property.</param>
        /// <returns>The Id of the inserted row or null or 1 (sucess on returnIdentity = false).</returns>
        public static async Task<object> InsertAsync<TEntity>(this DbContext dbContext, object entity, bool returnIdentity = true, string propertyKey = "Id")
            where TEntity : class, new()
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            var (sql, sqlParams) = await Task.Run(() =>
                dbContext.CompileInsertOper<TEntity>(entity)).ConfigureAwait(false);
            var (con, trans, timeout) = dbContext.GetDatabaseConfig();

            object result;

            if (returnIdentity)
            {
                result = await con.ExecuteScalarAsync<object>(sql, sqlParams, transaction: trans, commandTimeout: timeout);
                if (result != null)
                {
                    var propInfo = entity.GetType().GetProperty(propertyKey);

                    if (propInfo != null && propInfo.CanWrite)
                        propInfo.SetValue(entity, Convert.ChangeType(result, propInfo.PropertyType));
                }
            }
            else
            {
                result = await con.ExecuteAsync(sql, sqlParams, transaction: trans, commandTimeout: timeout);
            }


            return result;
        }

        /// <summary>
        ///  Inserts multiple rows (in a single trip) asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to insert.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entities">The entity instances.</param>
        /// <param name="batchSize">Number of rows in each batch. </param>
        /// <param name="cancelToken">A cancellation token that should be used to cancel the work.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> InsertAsync<TEntity>(this DbContext dbContext, IEnumerable<object> entities, int batchSize = 2000, CancellationToken cancelToken = default)
            where TEntity : class, new()
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            int res = 0;

            foreach (var (batch, idx) in entities.GetPartitionsBySize(batchSize).Select((x, idx) => (x.AsEnumerable(), idx)))
            {
                res += await dbContext.InsertInternAsync<TEntity>(batch, idx, cancelToken).ConfigureAwait(false);
            }

            return res;
        }

        private static async Task<int> InsertInternAsync<TEntity>(this DbContext dbContext, IEnumerable<object> entities, int batchIdx, CancellationToken cancelToken = default)
            where TEntity : class, new()
        {
            cancelToken.ThrowIfCancellationRequested();

            var (sql, sqlParams) = await Task.Run(() =>
                dbContext.CompileInsertOper<TEntity>(entities), cancelToken).ConfigureAwait(false);
            var (con, trans, timeout) = dbContext.GetDatabaseConfig();
            var result = await con.ExecuteAsync(sql, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Asynchronously updates entities that match the given predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to update.</typeparam>
        /// <param name="dbSet">The <see cref="DbSet{T}"/>.</param>
        /// <param name="entity">The entity instance.</param>
        /// <param name="predicate">The predicate expression for the condition in WHERE clause.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> UpdateAsync<TEntity>(this DbSet<TEntity> dbSet, object entity, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, new() =>
            (dbSet ?? throw new ArgumentNullException(nameof(dbSet)))
                .GetDbContext().UpdateAsync(entity, predicate);

        /// <summary>
        /// Asynchronously updates entities that match the given predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to update.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entity">The entity instance.</param>
        /// <param name="predicate">The predicate expression for the condition in WHERE clause.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> UpdateAsync<TEntity>(this DbContext dbContext, object entity, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, new()
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var (sql, sqlParams) = await Task.Run(() =>
            {
                var (sb, parameters) = dbContext.CompileUpdateOper<TEntity>(entity);

                if (predicate != null)
                {
                    var whereSql = GenerateWhereSql(dbContext, parameters, predicate);
                    AppendOrReplaceWhereClause(dbContext, sb, whereSql, "UPDATE ");
                }

                return (sb.ToString(), parameters);
            }).ConfigureAwait(false);

            var (con, trans, timeout) = dbContext.GetDatabaseConfig();

            var result = await con.ExecuteAsync(sql, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        ///  Updates multiple rows (in a single trip) asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to insert.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entities">The entity instances.</param>
        /// <param name="batchSize">Number of rows in each batch. </param>
        /// <param name="cancelToken">A cancellation token that should be used to cancel the work.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> UpdateAsync<TEntity>(this DbContext dbContext, IEnumerable<object> entities, int batchSize = 2000, CancellationToken cancelToken = default)
            where TEntity : class, new()
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            int res = 0;

            //can't parallelize within a given dbContext
            foreach (var (batch, idx) in entities.GetPartitionsBySize(batchSize).Select((x, idx) => (x.AsEnumerable(), idx)))
            {
                res += await dbContext.UpdateInternAsync<TEntity>(batch, idx, cancelToken).ConfigureAwait(false);
            }

            return res;
        }

        private static async Task<int> UpdateInternAsync<TEntity>(this DbContext dbContext, IEnumerable<object> entities, int batchIdx, CancellationToken cancelToken = default)
            where TEntity : class, new()
        {
            cancelToken.ThrowIfCancellationRequested();

            var (sql, sqlParams) = await Task.Run(() =>
                dbContext.CompileUpdateOper<TEntity>(entities), cancelToken).ConfigureAwait(false);
            var (con, trans, timeout) = dbContext.GetDatabaseConfig();
            var result = await con.ExecuteAsync(sql, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        ///  Asynchronously deletes entities that match the given predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to delete.</typeparam>
        /// <param name="dbSet">The <see cref="DbSet{T}"/>.</param>
        /// <param name="predicate">The predicate expression for the condition in WHERE clause.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> DeleteAsync<TEntity>(this DbSet<TEntity> dbSet, object entity, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, new() =>
            (dbSet ?? throw new ArgumentNullException(nameof(dbSet)))
                 .GetDbContext().DeleteAsync(entity, predicate);

        /// <summary>
        ///  Asynchronously deletes entities that match the given predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to delete.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="predicate">The predicate expression for the condition in WHERE clause.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> DeleteAsync<TEntity>(this DbContext dbContext, object entity, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, new()
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            var (sql, sqlParams) = await Task.Run(() =>
            {
                var (sb, parameters) = dbContext.CompileDeleteOper<TEntity>(entity);

                if (predicate != null)
                {
                    var whereSql = GenerateWhereSql(dbContext, parameters, predicate);
                    AppendOrReplaceWhereClause(dbContext, sb, whereSql, "DELETE ");
                }

                return (sb.ToString(), parameters);
            }).ConfigureAwait(false);

            var (con, trans, timeout) = dbContext.GetDatabaseConfig();
            var result = await con.ExecuteAsync(sql, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(true);

            return result;
        }

        /// <summary>
        /// Asynchronously deletes all entities
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to delete.</typeparam>
        /// <param name="dbSet">The <see cref="DbSet{T}"/>.</param>
        /// <returns></returns>
        public static Task<int> DeleteAllAsync<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class, new() =>
            (dbSet ?? throw new ArgumentNullException(nameof(dbSet)))
                .GetDbContext().DeleteAllAsync<TEntity>();

        /// <summary>
        /// Asynchronously deletes all entities
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to delete.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <returns></returns>
        public static async Task<int> DeleteAllAsync<TEntity>(this DbContext dbContext)
            where TEntity : class, new()
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            var (sql, sqlParams) = await Task.Run(() =>
            {
                var (sb, parameters) = dbContext.CompileDeleteOper<TEntity>();

                RemoveWhereClause(dbContext, sb, "DELETE ");

                return (sb.ToString(), parameters);
            }).ConfigureAwait(false);

            var (con, trans, timeout) = dbContext.GetDatabaseConfig();
            var result = await con.ExecuteAsync(sql, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Asynchronously executes a Query that returns TResult type Objects
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="query">An <see cref="IQueryable{T}"/> to use as the base of the raw SQL query.</param>
        /// <returns>Returns a collection of TResult type Objects.</returns>
        public static Task<IEnumerable<TResult>> QueryAsync<TResult>
            (this DbContext dbContext, IQueryable<TResult> query) =>
            QueryInternAsync<object, object, object, object, object, object, object, TResult>(dbContext, query, null);

        /// <summary>
        /// Asynchronously executes a Query that returns multiple entity types per row
        /// </summary>
        /// <typeparam name="TEntity1">The first type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity2">The second type of entity in the recordset.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="query">An <see cref="IQueryable{T}"/> to use as the base of the raw SQL query.</param>
        /// <param name="selector">A transform function to create a result value from the entities.</param>
        /// <returns>Returns a collection of TResult type Objects.</returns>
        public static Task<IEnumerable<TResult>> QueryAsync<TEntity1, TEntity2, TResult>
            (this DbContext dbContext, IQueryable<TResult> query
                , Func<TEntity1, TEntity2, TResult> selector) =>
            QueryInternAsync<TEntity1, TEntity2, object, object, object, object, object, TResult>(dbContext, query, null);

        /// <summary>
        /// Asynchronously executes a Query that returns multiple entity types per row
        /// </summary>
        /// <typeparam name="TEntity1">The first type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity2">The second type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity3">The third type of entity in the recordset.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="query">An <see cref="IQueryable{T}"/> to use as the base of the raw SQL query.</param>
        /// <param name="selector">A transform function to create a result value from the entities.</param>
        /// <returns>Returns a collection of TResult type Objects.</returns>
        public static Task<IEnumerable<TResult>> QueryAsync<TEntity1, TEntity2, TEntity3, TResult>
            (this DbContext dbContext, IQueryable<TResult> query
                , Func<TEntity1, TEntity2, TEntity3, TResult> selector) =>
            QueryInternAsync<TEntity1, TEntity2, TEntity3, object, object, object, object, TResult>(dbContext, query, null);

        /// <summary>
        /// Asynchronously executes a Query that returns multiple entity types per row
        /// </summary>
        /// <typeparam name="TEntity1">The first type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity2">The second type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity3">The third type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity4">The fourth type of entity in the recordset.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="query">An <see cref="IQueryable{T}"/> to use as the base of the raw SQL query.</param>
        /// <param name="selector">A transform function to create a result value from the entities.</param>
        /// <returns>Returns a collection of TResult type Objects.</returns>
        public static Task<IEnumerable<TResult>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TResult>
            (this DbContext dbContext, IQueryable<TResult> query
                , Func<TEntity1, TEntity2, TEntity3, TEntity4, TResult> selector) =>
            QueryInternAsync<TEntity1, TEntity2, TEntity3, TEntity4, object, object, object, TResult>(dbContext, query, null);

        /// <summary>
        /// Asynchronously executes a Query that returns multiple entity types per row
        /// </summary>
        /// <typeparam name="TEntity1">The first type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity2">The second type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity3">The third type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity4">The fourth type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity5">The fifth type of entity in the recordset.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="query">An <see cref="IQueryable{T}"/> to use as the base of the raw SQL query.</param>
        /// <param name="selector">A transform function to create a result value from the entities.</param>
        /// <returns>Returns a collection of TResult type Objects.</returns>
        public static Task<IEnumerable<TResult>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TResult>
            (this DbContext dbContext, IQueryable<TResult> query
            , Func<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TResult> selector) =>
            QueryInternAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, object, object, TResult>(dbContext, query, null);

        /// <summary>
        /// Asynchronously executes a Query that returns multiple entity types per row
        /// </summary>
        /// <typeparam name="TEntity1">The first type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity2">The second type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity3">The third type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity4">The fourth type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity5">The fifth type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity6">The sixth type of entity in the recordset.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="query">An <see cref="IQueryable{T}"/> to use as the base of the raw SQL query.</param>
        /// <param name="selector">A transform function to create a result value from the entities.</param>
        /// <returns>Returns a collection of TResult type Objects.</returns>
        public static Task<IEnumerable<TResult>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TResult>
            (this DbContext dbContext, IQueryable<TResult> query
                , Func<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TResult> selector) =>
            QueryInternAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, object, TResult>(dbContext, query, null);

        /// <summary>
        /// Asynchronously executes a Query that returns multiple entity types per row
        /// </summary>
        /// <typeparam name="TEntity1">The first type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity2">The second type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity3">The third type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity4">The fourth type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity5">The fifth type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity6">The sixth type of entity in the recordset.</typeparam>
        /// <typeparam name="TEntity7">The seventh type of entity in the recordset.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="query">An <see cref="IQueryable{T}"/> to use as the base of the raw SQL query.</param>
        /// <param name="selector">A transform function to create a result value from the entities.</param>
        /// <returns>Returns a collection of TResult type Objects.</returns>
        public static Task<IEnumerable<TResult>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TResult>
            (this DbContext dbContext, IQueryable<TResult> query
                , Func<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TResult> selector) =>
            QueryInternAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TResult>(dbContext, query, null);

        private static async Task<IEnumerable<TResult>> QueryInternAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TResult>
            (DbContext dbContext, IQueryable<TResult> query, Delegate del)
        {
            _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _ = query ?? throw new ArgumentNullException(nameof(query));

            var (sql, sqlParams) = await Task.Run(() =>
            {
                var (selectExp, expParams) = dbContext.Compile(query.Expression);
                return (selectExp.ToString(), expParams);
            }).ConfigureAwait(false);

            var (con, trans, timeout) = dbContext.GetDatabaseConfig();

            IEnumerable<TResult> result = null;

            switch (del)
            {
                case null:
                    result = await con.QueryAsync<TResult>(sql, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
                    break;
                case Func<TEntity1, TEntity2, TResult> selector:
                    result = await con.QueryAsync(sql, selector, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
                    break;
                case Func<TEntity1, TEntity2, TEntity3, TResult> selector:
                    result = await con.QueryAsync(sql, selector, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
                    break;
                case Func<TEntity1, TEntity2, TEntity3, TEntity4, TResult> selector:
                    result = await con.QueryAsync(sql, selector, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
                    break;
                case Func<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TResult> selector:
                    result = await con.QueryAsync(sql, selector, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
                    break;
                case Func<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TResult> selector:
                    result = await con.QueryAsync(sql, selector, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
                    break;
                case Func<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TResult> selector:
                    result = await con.QueryAsync(sql, selector, sqlParams, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentException("Invalid Type", nameof(del));
            }

            return result;
        }

        private static async Task<IEnumerable<TResult>> QueryAsync<TResult>(DbContext dbContext, string sql, object param)
        {
            var (con, trans, timeout) = dbContext.GetDatabaseConfig();

            return await con.QueryAsync<TResult>(sql, param, transaction: trans, commandTimeout: timeout).ConfigureAwait(false);
        }
    }
}
