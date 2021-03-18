namespace FRC3620_Gopher_Hero
{
    class MathUtil
    {
        public static double clamp(double val, double min, double max)
        {
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }
            else
            {
                return val;
            }
        }

        public static double copySign (double val, double sign)
        {
            double rv = val;
            if (System.Math.Sign(val) != System.Math.Sign(sign))
            {
                rv = -rv;
            }
            return rv;
        }

        public static double applyDeadband (double val, double deadband)
        {
            if (System.Math.Abs(val) < deadband) return 0;
            return val;
        }

    }
}
