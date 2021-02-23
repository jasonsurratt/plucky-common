using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Plucky.Common
{
    public static class BinaryWriterExtensions
    {
        public static readonly Dictionary<Type, byte> typeToByte = new Dictionary<Type, byte>();
        public static readonly Dictionary<byte, Type> byteToType = new Dictionary<byte, Type>();

        static BinaryWriterExtensions()
        {
            void AddType(Type type, byte value)
            {
                typeToByte[type] = value;
                byteToType[value] = type;
            }

            byte key = 1;
            AddType(typeof(ulong), key++);
            AddType(typeof(uint), key++);
            AddType(typeof(ushort), key++);
            AddType(typeof(string), key++);
            AddType(typeof(float), key++);
            AddType(typeof(sbyte), key++);
            AddType(typeof(long), key++);
            AddType(typeof(int), key++);
            AddType(typeof(short), key++);
            AddType(typeof(decimal), key++);
            AddType(typeof(byte), key++);
            AddType(typeof(bool), key++);
            AddType(typeof(double), key++);
            AddType(typeof(char), key++);
            AddType(typeof(Vector3), key++);
            AddType(typeof(ICollection), key++);
        }

        public static object ReadWithType(this BinaryReader reader)
        {
            byte typeKey = reader.ReadByte();
            if (!byteToType.TryGetValue(typeKey, out Type type))
            {
                throw new IOException($"{typeKey} is an unexpected type.");
            }

            if (type == typeof(ulong)) return reader.ReadUInt64();
            if (type == typeof(uint)) return reader.ReadUInt32();
            if (type == typeof(ushort)) return reader.ReadUInt16();
            if (type == typeof(string)) return reader.ReadString();
            if (type == typeof(float)) return reader.ReadSingle();
            if (type == typeof(sbyte)) return reader.ReadSByte();
            if (type == typeof(long)) return reader.ReadInt64();
            if (type == typeof(int)) return reader.ReadInt32();
            if (type == typeof(short)) return reader.ReadInt16();
            if (type == typeof(decimal)) return reader.ReadDecimal();
            if (type == typeof(byte)) return reader.ReadByte();
            if (type == typeof(bool)) return reader.ReadBoolean();
            if (type == typeof(double)) return reader.ReadDouble();
            if (type == typeof(char)) return reader.ReadChar();
            if (type == typeof(Vector3))
            {
                return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            if (type == typeof(ICollection))
            {
                int size = reader.ReadInt32();
                List<object> result = new List<object>(size);
                for (int i = 0; i < size; i++)
                {
                    result.Add(ReadWithType(reader));
                }
                return result;
            }

            throw new ArgumentException($"unexpected type {type}");
        }

        public static void WriteWithType(this BinaryWriter writer, object obj)
        {
            if (!typeToByte.TryGetValue(obj.GetType(), out byte key))
            {
                foreach (var kvp in typeToByte)
                {
                    Type objType = obj.GetType();
                    if (kvp.Key.IsAssignableFrom(objType))
                    {
                        key = kvp.Value;
                    }
                }

                if (key == 0)
                {
                    throw new ArgumentException($"{obj.GetType()} is not supported by WriteWithType");
                }
            }
            writer.Write(key);

            switch (obj)
            {
                case ulong objulong:
                    writer.Write(objulong);
                    break;
                case uint objuint:
                    writer.Write(objuint);
                    break;
                case ushort objushort:
                    writer.Write(objushort);
                    break;
                case string objstring:
                    writer.Write(objstring);
                    break;
                case float objfloat:
                    writer.Write(objfloat);
                    break;
                case sbyte objsbyte:
                    writer.Write(objsbyte);
                    break;
                case long objlong:
                    writer.Write(objlong);
                    break;
                case int objint:
                    writer.Write(objint);
                    break;
                case short objshort:
                    writer.Write(objshort);
                    break;
                case decimal objdecimal:
                    writer.Write(objdecimal);
                    break;
                case byte objbyte:
                    writer.Write(objbyte);
                    break;
                case bool objbool:
                    writer.Write(objbool);
                    break;
                case double objdouble:
                    writer.Write(objdouble);
                    break;
                case char objchar:
                    writer.Write(objchar);
                    break;
                case Vector3 objvector3:
                    writer.Write(objvector3.x);
                    writer.Write(objvector3.y);
                    writer.Write(objvector3.z);
                    break;
                case ICollection objcoll:
                    writer.Write(objcoll.Count);
                    foreach (var item in objcoll)
                    {
                        writer.WriteWithType(item);
                    }
                    break;
            }
        }
    }
}
