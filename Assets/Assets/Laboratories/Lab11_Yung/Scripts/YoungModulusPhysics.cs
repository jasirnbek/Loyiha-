using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab11
{
    public class YoungModulusPhysics : MonoBehaviour
    {
        private List<double> measuredE = new List<double>();

        public void AddMeasurement(float forceN, float initialLengthM, float wireDiameterMm, float deltaLMm)
        {
            double d = wireDiameterMm / 1000.0;
            double deltaL = deltaLMm / 1000.0;
            double S = (System.Math.PI * System.Math.Pow(d, 2)) / 4.0;
            double E = (forceN * initialLengthM) / (S * deltaL);
            measuredE.Add(E);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredE);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredE, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}