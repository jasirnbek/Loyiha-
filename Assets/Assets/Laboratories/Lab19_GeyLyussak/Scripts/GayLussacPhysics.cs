using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab19
{
    public class GayLussacPhysics : MonoBehaviour
    {
        private List<double> measuredAlpha = new List<double>();

        public void AddMeasurement(float volumeInitial, float volumeFinal, float deltaTempCelsius)
        {
            double alpha = (volumeFinal - volumeInitial) / (volumeInitial * deltaTempCelsius);
            measuredAlpha.Add(alpha);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredAlpha);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredAlpha, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}