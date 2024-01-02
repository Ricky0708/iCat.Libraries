using iCat.DB.Client.Delegates;
using iCat.DB.Client.Interfaces;
using System;
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
    public class DBClient2 : IConnection2, IUnitOfWork2
    {

        #region Property

        /// <inheritdoc/>
        public string? Category => _category;

        /// <inheritdoc/>
        public DbConnection Connection => _conn;

        /// <inheritdoc/>
        public int CommandTimeout { get; set; }

        /// <inheritdoc/>
        public IDbTransaction? Transaction => _tran;

        #endregion

        #region Field

        private IDbTransaction? _tran;
        private DbConnection _conn;
        private string? _category;

        #endregion

        #region Event

        /// <inheritdoc/>
        public event Handlers.ExectuedCommandHandler? ExecutedEvent;

        #endregion

        #region Constructor

        /// <inheritdoc/>
        public DBClient2(DbConnection connection)
        {
            _category = "default";
            _conn = connection;
        }

        /// <inheritdoc/>
        public DBClient2(string category, DbConnection connection)
        {
            _category = category;
            _conn = connection;
        }

        #endregion

        #region UnitOfWork

        /// <inheritdoc/>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = Connection.BeginTransaction(isolationLevel);
            CallEvent(Command.TransactionBegined, "").Wait();
            return _tran;
        }

        /// <inheritdoc/>
        public async Task<IDbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _tran = Connection.BeginTransaction(isolationLevel);
            await CallEvent(Command.TransactionBegined, "");
            return _tran;
        }

        /// <inheritdoc/>
        public void Close()
        {
            _tran?.Dispose();
            _tran = null;
            _conn.Close();
            CallEvent(Command.Closed, "").Wait();
        }

        /// <inheritdoc/>
        public async ValueTask CloseAsync()
        {
            _tran?.Dispose();
            _tran = null;
            _conn.Close();
            await CallEvent(Command.Closed, "");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Commit()
        {
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            CallEvent(Command.Commited, "").Wait();
        }

        /// <inheritdoc/>
        public async ValueTask CommitAsync()
        {
            _tran?.Commit();
            _tran?.Dispose();
            _tran = null;
            await CallEvent(Command.Commited, "");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public IDbConnection Open()
        {
            _conn.Open();
            CallEvent(Command.Opened, "").Wait();
            return _conn;
        }

        /// <inheritdoc/>
        public async Task<IDbConnection> OpenAsync()
        {
            await _conn.OpenAsync();
            await CallEvent(Command.Opened, "");
            return _conn;
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            _tran?.Rollback();
            _tran?.Dispose();
            _tran = null;
            CallEvent(Command.Rollbacked, "").Wait();
        }

        /// <inheritdoc/>
        public async ValueTask RollbackAsync()
        {
            _tran?.Rollback();
            _tran?.Dispose();
            _tran = null;
            await CallEvent(Command.Rollbacked, "");
            await Task.CompletedTask;
        }

        #endregion

        #region Connection

        /// <inheritdoc/>
        public int ExecuteNonQuery(string commandString, DbParameter[] @params, CommandType? commandType = null)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn);

            if (_conn.State == ConnectionState.Closed) _conn.Open();

            var result = cmd.ExecuteNonQuery();

            if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> ExecuteNonQueryAsync(string commandString, DbParameter[] @params, CommandType? commandType)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn);

            if (_conn.State == ConnectionState.Closed) await _conn.OpenAsync();

            var result = cmd.ExecuteNonQuery();

            if (_tran == null && _conn.State == ConnectionState.Open) await _conn.CloseAsync();

            return result;
        }

        /// <inheritdoc/>
        public object ExecuteScalar(string commandString, DbParameter[] @params, CommandType? commandType)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn);

            if (_conn.State == ConnectionState.Closed) _conn.Open();

            var result = cmd.ExecuteScalar();

            if (_tran == null && _conn.State == ConnectionState.Open) _conn.Close();

            return result;
        }

        /// <inheritdoc/>
        public async Task<object> ExecuteScalarAsync(string commandString, DbParameter[] @params, CommandType? commandType)
        {
            var commandDefinition = new CommandDefinition(commandString, @params, _tran, CommandTimeout, commandType);
            var cmd = commandDefinition.SetupCommand(_conn);

            if (_conn.State == ConnectionState.Closed) await _conn.OpenAsync();

            var result = cmd.ExecuteScalar();

            if (_tran == null && _conn.State == ConnectionState.Open) await _conn.CloseAsync();

            return result;
        }

        /// <inheritdoc/>
        public void ExecuteReader(string commandString, DbParameter[] @params, CommandType? commandType, Action<DbDataReader> action)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<DbDataReader> ExecuteReader(string commandString, DbParameter[] @params, CommandType? commandType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<V> ExecuteReader<V>(string commandString, DbParameter[] @params, CommandType? commandType, Func<DbDataReader, V> action)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask ExecuteReaderAsync(string commandString, DbParameter[] @params, CommandType? commandType, Action<DbDataReader> executedAction)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Protected Methods

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

        #endregion

        #region Private Methods



        #endregion

        #region Dispose

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
