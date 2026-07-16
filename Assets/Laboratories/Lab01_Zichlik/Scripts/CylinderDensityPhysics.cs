using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.LabDensity
{
    public class CylinderDensityPhysics : MonoBehaviour
    {
        [SerializeField] private float targetMassKg = 0.15f;
        private List<double> measuredDensities = new List<double>();

        public void AddMeasurement(float dMm, float hMm, float mGram)
        {
            double d = dMm / 1000.0;
            double h = hMm / 1000.0;
            double m = mGram / 1000.0;

            double volume = (System.Math.PI * System.Math.Pow(d, 2) * h) / 4.0;
            double density = m / volume;
            measuredDensities.Add(density);
        }

        public double GetFinalDensityResults(out double absoluteError, out double relativeError)
        {
            double meanDensity = PhysicsFormulaEvaluator.CalculateMean(measuredDensities);
            List<double> absErrors = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredDensities, meanDensity);
            absoluteError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(absErrors);
            relativeError = PhysicsFormulaEvaluator.CalculateRelativeError(absoluteError, meanDensity);

            return meanDensity;
        }
    }
}