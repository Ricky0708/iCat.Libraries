using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Worker.Models
{
    /// <summary>
    /// Task Option
    /// </summary>
    public abstract class BaseTaskOption
    {
        /// <summary>
        /// Is execute job first time when task start
        /// </summary>
        public bool IsExecuteWhenStart { get; set; } = false;

        /// <summary>
        /// limit retry times ( -1: forever )
        /// </summary>
        public int RetryTimes { get; set; } = 0;

        /// <summary>
        /// millisecond
        /// </summary>
        public int RetryInterval { get; set; } = 0;
    }
}
