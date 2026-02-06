using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Kafka.Implements
{
    /// <summary>
    /// A thread-safe hash set implementation using ConcurrentDictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ConcurrentHashSet<T>
    {
        /// <summary>
        /// The underlying ConcurrentDictionary to store the elements of the set. The value is a dummy byte since we only care about the keys.
        /// </summary>
        private readonly ConcurrentDictionary<T, byte> _dict;

        /// <summary>
        /// Adds the specified item to the ConcurrentHashSet.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(T item) => _dict.TryAdd(item, 0);

        /// <summary>
        /// Removes the specified item from the ConcurrentHashSet.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item) => _dict.TryRemove(item, out _);

        /// <summary>
        /// Determines whether the ConcurrentHashSet contains a specific value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) => _dict.ContainsKey(item);

        /// <summary>
        /// Gets the number of elements contained in the ConcurrentHashSet.
        /// </summary>
        public int Count => _dict.Count;

        /// <summary>
        /// Initializes a new instance of the ConcurrentHashSet class.
        /// </summary>
        /// <param name="comparer"></param>
        public ConcurrentHashSet(IEqualityComparer<T>? comparer = null)
        {
            _dict = new ConcurrentDictionary<T, byte>(comparer);
        }
    }
}
