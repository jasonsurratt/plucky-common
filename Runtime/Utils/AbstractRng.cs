using Plucky.Common;
using UnityEditor;
using UnityEngine;

namespace Plucky.Common
{
    public abstract class AbstractRng : IRng
    {
        public abstract IRng NewRng();

        public virtual double NextDouble()
        {
            // hmm, is this legit? I'm kind tired.
            return ((NextFloat() * 1e7) + NextFloat()) / (1e7 + 1);
        }

        public abstract float NextFloat();

        public virtual int NextInt()
        {
            return (int)(NextDouble() * (double)int.MaxValue);
        }

        public virtual float Range(float min, float max)
        {
            return Mathf.Lerp(min, max, NextFloat());
        }

        public virtual int Range(int min, int max) => NextInt() % (max - min) + min;

        public abstract void Reset();

        public Vector2 UnitCircle()
        {
            Vector2 result = new Vector2();
            do
            {
                result.x = NextFloat() * 2 - 1;
                result.y = NextFloat() * 2 - 1;
            } while (result.sqrMagnitude > 1);

            return result;
        }
    }
}