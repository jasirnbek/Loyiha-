using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab13
{
    public class SurfaceTensionPhysics : MonoBehaviour
    {
        private List<double> measuredSigma = new List<double>();

        public void AddMeasurement(float totalMassGrams, int dropCount, float pipetteDiameterMm)
        {
            double m = totalMassGrams / 1000.0;
            double d = pipetteDiameterMm / 1000.0;
            double g = 9.81;
            double sigma = (m * g) / (System.Math.PI * d * dropCount);
            measuredSigma.Add(sigma);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredSigma);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredSigma, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}