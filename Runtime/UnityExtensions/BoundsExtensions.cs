using UnityEngine;

namespace Plucky.Common
{
    public static class BoundsExtensions
    {
        public static Bounds max = new Bounds(Vector3.zero, Vector3.one * float.MaxValue / 2f);

        public static Bounds invalid = new Bounds(Vector3.zero, new Vector3(-1, -1, -1));

        public static bool Contains(this Bounds bounds, Bounds other)
        {
            if (!bounds.IsValid() || !other.IsValid()) return false;
            Vector3 otherMin = other.min;
            Vector3 boundsMin = bounds.min;
            Vector3 otherMax = other.max;
            Vector3 boundsMax = bounds.max;
            return otherMin.x >= boundsMin.x &&
                otherMin.y >= boundsMin.y &&
                otherMin.z >= boundsMin.z &&
                otherMax.x <= boundsMax.x &&
                otherMax.y <= boundsMax.y &&
                otherMax.z <= boundsMax.z;
        }

        /// <summary>
        /// Returns true if point is inside bounds, or lies on the minimum border.
        /// </summary>
        public static bool ContainsOrMinBorder(this Bounds bounds, Vector3 point)
        {
            if (!bounds.IsValid()) return false;
            Vector3 boundsMin = bounds.min;
            Vector3 boundsMax = bounds.max;
            bool result = point.x >= boundsMin.x &&
                point.y >= boundsMin.y &&
                point.z >= boundsMin.z &&
                point.x < boundsMax.x &&
                point.y < boundsMax.y &&
                point.z < boundsMax.z;

            return result;
        }

        /// <summary>
        /// Distance calculates the distance between two bounding boxes in 3 dimensions.
        /// </summary>
        public static float Distance(this Bounds bounds, Bounds other)
        {
            bool Overlap(float min1, float max1, float min2, float max2)
            {
                if (min1 >= min2 && min1 <= max2) return true;
                if (max1 >= min2 && max1 <= max2) return true;
                if (min1 <= min2 && max1 >= max2) return true;
                return false;
            }

            if (bounds.Intersects(other)) return 0;

            bool overlapX = Overlap(bounds.min.x, bounds.max.x, other.min.x, other.max.x);
            bool overlapY = Overlap(bounds.min.y, bounds.max.y, other.min.y, other.max.y);
            bool overlapZ = Overlap(bounds.min.z, bounds.max.z, other.min.z, other.max.z);

            if (overlapX && overlapY)
            {
                return Mathf.Min(
                    Mathf.Abs(bounds.max.z - other.min.z),
                    Mathf.Abs(bounds.min.z - other.max.z));
            }
            if (overlapX && overlapZ)
            {
                return Mathf.Min(
                    Mathf.Abs(bounds.max.y - other.min.y),
                    Mathf.Abs(bounds.min.y - other.max.y));
            }
            if (overlapY && overlapZ)
            {
                return Mathf.Min(
                    Mathf.Abs(bounds.max.x - other.min.x),
                    Mathf.Abs(bounds.min.x - other.max.x));
            }

            float d2 = Mathf.Min(new float[]
            {
                bounds.SqrDistance(other.min),
                bounds.SqrDistance(other.max),
                bounds.SqrDistance(new Vector3(other.max.x, other.min.y, other.min.z)),
                bounds.SqrDistance(new Vector3(other.min.x, other.max.y, other.min.z)),
                bounds.SqrDistance(new Vector3(other.max.x, other.max.y, other.min.z)),
                bounds.SqrDistance(new Vector3(other.min.x, other.min.y, other.max.z)),
                bounds.SqrDistance(new Vector3(other.max.x, other.min.y, other.max.z)),
                bounds.SqrDistance(new Vector3(other.min.x, other.max.y, other.max.z)),
            });

            return Mathf.Sqrt(d2);
        }

        public static Bounds Intersection(this Bounds bounds, Bounds other)
        {
            Bounds result = new Bounds();

            result.min = Vector3.Max(bounds.min, other.min);
            result.max = Vector3.Min(bounds.max, other.max);

            return result;
        }

        public static bool IsValid(this Bounds bounds)
        {
            if (bounds == null) return false;
            return bounds.size.x >= 0;
        }

        public static float Volume(this Bounds bounds)
        {
            if (!bounds.IsValid()) return -1;
            if (bounds == max) return float.MaxValue;
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }
    }
}
