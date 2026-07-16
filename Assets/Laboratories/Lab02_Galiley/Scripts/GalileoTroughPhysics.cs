using UnityEngine;
using System.Collections;

namespace VirtualPhysicalLab.Laboratories.LabGalileo
{
    public class GalileoTroughPhysics : MonoBehaviour
    {
        [SerializeField] private Transform ballTransform;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private float distanceS = 1.2f;
        [SerializeField] private float accelerationA = 0.5f;

        private float elapsedTime = 0f;
        private bool isRolling = false;

        public delegate void BallReachedEnd(float finalTime);
        public event BallReachedEnd OnBallReachedEnd;

        public void StartSimulation()
        {
            ballTransform.position = startPoint.position;
            elapsedTime = 0f;
            isRolling = true;
            StartCoroutine(RollBallRoutine());
        }

        private IEnumerator RollBallRoutine()
        {
            Vector3 startPos = startPoint.position;
            Vector3 endPos = endPoint.position;

            while (isRolling)
            {
                elapsedTime += Time.deltaTime;
                float currentDistance = 0.5f * accelerationA * Mathf.Pow(elapsedTime, 2);
                float progress = currentDistance / distanceS;

                if (progress >= 1.0f)
                {
                    progress = 1.0f;
                    isRolling = false;
                    ballTransform.position = endPos;
                    OnBallReachedEnd?.Invoke(elapsedTime);
                }
                else
                {
                    ballTransform.position = Vector3.Lerp(startPos, endPos, progress);
                }
                yield return null;
            }
        }
    }
}