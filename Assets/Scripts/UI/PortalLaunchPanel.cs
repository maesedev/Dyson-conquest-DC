using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DysonHarvest
{
    // Canvas panel that appears when the player clicks the portal.
    // Creates ship-launch buttons dynamically from the available ShipDataSO list.
    // Attach to a Panel GameObject inside the Canvas.
    public class PortalLaunchPanel : MonoBehaviour
    {
        private PortalController _portal;
        private static Sprite _roundedSprite;

        public void Initialize(PortalController portal, ShipDataSO[] ships)
        {
            _portal = portal;
            BuildButtons(ships);
            gameObject.SetActive(false);
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private const float BtnWidth   = 360f;
        private const float BtnHeight  = 70f;
        private const float BtnSpacing = 82f;

        private void BuildButtons(ShipDataSO[] ships)
        {
            float totalHeight = ships.Length * BtnSpacing;

            // Title
            var title = new GameObject("Title");
            title.transform.SetParent(transform, false);
            var titleRT = title.AddComponent<RectTransform>();
            titleRT.sizeDelta = new Vector2(BtnWidth, 50);
            titleRT.anchoredPosition = new Vector2(0, totalHeight * 0.5f + 30f);
            var titleTMP = title.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "PORTAL — Lanzar nave";
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.fontSize = 22;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(0.7f, 0.4f, 1f);

            // One button per ship type
            for (int i = 0; i < ships.Length; i++)
            {
                var data = ships[i];
                float yPos = (ships.Length - 1 - i) * BtnSpacing - (ships.Length - 1) * BtnSpacing * 0.5f;
                CreateButton(data, yPos);
            }

            // Close button
            CreateCloseButton(-(totalHeight * 0.5f + 40f));
        }

        private void CreateButton(ShipDataSO data, float yPos)
        {
            var btnGO = new GameObject($"Btn_{data.shipTypeName}");
            btnGO.transform.SetParent(transform, false);

            var rt = btnGO.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(BtnWidth, BtnHeight);
            rt.anchoredPosition = new Vector2(0, yPos);

            var img = btnGO.AddComponent<Image>();
            img.sprite = GetRoundedSprite();
            img.type = Image.Type.Sliced;
            Color dark = data.shipColor * 0.3f;
            dark.a = 1f;
            img.color = dark;

            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = img;
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.2f, 1.2f, 1.2f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
            btn.colors = colors;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(btnGO.transform, false);
            var labelRT = labelGO.AddComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.sizeDelta = Vector2.zero;
            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text = $"<b>{data.shipTypeName}</b>   <color=#ffdd88>{data.spawnCost:F0} E</color>";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 22;
            tmp.color = Color.white;

            var capturedData = data;
            btn.onClick.AddListener(() => _portal.LaunchShip(capturedData));
        }

        private void CreateCloseButton(float yPos)
        {
            var btnGO = new GameObject("Btn_Close");
            btnGO.transform.SetParent(transform, false);

            var rt = btnGO.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(160, 50);
            rt.anchoredPosition = new Vector2(0, yPos);

            var img = btnGO.AddComponent<Image>();
            img.sprite = GetRoundedSprite();
            img.type = Image.Type.Sliced;
            img.color = new Color(0.25f, 0.25f, 0.25f, 1f);

            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(Hide);

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(btnGO.transform, false);
            var labelRT = labelGO.AddComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.sizeDelta = Vector2.zero;
            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "Cerrar";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 20;
            tmp.color = Color.white;
        }

        // Generates a white rounded-rect sprite at runtime — no external asset needed.
        // The border Vector4 enables 9-slice so it scales without distortion.
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
                tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f, 0,
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
