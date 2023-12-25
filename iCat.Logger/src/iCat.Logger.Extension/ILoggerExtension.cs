using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Logger.Extension
{
    public static class ILoggerExtension
    {
        public static void LogPerformance(this ILogger logger, string description, Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action.Invoke();
            sw.Stop();
            logger.LogInformation("{description}, Performance {elapsedMilliseconds}", description, sw.ElapsedMilliseconds);
        }

        public static async Task LogPerformance(this ILogger logger, string description, Func<Task> func)
        {
            var sw = new Stopwatch();
            sw.Start();
            await func.Invoke();
            sw.Stop();
            logger.LogInformation("{description}, Performance {elapsedMilliseconds}ms", description, sw.ElapsedMilliseconds);
        }

        public static TResult LogPerformance<TResult>(this ILogger logger, string description, Func<TResult> func)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = func.Invoke();
            sw.Stop();
            logger.LogInformation("{description}, Performance {elapsedMilliseconds}", description, sw.ElapsedMilliseconds);
            return result;
        }

        public static async Task<TResult> LogPerformance<TResult>(this ILogger logger, string description, Func<Task<TResult>> func)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = await func.Invoke();
            sw.Stop();
            logger.LogInformation("{description}, Performance {elapsedMilliseconds}", description, sw.ElapsedMilliseconds);
            return result;
        }
    }
}
