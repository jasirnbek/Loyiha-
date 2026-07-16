using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab14
{
    public class ViscosityStokesPhysics : MonoBehaviour
    {
        private List<double> measuredEta = new List<double>();

        public void AddMeasurement(float ballRadiusMm, float rhoBall, float rhoLiquid, float velocityMS)
        {
            double r = ballRadiusMm / 1000.0;
            double g = 9.81;
            double eta = (2.0 * System.Math.Pow(r, 2) * (rhoBall - rhoLiquid) * g) / (9.0 * velocityMS);
            measuredEta.Add(eta);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredEta);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredEta, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}