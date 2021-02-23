using UnityEngine;

namespace Plucky.Common
{
    public static class Texture2DExtensions
    {
        public static Texture2D RoundCorners(this Texture2D sourceTex, float radius)
        {
            int h = sourceTex.height;
            int w = sourceTex.width;
            Color[] c = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
            Texture2D b = new Texture2D(w, h);

            Vector2[] cornerPoints = new Vector2[] {
                new Vector2(radius, radius),
                new Vector2(radius, h - radius - 1),
                new Vector2(w - radius - 1, radius),
                new Vector2(w - radius - 1, h - radius - 1),
            };

            int i = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    i++;
                    if ((x < radius || x >= w - radius) && (y < radius || y >= h - radius))
                    {
                        bool inRadius = false;
                        Vector2 p = new Vector2(x, y);
                        foreach (Vector2 cp in cornerPoints)
                        {
                            if (Vector2.Distance(cp, p) < radius)
                            {
                                inRadius = true;
                            }
                        }
                        if (!inRadius)
                        {
                            b.SetPixel(x, y, Color.clear);
                        }
                        else
                        {
                            b.SetPixel(x, y, c[i]);
                        }
                    }
                    else
                    {
                        b.SetPixel(x, y, c[i]);
                    }
                }
            }
            b.Apply();
            return b;
        }
    }
}
