using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace VirtualPhysicalLab.Core.UI
{
    public class GlassmorphismUIController : MonoBehaviour
    {
        [Header("UI Elementlari")]
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private TextMeshProUGUI measurementValueText;
        [SerializeField] private TextMeshProUGUI errorRateText;
        [SerializeField] private Image neonIndicator;

        private void Start()
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutBack);
        }

        public void UpdateUI(float measuredValue, float error)
        {
            measurementValueText.text = $"{measuredValue:F2} mm";
            errorRateText.text = $"Asbob xatoligi: ±{error:F2} mm";
            neonIndicator.transform.DOScale(1.1f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }
}