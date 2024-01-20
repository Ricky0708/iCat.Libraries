using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iCat.DB.Client.Delegates.Handlers;

namespace iCat.DB.Client.Interfaces
{
    /// <summary>
    /// Connection Operator
    /// </summary>
    public interface IConnection : IDisposable
    {
        #region events

        /// <summary>
        /// Executed event
        /// </summary>
        event ExectuedCommandHandler2? ExecutedEvent;

        #endregion

        #region properties

        /// <summary>
        /// category
        /// </summary>
        string? Category { get; }

        /// <summary>
        /// command timeout seconds
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// it could be used by dapper
        /// </summary>
        DbConnection Connection { get; }

        /// <summary>
        /// Transaction
        /// </summary>
        IDbTransaction? Transaction { get; }

        #endregion

        #region command executors

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute command asynchronous
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute command scalar
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        object? ExecuteScalar(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute command scalar asynchronous
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<object?> ExecuteScalarAsync(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        IDataReader ExecuteOriginalReader(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        IEnumerable<IDataReader> ExecuteReader(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerable<V> ExecuteReader<V>(string commandString, DbParameter[] @params, Func<IDataReader, V> action, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<IDataReader> ExecuteOriginalReaderAsync(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text);

        #endregion
    }
}
