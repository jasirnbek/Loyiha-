using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab07
{
    public class BallisticPendulumPhysics : MonoBehaviour
    {
        private List<double> measuredVelocities = new List<double>();

        public void CalculateBulletVelocity(float bulletMassG, float pendulumMassG, float lengthM, float angleDeg)
        {
            double m = bulletMassG / 1000.0;
            double M = pendulumMassG / 1000.0;
            double g = 9.81;
            double rad = angleDeg * Mathf.Deg2Rad;
            double h = lengthM * (1.0 - System.Math.Cos(rad));
            double v = ((m + M) / m) * System.Math.Sqrt(2.0 * g * h);
            measuredVelocities.Add(v);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredVelocities);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredVelocities, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}