using System;

namespace Plucky.Common
{
    public class DataRef<T> where T : class, ICloneable, IReleasable, new()
    {
        public T data;
        public int refCount;
    }

    public class CopyOnWrite<T> where T : class, ICloneable, IReleasable, new()
    {
        protected T data
        {
            get
            {
                if (_data == null)
                {
                    _data = new DataRef<T>
                    {
                        data = ObjectPool<T>.instance.Get(),
                        refCount = 1,
                    };
                }
                return _data.data;
            }
        }
        DataRef<T> _data = null;

        public static implicit operator T(CopyOnWrite<T> d) => d._data.data;

        ~CopyOnWrite()
        {
            if (_data != null)
            {
                _data.refCount--;
                if (_data.refCount == 0) _data.data.Release();
            }
        }

        public void Copy(CopyOnWrite<T> other)
        {
            if (_data != null)
            {
                _data.refCount--;
                if (_data.refCount == 0) _data.data.Release();
            }
            _data = other._data;
            _data.refCount++;
        }

        protected CopyOnWrite<T> Copy()
        {
            _data.refCount++;
            return new CopyOnWrite<T>() { _data = _data };
        }

        protected void Writing()
        {
            if (_data == null || _data.refCount == 1) return;
            _data.refCount--;

            _data = new DataRef<T> { data = _data.data.Clone() as T, refCount = 1 };
        }
    }
}
