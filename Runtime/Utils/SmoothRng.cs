using UnityEditor;
using UnityEngine;

namespace Plucky.Common
{
    public class SmoothRng : IRng
    {
        public float seed = 0;
        public float offset;
        public Vector2 point;
        public Vector2 sampleDelta = new Vector2(Mathf.PI * 2, Mathf.Exp(1) * 3);

        public SmoothRng(float seed, float offset)
        {
            this.seed = seed;
            this.offset = offset;
            Reset();
        }

        public IRng NewRng()
        {
            return new SmoothRng(Range(0, 100), offset);
        }

        public float NextFloat()
        {
            float result = Mathf.Clamp01(Mathf.PerlinNoise(point.x + offset, point.y));
            point += sampleDelta;
            return result;
        }

        public double NextDouble()
        {
            // hmm, is this legit? I'm kind tired.
            return ((NextFloat() * 1e7) + NextFloat()) / 1e7;
        }

        public int NextInt()
        {
            return (int)(NextDouble() * (double)int.MaxValue);
        }

        public float Range(float min, float max)
        {
            return Mathf.Lerp(min, max, NextFloat());
        }

        public int Range(int min, int max) => NextInt() % (max - min) + min;

        /// <summary>
        /// Reset the RNG to the state with the specified seed/offset
        /// </summary>
        public void Reset()
        {
            point = new Vector2((seed * 1.3215842f) % 100, (seed * 0.8235121f) % 100);
        }
    }
}