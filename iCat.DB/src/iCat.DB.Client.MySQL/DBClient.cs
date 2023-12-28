using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using iCat.DB.Client.Implements;
using static iCat.DB.Client.Constants.ExecuteCommand;
using iCat.DB.Client.Models;


namespace iCat.DB.Client.MySQL
{
    public class DBClient : iCat.DB.Client.Implements.DBClient
    {
        public override DbConnection Connection => _conn;

        private readonly MySqlConnection _conn;

        public DBClient(DBClientInfo clientInfo, string connectionString) : base(clientInfo)
        {
            _conn = new MySqlConnection(connectionString);
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
            using (var cmd = new MySqlCommand(commandString, _conn))
            {
                cmd.CommandTimeout = CommandTimeout;
                if (_tran != null) cmd.Transaction = _tran as MySqlTransaction;
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
        private static void AssignParameters(MySqlCommand cmd, DbParameter[] @params)
        {
            cmd.CommandType = CommandType.Text;
            if (@params != null && @params.Length > 0)
            {
                var mySqlParams = (MySqlParameter[])@params;
                foreach (var param in mySqlParams)
                {
                    cmd.Parameters.Add(param);
                }
            }
        }

        #endregion
    }
}
