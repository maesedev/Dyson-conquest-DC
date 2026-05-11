using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DysonHarvest
{
    // Attach to GameEndPanel (an empty GameObject inside Canvas).
    // Builds its own card UI at runtime — no child GameObjects needed in the Editor.
    public class GameEndPanel : MonoBehaviour
    {
        private static readonly Color VictoryColor  = new Color(1f, 0.85f, 0.1f);
        private static readonly Color GameOverColor = new Color(1f, 0.25f, 0.25f);
        private static readonly Color CardBg        = new Color(0.08f, 0.08f, 0.14f, 1f);
        private static readonly Color OverlayBg     = new Color(0f, 0f, 0f, 0.65f);

        private TMP_Text _titleText;
        private TMP_Text _subtitleText;

        private void Awake()
        {
            BuildCard();

            // Subscribe before SetActive(false) — Start() never runs on inactive GOs
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.OnVictory  += ShowVictory;
                gm.OnGameOver += ShowGameOver;
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.OnVictory  -= ShowVictory;
                gm.OnGameOver -= ShowGameOver;
            }
        }

        // ── Card construction ────────────────────────────────────────────────

        private void BuildCard()
        {
            // Root: stretch to fill canvas, no Image (avoid conflict with Editor Panel Image)
            var rt = GetComponent<RectTransform>();
            if (rt == null) rt = gameObject.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // Destroy any Image the Editor may have added to the root Panel
            var existing = GetComponent<Image>();
            if (existing != null) Destroy(existing);

            // Full-screen overlay as a child
            var overlayGO = new GameObject("Overlay");
            overlayGO.transform.SetParent(transform, false);
            var overlayRT = overlayGO.AddComponent<RectTransform>();
            overlayRT.anchorMin = Vector2.zero;
            overlayRT.anchorMax = Vector2.one;
            overlayRT.offsetMin = Vector2.zero;
            overlayRT.offsetMax = Vector2.zero;
            var overlayImg = overlayGO.AddComponent<Image>();
            overlayImg.color = OverlayBg;
            overlayImg.raycastTarget = true;

            // Card
            var card = new GameObject("Card");
            card.transform.SetParent(transform, false);
            var cardRT = card.AddComponent<RectTransform>();
            cardRT.anchorMin = new Vector2(0.5f, 0.5f);
            cardRT.anchorMax = new Vector2(0.5f, 0.5f);
            cardRT.pivot     = new Vector2(0.5f, 0.5f);
            cardRT.sizeDelta = new Vector2(520, 340);
            cardRT.anchoredPosition = Vector2.zero;

            var cardImg = card.AddComponent<Image>();
            cardImg.sprite = GetRoundedSprite();
            cardImg.type   = Image.Type.Sliced;
            cardImg.color  = CardBg;

            // Title
            _titleText = MakeText(card.transform, "Title",
                anchorMin: new Vector2(0.1f, 0.62f),
                anchorMax: new Vector2(0.9f, 0.92f),
                fontSize: 52, bold: true, color: VictoryColor);

            // Subtitle
            _subtitleText = MakeText(card.transform, "Subtitle",
                anchorMin: new Vector2(0.08f, 0.32f),
                anchorMax: new Vector2(0.92f, 0.62f),
                fontSize: 22, bold: false, color: new Color(0.85f, 0.85f, 0.85f));

            // Restart button
            MakeRestartButton(card.transform);
        }

        private static TMP_Text MakeText(Transform parent, string goName,
            Vector2 anchorMin, Vector2 anchorMax,
            int fontSize, bool bold, Color color)
        {
            var go = new GameObject(goName);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin  = anchorMin;
            rt.anchorMax  = anchorMax;
            rt.offsetMin  = Vector2.zero;
            rt.offsetMax  = Vector2.zero;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize  = fontSize;
            tmp.color     = color;
            tmp.enableAutoSizing = false;
            if (bold) tmp.fontStyle = FontStyles.Bold;
            return tmp;
        }

        private void MakeRestartButton(Transform parent)
        {
            var btnGO = new GameObject("RestartButton");
            btnGO.transform.SetParent(parent, false);
            var rt = btnGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.06f);
            rt.anchorMax = new Vector2(0.5f, 0.06f);
            rt.pivot     = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(220, 58);
            rt.anchoredPosition = Vector2.zero;

            var img = btnGO.AddComponent<Image>();
            img.sprite = GetRoundedSprite();
            img.type   = Image.Type.Sliced;
            img.color  = new Color(0.2f, 0.2f, 0.35f, 1f);

            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = img;
            var colors = btn.colors;
            colors.normalColor      = Color.white;
            colors.highlightedColor = new Color(1.15f, 1.15f, 1.15f);
            colors.pressedColor     = new Color(0.8f, 0.8f, 0.8f);
            btn.colors = colors;
            btn.onClick.AddListener(Restart);

            var label = MakeText(btnGO.transform, "Label",
                anchorMin: Vector2.zero, anchorMax: Vector2.one,
                fontSize: 22, bold: true, color: Color.white);
            label.text = "Reiniciar";
        }

        // ── Public API ───────────────────────────────────────────────────────

        public void ShowVictory()
        {
            gameObject.SetActive(true);
            if (_titleText != null)   { _titleText.text = "VICTORIA";   _titleText.color = VictoryColor; }
            if (_subtitleText != null)  _subtitleText.text = "La Esfera de Dyson está completa.\n¡El Sol está bajo tu control!";
        }

        public void ShowGameOver()
        {
            gameObject.SetActive(true);
            if (_titleText != null)   { _titleText.text = "DERROTA";    _titleText.color = GameOverColor; }
            if (_subtitleText != null)  _subtitleText.text = "La energía se agotó.\nEl portal se ha cerrado.";
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ── Rounded sprite (shared with PortalLaunchPanel) ──────────────────

        private static Sprite _roundedSprite;

        private static Sprite GetRoundedSprite()
        {
            if (_roundedSprite != null) return _roundedSprite;

            const int size = 64, radius = 12;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color32[size * size];

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    pixels[y * size + x] = InsideRoundedRect(x, y, size, size, radius)
                        ? new Color32(255, 255, 255, 255)
                        : new Color32(0, 0, 0, 0);

            tex.SetPixels32(pixels);
            tex.Apply();

            _roundedSprite = Sprite.Create(
                tex, new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));

            return _roundedSprite;
        }

        private static bool InsideRoundedRect(int x, int y, int w, int h, int r)
        {
            if (x >= r && x < w - r) return true;
            if (y >= r && y < h - r) return true;
            int cx = x < r ? r : w - r - 1;
            int cy = y < r ? r : h - r - 1;
            float dx = x - cx, dy = y - cy;
            return dx * dx + dy * dy <= (float)r * r;
        }
    }
}
