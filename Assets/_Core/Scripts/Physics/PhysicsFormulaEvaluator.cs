using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualPhysicalLab.Core.Physics
{
    public static class PhysicsFormulaEvaluator
    {
        public static double CalculateMean(List<double> values)
        {
            if (values == null || values.Count == 0) return 0;
            return values.Average();
        }

        public static List<double> CalculateAbsoluteErrors(List<double> values, double mean)
        {
            return values.Select(v => Math.Abs(mean - v)).ToList();
        }

        public static double CalculateMeanAbsoluteError(List<double> absoluteErrors)
        {
            if (absoluteErrors == null || absoluteErrors.Count == 0) return 0;
            return absoluteErrors.Average();
        }

        public static double CalculateRelativeError(double meanAbsoluteError, double mean)
        {
            if (mean == 0) return 0;
            return (meanAbsoluteError / mean) * 100.0;
        }
    }
}