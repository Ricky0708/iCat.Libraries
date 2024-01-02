using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace iCat.DB.Client.Implements
{
    internal readonly struct CommandDefinition
    {
        private readonly string _commandText;
        private readonly DbParameter[]? _parameters;
        private readonly IDbTransaction? _transaction;
        private readonly int? _commandTimeout;
        private readonly CommandType? _commandType;
        //private readonly bool _buffered;
        private readonly CancellationToken _cancellationToken;
        public CancellationToken CancellationToken => _cancellationToken;


        /// <summary>
        /// The command (sql or a stored-procedure name) to execute
        /// </summary>
        public string CommandText => _commandText;
        /// <summary>
        /// The parameters associated with the command
        /// </summary>
        public DbParameter[]? Parameters => _parameters;
        /// <summary>
        /// The active transaction for the command
        /// </summary>
        public IDbTransaction? Transaction => _transaction;
        /// <summary>
        /// The effective timeout for the command
        /// </summary>
        public int? CommandTimeout => _commandTimeout;
        /// <summary>
        /// The type of command that the command-text represents
        /// </summary>
        public CommandType? CommandType => _commandType;

        /// <summary>
        /// Initialize the command definition
        /// </summary>
        public CommandDefinition(string commandText, DbParameter[]? parameters, IDbTransaction? transaction, int? commandTimeout = null,
            CommandType? commandType = System.Data.CommandType.Text
            , CancellationToken cancellationToken = default)
        {
            this._commandText = commandText;
            this._parameters = parameters;
            this._transaction = transaction;
            this._commandTimeout = commandTimeout;
            this._commandType = commandType;
            this._cancellationToken = cancellationToken;
        }

        internal IDbCommand SetupCommand(IDbConnection conn, Action<IDbCommand, IDbDataParameter[]?>? paramReader = null)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = _commandText;
            if (_transaction != null) cmd.Transaction = _transaction;
            if (_commandTimeout.HasValue) cmd.CommandTimeout = _commandTimeout.Value;
            if (_commandType.HasValue) cmd.CommandType = _commandType.Value;

            if (_parameters != null)
                foreach (var parameter in _parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

            paramReader?.Invoke(cmd, _parameters);
            return cmd;
        }
    }
}
