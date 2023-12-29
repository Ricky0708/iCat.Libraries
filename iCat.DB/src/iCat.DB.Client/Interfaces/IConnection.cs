using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iCat.DB.Client.Delegates.Handlers;

namespace iCat.DB.Client.Interfaces
{
    public interface IConnection : IDisposable
    {
        #region events

        /// <summary>
        /// Executed event
        /// </summary>
        event ExectuedCommandHandler? ExecutedEvent;

        #endregion

        #region properties

        /// <summary>
        /// category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// command timeout seconds
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// it could be used by dapper
        /// </summary>
        DbConnection Connection { get; }

        #endregion

        #region command executors

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandString, DbParameter[] @params);

        /// <summary>
        /// Execute command asynchronous
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string commandString, DbParameter[] @params);

        /// <summary>
        /// Execute command scalar
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        object ExecuteScalar(string commandString, DbParameter[] @params);

        /// <summary>
        /// Execute command scalar asynchronous
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string commandString, DbParameter[] @params);

        /// <summary>
        /// Execute command reader
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="action"></param>
        void ExecuteReader(string commandString, DbParameter[] @params, Action<DbDataReader> action);

        /// <summary>
        /// Execute command reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        IEnumerable<DbDataReader> ExecuteReader(string commandString, DbParameter[] @params);

        /// <summary>
        /// Execute command reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerable<V> ExecuteReader<V>(string commandString, DbParameter[] @params, Func<DbDataReader, V> action);

        /// <summary>
        /// Execute command reader asynchronous
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="executedAction"></param>
        /// <returns></returns>
        ValueTask ExecuteReaderAsync(string commandString, DbParameter[] @params, Action<DbDataReader> executedAction);

        #endregion
    }
}
