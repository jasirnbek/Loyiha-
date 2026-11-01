using UnityEngine;

namespace VirtualPhysicalLab.Laboratories.Lab04
{
    public class SpringPendulumPhysics : MonoBehaviour
    {
        [SerializeField] private Transform weightTransform;
        [SerializeField] private LineRenderer springMesh;
        [SerializeField, Min(0.0001f)] private float massM = 0.2f;
        [SerializeField, Min(0f)] private float stiffnessK = 40f;
        [SerializeField, Min(0f)] private float dampingGamma = 0.15f;
        [SerializeField] private float amplitudeA = 0.2f;
        [SerializeField, Min(2)] private int verticesCount = 50;

        private float time;
        private Vector3 initialWeightPosition;
        private bool isOscillating;

        private void Start()
        {
            if (weightTransform == null)
            {
                Debug.LogError("Prujina yuk massasi (Weight Transform) biriktirilmagan.", this);
                enabled = false;
                return;
            }
            initialWeightPosition = weightTransform.localPosition;
            if (springMesh != null) UpdateSpringVisuals();
        }

        public void ToggleOscillation(bool start)
        {
            if (weightTransform == null) return;
            isOscillating = start;
            if (!start)
            {
                weightTransform.localPosition = initialWeightPosition;
                time = 0f;
                UpdateSpringVisuals();
            }
        }

        private void Update()
        {
            if (!isOscillating || weightTransform == null) return;
            if (massM <= 0f || stiffnessK < 0f || dampingGamma < 0f) return;

            time += Time.deltaTime;
            float omegaSquared = stiffnessK / massM - dampingGamma * dampingGamma;
            if (omegaSquared <= 0f) return;

            float omega = Mathf.Sqrt(omegaSquared);
            float displacement = amplitudeA * Mathf.Exp(-dampingGamma * time) * Mathf.Cos(omega * time);
            weightTransform.localPosition = initialWeightPosition + Vector3.up * displacement;
            UpdateSpringVisuals();
        }

        private void UpdateSpringVisuals()
        {
            if (springMesh == null || weightTransform == null) return;
            int count = Mathf.Max(2, verticesCount);
            springMesh.useWorldSpace = false;
            springMesh.positionCount = count;
            float length = Mathf.Max(0.01f, Mathf.Abs(weightTransform.localPosition.y) + 0.5f);

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (count - 1);
                float angle = t * Mathf.PI * 20f;
                springMesh.SetPosition(i, new Vector3(Mathf.Sin(angle) * 0.05f, -t * length, Mathf.Cos(angle) * 0.05f));
            }
        }
    }
}