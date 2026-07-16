using UnityEngine;
using System.Collections.Generic;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Lab16
{
    public class CalorimetryPhysics : MonoBehaviour
    {
        private List<double> measuredC = new List<double>();

        public void AddMeasurement(float mSuvG, float mJismG, float tSuv, float tJism, float tAralash, float cWater = 4187f, float cCalorimeter = 50f)
        {
            double mSuv = mSuvG / 1000.0;
            double mJism = mJismG / 1000.0;
            double numerator = (mSuv * cWater + cCalorimeter) * (tAralash - tSuv);
            double denominator = mJism * (tJism - tAralash);
            measuredC.Add(numerator / denominator);
        }

        public double GetFinalResults(out double absError, out double relError)
        {
            double mean = PhysicsFormulaEvaluator.CalculateMean(measuredC);
            List<double> abs = PhysicsFormulaEvaluator.CalculateAbsoluteErrors(measuredC, mean);
            absError = PhysicsFormulaEvaluator.CalculateMeanAbsoluteError(abs);
            relError = PhysicsFormulaEvaluator.CalculateRelativeError(absError, mean);
            return mean;
        }
    }
}