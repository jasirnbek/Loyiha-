using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab09
{
    public class MaxwellPendulumPhysics : MonoBehaviour
    {
        private List<double> measuredInertia = new List<double>();

        public void AddMeasurement(float massGrams, float shaftRadiusM, float heightM, float timeSec)
        {
            double m = massGrams / 1000.0;
            double g = 9.81;
            double I = m * System.Math.Pow(shaftRadiusM, 2) * (((g * System.Math.Pow(timeSec, 2)) / (2.0 * heightM)) - 1.0);
            measuredInertia.Add(I);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredInertia);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredInertia, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}