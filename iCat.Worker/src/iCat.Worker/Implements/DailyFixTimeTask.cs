using iCat.Worker.Interfaces;
using iCat.Worker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Worker.Implements
{
    public class DailyFixTimeTask : BaseTask
    {
        private readonly int _hour;
        private readonly int _minute;
        private readonly DailyFixTimeTaskOption _option;

        /// <summary>
        /// UTC Hour and Minute
        /// </summary>
        /// <param name="job"></param>
        /// <param name="hour">UTC H (24)</param>
        /// <param name="minute">mm</param>
        /// <param name="taskOption"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DailyFixTimeTask(IJob job, int hour, int minute, DailyFixTimeTaskOption taskOption) : base(job, taskOption)
        {
            _hour = hour;
            _minute = minute;
            _option = taskOption ?? throw new ArgumentNullException(nameof(taskOption));
        }

        protected override (bool isRetry, int times) CheckRetry()
        {
            _currentRetry++;
            if (_option.RetryTimes < 0)
            {
                return (true, _currentRetry);
            }
            return (_currentRetry < _option.RetryTimes, _currentRetry);
        }

        protected override int NextInterval()
        {
            var currentDatetime = DateTimeOffset.UtcNow;
            var todayExecuteTime = new DateTimeOffset(currentDatetime.Year, currentDatetime.Month, currentDatetime.Day, _hour, _minute, 0, new TimeSpan());
            var nextDayExecuteTime = currentDatetime
                .AddDays(1)
                .AddHours(_hour - DateTimeOffset.UtcNow.Hour)
                .AddMinutes(_minute - DateTimeOffset.UtcNow.Minute)
                .AddSeconds(-currentDatetime.Second);

            if (todayExecuteTime > currentDatetime)
            {
                return Convert.ToInt32((todayExecuteTime.Ticks - currentDatetime.Ticks) / 10000);
            }
            else
            {
                return Convert.ToInt32((nextDayExecuteTime.Ticks - currentDatetime.Ticks) / 10000);
            }

        }

        protected override int RetryInterval()
        {
            return _option.RetryInterval;
        }
    }
}
