using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab06
{
    public class AttwoodMachinePhysics : MonoBehaviour
    {
        private List<double> measuredGValues = new List<double>();

        public void CalculateGravity(float m1Grams, float m2Grams, float measuredAcceleration)
        {
            double m1 = m1Grams / 1000.0;
            double m2 = m2Grams / 1000.0;
            double g = (measuredAcceleration * (m1 + m2)) / (m2 - m1);
            measuredGValues.Add(g);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredGValues);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredGValues, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}