using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab12
{
    public class ShearModulusPhysics : MonoBehaviour
    {
        private List<double> measuredG = new List<double>();

        public void AddMeasurement(float torqueM, float lengthM, float radiusMm, float angleDeg)
        {
            double r = radiusMm / 1000.0;
            double alpha = angleDeg * System.Math.PI / 180.0;
            double G = (2.0 * torqueM * lengthM) / (System.Math.PI * System.Math.Pow(r, 4) * alpha);
            measuredG.Add(G);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredG);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredG, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}