using UnityEngine;
using System.Collections;

namespace VirtualPhysicalLab.Laboratories.Lab02
{
    public class GalileoTroughPhysics : MonoBehaviour
    {
        [SerializeField] private Transform ballTransform;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private float distanceS = 1.2f;
        [SerializeField] private float accelerationA = 0.5f;

        private float elapsedTime;
        private bool isRolling;
        private Coroutine rollRoutine;

        public delegate void BallReachedEnd(float finalTime);
        public event BallReachedEnd OnBallReachedEnd;

        public void StartSimulation()
        {
            if (ballTransform == null || startPoint == null || endPoint == null)
            {
                Debug.LogError("Galiley tajribasi uchun ball, start va end nuqtalari Inspector'da biriktirilishi kerak.", this);
                return;
            }
            if (distanceS <= 0f || accelerationA <= 0f)
            {
                Debug.LogError("Masofa va tezlanish noldan katta bo'lishi kerak.", this);
                return;
            }

            StopSimulation();
            elapsedTime = 0f;
            ballTransform.position = startPoint.position;
            isRolling = true;
            rollRoutine = StartCoroutine(RollBallRoutine());
        }

        public void StopSimulation()
        {
            isRolling = false;
            if (rollRoutine != null)
            {
                StopCoroutine(rollRoutine);
                rollRoutine = null;
            }
        }

        private IEnumerator RollBallRoutine()
        {
            Vector3 startPos = startPoint.position;
            Vector3 endPos = endPoint.position;

            while (isRolling)
            {
                elapsedTime += Time.deltaTime;
                float currentDistance = 0.5f * accelerationA * elapsedTime * elapsedTime;
                float progress = Mathf.Clamp01(currentDistance / distanceS);
                ballTransform.position = Vector3.Lerp(startPos, endPos, progress);

                if (progress >= 1f)
                {
                    isRolling = false;
                    rollRoutine = null;
                    OnBallReachedEnd?.Invoke(elapsedTime);
                    yield break;
                }
                yield return null;
            }
            rollRoutine = null;
        }

        private void OnDisable() => StopSimulation();
    }
}