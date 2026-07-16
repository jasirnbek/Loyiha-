using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab01
{
    public class CylinderDensityPhysics : MonoBehaviour
    {
        private List<double> measuredDensities = new List<double>();

        public void AddMeasurement(float dMm, float hMm, float mGram)
        {
            double d = dMm / 1000.0;
            double h = hMm / 1000.0;
            double m = mGram / 1000.0;
            double volume = (System.Math.PI * System.Math.Pow(d, 2) * h) / 4.0;
            measuredDensities.Add(m / volume);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredDensities);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredDensities, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}