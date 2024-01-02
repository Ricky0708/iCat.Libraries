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
    /// UnitOfWork
    /// </summary>
    public interface IUnitOfWork2 : IDisposable
    {
        /// <summary>
        /// Executed event
        /// </summary>
        event ExectuedCommandHandler? ExecutedEvent;

        /// <summary>
        /// Category
        /// </summary>
        string? Category { get; }

        /// <summary>
        /// Export connection, it could be used by dapper
        /// </summary>
        DbConnection Connection { get; }

        /// <summary>
        /// Transaction
        /// </summary>
        IDbTransaction? Transaction { get; }

        #region operators

        /// <summary>
        /// Open connection
        /// </summary>
        /// <returns></returns>
        IDbConnection Open();

        /// <summary>
        /// Open connection asynchronous
        /// </summary>
        /// <returns></returns>
        Task<IDbConnection> OpenAsync();

        /// <summary>
        /// Close connection
        /// </summary>
        void Close();

        /// <summary>
        /// Close connection asynchronous
        /// </summary>
        /// <returns></returns>
        ValueTask CloseAsync();

        /// <summary>
        /// Begin db transaction
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);

        /// <summary>
        /// Begin transaction asynchronous
        /// </summary>
        /// <returns></returns>
        Task<IDbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);

        /// <summary>
        /// Commit transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Commit transaction asynchronous 
        /// </summary>
        /// <returns></returns>
        ValueTask CommitAsync();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rollback asynchronous 
        /// </summary>
        /// <returns></returns>
        ValueTask RollbackAsync();

        #endregion
    }
}
