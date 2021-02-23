using NUnit.Framework;
using UnityEngine;

namespace Plucky.Common
{
    public class PseudoSimulatedAnnealingTest
    {
        int callCount = 0;

        public float Rosenbrock(float[] x)
        {
            callCount++;
            return Mathf.Pow(1 - x[0], 2) + 100 * Mathf.Pow(x[1] - Mathf.Pow(x[0], 2), 2);
        }

        public float Parabola(float[] x)
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
        public void ParabolaTest()
        {
            callCount = 0;
            float[] result = new float[2];
            PseudoSimulatedAnnealing uut = new PseudoSimulatedAnnealing();
            uut.populationSize = 5;
            uut.candidatesPerIteration = 2;
            uut.SetMinimumStepSize(100, 20, 0.005f);
            result = uut.Optimize(Parabola, new float[] { 20, 10 });
            Debug.Log($"Call Count: {callCount} Solution: {result[0]}, {result[1]} ({Parabola(result)})");

            Assert.That(callCount, Is.LessThan(uut.iterations * uut.populationSize * uut.candidatesPerIteration + 2));

            // give the error check a bit of slop.
            Assert.That(Parabola(result), Is.EqualTo(0).Within(0.01f));
        }

        [Test]
        public void RosenbrockTest()
        {
            float[] result = new float[2];
            PseudoSimulatedAnnealing uut = new PseudoSimulatedAnnealing();
            uut.populationSize = 10;
            uut.candidatesPerIteration = 2;
            uut.SetMinimumStepSize(100, 20, 0.005f);

            int success = 0;
            for (int i = 0; i < 100; i++)
            {
                callCount = 0;
                result = uut.Optimize(Rosenbrock, new float[] { 20, 10 });

                if (Rosenbrock(result) < 0.01f) success++;
                else
                    Debug.Log($"Call Count: {callCount} Solution: {result[0]}, {result[1]} ({Rosenbrock(result)})");
            }

            Debug.Log(success);

            Assert.That(callCount, Is.LessThan(uut.iterations * uut.populationSize * uut.candidatesPerIteration + 2));

            // give the error check a bit of slop.
            Assert.That(Rosenbrock(result), Is.EqualTo(0).Within(0.01f));
        }
    }
}
