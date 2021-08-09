using UnityEditor;
using UnityEngine;

namespace Plucky.Common
{
    public class SmoothRng : AbstractRng
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

        public override IRng NewRng()
        {
            return new SmoothRng(Range(0, 100), offset);
        }

        public override float NextFloat()
        {
            float result = Mathf.Clamp01(Mathf.PerlinNoise(point.x + offset, point.y));
            point += sampleDelta;
            return result;
        }

        /// <summary>
        /// Reset the RNG to the state with the specified seed/offset
        /// </summary>
        public override void Reset()
        {
            point = new Vector2((seed * 1.3215842f) % 100, (seed * 0.8235121f) % 100);
        }
    }
}