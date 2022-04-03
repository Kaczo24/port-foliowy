using System;
using Physics2D;

namespace FlappyBird_Evolution
{
    static class Mathf
    {
        public static float Constraint(float value, float min, float max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        public static double Gaussian(double stdDev)
        {
            double u1 = 1.0 - World.rng.NextDouble();
            double u2 = 1.0 - World.rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return stdDev * randStdNormal;
        }
    }
}
