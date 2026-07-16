using UnityEngine;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Tools
{
    public class VernierCaliperController : MonoBehaviour, IMeasurable
    {
        [SerializeField] private Transform sliderTransform;
        [SerializeField] private float minLimitLocalX = 0f;
        [SerializeField] private float maxLimitLocalX = 0.15f;
        [SerializeField] private float realWorldMaxRangeMm = 150f;
        [SerializeField] private float instrumentError = 0.05f;

        private float currentLocalX = 0f;
        private Camera mainCamera;

        private void Start() => mainCamera = Camera.main;

        private void OnMouseDrag()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 localPoint = transform.InverseTransformPoint(hit.point);
                currentLocalX = Mathf.Clamp(localPoint.x, minLimitLocalX, maxLimitLocalX);
                sliderTransform.localPosition = new Vector3(currentLocalX, sliderTransform.localPosition.y, sliderTransform.localPosition.z);
            }
        }

        public float GetMeasurement()
        {
            float t = (currentLocalX - minLimitLocalX) / (maxLimitLocalX - minLimitLocalX);
            float calculatedValue = t * realWorldMaxRangeMm;
            return Mathf.Round(calculatedValue / instrumentError) * instrumentError;
        }

        public float GetInstrumentError() => instrumentError;
    }
}