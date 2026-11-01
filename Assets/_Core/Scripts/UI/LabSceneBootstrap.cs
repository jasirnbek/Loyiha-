using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VirtualPhysicalLab.Core.UI
{
    public class LabSceneBootstrap : MonoBehaviour
    {
        private static readonly string[] LabNames = {
            "Zichlik", "Galiley tajribasi", "Ishqalanish", "Prujina", "Matematik mayatnik",
            "Atvud mashinasi", "Ballistik mayatnik", "Oberbek usuli", "Maksvel mayatnigi",
            "Markazdan qochma kuch", "Yung moduli", "Siljish moduli", "Yuzaki taranglik",
            "Yopishqoqlik", "Adiabatik jarayon", "Kalorimetriya", "Issiqlik o'tkazuvchanlik",
            "Boyl-Mariott qonuni", "Gey-Lyussak qonuni", "Sharl qonuni", "Van der Vaals tenglamasi"
        };

        private static readonly string[] Formulas = {
            "ρ = m / V", "s = at² / 2", "Fᶠ = μN", "F = −kx", "T = 2π√(l/g)",
            "a = (m₂−m₁)g/(m₁+m₂)", "m₁v₁ = (m₁+m₂)V", "τ = Iα", "I = Σmr²",
            "F = mv²/r", "E = σ/ε", "G = τ/γ", "σ = F/l", "η = 2r²g(ρₛ−ρₗ)/(9v)",
            "pVᵞ = const", "Q = mcΔT", "Q/t = λAΔT/l", "pV = const", "V/T = const",
            "p/T = const", "pV = nRT + ..."
        };

        private void Start()
        {
            BuildInterface();
        }

        private void BuildInterface()
        {
            var canvasObject = new GameObject("VirtualLabCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                DontDestroyOnLoad(eventSystem);
            }

            var panel = CreatePanel(canvasObject.transform, new Vector2(0.5f, 0.5f), new Vector2(0.9f, 0.88f));
            if (SceneManager.GetActiveScene().name == "MainMenu")
                BuildMenu(panel);
            else
                BuildLabPage(panel, SceneManager.GetActiveScene().name);
        }

        private void BuildMenu(Transform parent)
        {
            AddText(parent, "VIRTUAL PHYSICAL LAB", 46, new Vector2(0.5f, 0.94f), new Vector2(0.9f, 0.08f));
            AddText(parent, "21 ta virtual fizika laboratoriyasi", 27, new Vector2(0.5f, 0.88f), new Vector2(0.9f, 0.05f));
            for (int i = 0; i < LabNames.Length; i++)
            {
                int index = i;
                float y = 0.81f - i * 0.035f;
                var button = CreateButton(parent, $"{i + 1:00}. {LabNames[i]}", new Vector2(0.5f, y), new Vector2(0.82f, 0.031f), 20);
                button.onClick.AddListener(() => SceneManager.LoadScene($"Lab{index + 1:00}"));
            }
        }

        private void BuildLabPage(Transform parent, string sceneName)
        {
            int index = 0;
            if (sceneName.StartsWith("Lab") && int.TryParse(sceneName.Substring(3), out int parsed))
                index = Mathf.Clamp(parsed - 1, 0, LabNames.Length - 1);

            AddText(parent, $"LABORATORIYA №{index + 1}", 36, new Vector2(0.5f, 0.92f), new Vector2(0.9f, 0.07f));
            AddText(parent, LabNames[index], 32, new Vector2(0.5f, 0.84f), new Vector2(0.9f, 0.07f));
            AddText(parent, $"Asosiy formula: {Formulas[index]}", 28, new Vector2(0.5f, 0.72f), new Vector2(0.9f, 0.08f));
            AddText(parent,
                "Bu sahna laboratoriya interfeysining boshlang'ich namunasi. " +
                "O'lchov asboblari, fizik model parametrlari va natijalar paneli tegishli tajriba skriptlariga ulanishi kerak.",
                23, new Vector2(0.5f, 0.53f), new Vector2(0.84f, 0.2f));
            CreateButton(parent, "BOSH MENYUGA QAYTISH", new Vector2(0.5f, 0.12f), new Vector2(0.75f, 0.07f), 24)
                .onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        }

        private static Transform CreatePanel(Transform parent, Vector2 anchor, Vector2 size)
        {
            var go = new GameObject("ContentPanel", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(1080 * size.x, 1920 * size.y);
            go.GetComponent<Image>().color = new Color(0.055f, 0.09f, 0.14f, 0.96f);
            return go.transform;
        }

        private static Text AddText(Transform parent, string value, int size, Vector2 anchor, Vector2 dimensions)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(1080 * dimensions.x, 1920 * dimensions.y);
            var text = go.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.color = Color.white;
            return text;
        }

        private static Button CreateButton(Transform parent, string label, Vector2 anchor, Vector2 dimensions, int fontSize)
        {
            var go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(1080 * dimensions.x, 1920 * dimensions.y);
            go.GetComponent<Image>().color = new Color(0.08f, 0.48f, 0.36f, 1f);
            var button = go.GetComponent<Button>();
            var labelText = AddText(go.transform, label, fontSize, new Vector2(0.5f, 0.5f), new Vector2(0.95f, 0.9f));
            labelText.raycastTarget = false;
            return button;
        }
    }
}