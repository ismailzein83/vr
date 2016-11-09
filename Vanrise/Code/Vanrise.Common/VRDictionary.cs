using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public class VRDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public VRDictionary() : this(false)
        {

        }

        bool _isConcurrent;
        public VRDictionary(bool isConcurrent)
        {
            _isConcurrent = isConcurrent;
        }

        ConcurrentDictionary<TKey, TValue> _localDictionary = new ConcurrentDictionary<TKey, TValue>();
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new Exception(String.Format("Cannot add Item to dictionary. Item Key '{0}'", key));
            //if (_isConcurrent)
            //{
            //    lock(_localDictionary)
            //    {
            //        Add_Private(key, value, false);
            //    }
            //}
            //else
            //    Add_Private(key, value, false);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return _localDictionary.TryAdd(key, value);
            //if (_isConcurrent)
            //{
            //    lock (_localDictionary)
            //    {
            //        return Add_Private(key, value, true);
            //    }
            //}
            //else
            //    return Add_Private(key, value, true);
                        
        }

        //bool Add_Private(TKey key, TValue value, bool addIfNotExists)
        //{
        //    if (!addIfNotExists || !_localDictionary.ContainsKey(key))
        //    {
        //        _localDictionary.Add(key, value);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        public bool ContainsKey(TKey key)
        {
            return _localDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _localDictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            TValue dummy;
            return _localDictionary.TryRemove(key, out dummy);
            //if (_isConcurrent)
            //{
            //    lock (_localDictionary)
            //    {
            //        return Remove_Private(key);
            //    }
            //}
            //else
            //    return Remove_Private(key);
        }

        //bool Remove_Private(TKey key)
        //{
        //    return _localDictionary.Remove(key);
        //}

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _localDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _localDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _localDictionary[key];
            }
            set
            {
                _localDictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (_isConcurrent)
            {
                lock (_localDictionary)
                    _localDictionary.Clear();
            }
            else
                _localDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {            
            return _localDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey,TValue>)_localDictionary).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _localDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary<TKey, TValue>)_localDictionary).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)_localDictionary).Remove(item);
            //if (_isConcurrent)
            //{
            //    lock (_localDictionary)
            //        return _localDictionary.Remove(item);
            //}
            //else
            //    return _localDictionary.Remove(item);           
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _localDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _localDictionary.GetEnumerator();
        }
    }
}
