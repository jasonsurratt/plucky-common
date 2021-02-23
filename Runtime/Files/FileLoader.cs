using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Plucky.Common
{
    public class FileLoader
    {
        /// <summary>
        /// LoadFilesRecursively recursively finds all files with the specified extension in the
        /// specified path and loads them into strings. Each file is loaded and returned while
        /// iterating to avoid loading them all at once.
        /// </summary>
        /// <param name="path">Directory to load</param>
        /// <param name="endsWith">file extension, the extension should include the '.', if necessary.</param>
        /// <returns></returns>
        public static IEnumerable<string> LoadFilesRecursively(string path, string endsWith)
        {
            var files = Directory.GetFiles(path).Where(x => x.ToUpper().EndsWith(endsWith.ToUpper()));
            var sorted = files.ToList();
            sorted.Sort();

            foreach (var f in sorted)
            {
                string txt = null;
                try
                {
                    txt = File.ReadAllText(f, System.Text.Encoding.UTF8);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load file: {e}");
                }
                if (txt != null) yield return txt;
            }

            string[] dirs = Directory.GetDirectories(path);
            foreach (var d in dirs)
            {
                var it = LoadFilesRecursively(d, endsWith).GetEnumerator();
                while (it.MoveNext()) yield return it.Current;
            }
        }
    }
}
