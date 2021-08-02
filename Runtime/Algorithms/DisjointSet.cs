using System.Collections.Generic;

namespace Plucky.Common
{
    /// <summary>
    /// DisjointSet is a data structure for joining sets together. By joining any two elements that
    /// are in a set, you join the two parent sets.
    /// 
    /// https://en.wikipedia.org/wiki/Disjoint-set_data_structure
    /// </summary>
    public class DisjointSet
    {
        public struct Element
        {
            public int rank;
            public int p;
            public int size;
        }

        Element[] elements;

        public DisjointSet(int size)
        {
            elements = new Element[size];

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].rank = 0;
                elements[i].p = i;
                elements[i].size = 1;
            }
        }

        public int Find(int x)
        {
            int y = x;
            while (y != elements[y].p)
            {
                y = elements[y].p;
            }
            elements[x].p = y;
            return y;
        }

        public IEnumerable<int> GetSet(int x)
        {
            int y = Find(x);
            for (int i = 0; i < elements.Length; i++)
            {
                if (Find(i) == y) yield return i;
            }
        }

        public void Join(int x, int y)
        {
            x = Find(x);
            y = Find(y);

            if (elements[x].rank > elements[y].rank)
            {
                elements[y].p = x;
                elements[x].size += elements[y].size;
            }
            else
            {
                elements[x].p = y;
                elements[y].size += elements[x].size;
                if (elements[x].rank == elements[y].rank)
                {
                    elements[y].rank++;
                }
            }
        }
    }
}
