
using UnityEngine;

namespace Plucky.Common
{
    /// <summary>
    /// DistanceTransform transforms an raster by calcuating the distance from any pixel to the "best"
    /// pixel. The "best" pixel is the pixel that has the smallest value + distance. Distance is
    /// calculated as number of pixels away.
    /// 
    /// This is using an algorithm I previously knew as 8SED but I can no longer find that paper. :(
    /// 
    /// Runs in O(n) where n is the number of pixels
    /// </summary>
    public class DistanceTransform
    {
        /// <summary>
        /// Apply a distance transform to the specified raster. Typically v should contain zeros (or
        /// small values) at the source pixels and high values (much larger than raster size) at all
        /// other pixels.
        /// </summary>
        /// <param name="v">The raster to operate on.</param>
        /// <param name="source">An array for storing source pixels, specifying the array may reduce
        ///     memory churn but isn't necessary.</param>
        public static void Apply(float[,] v, Vector2Int[,] source = null)
        {
            if (source == null || source.GetLength(0) < v.GetLength(0) || source.GetLength(1) < v.GetLength(1))
            {
                source = new Vector2Int[v.GetLength(0), v.GetLength(1)];
            }
            for (int i = 0; i < v.GetLength(0); i++)
            {
                for (int j = 0; j < v.GetLength(1); j++)
                {
                    source[i, j] = new Vector2Int(i, j);
                }
            }
            Pass(v, source, 0, 0, 1, 1);
            Pass(v, source, v.GetLength(0) - 1, v.GetLength(1) - 1, -1, -1);
            Pass(v, source, v.GetLength(0) - 1, 0, -1, 1);
            Pass(v, source, 0, v.GetLength(1) - 1, 1, -1);
        }

        static void Pass(float[,] v, Vector2Int[,] source, int startX, int startY, int dx, int dy)
        {
            for (int y = startY; y < v.GetLength(1) && y >= 0; y += dy)
            {
                for (int x = startX; x < v.GetLength(0) && x >= 0; x += dx)
                {
                    bool xin = x - dx >= 0 && x - dx < v.GetLength(0);
                    bool yin = y - dy >= 0 && y - dy < v.GetLength(1);
                    if (xin && v[x - dx, y] + 1 < v[x, y])
                    {
                        float dist;
                        Vector2Int src = source[x - dx, y];
                        dist = Vector2.Distance(new Vector2(x, y), src) + v[src.x, src.y];

                        if (dist < v[x, y])
                        {
                            source[x, y] = src;
                            v[x, y] = dist;
                        }
                    }
                    if (yin && v[x, y - dy] + 1 < v[x, y])
                    {
                        float dist;
                        Vector2Int src = source[x, y - dy];
                        dist = Vector2.Distance(new Vector2(x, y), src) + v[src.x, src.y];

                        if (dist < v[x, y])
                        {
                            source[x, y] = src;
                            v[x, y] = dist;
                        }
                    }
                    if (xin && yin)
                    {
                        float dist;
                        Vector2Int src = source[x - dx, y - dy];
                        dist = Vector2.Distance(new Vector2(x, y), src) + v[src.x, src.y];

                        if (dist < v[x, y])
                        {
                            source[x, y] = src;
                            v[x, y] = dist;
                        }
                    }
                }
            }
        }
    }
}
