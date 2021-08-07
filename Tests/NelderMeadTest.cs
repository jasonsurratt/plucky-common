using NUnit.Framework;
using UnityEngine;

namespace Plucky.Common
{
    public class NelderMeadTest
    {
        public static int callCount = 0;

        public static float Rosenbrock(float[] x)
        {
            callCount++;
            return Mathf.Pow(1 - x[0], 2) + 100 * Mathf.Pow(x[1] - Mathf.Pow(x[0], 2), 2);
        }

        public static float Parabola(float[] x)
        {
            float result = 0;
            for (int i = 0; i < x.Length; i++)
            {
                result += Mathf.Pow(x[i] - (i + 1), 2);
            }
            callCount++;
            return result;
        }

        [Test]
        public void BasicTest()
        {
            callCount = 0;
            float[] result = new float[2];
            NelderMead uut = new NelderMead
            {
                convergenceDiff = 1e-2f,
                guessSpread = 5,
                maxIterations = 1000,
                sigma = 0.75f,
                gamma = 2
            };
            result = uut.Optimize(Parabola, new float[] { 20, 10 });
            Debug.Log($"Call Count: {callCount} Solution: {result[0]}, {result[1]} ({Parabola(result)})");

            Assert.IsTrue(callCount < 100);

            // give the error check a bit of slop.
            Assert.IsTrue(Parabola(result) < 2e-2f);
        }
    }
}
