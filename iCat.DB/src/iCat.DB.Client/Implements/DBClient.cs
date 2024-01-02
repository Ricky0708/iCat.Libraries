using iCat.DB.Client.Delegates;
using iCat.DB.Client.Interfaces;
using iCat.DB.Client.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static iCat.DB.Client.Constants.ExecuteCommand;

namespace iCat.DB.Client.Implements
{
    /// <inheritdoc/>
    public class DBClient : IConnection, IUnitOfWork
    {

        #region Property

        /// <inheritdoc/>
        public string? Category => _category;

        /// <inheritdoc/>
        public IDbConnection Connection => _conn;

        /// <inheritdoc/>
        public int CommandTimeout { get; set; }

        /// <inheritdoc/>
        public IDbTransaction? Transaction => _tran;

        #endregion

        #region Field

        private IDbTransaction? _tran;
        private readonly DbConnection _conn;
        private readonly string? _category;
        private readonly ConcurrentDictionary<string, IDataReader> _readerCache;
        private bool _disposed = false;

        #endregion

        #region Event

        /// <inheritdoc/>
        public event Handlers.ExectuedCommandHandler2? ExecutedEvent;

        /// <summary>
        /// disposing event
        /// </summary>
        public event EventHandler? DisposingHandler;

        #endregion

        #region Constructor

        /// <inheritdoc/>
        public DBClient(DbConnection connection) : this(new DBClientInfo("default", connection))
        {
        }

        /// <inheritdoc/>
        public DBClient(DBClientInfo info)
        {
            _category = info.Category;
            _conn = info.Connection;
            _readerCache = new ConcurrentDictionary<string, IDataReader>();
        }

        #endregion

        #region UnitOfWork

        /// <inheritdoc/>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = Connection.BeginTransaction(isolationLevel);
            CallEvent(CommandKind.TransactionBegined, null, null).Wait();
            return _tran;
        }

