using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plucky.Common
{
    /// <summary>
    /// A simple and efficient LRU cache.
    /// 
    /// Take from here: https://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary
    /// </summary>
    public class LruCache<K, V> : IEnumerable<KeyValuePair<K, V>>
    {
        public int capacity;
        private Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();

        public Action<K, V> evictionCallback = null;

        private LinkedList<LRUCacheItem<K, V>> lruList = new LinkedList<LRUCacheItem<K, V>>();

        ObjectPool<LinkedListNode<LRUCacheItem<K, V>>> llNodePool;

        public LruCache(int capacity)
        {
            this.capacity = capacity;

            // reuse the linked list nodes whenever possible.
            llNodePool = new ObjectPool<LinkedListNode<LRUCacheItem<K, V>>>(
                delegate ()
                {
                    LRUCacheItem<K, V> item = new LRUCacheItem<K, V>();
                    var node = new LinkedListNode<LRUCacheItem<K, V>>(item);
                    return node;
                },
                delegate (LinkedListNode<LRUCacheItem<K, V>> item) { }
            );
            llNodePool.maxItems = Math.Max(50, capacity + 1);
        }

        public bool Evict(K key)
        {
            bool removed = false;
            if (cacheMap.TryGetValue(key, out LinkedListNode<LRUCacheItem<K, V>> node))
            {
                cacheMap.Remove(node.Value.key);
                lruList.Remove(node);
                node.Value.Clear();
                llNodePool.Release(node);

                evictionCallback?.Invoke(node.Value.key, node.Value.value);
                removed = true;
            }

            return removed;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key)
        {
            LinkedListNode<LRUCacheItem<K, V>> node;
            if (cacheMap.TryGetValue(key, out node))
            {
                V value = node.Value.value;
                lruList.Remove(node);
                lruList.AddLast(node);
                return value;
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetCount() => cacheMap.Count;


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val)
        {
            if (cacheMap.Count >= capacity && capacity > 0)
            {
                RemoveOldest();
            }

            if (cacheMap.TryGetValue(key, out LinkedListNode<LRUCacheItem<K, V>> node))
            {
                node.Value.Init(key, val);
                lruList.Remove(node);
                lruList.AddLast(node);
            }
            else
            {
                LinkedListNode<LRUCacheItem<K, V>> newNode = llNodePool.Get();
                newNode.Value.Init(key, val);
                lruList.AddLast(newNode);
                cacheMap[key] = newNode;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public KeyValuePair<K, V> RemoveOldest()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem<K, V>> node = lruList.First;
            if (node == null)
            {
                return new KeyValuePair<K, V>();
            }
            lruList.RemoveFirst();

            // Remove from cache
            cacheMap.Remove(node.Value.key);

            evictionCallback?.Invoke(node.Value.key, node.Value.value);

            var result = new KeyValuePair<K, V>(node.Value.key, node.Value.value);
            node.Value.Clear();
            llNodePool.Release(node);

            return result;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            lock (this)
            {
                foreach (var value in cacheMap.Values)
                {
                    yield return new KeyValuePair<K, V>(value.Value.key, value.Value.value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class LRUCacheItem<K, V>
    {
        public LRUCacheItem() { }

        public LRUCacheItem<K, V> Init(K k, V v)
        {
            key = k;
            value = v;
            return this;
        }

        public K key;
        public V value;

        public void Clear()
        {
            key = default;
            value = default;
        }
    }
}
