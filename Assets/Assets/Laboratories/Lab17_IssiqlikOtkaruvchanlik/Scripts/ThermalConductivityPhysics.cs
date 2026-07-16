using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab17
{
    public class ThermalConductivityPhysics : MonoBehaviour
    {
        private List<double> measuredLambda = new List<double>();

        public void AddMeasurement(float heatQ, float thicknessM, float areaS2, float deltaT, float timeSec)
        {
            double lambda = (heatQ * thicknessM) / (areaS2 * deltaT * timeSec);
            measuredLambda.Add(lambda);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredLambda);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredLambda, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}