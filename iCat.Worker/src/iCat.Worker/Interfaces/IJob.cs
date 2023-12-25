using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Worker.Interfaces
{
    /// <summary>
    /// Job interface
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Excution
        /// </summary>
        /// <param name="obj">Come from Result of StartProcess </param>
        /// <returns></returns>
        Task<object?> DoJob(object? obj);
    }
}
