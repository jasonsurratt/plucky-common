using System;

namespace Plucky.Common
{
    public class CompositeOptimizer : IOptimizer
    {
        public IOptimizer pass1;
        public IOptimizer pass2;

        public float[] Optimize(Func<float[], float> function, float[] guessCenter)
        {
            float[] result1 = pass1.Optimize(function, guessCenter);
            return pass2.Optimize(function, result1);
        }
    }
}
