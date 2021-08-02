using System;
using System.Collections.Generic;
using System.Linq;

namespace Plucky.Common
{
    public class Randomize
    {
        public static System.Random rng = new System.Random();

        public static ulong RandULong()
        {
            ulong lower = (uint)rng.Next();
            ulong upper = (uint)rng.Next();

            return (upper << 32) | lower;
        }

        public static IList<T> List<T>(IList<T> arr)
        {
            return List(rng, arr);
        }

        public static IList<T> List<T>(System.Random rn, IList<T> arr)
        {
            for (int j = 0; j < arr.Count; j++)
            {
                int s1 = rn.Next() % arr.Count;
                int s2 = rn.Next() % arr.Count;
                T tmp = arr[s1];
                arr[s1] = arr[s2];
                arr[s2] = tmp;
            }

            return arr;
        }

        public static void WeightedRandomize<T>(float[] weights, List<T> input, List<T> result) => WeightedRandomize<T>(rng, weights, input, result);

        /// <summary>
        /// Randomize the given list (result) using weights. Indexes with higher weights are more
        /// likely to be at the beginning of the list.
        /// </summary>
        public static void WeightedRandomize<T>(System.Random rng, float[] weights, List<T> input, List<T> result)
        {
            float sum = weights.Sum();

            float[] samples = new float[weights.Length];
            List<int> indices = new List<int>(weights.Length);

            for (int i = 0; i < weights.Length; i++)
            {
                // negative to give descending order
                samples[i] = -weights[i] * (float)rng.NextDouble();
                indices.Add(i);
            }

            int Compare(int x, int y)
            {
                return samples[x].CompareTo(samples[y]);
            }

            indices.Sort(Compare);

            result.Clear();
            result.Capacity = Math.Max(result.Capacity, input.Count);
            for (int i = 0; i < indices.Count; i++)
            {
                result.Add(input[indices[i]]);
            }
        }

        public static void PickN<T>(float[] weights, int n, List<T> result) => PickN(rng, weights, n, result);

        public static void PickN<T>(System.Random rng, float[] weights, int n, List<T> result)
        {
            List<T> tmp = new List<T>(result.Count);
            WeightedRandomize<T>(rng, weights, result, tmp);

            if (tmp.Count > n) tmp.RemoveRange(n, tmp.Count - n);
            result.Clear();
            result.AddRange(tmp);
        }

        public static int PickIndex(float[] weights) => PickIndex(rng, weights);

        /// <summary>
        /// PickIndex returns a random weighted index.
        /// </summary>
        public static int PickIndex(System.Random rng, float[] weights)
        {
            float sum = weights.Sum();

            // if there are no weights, return a random index
            if (sum == 0)
            {
                return rng.Next() % weights.Length;
            }

            float pick = (float)rng.NextDouble() * sum;
            float runSum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                runSum += weights[i];
                if (pick <= runSum)
                {
                    return i;
                }
            }
            return weights.Length - 1;
        }
    }
}
