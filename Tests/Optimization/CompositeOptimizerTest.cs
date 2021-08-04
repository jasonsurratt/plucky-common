using NUnit.Framework;
using UnityEngine;

namespace Plucky.Common
{
    public class CompositeOptimizerTest
    {
        CompositeOptimizer CreateUut()
        {
            CompositeOptimizer uut = new CompositeOptimizer();

            var pass1 = new PseudoSimulatedAnnealing();
            pass1.populationSize = 5;
            pass1.candidatesPerIteration = 2;
            pass1.convergenceAbsolute = 10f;
            pass1.SetMinimumStepSize(50, 20, .005f);
            uut.pass1 = pass1;

            var pass2 = new NelderMead
            {
                guessSpread = 0.1f,
                convergenceAbsolute = 0.005f,
                maxIterations = 1000
            };
            uut.pass2 = pass2;


            return uut;
        }

        [Test]
        public void ParabolaTest()
        {
            NelderMeadTest.callCount = 0;
            float[] result = new float[2];
            CompositeOptimizer uut = CreateUut();

            result = uut.Optimize(NelderMeadTest.Parabola, new float[] { 20, 10 });
            Debug.Log($"Call Count: {NelderMeadTest.callCount} Solution: {result[0]}, {result[1]} ({NelderMeadTest.Parabola(result)})");

            //Assert.That(callCount, Is.LessThan(uut.iterations * uut.populationSize));

            // give the error check a bit of slop.
            Assert.That(NelderMeadTest.Parabola(result), Is.EqualTo(0).Within(0.01f));
        }

        [Test]
        public void RosenbrockTest()
        {
            float[] result = new float[2];
            CompositeOptimizer uut = CreateUut();
            var rng = new System.Random();

            int callCountSum = 0;
            int success = 0;
            int iterations = 100;
            for (int i = 0; i < iterations; i++)
            {
                NelderMeadTest.callCount = 0;
                result = uut.Optimize(NelderMeadTest.Rosenbrock, new float[] { 40f * (float)rng.NextDouble() - 20f, 40f * (float)rng.NextDouble() - 20f });

                if (NelderMeadTest.Rosenbrock(result) < 0.01f) success++;
                else
                    Debug.Log($"Call Count: {NelderMeadTest.callCount} Solution: {result[0]}, {result[1]} ({NelderMeadTest.Rosenbrock(result)})");

                callCountSum += NelderMeadTest.callCount;
            }

            Debug.Log($"iteration avg: {callCountSum / (float)iterations}");
            Debug.Log(success);

            //Assert.That(NelderMeadTest.callCount, Is.LessThan(uut.iterations * uut.populationSize * uut.candidatesPerIteration + 2));

            // give the error check a bit of slop.
            Assert.That(NelderMeadTest.Rosenbrock(result), Is.EqualTo(0).Within(0.01f));
        }
    }
}
