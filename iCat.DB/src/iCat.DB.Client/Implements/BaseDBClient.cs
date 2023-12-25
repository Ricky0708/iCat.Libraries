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
    public abstract class BaseDBClient<T> : IConnection, IUnitOfWork where T : DbConnection
    {

        #region events

        public event Handlers.ExectuedCommandHandler? ExecutedEvent;

        #endregion

        #region properties

        public string Category => _category ?? "";

        public int CommandTimeout { get; set; } = 30;

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
        /// <see cref="ISQLDBManager.Open"/>
        /// </summary>
        public IDbConnection Open()
        {
            _conn.Open();
            CallEvent(Command.Opened, "").Wait();
            return _conn;
        }

        /// <summary>
        /// <see cref="ISQLDBManager.OpenAsync"/>
        /// </summary>
        public async Task<IDbConnection> OpenAsync()
        {
            await _conn.OpenAsync();
            await CallEvent(Command.Opened, "");
            return _conn;
        }

        /// <summary>
        /// <see cref="ISQLDBManager.Close"/>
        /// </summary>
        public void Close()
        {
            _tran?.Dispose();
            _tran = null;
            _conn.Close();

            CallEvent(Command.Closed, "").Wait();
        }

        /// <summary>
        /// <see cref="ISQLDBManager.CloseAsync"/>
        /// </summary>
        public async ValueTask CloseAsync()
        {
            _tran?.Dispose();
            _tran = null;
            _conn.Close();
            await CallEvent(Command.Closed, "");
            await Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="ISQLDBManager.BeginTransaction(IsolationLevel)"/>
        /// </summary>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = _conn.BeginTransaction(isolationLevel);
            CallEvent(Command.TransactionBegined, "").Wait();
            return _tran;
        }

        /// <summary>
        /// <see cref="ISQLDBManager.BeginTransactionAsync(IsolationLevel)"/>
        /// </summary>
        public async Task<IDbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = await _conn.BeginTransactionAsync(isolationLevel);
            await CallEvent(Command.TransactionBegined, "");
            return _tran;
        }

        /// <summary>
        /// <see cref="ISQLDBManager.Commit"/>
        /// </summary>
        public void Commit()
        {
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            CallEvent(Command.Commited, "").Wait();
        }

        /// <summary>
        /// <see cref="ISQLDBManager.CommitAsync"/>
        /// </summary>
        public async ValueTask CommitAsync()
        {
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            await CallEvent(Command.Commited, "");
            await Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="ISQLDBManager.Rollback"/>
        /// </summary>
        public void Rollback()
        {
            _tran?.Rollback();
            _tran?.Dispose();
            _tran = null;
            CallEvent(Command.Rollbacked, "").Wait();
        }

        /// <summary>
        /// <see cref="ISQLDBManager.RollbackAsync"/>
        /// </summary>
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

        public abstract int ExecuteNonQuery(string commandString, DbParameter[] @params);

        public abstract Task<int> ExecuteNonQueryAsync(string commandString, DbParameter[] @params);

        public abstract void ExecuteReader(string commandString, DbParameter[] @params, Action<DbDataReader> action);

        public abstract IEnumerable<DbDataReader> ExecuteReader(string commandString, DbParameter[] @params);

        public abstract IEnumerable<V> ExecuteReader<V>(string commandString, DbParameter[] @params, Func<DbDataReader, V> action);

        public abstract ValueTask ExecuteReaderAsync(string commandString, DbParameter[] @params, Action<DbDataReader> executedAction);

        public abstract object ExecuteScalar(string commandString, DbParameter[] @params);

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
