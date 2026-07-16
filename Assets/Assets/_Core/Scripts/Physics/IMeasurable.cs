using UnityEngine;

namespace VirtualPhysicalLab.Core.Physics
{
    public interface IMeasurable
    {
        float GetMeasurement();
        float GetInstrumentError();
    }
}