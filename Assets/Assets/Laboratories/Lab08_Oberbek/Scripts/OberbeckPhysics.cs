using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab08
{
    public class OberbeckPhysics : MonoBehaviour
    {
        private List<double> measuredInertia = new List<double>();

        public void AddMeasurement(float massGrams, float pulleyRadiusM, float angleRad, float timeSec)
        {
            double m = massGrams / 1000.0;
            double g = 9.81;
            double F = m * g;
            double momentM = F * pulleyRadiusM;
            double epsilon = (2.0 * angleRad) / System.Math.Pow(timeSec, 2);
            measuredInertia.Add(momentM / epsilon);
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