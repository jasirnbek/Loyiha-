using UnityEngine;
using TMPro;
using DG.Tweening;
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

        private void Start()
        {
            if (panelCanvasGroup == null) return;
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutCubic);
        }

        public void UpdateUI(float measuredValue, float error)
        {
            if (measurementValueText != null)
                measurementValueText.text = $"{measuredValue:F2} mm";
            if (errorRateText != null)
                errorRateText.text = $"Asbob xatoligi: ±{error:F2} mm";
            if (neonIndicator != null)
            {
                neonIndicator.transform.DOKill();
                neonIndicator.transform.localScale = Vector3.one;
                neonIndicator.transform.DOScale(1.1f, 0.3f)
                    .SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
        }

        private void OnDestroy()
        {
            if (panelCanvasGroup != null) panelCanvasGroup.DOKill();
            if (neonIndicator != null) neonIndicator.transform.DOKill();
        }
    }
}