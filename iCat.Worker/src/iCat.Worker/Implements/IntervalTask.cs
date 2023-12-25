using iCat.Worker.Interfaces;
using iCat.Worker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Worker.Implements
{
    /// <summary>
    /// Interval Task
    /// </summary>
    public class IntervalTask : BaseTask
    {
        private readonly int _interval;
        private readonly IntervalTaskOption _option;

        /// <summary>
        /// initial task
        /// </summary>
        /// <param name="job"></param>
        /// <param name="interval">millisecond</param>
        /// <param name="option"></param>
        public IntervalTask(IJob job, int interval, IntervalTaskOption option) : base(job, option)
        {
            _option = option;
            _interval = interval;
        }

        /// <summary>
        /// <see cref="BaseTask.CheckRetry"/>
        /// </summary>
        /// <returns></returns>
        protected override (bool isRetry, int times) CheckRetry()
        {
            _currentRetry++;
            if (_option.RetryTimes < 0)
            {
                return (true, _currentRetry);
            }
            return (_currentRetry < _option.RetryTimes, _currentRetry);
        }

        /// <summary>
        /// <see cref="BaseTask.NextInterval"/>
        /// </summary>
        /// <returns></returns>
        protected override int NextInterval()
        {
            return _interval;
        }

        /// <summary>
        /// <see cref="BaseTask.RetryInterval"/>
        /// </summary>
        /// <returns></returns>
        protected override int RetryInterval()
        {
            return _option.RetryInterval;
        }
    }
}
