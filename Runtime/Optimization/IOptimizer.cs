using System;

namespace Plucky.Common
{
    public interface IOptimizer
    {
        float[] Optimize(Func<float[], float> function, float[] guessCenter);
    }
}
