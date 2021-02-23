using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Plucky.Common
{
    public class RefCounter<T> where T : class, new()
    {
        public int count = 0;
        T reference = null;

        public RefCounter(T r)
        {
            reference = r;
            count = 1;
            Debug.Log(r);
        }

        ~RefCounter()
        {
            if (count > 0)
            {
                Debug.LogWarning($"{reference} is being destroyed with a positive ref count.");
            }
        }

        public void Release()
        {
            Debug.Log(reference);
            count--;

            if (count < 0)
            {
                Debug.LogError($"Reference count went below zero! {typeof(T)}");
            }
        }
    }

    /// <summary>
    /// OP is simply shorthand for ObjectPool.
    /// </summary>
    public static class OP<T> where T : class, new()
    {
        public static T Get() => ObjectPool<T>.instance.Get();
        public static void Release(T obj) => ObjectPool<T>.instance.Release(obj);
    }

    public class ObjectPool<T> where T : class
    {
        private ConcurrentBag<T> _objects = new ConcurrentBag<T>();
        private Action<T> _objectDestroyer;
        private Func<T> _objectGenerator;
        public int allocateChunks = 0;
        public int maxItems = -1;

        private static ObjectPool<T> _instance;
        public static ObjectPool<T> instance
        {
            get
            {
                if (_instance == null)
                {
                    // Does this type have a parameterless constructor?
                    ConstructorInfo constructor = typeof(T).GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                    {
                        throw new ArgumentException("ObjectPool type must have a parameterless constructor to use the default instance, if there is no parameterless constructor, create a custom instance with Create().");
                    }
                    _instance = new ObjectPool<T>(
                        delegate () { return constructor.Invoke(null) as T; },
                        delegate (T item) { });
                }
                return _instance;
            }
        }

        public static ObjectPool<T> Create(Func<T> objectGenerator, Action<T> objectDestroyer)
        {
            if (_instance == null)
            {
                _instance = new ObjectPool<T>(objectGenerator, objectDestroyer);
            }
            return _instance;
        }

        public ObjectPool(Func<T> objectGenerator, Action<T> objectDestroyer)
        {
            Assert.IsNotNull(objectDestroyer);
            Assert.IsNotNull(objectGenerator);
            _objectDestroyer = objectDestroyer;
            _objectGenerator = objectGenerator;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _instance = null;
        }

        public void Clear()
        {
            while (!_objects.IsEmpty)
            {
                if (_objects.TryTake(out T item))
                {
                    _objectDestroyer(item);
                }
            }
        }

        public T Get()
        {
            T item;
            if (_objects.TryTake(out item))
            {
                return item;
            }
            // allocate a chunk of objects all at once for serving. This may increase the odds
            // that the objects are in a similar section of RAM. Maybe? Will the GC just move them
            // around later? Dunno.
            for (int i = 0; i < allocateChunks - 1 && i < maxItems; i++)
            {
                Release(_objectGenerator());
            }
            return _objectGenerator();
        }

        public void Release(T item)
        {
            if ((maxItems < 0 || _objects.Count <= maxItems) && item != null)
            {
                if (item is IList c)
                {
                    c.Clear();
                }
                else if (item is IDictionary d)
                {
                    d.Clear();
                }
                _objects.Add(item);
            }
            else
            {
                _objectDestroyer(item);
            }
        }
    }
}
