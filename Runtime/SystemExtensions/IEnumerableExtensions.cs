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

        public static T PickOne<T>(this IEnumerable<T> coll)
        {
            switch (coll)
            {
                case IList<T> list:
                    return list[rng.Next() % list.Count];
                default:
                    return coll.ElementAt(rng.Next() % coll.Count());
            }
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
    }
}
