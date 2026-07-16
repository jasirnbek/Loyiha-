using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab18
{
    public class BoyleMariottePhysics : MonoBehaviour
    {
        private List<double> measuredK = new List<double>();

        public void AddMeasurement(float pressurePascal, float volumeM3)
        {
            double K = (double)pressurePascal * volumeM3;
            measuredK.Add(K);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredK);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredK, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}