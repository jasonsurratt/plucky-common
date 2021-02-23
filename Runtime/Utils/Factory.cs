using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Plucky.Common
{
    /// Factory provides a simple class for retrieving and allocating classes/objects by name.
    public class Factory
    {
        static Dictionary<string, Type> classes = new Dictionary<string, Type>();

        /// Create an object from a class with the specified name.
        public static T Create<T>(string name)
        {
            Type type = Get(name);
            return (T)Activator.CreateInstance(type);
        }

        public static void DeepCopy<T>(T source, T destination) where T : class
        {
            FieldInfo[] fields = source.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty);
            Debug.Log(fields.Length);

            foreach (var fi in fields)
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }

        /// Find type with the full name of typeName. This will search all available assemblies and
        /// may be kinda slow.
        ///
        /// On failure a null is returned.
        public static Type Find(string typeName)
        {
            // Check the local assembly first. This will typically work and short circuit the 
            // search.
            Type result = Type.GetType(typeName);
            if (result != null)
            {
                return result;
            }

            // Remove any assembly information
            if (typeName.Contains(","))
            {
                typeName = Regex.Replace(typeName, @", [^,]*, Version[^\]]*PublicKeyToken=[^\s\]]*", "");
            }

            // go through all assemblies and types to find anything that matches.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                var t = assembly.GetType(typeName);
                if (t != null) return t;

                foreach (var type in types)
                {
                    if (type.FullName == typeName)
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        /// Get the specified type by name and cache the result. This should typically be faster
        /// than Find, but give the same result.
        ///
        /// On failure an exception is thrown.
        public static Type Get(string name)
        {
            Type t;
            if (classes.TryGetValue(name, out t))
            {
                return t;
            }
            t = Find(name);

            if (t == null)
            {
                throw new Exception("unexpected type name " + name);
            }

            classes[name] = t;

            return t;
        }
    }
}
