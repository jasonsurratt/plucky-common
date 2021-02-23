using System.Collections.Generic;
using System.Linq;

namespace Plucky.Common
{
    public static class IEnumerableExtensions
    {
        public static System.Random rng = new System.Random();

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
    }
}
