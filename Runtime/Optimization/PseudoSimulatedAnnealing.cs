using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plucky.Common
{
    public class PseudoSimulatedAnnealing : IOptimizer
    {
        class Sample : IComparable<Sample>
        {
            public float[] values;
            public float score;

            public int CompareTo(Sample other)
            {
                return other.score.CompareTo(score);
            }
        }

        public int candidatesPerIteration = 1;
        public float convergenceAbsolute = -float.MaxValue;
        public float contraction = 0.9f;
        public float sigma = 1;
        public int iterations = 100;
        public int populationSize = 3;

        ObjectPool<Sample> pool;

        void CreateNewCandidate(Sample from, Sample to, float sd)
        {
            for (int i = 0; i < from.values.Length; i++)
            {
                to.values[i] = Gaussian.RandomGaussian(from.values[i], sd);
            }
        }

        public float[] Optimize(Func<float[], float> fitness, float[] guessCenter)
        {
            PriorityQueue<Sample> best = new PriorityQueue<Sample>();
            int arraySize = guessCenter.Length;
            pool = new ObjectPool<Sample>(
                () => new Sample { values = new float[arraySize], score = float.MaxValue },
                x => x = null);

            List<Sample> pop = new List<Sample>();

            Sample start = new Sample { values = guessCenter.ToArray(), score = fitness(guessCenter) };
            best.Enqueue(start);

            float sd = sigma;
            // while we aren't done
            for (int i = 0; i < iterations; i++)
            {
                pop.Clear();
                pop.AddRange(best);
                // use the current population to create new candidates
                for (int popIndex = 0; popIndex < pop.Count; popIndex++)
                {
                    for (int j = 0; j < candidatesPerIteration || best.Count() < populationSize; j++)
                    {
                        Sample newSample = pool.Get();
                        CreateNewCandidate(pop[popIndex], newSample, sd);
                        newSample.score = fitness(newSample.values);
                        if (newSample.score <= convergenceAbsolute)
                        {
                            return newSample.values;
                        }

                        best.Enqueue(newSample);
                    }
                }

                // only keep the top populationSize samples.
                while (best.Count() > populationSize)
                {
                    pool.Release(best.Dequeue());
                }

                sd *= contraction;
            }

            while (best.Count() > 1) best.Dequeue();

            return best.Dequeue().values;
        }

        public void SetMinimumStepSize(int iterations, float sigmaStart, float minStepSize)
        {
            this.iterations = iterations;
            sigma = sigmaStart;
            contraction = Mathf.Pow(minStepSize / sigmaStart, 1f / iterations);
            if (contraction < 0.5f)
            {
                Debug.LogWarning($"Contraction is less than 0.5, you may want to rethink this. {contraction}");
            }
        }
    }
}
