using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualPhysicalLab.Core.Physics
{
    public static class PhysicsFormulaEvaluator
    {
        public static double CalculateMean(List<double> values)
        {
            if (values == null || values.Count == 0) return 0d;
            if (values.Any(v => double.IsNaN(v) || double.IsInfinity(v)))
                throw new ArgumentException("O'lchovlar faqat chekli sonlardan iborat bo'lishi kerak.", nameof(values));
            return values.Average();
        }

        public static List<double> CalculateAbsoluteErrors(List<double> values, double mean)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (double.IsNaN(mean) || double.IsInfinity(mean))
                throw new ArgumentOutOfRangeException(nameof(mean));
            return values.Select(v =>
            {
                if (double.IsNaN(v) || double.IsInfinity(v))
                    throw new ArgumentException("O'lchovlar faqat chekli sonlardan iborat bo'lishi kerak.", nameof(values));
                return Math.Abs(mean - v);
            }).ToList();
        }

        public static double CalculateMeanAbsoluteError(List<double> absoluteErrors)
        {
            if (absoluteErrors == null || absoluteErrors.Count == 0) return 0d;
            if (absoluteErrors.Any(v => double.IsNaN(v) || double.IsInfinity(v) || v < 0d))
                throw new ArgumentException("Mutlaq xatoliklar manfiy yoki cheksiz bo'lmasligi kerak.", nameof(absoluteErrors));
            return absoluteErrors.Average();
        }

        public static double CalculateRelativeError(double meanAbsoluteError, double mean)
        {
            if (double.IsNaN(meanAbsoluteError) || double.IsInfinity(meanAbsoluteError) || meanAbsoluteError < 0d)
                throw new ArgumentOutOfRangeException(nameof(meanAbsoluteError));
            if (double.IsNaN(mean) || double.IsInfinity(mean))
                throw new ArgumentOutOfRangeException(nameof(mean));
            if (Math.Abs(mean) < double.Epsilon) return 0d;
            return (meanAbsoluteError / Math.Abs(mean)) * 100.0;
        }
    }
}