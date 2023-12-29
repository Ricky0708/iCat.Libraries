using iCat.RabbitMQ.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.RabbitMQ.Implements
{
    /// <summary>
    /// Channel Pool
    /// </summary>
    internal class ChannelManager
    {
        //private readonly ConcurrentQueue<IModel> _channelPool_Queue;
        private readonly ConcurrentQueue<StackChannelModel> _channelPool;
        private readonly IConnection _connection;
        private const short _maxChannel = 30;
        private const short _minChannel = 5;
        private const short _cleanPoolIntervalSeconds = 30;
        private volatile int _counter = 0;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        internal ChannelManager(IConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            //_channelPool_Queue = new ConcurrentQueue<IModel>();
            _channelPool = new ConcurrentQueue<StackChannelModel>();
            var cleanTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    SpinWait.SpinUntil(() => false, 1 * 1000 * _cleanPoolIntervalSeconds);
                    var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    while (true)
                    {
                        if (_channelPool.TryDequeue(out var channel) && (now - channel.LastUseTime) > 60 && _counter > _minChannel)
                        {
                            _semaphore.Wait();
                            _counter--;
                            _semaphore.Release();
                        }
                        else
                        {
                            if (channel != null) _channelPool.Enqueue(channel);
                            break;
                        }

                    }
                }
            });
        }

        internal async Task Execute(Action<IModel> method)
        {
            var channel = await GetChannel();
            method.Invoke(channel.Channel);
            ReQueueChannel(channel);
            await Task.CompletedTask;
        }

        private async Task<StackChannelModel> GetChannel()
        {
            if (!_channelPool.TryDequeue(out var channel))
            {
                if (_counter < _maxChannel)
                {
                    _semaphore.Wait();
                    {
                        if (_counter < _maxChannel)
                        {
                            _channelPool.Enqueue(new StackChannelModel { LastUseTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Channel = _connection.CreateModel() });
                            _counter++;
                        }
                    }
                    _semaphore.Release();
                }
                if (!SpinWait.SpinUntil(() => _channelPool.TryDequeue(out channel), 1 * 1000 * 30))
                {
                    throw new Exception("Get Channel time out.");
                }
            }

            return await Task.FromResult(channel!);

        }

        private void ReQueueChannel(StackChannelModel channel)
        {
            channel.LastUseTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _channelPool.Enqueue(channel);
        }


    }
}
