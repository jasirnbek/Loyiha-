using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab05
{
    public class PendulumGravityPhysics : MonoBehaviour
    {
        [SerializeField] private float threadLengthL = 1.0f;
        private List<double> measuredGravityValues = new List<double>();

        public void AddMeasurement(float totalTimeSec, int oscillationsN)
        {
            double T = (double)totalTimeSec / oscillationsN;
            double g = (4.0 * System.Math.Pow(System.Math.PI, 2) * threadLengthL) / System.Math.Pow(T, 2);
            measuredGravityValues.Add(g);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredGravityValues);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredGravityValues, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}