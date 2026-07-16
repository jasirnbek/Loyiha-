using System;

namespace VirtualPhysicalLab.Core.Reports
{
    [Serializable]
    public class LabReport
    {
        public string id;
        public int labId;
        public string studentName;
        public string groupName;
        public string timestamp;
        public string rawMeasurementsJson;
        public double finalResult;
        public double absoluteError;
        public double relativeError;
        public bool isPassed;
    }
}