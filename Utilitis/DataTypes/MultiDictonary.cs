using System.Collections.Generic;

namespace HelpUtility.DataTypes
{
    public class MultiDictonary<T,V>
    {
        private List<KeyValuePair<T, V>> _data;
        public  MultiDictonary()
        {
            _data = new List<KeyValuePair<T, V>>();
        }

        public void Add(T key, V value)
        {
            _data.Add(new KeyValuePair<T, V>(key,value));
        }

        public void RemoveKey(T Key)
        {
            _data.RemoveAll(p => p.Key.Equals(Key));
        }

        public void RemoveValues(V value)
        {
            _data.RemoveAll(p => p.Value.Equals(value));
        }

        public List<V> GetValues(T key)
        {
            return _data.FindAll(p => p.Key.Equals(key)).ConvertAll(p => p.Value);
        }

        public bool ContainsKey(T key)
        {
            foreach (KeyValuePair<T, V> keyValuePair in _data)
            {
                if (Equals(keyValuePair.Key, key))
                    return true;
            }
           return false; 
        }
        public bool ContainsValue(V value)
        {
            foreach (KeyValuePair<T, V> keyValuePair in _data)
            {
                if (Equals(keyValuePair.Value, value))
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            _data.Clear();
        }
    }
}
