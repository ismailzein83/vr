using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching
{
    public class CacheStore
    {
        bool _hasFirstItem;
        Object _firstItemKey;
        CachedObject _firstItemValue;
        ConcurrentDictionary<Object, CachedObject> _dictionary = new ConcurrentDictionary<object, CachedObject>();

        public DateTime LastExpirationCheckTime { get; set; }

        public void Add(Object key, CachedObject cachedObject)
        {
            lock (this)
            {
                if (!_hasFirstItem)
                {
                    _firstItemKey = key;
                    _firstItemValue = cachedObject;
                    _hasFirstItem = true;
                }
                else
                {
                    _dictionary.TryAdd(key, cachedObject);
                }
            }
        }

        public bool TryGetValue(Object key, out CachedObject cachedObject)
        {
            if (_hasFirstItem && _firstItemKey == key)
            {
                cachedObject = _firstItemValue;
                return true;
            }
            else
                return _dictionary.TryGetValue(key, out cachedObject);
        }

        public void TryRemove(Object key)
        {
            lock (this)
            {
                if (_hasFirstItem && _firstItemKey == key)
                {
                    _firstItemKey = null;
                    _firstItemValue = null;
                    _hasFirstItem = false;
                }
                else
                {
                    CachedObject dummy;
                    _dictionary.TryRemove(key, out dummy);
                }
            }
        }

        public IEnumerable<CachedObject> Values
        {
            get
            {
                List<CachedObject> allValues = _dictionary.Values.ToList();
                if (_hasFirstItem)
                    allValues.Add(_firstItemValue);
                return allValues;
            }
        }

        internal void Clear()
        {
            lock (this)
            {
                _firstItemKey = null;
                _firstItemValue = null;
                _hasFirstItem = false;
                _dictionary.Clear();
            }
        }
    }

    //public class CacheStore
    //{
    //    bool _hasFirstItem;
    //    Object _firstItemKey;
    //    CachedObject _firstItemValue;
    //    List<CachedObject> _list = new List<CachedObject>();

    //    public void Add(Object key, CachedObject cachedObject)
    //    {
    //        lock (this)
    //        {
    //            if (!_hasFirstItem)
    //            {
    //                _firstItemKey = key;
    //                _firstItemValue = cachedObject;
    //                _hasFirstItem = true;
    //            }
    //            else
    //            {
    //                _list.Add(cachedObject);
    //            }
    //        }
    //    }

    //    public bool TryGetValue(Object key, out CachedObject cachedObject)
    //    {
    //        if (_hasFirstItem && _firstItemKey == key)
    //        {
    //            cachedObject = _firstItemValue;
    //            return true;
    //        }
    //        else
    //        {
    //            cachedObject = GetFromList(key);
    //            return cachedObject != null;
    //        }
    //    }

    //    CachedObject GetFromList(Object key)
    //    {
    //        int count = _list.Count;
    //        for (int i = 0; i < count; i++)
    //        {
    //            var obj = _list[i];
    //            if (obj.CacheName.Equals(key))
    //                return obj;
    //        }
    //        return null;
    //    }

    //    public void TryRemove(Object key)
    //    {
    //        lock (this)
    //        {
    //            if (_hasFirstItem && _firstItemKey == key)
    //            {
    //                _firstItemKey = null;
    //                _firstItemValue = null;
    //                _hasFirstItem = false;
    //            }
    //            else
    //            {
    //                var cachedObject = GetFromList(key);
    //                if (cachedObject != null)
    //                    _list.Remove(cachedObject);
    //            }
    //        }
    //    }

    //    public IEnumerable<CachedObject> Values
    //    {
    //        get
    //        {
    //            List<CachedObject> allValues = new List<CachedObject>();
    //            allValues.AddRange(_list);
    //            if (_hasFirstItem)
    //                allValues.Add(_firstItemValue);
    //            return allValues;
    //        }
    //    }

    //    internal void Clear()
    //    {
    //        lock (this)
    //        {
    //            _firstItemKey = null;
    //            _firstItemValue = null;
    //            _hasFirstItem = false;
    //            _list.Clear();
    //        }
    //    }
    //}



    //public class CacheStore
    //{
    //    bool _hasFirstItem;
    //    Object _firstItemKey;
    //    CachedObject _firstItemValue;
    //    Hashtable _dictionary = new Hashtable();
    //    public void Add(Object key, CachedObject cachedObject)
    //    {
    //        lock (this)
    //        {
    //            if (!_hasFirstItem)
    //            {
    //                _firstItemKey = key;
    //                _firstItemValue = cachedObject;
    //                _hasFirstItem = true;
    //            }
    //            else
    //            {
    //                _dictionary.Add(key, cachedObject);
    //            }
    //        }
    //    }

    //    public bool TryGetValue(Object key, out CachedObject cachedObject)
    //    {
    //        if (_hasFirstItem && _firstItemKey == key)
    //        {
    //            cachedObject = _firstItemValue;
    //            return true;
    //        }
    //        else
    //        {
    //            cachedObject = _dictionary[key] as CachedObject;
    //            return cachedObject != null;
    //        }
    //    }

    //    public void TryRemove(Object key)
    //    {
    //        lock (this)
    //        {
    //            if (_hasFirstItem && _firstItemKey == key)
    //            {
    //                _firstItemKey = null;
    //                _firstItemValue = null;
    //                _hasFirstItem = false;
    //            }
    //            else
    //            {
    //                if (_dictionary.ContainsKey(key))
    //                    _dictionary.Remove(key);
    //            }
    //        }
    //    }

    //    public IEnumerable<CachedObject> Values
    //    {
    //        get
    //        {
    //            List<CachedObject> allValues = new List<CachedObject>();
    //            foreach (CachedObject val in _dictionary.Values)
    //            {
    //                allValues.Add(val);
    //            }
    //            if (_hasFirstItem)
    //                allValues.Add(_firstItemValue);
    //            return allValues;
    //        }
    //    }

    //    internal void Clear()
    //    {
    //        lock (this)
    //        {
    //            _firstItemKey = null;
    //            _firstItemValue = null;
    //            _hasFirstItem = false;
    //            _dictionary.Clear();
    //        }
    //    }
    //}

    //public class CacheStore
    //{
    //    bool _hasFirstItem;
    //    Object _firstItemKey;
    //    CachedObject _firstItemValue;
    //    SortedList<Object, CachedObject> _sortedDictionary = new SortedList<object, CachedObject>();

    //    ConcurrentDictionary<Object, CachedObject> _dictionary = new ConcurrentDictionary<object, CachedObject>();
    //    public void Add(Object key, CachedObject cachedObject)
    //    {
    //        lock (this)
    //        {
    //            if (!_hasFirstItem)
    //            {
    //                _firstItemKey = key;
    //                _firstItemValue = cachedObject;
    //                _hasFirstItem = true;
    //            }
    //            else
    //            {
    //                if (key is string)
    //                    _sortedDictionary.Add(key, cachedObject);
    //                else
    //                    _dictionary.TryAdd(key, cachedObject);
    //            }
    //        }
    //    }

    //    public bool TryGetValue(Object key, out CachedObject cachedObject)
    //    {
    //        if (_hasFirstItem && _firstItemKey == key)
    //        {
    //            cachedObject = _firstItemValue;
    //            return true;
    //        }
    //        else
    //            return key is string ? _sortedDictionary.TryGetValue(key, out cachedObject) : _dictionary.TryGetValue(key, out cachedObject);
    //    }

    //    public void TryRemove(Object key)
    //    {
    //        lock (this)
    //        {
    //            if (_hasFirstItem && _firstItemKey == key)
    //            {
    //                _firstItemKey = null;
    //                _firstItemValue = null;
    //                _hasFirstItem = false;
    //            }
    //            else
    //            {
    //                if (key is string)
    //                {
    //                    if (_sortedDictionary.ContainsKey(key)) ;
    //                    _sortedDictionary.Remove(key);
    //                }
    //                else
    //                {
    //                    CachedObject dummy;
    //                    _dictionary.TryRemove(key, out dummy);
    //                }
    //            }
    //        }
    //    }

    //    public IEnumerable<CachedObject> Values
    //    {
    //        get
    //        {
    //            List<CachedObject> allValues = _sortedDictionary.Values.ToList();
    //            allValues.AddRange(_dictionary.Values);
    //            if (_hasFirstItem)
    //                allValues.Add(_firstItemValue);
    //            return allValues;
    //        }
    //    }

    //    internal void Clear()
    //    {
    //        lock (this)
    //        {
    //            _firstItemKey = null;
    //            _firstItemValue = null;
    //            _hasFirstItem = false;
    //            _sortedDictionary.Clear();
    //            _dictionary.Clear();
    //        }
    //    }
    //} 

    //public class CacheStore
    //{
    //    bool _hasFirstItem;
    //    Object _firstItemKey;
    //    CachedObject _firstItemValue;
    //    List<Object> _keys = new List<object>();
    //    List<CachedObject> _values = new List<CachedObject>();

    //    public void Add(Object key, CachedObject cachedObject)
    //    {
    //        lock (this)
    //        {
    //            if (!_hasFirstItem)
    //            {
    //                _firstItemKey = key;
    //                _firstItemValue = cachedObject;
    //                _hasFirstItem = true;
    //            }
    //            else
    //            {
    //                _keys.Add(key);
    //                _values.Add(cachedObject);
    //            }
    //        }
    //    }

    //    public bool TryGetValue(Object key, out CachedObject cachedObject)
    //    {
    //        if (_hasFirstItem && _firstItemKey == key)
    //        {
    //            cachedObject = _firstItemValue;
    //            return true;
    //        }
    //        else
    //        {
    //             cachedObject = GetFromList(key);
    //            return cachedObject != null;
    //        }
    //    }

    //    CachedObject GetFromList(Object key)
    //    {
    //        int keysCount = _keys.Count;
    //        for(int i=0;i<keysCount;i++)
    //        {
    //            if (_keys[i].Equals(key))
    //                return _values[i];
    //        }
    //        return null;
    //    }

    //    public void TryRemove(Object key)
    //    {
    //        lock (this)
    //        {
    //            if (_hasFirstItem && _firstItemKey == key)
    //            {
    //                _firstItemKey = null;
    //                _firstItemValue = null;
    //                _hasFirstItem = false;
    //            }
    //            else
    //            {
    //                var index = _keys.IndexOf(key);
    //                if (index >= 0)
    //                {
    //                    _keys.RemoveAt(index);
    //                    _values.RemoveAt(index);
    //                }
    //            }
    //        }
    //    }

    //    public IEnumerable<CachedObject> Values
    //    {
    //        get
    //        {
    //            List<CachedObject> allValues = new List<CachedObject>();
    //            allValues.AddRange(_values);
    //            if (_hasFirstItem)
    //                allValues.Add(_firstItemValue);
    //            return allValues;
    //        }
    //    }

    //    internal void Clear()
    //    {
    //        lock (this)
    //        {
    //            _firstItemKey = null;
    //            _firstItemValue = null;
    //            _hasFirstItem = false;
    //            _keys.Clear();
    //            _values.Clear();
    //        }
    //    }
    //}
}
