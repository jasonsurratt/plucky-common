using UnityEngine;

namespace Plucky.Common
{
    public class Gaussian
    {
        public static System.Random rng = new System.Random();

        /// Generate a random gaussian number.
        /// 
        /// Taken from: https://stackoverflow.com/questions/218060/random-gaussian-variables
        public static float RandomGaussian(float mean, float stdDev)
        {
            //uniform(0,1] random doubles
            float u1 = 1.0f - (float)rng.NextDouble();
            float u2 = 1.0f - (float)rng.NextDouble();
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                         Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
            //random normal(mean,stdDev^2)
            return mean + stdDev * randStdNormal;
        }
    }
}
