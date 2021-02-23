using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic Heap/PriorityQueue.
/// 
/// Take from https://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
/// </summary>
/// <typeparam name="T"></typeparam>
public class PriorityQueue<T> : ICloneable, IEnumerable<T>, IEnumerable where T : IComparable<T>
{
    private List<T> data = new List<T>();

    public IReadOnlyList<T> dataConst => data;

    public void Clear()
    {
        data.Clear();
    }

    public object Clone()
    {
        PriorityQueue<T> result = new PriorityQueue<T>();
        result.Copy(this);
        return result;
    }

    public void Copy(PriorityQueue<T> from)
    {
        Clear();

        if (typeof(ICloneable).IsAssignableFrom(typeof(T)) &&
            typeof(T).IsClass)
        {
            data.Capacity = Mathf.Max(data.Capacity, from.data.Capacity);
            for (int i = 0; i < from.data.Count; i++)
            {
                data.Add((T)((ICloneable)from.data[i]).Clone());
            }
        }
        else
        {
            data.AddRange(from.data);
        }
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1; // child index; start at end
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // parent index
            if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
            T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        if (data.Count == 0)
        {
            throw new Exception("empty queue");
        }
        // assumes pq is not empty; up to calling code
        int li = data.Count - 1; // last index (before removal)
        T frontItem = data[0];   // fetch the front
        data[0] = data[li];
        data.RemoveAt(li);
        if (data.Count == 0)
        {
            return frontItem;
        }

        --li; // last index (after removal)
        int pi = 0; // parent index. start at front of pq
        int watchdog = Count() + 10;
        while (true && watchdog > 0)
        {
            watchdog--;
            int ci = pi * 2 + 1; // left child index of parent
            if (ci > li) break;  // no children so done
            int rc = ci + 1;     // right child
            if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                ci = rc;
            if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
            T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
            pi = ci;
        }
        if (watchdog == 0)
        {
            Debug.LogError($"Count: {data.Count}");
            if (!IsConsistent())
            {
                Debug.LogError("Priority queue is not consistent.");
            }
            throw new Exception("Watchdog hit in priority queue.");
        }
        return frontItem;
    }

    public IEnumerator<T> GetEnumerator() => data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();

    public T Peek()
    {
        T frontItem = data[0];
        return frontItem;
    }

    public int Count()
    {
        return data.Count;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < data.Count; ++i)
            s += data[i].ToString() + " ";
        s += "count = " + data.Count;
        return s;
    }

    public bool IsConsistent()
    {
        // is the heap property true for all data?
        if (data.Count == 0) return true;
        int li = data.Count - 1; // last index
        for (int pi = 0; pi < data.Count; ++pi) // each parent index
        {
            int lci = 2 * pi + 1; // left child index
            int rci = 2 * pi + 2; // right child index

            if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
            if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
        }
        return true; // passed all checks
    } // IsConsistent

} // PriorityQueue
