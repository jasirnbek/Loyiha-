using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VirtualPhysicalLab.Core.UI
{
    public class GlassmorphismUIController : MonoBehaviour
    {
        [Header("UI elementlari")]
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private TextMeshProUGUI measurementValueText;
        [SerializeField] private TextMeshProUGUI errorRateText;
        [SerializeField] private Image neonIndicator;
        [SerializeField, Min(0.01f)] private float fadeDuration = 0.8f;
        [SerializeField, Min(1f)] private float pulseScale = 1.1f;

        private Coroutine fadeRoutine;
        private Coroutine pulseRoutine;

        private void Start()
        {
            if (panelCanvasGroup == null) return;
            panelCanvasGroup.alpha = 0f;
            fadeRoutine = StartCoroutine(FadeIn());
        }

        public void UpdateUI(float measuredValue, float error)
        {
            if (measurementValueText != null)
                measurementValueText.text = $"{measuredValue:F2} mm";
            if (errorRateText != null)
                errorRateText.text = $"Asbob xatoligi: ±{error:F2} mm";

            if (neonIndicator != null)
            {
                if (pulseRoutine != null) StopCoroutine(pulseRoutine);
                pulseRoutine = StartCoroutine(PulseIndicator());
            }
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                panelCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            panelCanvasGroup.alpha = 1f;
            fadeRoutine = null;
        }

        private IEnumerator PulseIndicator()
        {
            Transform target = neonIndicator.transform;
            Vector3 original = Vector3.one;
            const float duration = 0.3f;

            for (int loop = 0; loop < 2; loop++)
            {
                float t = 0f;
                while (t < duration)
                {
                    t += Time.unscaledDeltaTime;
                    target.localScale = Vector3.Lerp(original, original * pulseScale, Mathf.Clamp01(t / duration));
                    yield return null;
                }

                t = 0f;
                while (t < duration)
                {
                    t += Time.unscaledDeltaTime;
                    target.localScale = Vector3.Lerp(original * pulseScale, original, Mathf.Clamp01(t / duration));
                    yield return null;
                }
            }

            target.localScale = original;
            pulseRoutine = null;
        }

        private void OnDisable()
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);
            fadeRoutine = null;
            pulseRoutine = null;
            if (neonIndicator != null) neonIndicator.transform.localScale = Vector3.one;
        }
    }
}