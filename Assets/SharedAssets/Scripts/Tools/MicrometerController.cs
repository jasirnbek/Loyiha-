using UnityEngine;
using VirtualPhysicalLab.Core.Physics;

namespace VirtualPhysicalLab.Laboratories.Tools
{
    public class MicrometerController : MonoBehaviour, IMeasurable
    {
        [SerializeField] private Transform thimbleTransform;
        [SerializeField] private Transform spindleTransform;
        [SerializeField] private float threadPitchMm = 0.5f;
        [SerializeField] private float spindleTravelPerRotation = 0.005f;
        [SerializeField] private float instrumentError = 0.01f;
        
        private float currentRotationAngle = 0f;
        private float maxRotations = 50f;

        private void OnMouseDrag()
        {
            float mouseY = Input.GetAxis("Mouse Y");
            float rotationAmount = mouseY * 150f * Time.deltaTime;
            currentRotationAngle = Mathf.Clamp(currentRotationAngle + rotationAmount, 0f, maxRotations * 360f);
            
            thimbleTransform.localRotation = Quaternion.Euler(currentRotationAngle, 0f, 0f);
            float newSpindleLocalX = (currentRotationAngle / 360f) * spindleTravelPerRotation;
            spindleTransform.localPosition = new Vector3(newSpindleLocalX, spindleTransform.localPosition.y, spindleTransform.localPosition.z);
        }

        public float GetMeasurement()
        {
            float rawValue = (currentRotationAngle / 360f) * threadPitchMm;
            return Mathf.Round(rawValue / instrumentError) * instrumentError;
        }

        public float GetInstrumentError() => instrumentError;
    }
}