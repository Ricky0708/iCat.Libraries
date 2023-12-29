using iCat.DB.Client.Implements;
using iCat.DB.Client.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iCat.DB.Client.Constants.ExecuteCommand;

namespace iCat.DB.Client.MSSQL
{
    /// <summary>
    /// MSSQL DBConnection
    /// </summary>
    public class DBClient : iCat.DB.Client.Implements.DBClient
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override DbConnection Connection => _conn;

        private readonly SqlConnection _conn;

        /// <summary>
        /// MSSQL DBConnection
        /// </summary>
        /// <param name="info"></param>
        public DBClient(DBClientInfo info) : base(info)
        {
            _conn = new SqlConnection(info.ConnectionString);
        }

        #region command executors

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public override int ExecuteNonQuery(string commandString, DbParameter[] @params)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var result = cmd.ExecuteNonQuery();
                base.CallEvent(Command.Executed, commandString).Wait();
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
                return result;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public override async Task<int> ExecuteNonQueryAsync(string commandString, DbParameter[] @params)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var result = await cmd.ExecuteNonQueryAsync();
                await base.CallEvent(Command.Executed, commandString);
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
                return result;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public override object ExecuteScalar(string commandString, DbParameter[] @params)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var result = cmd.ExecuteScalar();
                base.CallEvent(Command.Executed, commandString).Wait();
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
                return result;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public override async Task<object> ExecuteScalarAsync(string commandString, DbParameter[] @params)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var result = await cmd.ExecuteScalarAsync();
                await base.CallEvent(Command.Executed, commandString);
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
                return result!;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="action"></param>
        public override void ExecuteReader(string commandString, DbParameter[] @params, Action<DbDataReader> action)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    action.Invoke(dr);
                }
                dr.Close();
                base.CallEvent(Command.Executed, commandString).Wait();
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public override IEnumerable<DbDataReader> ExecuteReader(string commandString, DbParameter[] @params)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    yield return dr;
                }
                dr.Close();
                base.CallEvent(Command.Executed, commandString).Wait();
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public override IEnumerable<V> ExecuteReader<V>(string commandString, DbParameter[] @params, Func<DbDataReader, V> func)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    yield return func.Invoke(dr);
                }
                dr.Close();
                base.CallEvent(Command.Executed, commandString).Wait();
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="executedAction"></param>
        /// <returns></returns>
        public override async ValueTask ExecuteReaderAsync(string commandString, DbParameter[] @params, Action<DbDataReader> executedAction)
        {
            using (var cmd = new SqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as SqlTransaction;
                AssignParameters(cmd, @params);
                if (_tran == null && _conn.State == ConnectionState.Closed) _conn.Open();
                var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    executedAction.Invoke(dr);
                }
                dr.Close();
                await base.CallEvent(Command.Executed, commandString);
                if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Assign parameters to sql command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="params"></param>
        private static void AssignParameters(SqlCommand cmd, DbParameter[] @params)
        {
            cmd.CommandType = CommandType.Text;
            if (@params != null && @params.Length > 0)
            {
                var SqlParams = (SqlParameter[])@params;
                foreach (var param in SqlParams)
                {
                    cmd.Parameters.Add(param);
                }
            }
        }

        #endregion

    }
}
