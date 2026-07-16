using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab15
{
    public class AdiabaticPhysics : MonoBehaviour
    {
        private List<double> measuredGamma = new List<double>();

        public void AddMeasurement(float h1Mm, float h2Mm)
        {
            if (Mathf.Approximately(h1Mm, h2Mm)) return;
            double gamma = (double)h1Mm / (h1Mm - h2Mm);
            measuredGamma.Add(gamma);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredGamma);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredGamma, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}