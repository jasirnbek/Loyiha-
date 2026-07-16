using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab03
{
    public class FrictionPhysics : MonoBehaviour
    {
        [SerializeField] private float normalForceN = 2.0f;
        private List<double> measuredCoefficients = new List<double>();

        public void AddMeasurement(float frictionForceN)
        {
            double mu = frictionForceN / normalForceN;
            measuredCoefficients.Add(mu);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredCoefficients);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredCoefficients, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}