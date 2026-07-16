using UnityEngine;

namespace VirtualPhysicalLab.Laboratories.Lab04
{
    public class SpringPendulumPhysics : MonoBehaviour
    {
        [SerializeField] private Transform weightTransform;
        [SerializeField] private LineRenderer springMesh;
        [SerializeField] private float massM = 0.2f;
        [SerializeField] private float stiffnessK = 40f;
        [SerializeField] private float dampingGamma = 0.15f;
        [SerializeField] private float amplitudeA = 0.2f;

        private float time = 0f;
        private Vector3 initialWeightPosition;
        private bool isOscillating = false;

        private void Start() => initialWeightPosition = weightTransform.localPosition;

        public void ToggleOscillation(bool start)
        {
            isOscillating = start;
            if (!start)
            {
                weightTransform.localPosition = initialWeightPosition;
                time = 0f;
            }
        }

        private void Update()
        {
            if (!isOscillating) return;
            time += Time.deltaTime;
            float omegaSqr = (stiffnessK / massM) - Mathf.Pow(dampingGamma, 2);
            if (omegaSqr <= 0) return;
            
            float omega = Mathf.Sqrt(omegaSqr);
            float displacement = amplitudeA * Mathf.Exp(-dampingGamma * time) * Mathf.Cos(omega * time);
            weightTransform.localPosition = initialWeightPosition + new Vector3(0, displacement, 0);
            UpdateSpringVisuals(displacement);
        }

        private void UpdateSpringVisuals(float currentDisplacement)
        {
            int verticesCount = 50;
            springMesh.positionCount = verticesCount;
            float totalLength = Mathf.Abs(initialWeightPosition.y - weightTransform.localPosition.y) + 0.5f;
            for (int i = 0; i < verticesCount; i++)
            {
                float t = (float)i / (verticesCount - 1);
                float x = Mathf.Sin(t * Mathf.PI * 20) * 0.05f;
                float z = Mathf.Cos(t * Mathf.PI * 20) * 0.05f;
                float y = -t * totalLength;
                springMesh.SetPosition(i, new Vector3(x, y, z));
            }
        }
    }
}