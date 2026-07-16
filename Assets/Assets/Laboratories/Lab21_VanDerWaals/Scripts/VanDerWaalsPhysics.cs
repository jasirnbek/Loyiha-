using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab21
{
    public class VanDerWaalsPhysics : MonoBehaviour
    {
        private List<double> measuredA = new List<double>();
        private List<double> measuredB = new List<double>();

        public void CalculateConstants(float criticalPressurePascal, float criticalVolumeM3)
        {
            double a = 3.0 * criticalPressurePascal * System.Math.Pow(criticalVolumeM3, 2);
            double b = criticalVolumeM3 / 3.0;
            measuredA.Add(a);
            measuredB.Add(b);
        }

        public void GetFinalResults(out double meanA, out double meanB)
        {
            meanA = PhysicsFormulaEvaluator.CalculateMean(measuredA);
            meanB = PhysicsFormulaEvaluator.CalculateMean(measuredB);
        }
    }
}