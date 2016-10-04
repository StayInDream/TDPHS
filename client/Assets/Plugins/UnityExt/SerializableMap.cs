using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace UnityExt
{
    [Serializable]
    public sealed class SerializableMap : IDisposable, IEnumerable<KeyValuePair<string, Object>>
    {
        [SerializeField]
        [HideInInspector]
        private List<string> keys;

        [SerializeField]
        [HideInInspector]
        private List<Object> values;

        [SerializeField]
        [HideInInspector]
        private string valueTypeName;

        public SerializableMap() : this(typeof(Object)) { }

        public SerializableMap(Type type) : this(type, 10) { }

        public SerializableMap(Type type, int capacity)
        {
            keys = new List<string>(capacity);
            values = new List<Object>(capacity);
            if (type == null)
                type = typeof(Object);
            valueTypeName = type.FullName;
        }

        public string ValueType
        {
            get { return valueTypeName; }
        }

        public Object this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                Object val;
                TryGetValue(key, out val);
                return val;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                int index = GetKeyIndex(key);
                if (index >= 0)
                {
                    // 修改
                    values[index] = value;
                }
                else
                {
                    // 新增
                    keys.Add(key);
                    values.Add(value);
                }
            }
        }


        public void Add(string key, Object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (ContainsKey(key))
                throw new ArgumentException("key already present in dictionary", "key");
            this[key] = value;
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public bool ContainsKey(string key)
        {
            return GetKeyIndex(key) >= 0;
        }

        public bool ContainsValue(Object value)
        {
            bool ret = false;
            for (int i = 0; i != values.Count; i++)
            {
                if (Equals(values[i], value))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }


        public void Dispose()
        {
            Clear();
            keys = null;
            values = null;
        }

        public bool Remove(string key)
        {
            int index = GetKeyIndex(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public TValue GetValue<TValue>(string key) where TValue : Object
        {
            return (this[key] as TValue);
        }

        public bool TryGetValue(string key, out Object value)
        {
            int index = GetKeyIndex(key);
            if (index >= 0)
            {
                value = values[index];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool TryGetValue<TValue>(string key, out TValue value) where TValue : Object
        {
            Object val;
            bool ret = TryGetValue(key, out val);
            value = val as TValue;
            return ret;
        }

        private int GetKeyIndex(string key)
        {
            return keys.IndexOf(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator<KeyValuePair<string, Object>> GetEnumerator()
        {
            return new Enumerator(this);
        }


        public struct Enumerator : IEnumerator<KeyValuePair<string, Object>>
        {
            private SerializableMap mMap;
            private int mIndex;
            private KeyValuePair<string, Object> mCurrent;


            public Enumerator(SerializableMap map)
            {
                mIndex = -1;
                mMap = map;
                mCurrent = new KeyValuePair<string, Object>(null, null);
            }

            public object Current
            {
                get
                {
                    return mCurrent;
                }
            }

            KeyValuePair<string, Object> IEnumerator<KeyValuePair<string, Object>>.Current
            {
                get
                {
                    return mCurrent;
                }
            }

            public void Dispose()
            {
                mMap.Dispose();
                mIndex = -1;
                mMap = null;
            }

            public bool MoveNext()
            {
                mIndex++;
                if (mIndex >= 0 && mIndex < mMap.keys.Count)
                {
                    mCurrent = new KeyValuePair<string, Object>(mMap.keys[mIndex], mMap.values[mIndex]);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                mIndex = -1;
                mCurrent = new KeyValuePair<string, Object>(null, null);
            }
        }
    }
}
