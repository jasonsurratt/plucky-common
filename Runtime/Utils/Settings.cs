using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Plucky.Common
{
    /// <summary>
    /// Settings is a generic convenience structure for storing/retrieving key/value types. This is mostly
    /// used for flexibile user settings.
    /// 
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    public class Settings<Key>
    {
        Dictionary<Key, Variant> entries = new Dictionary<Key, Variant>();

        public Variant this[Key key]
        {
            get { return entries[key]; }
            set { entries[key] = value; }
        }

        public Variant Get(Key key) => entries[key];

        public Dictionary<Key, Variant>.Enumerator GetEnumerator() => entries.GetEnumerator();

        public float GetFloat(Key key, float defaultValue)
        {
            try
            {
                if (entries.ContainsKey(key)) return entries[key].floatValue;
            }
            catch (FormatException ex)
            {
                Debug.LogWarning($"Failure parsing: {key}, {entries[key]}, {ex.Message}");
                // ignore and return default.
            }
            return defaultValue;
        }

        public string GetString(Key key, string defaultValue)
        {
            if (entries.ContainsKey(key)) return entries[key].stringValue;
            return defaultValue;
        }

        public Vector3 GetVector3(Key key, Vector3 defaultValue)
        {
            if (entries.ContainsKey(key))
            {
                return entries[key].vector3Value;
            }

            return defaultValue;
        }
    }
}