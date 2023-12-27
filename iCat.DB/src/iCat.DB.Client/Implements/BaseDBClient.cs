using iCat.DB.Client.Delegates;
using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static iCat.DB.Client.Constants.ExecuteCommand;

namespace iCat.DB.Client.Implements
{
    public abstract class DBClient
    {

    }
    public abstract class BaseDBClient<T> : DBClient, IConnection, IUnitOfWork where T : DbConnection
    {

        #region events

        public event Handlers.ExectuedCommandHandler? ExecutedEvent;

        #endregion

        #region properties

        public string Category => _category ?? "";

        public int CommandTimeout { get; set; } = 30;

        public DbConnection Connection => _conn;

        #endregion

        #region private fields

        private readonly string? _category;
        protected readonly DbConnection _conn = default!;
        protected IDbTransaction? _tran;
        private bool _disposed;

        #endregion

        #region constructors

        public BaseDBClient(string category, string connectionString)
        {
            _conn = (T)Activator.CreateInstance(typeof(T), connectionString)!;
            _category = category;
        }

        #endregion

        #region IUnitOfWork

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IDbConnection Open()
        {
            _conn.Open();
            CallEvent(Command.Opened, "").Wait();
            return _conn;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async Task<IDbConnection> OpenAsync()
        {
            await _conn.OpenAsync();
            await CallEvent(Command.Opened, "");
            return _conn;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Close()
        {
            _tran?.Dispose();
            _tran = null;
            _conn.Close();

            CallEvent(Command.Closed, "").Wait();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async ValueTask CloseAsync()
        {
            _tran?.Dispose();
            _tran = null;
            _conn.Close();
            await CallEvent(Command.Closed, "");
            await Task.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = _conn.BeginTransaction(isolationLevel);
            CallEvent(Command.TransactionBegined, "").Wait();
            return _tran;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public async Task<IDbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = await _conn.BeginTransactionAsync(isolationLevel);
            await CallEvent(Command.TransactionBegined, "");
            return _tran;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Commit()
        {
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            CallEvent(Command.Commited, "").Wait();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async ValueTask CommitAsync()
        {
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            await CallEvent(Command.Commited, "");
            await Task.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Rollback()
        {
            _tran?.Rollback();
            _tran?.Dispose();
            _tran = null;
            CallEvent(Command.Rollbacked, "").Wait();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async ValueTask RollbackAsync()
        {
            _tran?.Rollback();
            _tran?.Dispose();
            _tran = null;
            await CallEvent(Command.Rollbacked, "");
            await Task.CompletedTask;
        }

        #endregion

        #region IConnection

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public abstract int ExecuteNonQuery(string commandString, DbParameter[] @params);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public abstract Task<int> ExecuteNonQueryAsync(string commandString, DbParameter[] @params);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="action"></param>
        public abstract void ExecuteReader(string commandString, DbParameter[] @params, Action<DbDataReader> action);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public abstract IEnumerable<DbDataReader> ExecuteReader(string commandString, DbParameter[] @params);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public abstract IEnumerable<V> ExecuteReader<V>(string commandString, DbParameter[] @params, Func<DbDataReader, V> action);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <param name="executedAction"></param>
        /// <returns></returns>
        public abstract ValueTask ExecuteReaderAsync(string commandString, DbParameter[] @params, Action<DbDataReader> executedAction);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public abstract object ExecuteScalar(string commandString, DbParameter[] @params);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public abstract Task<object> ExecuteScalarAsync(string commandString, DbParameter[] @params);

        #endregion

        #region protected methods

        protected async Task CallEvent(Command command, string script)
        {
            ExecutedEvent?.Invoke(Category, command, script);
            await Task.CompletedTask;
        }

        #endregion

        #region dispose

        public void Dispose() => Dispose(true);

        public async ValueTask DisposeAsync()
        {
            Dispose(true);
            await Task.CompletedTask;
        }


        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _conn.Close();
                _tran?.Dispose();
                _conn.Dispose();
            }

            _disposed = true;
        }



        ~BaseDBClient()
        {

            Dispose(false);
        }

        #endregion
    }
}
