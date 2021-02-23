using System;

namespace Plucky.Common
{
    public class NelderMead : IOptimizer
    {
        public float convergenceDiff = 1e-3f;
        public float convergenceAbsolute = float.MaxValue;
        public int evaluations = 0;

        public Func<float[], float> fitnessFunction;

        public static float guessSpreadDefault = 1;
        public static float alphaDefault = 1;
        public static float gammaDefault = 2;
        public static float rhoDefault = 0.5f;
        public static float sigmaDefault = 0.5f;

        // the random spread around the initial guess for the starting points.
        public float guessSpread = guessSpreadDefault;
        public int maxIterations = 100;

        // coefficient used during reflection. Larger is more aggressive.
        public float alpha = alphaDefault;
        // coefficient used during expansion. Larger is more aggressive.
        public float gamma = gammaDefault;
        // coefficient used during contraction. Larger is more aggressive. probably should be < 1.
        public float rho = rhoDefault;
        public float sigma = sigmaDefault;

        public float FitnessFunction(float[] vars)
        {
            evaluations++;
            return fitnessFunction(vars);
        }

        public void Clear()
        {
            evaluations = 0;
            convergenceDiff = 1e-3f;
        }

        public float[] Optimize(Func<float[], float> function, float[] guessCenter)
        {
            return Optimize(function, guessCenter, -1);
        }

        public float[] Optimize(Func<float[], float> function, float[] guessCenter, int seed = -1)
        {
            int N = guessCenter.Length;
            fitnessFunction = function;
            evaluations = 0;
            Random rng;
            if (seed >= 0) rng = new Random(seed);
            else rng = new Random();
            float[][] simplex = new float[N + 1][];


            // Generate N + 1 initial arrays.
            for (int array = 0; array <= N; array++)
            {
                simplex[array] = new float[N];
                for (int index = 0; index < N; index++)
                {
                    simplex[array][index] = guessCenter[index] + (float)rng.NextDouble() * guessSpread * 2 - guessSpread;
                }
            }

            // Evaluation
            float[] functionValues = new float[N + 1];
            for (int vertex = 0; vertex <= N; vertex++)
            {
                functionValues[vertex] = FitnessFunction(simplex[vertex]);
            }

            int iterationCount = 0;
            while (iterationCount < maxIterations)
            {
                iterationCount++;

                float[] contractionPoint = new float[N];
                float[] reflectionPoint = new float[N];

                int worstIndex = 0;
                int bestIndex = 0;
                for (int vertex = 1; vertex <= N; vertex++)
                {
                    if (functionValues[vertex] > functionValues[worstIndex])
                    {
                        worstIndex = vertex;
                    }
                    if (functionValues[vertex] < functionValues[bestIndex])
                    {
                        bestIndex = vertex;
                    }
                }

                // Find centroid of the simplex excluding the vertex with highest functionvalue.
                float[] centroid = new float[N];

                for (int i = 0; i < N; i++)
                {
                    centroid[i] = 0;
                    for (int vertex = 0; vertex <= N; vertex++)
                    {
                        if (vertex != worstIndex)
                        {
                            centroid[i] += simplex[vertex][i] / N;
                        }
                    }
                }

                // Check for convergence
                if (Math.Abs(functionValues[bestIndex] - functionValues[worstIndex]) < convergenceDiff &&
                    functionValues[bestIndex] < convergenceAbsolute)
                {
                    break;
                }

                //Reflection
                for (int i = 0; i < N; i++)
                {
                    reflectionPoint[i] = centroid[i] + alpha * (centroid[i] - simplex[worstIndex][i]);
                }

                float reflectionValue = FitnessFunction(reflectionPoint);

                if (reflectionValue >= functionValues[bestIndex] && reflectionValue < functionValues[worstIndex])
                {
                    reflectionPoint.CopyTo(simplex[worstIndex], 0);
                    functionValues[worstIndex] = reflectionValue;
                    continue;
                }


                // Expand past the reflection point, if it is better
                if (reflectionValue < functionValues[bestIndex])
                {
                    float[] expansionPoint = new float[N];
                    for (int index = 0; index < N; index++)
                    {
                        expansionPoint[index] = centroid[index] + gamma * (reflectionPoint[index] - centroid[index]);
                    }
                    float expansionValue = FitnessFunction(expansionPoint);

                    if (expansionValue < reflectionValue)
                    {
                        simplex[worstIndex] = expansionPoint;
                        functionValues[worstIndex] = expansionValue;
                    }
                    else
                    {
                        simplex[worstIndex] = reflectionPoint;
                        functionValues[worstIndex] = reflectionValue;
                    }
                    continue;
                }

                // Contract the worst point in towards the centroid
                for (int index = 0; index < N; index++)
                {
                    contractionPoint[index] = centroid[index] + rho * (simplex[worstIndex][index] - centroid[index]);
                }

                float contractionValue = FitnessFunction(contractionPoint);

                if (contractionValue < functionValues[worstIndex])
                {
                    simplex[worstIndex] = contractionPoint;
                    functionValues[worstIndex] = contractionValue;
                    continue;
                }

                // Shrink all other points towards the best known point
                float[] best_point = simplex[bestIndex];
                for (int vertex = 0; vertex <= N; vertex++)
                {
                    if (vertex != bestIndex)
                    {
                        for (int i = 0; i < N; i++)
                        {
                            simplex[vertex][i] = best_point[i] + sigma * (simplex[vertex][i] - best_point[i]);
                        }
                        functionValues[vertex] = FitnessFunction(simplex[vertex]);
                    }
                }

            }

            return simplex[0];
        }

        public string ToString(float[] values)
        {
            return string.Join(", ", values);
        }
    }
}
