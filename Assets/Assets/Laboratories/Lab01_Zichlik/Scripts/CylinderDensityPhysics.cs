using UnityEngine;
using System;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab01
{
    public class CylinderDensityPhysics : MonoBehaviour
    {
        private readonly List<double> measuredDensities = new List<double>();

        public bool AddMeasurement(float dMm, float hMm, float mGram)
        {
            if (float.IsNaN(dMm) || float.IsInfinity(dMm) || dMm <= 0f ||
                float.IsNaN(hMm) || float.IsInfinity(hMm) || hMm <= 0f ||
                float.IsNaN(mGram) || float.IsInfinity(mGram) || mGram <= 0f)
            {
                Debug.LogWarning("Diametr, balandlik va massa musbat, chekli qiymat bo'lishi kerak.", this);
                return false;
            }

            double d = dMm / 1000.0;
            double h = hMm / 1000.0;
            double m = mGram / 1000.0;
            double volume = Math.PI * d * d * h / 4.0;
            double density = m / volume;
            if (double.IsNaN(density) || double.IsInfinity(density) || density <= 0d)
                return false;

            measuredDensities.Add(density);
            return true;
        }

        public void ClearMeasurements() => measuredDensities.Clear();

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredDensities);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredDensities, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }

        public int MeasurementCount => measuredDensities.Count;
    }
}