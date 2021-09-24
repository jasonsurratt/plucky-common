using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Plucky.Common
{
    public enum DataType
    {
        Object,
        String,
        Int,
        Short,
        Float,
    }

    /// <summary>
    /// Initially I tried setting the location explicitly, but that caused errors with Vector3.
    /// This probably caused problems because Vector3 is some kinda of weird native type.
    /// </summary>
    public struct Variant
    {
        Type type;
        //[FieldOffset(0)] public object objectValue;

        object _value;

        float _floatValue;
        public float floatValue
        {
            get
            {
                if (type == typeof(string)) return Convert.ToSingle(_value);
                else if (type == typeof(float)) return _floatValue;
                else if (type == typeof(int)) return Convert.ToSingle(_intValue);
                else if (type == typeof(short)) return Convert.ToSingle(_shortValue);
                throw new ArgumentException();
            }
            set { type = typeof(int); _floatValue = value; }
        }

        int _intValue;
        public int intValue
        {
            get
            {
                if (type == typeof(string)) return Convert.ToInt32(_value);
                else if (type == typeof(float)) return Convert.ToInt32(_floatValue);
                else if (type == typeof(int)) return _intValue;
                else if (type == typeof(short)) return _shortValue;
                throw new ArgumentException();
            }
            set { type = typeof(int); _intValue = value; }
        }

        public string stringValue
        {
            get
            {
                if (type == typeof(string)) return Convert.ToString(_value);
                else if (type == typeof(float)) return Convert.ToString(_floatValue);
                else if (type == typeof(int)) return Convert.ToString(_intValue);
                else if (type == typeof(short)) return Convert.ToString(_shortValue);
                throw new ArgumentException();
            }
            set { type = typeof(string); _value = value; }
        }

        short _shortValue;
        public short shortValue
        {
            get
            {
                if (type == typeof(string)) return Convert.ToInt16(_value);
                else if (type == typeof(float)) return Convert.ToInt16(_floatValue);
                else if (type == typeof(int)) return Convert.ToInt16(_intValue);
                else if (type == typeof(short)) return _shortValue;
                throw new ArgumentException();
            }
            set { type = typeof(short); _shortValue = value; }
        }

        Vector3 _vector3Value;
        public Vector3 vector3Value
        {
            get
            {
                if (type == typeof(string))
                {
                    var splits = Convert.ToString(_value).Split(',');
                    if (splits.Length != 3) throw new ArgumentException("vetor3 must have 3 values");
                    Vector3 result = new Vector3();
                    result.x = Convert.ToSingle(splits[0]);
                    result.y = Convert.ToSingle(splits[1]);
                    result.z = Convert.ToSingle(splits[2]);
                    return result;
                }
                else if (type == typeof(Vector3)) return _vector3Value;
                throw new ArgumentException();
            }
            set { type = typeof(Vector3); _vector3Value = value; }
        }

        public static implicit operator Variant(float v)
        {
            var result = new Variant();
            result.floatValue = v;
            return result;
        }

        public static implicit operator Variant(int v)
        {
            var result = new Variant();
            result.intValue = v;
            return result;
        }

        public static implicit operator Variant(string v)
        {
            var result = new Variant();
            result.stringValue = v;
            return result;
        }

        public static implicit operator Variant(Vector3 v)
        {
            var result = new Variant();
            result.vector3Value = v;
            return result;
        }

        public override string ToString()
        {
            return stringValue;
        }
    }
}
