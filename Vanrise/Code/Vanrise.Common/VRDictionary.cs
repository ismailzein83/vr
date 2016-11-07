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
        ConcurrentDictionary<TKey, TValue> _localDictionary = new ConcurrentDictionary<TKey, TValue>();
        public void Add(TKey key, TValue value)
        {
            _localDictionary.TryAdd(key, value);
        }

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
        }

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
            _localDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue val;
            return _localDictionary.TryGetValue(item.Key, out val) && ((val == null && item.Value == null) || val.Equals(item.Value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)_localDictionary).CopyTo(array, arrayIndex);
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
