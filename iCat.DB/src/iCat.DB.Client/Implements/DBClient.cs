﻿using iCat.DB.Client.Delegates;
using iCat.DB.Client.Interfaces;
using iCat.DB.Client.Models;
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
    /// <summary>
    /// Adapter for DBConnection
    /// </summary>
    public abstract class DBClient : IConnection, IUnitOfWork
    {

        #region events

        /// <summary>
        /// disposing event
        /// </summary>
        public event EventHandler? DisposingHandler;

        /// <summary>
        /// event trigger at each commend executed
        /// </summary>
        public event Handlers.ExectuedCommandHandler? ExecutedEvent;

        #endregion

        #region properties

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Category => _category;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int CommandTimeout { get; set; } = 30;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract DbConnection Connection { get; }

        #endregion

        #region fields

        private readonly string _category;
        /// <summary>
        /// Transaction instance
        /// </summary>
        protected IDbTransaction? _tran;
        private bool _disposed;

        #endregion

        #region constructors

        /// <summary>
        /// Adapter for DBConnection
        /// </summary>
        /// <param name="info"></param>
        public DBClient(DBClientInfo info)
        {
            _category = info.Category;
        }

        #endregion

        #region IUnitOfWork

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IDbConnection Open()
        {
            Connection.Open();
            CallEvent(Command.Opened, "").Wait();
            return Connection;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async Task<IDbConnection> OpenAsync()
        {
            await Connection.OpenAsync();
            await CallEvent(Command.Opened, "");
            return Connection;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Close()
        {
            _tran?.Dispose();
            _tran = null;
            Connection.Close();

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
            Connection.Close();
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
            _tran = Connection.BeginTransaction(isolationLevel);
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
            _tran = await Connection.BeginTransactionAsync(isolationLevel);
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

        /// <summary>
        /// Call executed event
        /// </summary>
        /// <param name="command"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        protected async Task CallEvent(Command command, string script)
        {
            ExecutedEvent?.Invoke(Category, command, script);
            await Task.CompletedTask;
        }

        #endregion

        #region dispose

        /// <summary>
        /// Can be removed cache from DBClientFactory
        /// </summary>
        protected void Disposing()
        {
            DisposingHandler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// DisposeAsync
        /// </summary>
        /// <returns></returns>
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
                Connection.Close();
                _tran?.Dispose();
                Connection.Dispose();
                Disposing();
            }

            _disposed = true;
        }

        /// <summary>
        /// Deconstructive
        /// </summary>
        ~DBClient()
        {
            Dispose(false);
        }

        #endregion

    }
}