using PROTR.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper
{
    public static partial class SqlMapper
    {
        public static Task ReadBusinessObjectAsync(
                    this IDbConnection cnn, BusinessBase obj, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null
                )
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default(CancellationToken));
            return ReadBusinessObjectAsyncImpl(cnn, Row.Single, command, obj);
        }
        public static Task ReadBusinessCollectionAsync(
                    this IDbConnection cnn, BusinessCollectionBase col, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null
                )
        {
            string sql = col.SQLQuery;
            object param = col.SQLParameters;
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default(CancellationToken));

            return ReadBusinessCollectionAsyncImpl(cnn, command, col);
        }

        private static async Task ReadBusinessObjectAsyncImpl(IDbConnection cnn, Row row, CommandDefinition command, BusinessBase obj)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, obj.GetType(), param?.GetType(), null);
            var info = GetCacheInfo(identity, param, command.AddToCache);
            var cancel = command.CancellationToken;
            bool wasClosed = cnn.State == ConnectionState.Closed;

            using (var cmd = (DbCommand)command.SetupCommand(cnn, info.ParamReader))
            {
                DbDataReader reader = null;

                try
                {
                    if (wasClosed) await ((DbConnection)cnn).OpenAsync(cancel).ConfigureAwait(false);
                    reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, (row & Row.Single) != 0
                    ? CommandBehavior.SequentialAccess | CommandBehavior.SingleResult // need to allow multiple rows, to check fail condition
                    : CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow, cancel).ConfigureAwait(false);

                    int fieldCount = reader.FieldCount;
                    if (await reader.ReadAsync(cancel).ConfigureAwait(false) && reader.FieldCount != 0)
                    {
                        // with the CloseConnection flag, so the reader will deal with the connection; we
                        // still need something in the "finally" to ensure that broken SQL still results
                        // in the connection closing itself
                        if (command.AddToCache) SetQueryCache(identity, info);

                        //Store data into BusinessObject
                        for (int i = 0; i < fieldCount; ++i)
                        {
                            object val = reader.GetValue(i);

                            obj[i] = val is DBNull ? null : val;
                        }

                        if ((row & Row.Single) != 0 && await reader.ReadAsync(cancel).ConfigureAwait(false)) ThrowMultipleRows(row);
                        while (await reader.ReadAsync(cancel).ConfigureAwait(false)) { }
                    }
                    else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
                    {
                        ThrowZeroRows(row);
                    }
                    while (await reader.NextResultAsync(cancel).ConfigureAwait(false)) { }

                    command.OnCompleted();
                }
                finally
                {
                    using (reader) { } // dispose if non-null
                    if (wasClosed) cnn.Close();
                }
            }
        }

        private static async Task ReadBusinessCollectionAsyncImpl(IDbConnection cnn, CommandDefinition command, BusinessCollectionBase col)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, col.GetType(), param?.GetType(), null);
            var info = GetCacheInfo(identity, param, command.AddToCache);

            bool wasClosed = cnn.State == ConnectionState.Closed;
            var cancel = command.CancellationToken;

            using (var cmd = (DbCommand)command.SetupCommand(cnn, info.ParamReader))
            {
                DbDataReader reader = null;
                try
                {
                    if (wasClosed) await ((DbConnection)cnn).OpenAsync(cancel).ConfigureAwait(false);
                    reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, cancel).ConfigureAwait(false);
                    if (command.AddToCache) SetQueryCache(identity, info);

                    int fieldCount = reader.FieldCount;

                    col.Clear();

                    if (fieldCount != 0)
                    {
                        while (await reader.ReadAsync(cancel).ConfigureAwait(false))
                        {
                            BusinessBase obj = col.CreateNew();

                            //Store data into BusinessObject
                            for (int i = 0; i < fieldCount; ++i)
                            {
                                object val = reader.GetValue(i);

                                obj[i] = val is DBNull ? null : val;
                            }

                            obj.IsModified = false;
                            obj.IsNew = false;

                            col.Add(obj);
                        }
                    }
                    while (await reader.NextResultAsync(cancel).ConfigureAwait(false)) { }

                    command.OnCompleted();
                }
                finally
                {
                    using (reader) { } // dispose if non-null
                    if (wasClosed) cnn.Close();
                }
            }
        }

        public static List<object[]> ToList(IEnumerable<dynamic> data)
        {
            List<object[]> resp = new List<object[]>(100);

            foreach (dynamic item in data)
            {
                DapperRow row = (DapperRow)item;

                resp.Add(row.Values);
            }

            return resp;
        }


        public static void ReadBusinessObject(
                    this IDbConnection cnn, BusinessBase obj, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null
                )
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);

            ReadBusinessObjectImpl(cnn, Row.Single, ref command, obj);
        }


        public static void ReadBusinessCollection(
                    this IDbConnection cnn, BusinessCollectionBase col, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null
                )
        {
            string sql = col.SQLQuery;
            object param = col.SQLParameters;
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);

            ReadBusinessCollectionImpl(cnn, command, col);
        }

        private static void ReadBusinessObjectImpl(IDbConnection cnn, Row row, ref CommandDefinition command, BusinessBase obj)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, obj.GetType(), param?.GetType(), null);
            var info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;

            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, info.ParamReader);

                if (wasClosed) cnn.Open();
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, (row & Row.Single) != 0
                    ? CommandBehavior.SequentialAccess | CommandBehavior.SingleResult // need to allow multiple rows, to check fail condition
                    : CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader

                int fieldCount = reader.FieldCount;
                if (reader.Read() && fieldCount != 0)
                {
                    // with the CloseConnection flag, so the reader will deal with the connection; we
                    // still need something in the "finally" to ensure that broken SQL still results
                    // in the connection closing itself
                    if (command.AddToCache) SetQueryCache(identity, info);

                    //Store data into BusinessObject
                    for (int i = 0; i < fieldCount; ++i)
                    {
                        object val = reader.GetValue(i);

                        obj[i] = val is DBNull ? null : val;
                    }

                    if ((row & Row.Single) != 0 && reader.Read()) ThrowMultipleRows(row);
                    while (reader.Read()) { }
                }
                else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
                {
                    ThrowZeroRows(row);
                }
                while (reader.NextResult()) { }
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;

                command.OnCompleted();
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed) try { cmd.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    reader.Dispose();
                }
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }

        private static void ReadBusinessCollectionImpl(IDbConnection cnn, CommandDefinition command, BusinessCollectionBase col)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, col.GetType(), param?.GetType(), null);
            var info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;

            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, info.ParamReader);

                if (wasClosed) cnn.Open();
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader
                                   // with the CloseConnection flag, so the reader will deal with the connection; we
                                   // still need something in the "finally" to ensure that broken SQL still results
                                   // in the connection closing itself
                if (command.AddToCache) SetQueryCache(identity, info);

                int fieldCount = reader.FieldCount;

                col.Clear();

                if (fieldCount != 0)
                {
                    while (reader.Read())
                    {
                        BusinessBase obj = col.CreateNew();

                        //Store data into BusinessObject
                        for (int i = 0; i < fieldCount; ++i)
                        {
                            object val = reader.GetValue(i);

                            obj[i] = val is DBNull ? null : val;
                        }

                        obj.IsModified = false;
                        obj.IsNew = false;

                        col.Add(obj);
                    }
                }
                while (reader.NextResult()) { }
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;

                command.OnCompleted();
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed) try { cmd.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    reader.Dispose();
                }
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }
    }
}
