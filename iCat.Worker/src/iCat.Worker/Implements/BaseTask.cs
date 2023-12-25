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
    /// Task base
    /// </summary>
    public abstract class BaseTask
    {


        #region properties

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get; internal set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public TaskStatus Status => _task.Status;

        /// <summary>
        /// Gets the task identifier.
        /// </summary>
        /// <value>
        /// The task identifier.
        /// </value>
        public long TaskID => _task.Id;

        #endregion

        #region fields

        private bool _isCanceled = false;

        private bool _isNeedWait = true;

        /// <summary>
        /// The task
        /// </summary>
        protected Task _task;

        /// <summary>
        /// The token source
        /// </summary>
        protected CancellationTokenSource? _tokenSource;

        /// <summary>
        /// The token
        /// </summary>
        protected CancellationToken _token;

        /// <summary>
        /// The job
        /// </summary>
        protected readonly IJob _job;

        /// <summary>
        /// The current retry
        /// </summary>
        protected int _currentRetry = 0;

        #endregion

        #region event delegates

        public delegate bool BeforeTaskStartHandler(string category);
        public delegate void ProcessedHandle(string category, object? processedResult);
        public delegate object? BeforeStartHandler(string category);
        public delegate void CallStopHandler(string category);
        public delegate void CancelCalledHandler(string category);
        public delegate void StopedHandler(string category);
        public delegate void ExceptionInRetriedHandler(string category, Exception ex);
        public delegate void OnRetryHandler(string category, Exception ex, int retryTimes);

        /// <summary>
        /// Before start
        /// </summary>
        public event BeforeTaskStartHandler? BeforeTaskStart;

        /// <summary>
        /// Processed result
        /// </summary>
        public event ProcessedHandle? Processed;

        /// <summary>
        /// started, result will be passed to DoJob(params object[] obj)
        /// </summary>
        public event BeforeStartHandler? BeforeStart;

        /// <summary>
        /// Call stop
        /// </summary>
        public event CallStopHandler? CallStop;

        /// <summary>
        /// After CancellationTokenSource is called
        /// </summary>
        public event CancelCalledHandler? CancelCalled;

        /// <summary>
        /// Stoped
        /// </summary>
        public event StopedHandler? Stoped;

        /// <summary>
        /// Called when out of limit retry times
        /// </summary>
        public event ExceptionInRetriedHandler? Exception;

        /// <summary>
        /// On retry
        /// </summary>
        public event OnRetryHandler? OnRetry;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTask"/> class.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="baseOption"></param>
        protected BaseTask(IJob job, BaseTaskOption baseOption)
        {
            _isNeedWait = !baseOption.IsExecuteWhenStart;
            _job = job;
            _task = CreateTask();
            Category = _job.Category;

        }

        #region public method   

        /// <summary>
        /// start task
        /// </summary>
        public void Start()
        {
            if (_task.Status == TaskStatus.RanToCompletion || _task.Status == TaskStatus.Created || _task.Status == TaskStatus.Faulted)
            {
                _isCanceled = false;
                _currentRetry = 0;
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;
                _token.Register(() =>
                {
                    //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"CancelCalled", null, this.Category, "Start" });
                    CancelCalled?.Invoke(Category);
                });
                _task = CreateTask();
                //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"BeforeStart", null, this.Category, "Start" });
                if (BeforeTaskStart?.Invoke(Category) ?? true) _task.Start();
            }
        }

        /// <summary>
        /// stop task
        /// </summary>
        public void Stop()
        {
            //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"BeforeCallCancel", null, this.Category, "Stop" });
            CallStop?.Invoke(Category);
            if (_task.Status == TaskStatus.Running) _tokenSource?.Cancel();
            _isCanceled = true;
        }

        #endregion 

        #region abstract

        /// <summary>
        /// next interval
        /// </summary>
        protected abstract int NextInterval();

        /// <summary>
        /// retry interval
        /// </summary>
        /// <returns></returns>
        protected abstract int RetryInterval();

        /// <summary>
        /// check should keep retry and get current retry times
        /// </summary>
        /// <returns></returns>
        protected abstract (bool isRetry, int times) CheckRetry();

        #endregion

        #region private method

        private Task CreateTask()
        {
            return new Task(async () =>
            {
                while (!_isCanceled)
                {
                    try
                    {

                        if (_isNeedWait)
                        {
                            SpinWait.SpinUntil(() => _isCanceled, NextInterval());
                        }
                        //var methodInfo = this.GetType().GetMethod("Log");

                        _token.ThrowIfCancellationRequested();
                        //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"StartProcess", null, this.Category, "ExecuteTask" });
                        var startResult = BeforeStart?.Invoke(Category) ?? null;
                        var result = await _job.DoJob(startResult);
                        //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"JobResult {result}", null, this.Category, "ExecuteTask" });
                        Processed?.Invoke(Category, result);
                        _isNeedWait = true;
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        var isRetry = CheckRetry();
                        if (!isRetry.isRetry)
                        {
                            //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"AfterExceptionCanceled", ex, this.Category, "ExecuteTask" });
                            Exception?.Invoke(Category, ex);
                            throw;
                        }
                        //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"OnRetry: {isRetry.Item2}", null, this.Category, "ExecuteTask" });
                        OnRetry?.Invoke(Category, ex, isRetry.times);
                        SpinWait.SpinUntil(() => _isCanceled, RetryInterval());
                    }
                }
                Stoped?.Invoke(Category);
                //_methodInfo.Invoke(this, new object[] { LogLevel.Information, LogSourcePoint.Job, $"AfterCanceled", null, this.Category, "ExecuteTask" });
            }, _token);
        }

        #endregion
    }
}
