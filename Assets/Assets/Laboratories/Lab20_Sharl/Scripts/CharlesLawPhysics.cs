using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab20
{
    public class CharlesLawPhysics : MonoBehaviour
    {
        private List<double> measuredBeta = new List<double>();

        public void AddMeasurement(float pressureInitial, float pressureFinal, float deltaTempCelsius)
        {
            double beta = (pressureFinal - pressureInitial) / (pressureInitial * deltaTempCelsius);
            measuredBeta.Add(beta);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredBeta);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredBeta, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}