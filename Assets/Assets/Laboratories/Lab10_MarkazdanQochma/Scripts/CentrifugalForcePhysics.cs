using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab10
{
    public class CentrifugalForcePhysics : MonoBehaviour
    {
        private List<double> measuredForces = new List<double>();

        public void AddMeasurement(float massGrams, float radiusM, int rotations, float timeSec)
        {
            double m = massGrams / 1000.0;
            double omega = (2.0 * System.Math.PI * rotations) / timeSec;
            double Fc = m * System.Math.Pow(omega, 2) * radiusM;
            measuredForces.Add(Fc);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredForces);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredForces, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}