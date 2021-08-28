using System.Collections.Generic;
using System.Linq;

namespace Plucky.Common
{
    public static class IEnumerableExtensions
    {
        public static System.Random rng = new System.Random();

        public static IEnumerable<T> PickN<T>(this IEnumerable<T> coll, IRng rng, int n)
        {
            return coll.RandSort(rng).Take(n);
        }

        public static IEnumerable<T> PickN<T>(this IEnumerable<T> coll, int n)
        {
            return coll.RandSort().Take(n);
        }

        public static T PickOne<T>(this IEnumerable<T> coll, IRng rng)
        {
            switch (coll)
            {
                case IList<T> list:
                    return list[rng.NextInt() % list.Count];
                default:
                    return coll.ElementAt(rng.NextInt() % coll.Count());
            }
        }

        public static T PickOne<T>(this IEnumerable<T> coll)
        {
            return coll.PickOne<T>(new SystemRng(rng.Next()));
        }

        public static IList<T> RandSort<T>(this IEnumerable<T> coll)
        {
            var list = coll.ToList();
            return Randomize.List(list);
        }

        public static IList<T> RandSort<T>(this IEnumerable<T> coll, IRng rng)
        {
            var list = coll.ToList();
            return Randomize.List(rng, list);
        }

        public static T[] Sample<T>(this IEnumerable<T> coll, int n, IRng rng)
        {
            T[] result = new T[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = coll.PickOne();
            }
            return result;
        }

        public static T[] Sample<T>(this IEnumerable<T> coll, int n, float[] weights, IRng rng = null)
        {
            if (rng == null) rng = SystemRng.sys;
            T[] result = new T[n];
            for (int i = 0; i < n; i++)
            {
                int index = Randomize.PickIndex(rng, weights);
                result[i] = coll.ElementAt(index);
            }
            return result;
        }
    }
}