        /// <inheritdoc/>
        public async Task<IDbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = Connection.BeginTransaction(isolationLevel);
            await CallEvent(CommandKind.TransactionBegined, null, null);
            return _tran;
        }

        /// <inheritdoc/>
        public void Close()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            _tran?.Dispose();
            _tran = null;
            _conn.Close();
            CallEvent(CommandKind.Closed, null, null).Wait();
        }

        /// <inheritdoc/>
        public async ValueTask CloseAsync()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            _tran?.Dispose();
            _tran = null;
            await _conn.CloseAsync();
            await CallEvent(CommandKind.Closed, null, null);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Commit()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            CallEvent(CommandKind.Commited, null, null).Wait();
        }

        /// <inheritdoc/>
        public async ValueTask CommitAsync()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            await CallEvent(CommandKind.Commited, null, null);
        }

        /// <inheritdoc/>
        public IDbConnection Open()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            _conn.Open();
            CallEvent(CommandKind.Opened, null, null).Wait();
            return _conn;
        }

        /// <inheritdoc/>
        public async Task<IDbConnection> OpenAsync()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            await _conn.OpenAsync();
            await CallEvent(CommandKind.Opened, null, null);
            return _conn;
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            _tran?.Rollback();
            _tran?.Dispose();
            _tran = null;
            CallEvent(CommandKind.Rollbacked, null, null).Wait();
        }

        /// <inheritdoc/>
        public async ValueTask RollbackAsync()
        {
            foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
            _tran?.Rollback();
            _tran?.Dispose();
            _tran = null;
            await CallEvent(CommandKind.Rollbacked, null, null);
        }

        #endregion

        #region Connection

        /// <inheritdoc/>
        public int ExecuteNonQuery(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn, (command, parameters) =>
            {
                CallEvent(CommandKind.Executing, command, parameters).Wait();
            });

            var isClose = _conn.State == ConnectionState.Closed;

            if (!isClose) _conn.Open();

            var result = cmd.ExecuteNonQuery();

            if (!isClose) _conn.Close();

            cmd.Dispose();

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> ExecuteNonQueryAsync(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn, async (command, parameters) =>
            {
                await CallEvent(CommandKind.Executing, command, parameters);
            });

            var isClose = _conn.State == ConnectionState.Closed;

            if (!isClose) await _conn.OpenAsync();

            var result = cmd.ExecuteNonQuery();

            if (!isClose) await _conn.CloseAsync();

            cmd.Dispose();

            return result;
        }

        /// <inheritdoc/>
        public object? ExecuteScalar(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn, (command, parameters) =>
            {
                CallEvent(CommandKind.Executing, command, parameters).Wait();
            });

            var isClose = _conn.State == ConnectionState.Closed;

            if (!isClose) _conn.Open();

            var result = cmd.ExecuteScalar();

            if (!isClose) _conn.Close();

            cmd.Dispose();

            return result;
        }

        /// <inheritdoc/>
        public async Task<object?> ExecuteScalarAsync(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn, async (command, parameters) =>
            {
                await CallEvent(CommandKind.Executing, command, parameters);
            });

            var isClose = _conn.State == ConnectionState.Closed;

            if (!isClose) await _conn.OpenAsync();

            var result = cmd.ExecuteScalar();

            if (!isClose) await _conn.CloseAsync();

            cmd.Dispose();

            return result;
        }

        /// <inheritdoc/>
        public IDataReader ExecuteOriginalReader(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn, (command, parameters) =>
            {
                CallEvent(CommandKind.Executing, command, parameters).Wait();
            });

            var isClose = _conn.State == ConnectionState.Closed;

            if (!isClose) _conn.Open();

            var dr = cmd.ExecuteReader();
            var guid = Guid.NewGuid().ToString();
            _readerCache.TryAdd(guid, dr);

            return dr;
        }

        /// <inheritdoc/>
        public IEnumerable<IDataReader> ExecuteReader(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn, (command, parameters) =>
            {
                CallEvent(CommandKind.Executing, command, parameters).Wait();
            });

            var isClose = _conn.State == ConnectionState.Closed;

            if (!isClose) _conn.Open();

            var dr = cmd.ExecuteReader();
            var guid = Guid.NewGuid().ToString();
            _readerCache.TryAdd(guid, dr);

            while (dr.Read())
            {
                yield return dr;
            }

            dr.Close();
            _readerCache.TryRemove(guid, out dr);
            if (!isClose) _conn.Close();

            cmd.Dispose();
        }

        /// <inheritdoc/>
        public IEnumerable<V> ExecuteReader<V>(string commandString, DbParameter[] @params, Func<IDataReader, V> action, CommandType? commandType = CommandType.Text)
        {
            foreach (var item in ExecuteReader(commandString, @params, commandType))
            {
                yield return action(item);
            }
        }

        /// <inheritdoc/>
        public async Task<IDataReader> ExecuteOriginalReaderAsync(string commandString, DbParameter[] @params, CommandType? commandType = CommandType.Text)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn, async (command, parameters) =>
            {
                await CallEvent(CommandKind.Executing, command, parameters);
            });

            var isClose = _conn.State == ConnectionState.Closed;

            if (!isClose) await _conn.OpenAsync();

            var dr = cmd.ExecuteReader();
            var guid = Guid.NewGuid().ToString();
            _readerCache.TryAdd(guid, dr);

            return dr;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region protected methods

        /// <summary>
        /// Call executed event
        /// </summary>
        /// <param name="commandKind"></param>
        /// <param name="command"></param>
        /// <param name="dbDataParameter"></param>
        /// <returns></returns>
        protected async Task CallEvent(CommandKind commandKind, IDbCommand? command, IDbDataParameter[]? dbDataParameter)
        {
            ExecutedEvent?.Invoke(Category, commandKind, command, dbDataParameter);
            await Task.CompletedTask;
        }


        #endregion

        #region Private Methods

        #endregion

        #region Dispose

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }


        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                DisposingHandler?.Invoke(this, EventArgs.Empty);
                foreach (var dr in _readerCache) if (!dr.Value.IsClosed) dr.Value.Close();
                _tran?.Dispose();
                _conn.Close();
                _conn.Dispose();
            }

            _disposed = true;
        }

        #endregion

    }
}
