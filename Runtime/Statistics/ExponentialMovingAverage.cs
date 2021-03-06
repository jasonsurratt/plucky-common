﻿using System;

namespace Plucky.Common
{
    // Taken from Mirror
    // implementation of N-day EMA
    // it calculates an exponential moving average roughy equivalent to the last n observations
    // https://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
    public class ExponentialMovingAverage
    {
        readonly float alpha;
        bool initialized;

        public ExponentialMovingAverage(int n)
        {
            // standard N-day EMA alpha calculation
            alpha = 2.0f / (n + 1);
        }

        public void Add(double newValue)
        {
            // simple algorithm for EMA described here:
            // https://en.wikipedia.org/wiki/Moving_average#Exponentially_weighted_moving_variance_and_standard_deviation
            if (initialized)
            {
                double delta = newValue - value;
                value += alpha * delta;
                var = (1 - alpha) * (var + alpha * delta * delta);
            }
            else
            {
                value = newValue;
                initialized = true;
            }
        }

        public double std => Math.Sqrt(var);

        public double value { get; private set; }

        public double var { get; private set; }
    }
}
