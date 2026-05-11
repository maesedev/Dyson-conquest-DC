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
            Color dark = data.shipColor * 0.25f;
            dark.a = 0.92f;
            img.color = dark;

            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = img;
            var colors = btn.colors;
            colors.highlightedColor = data.shipColor * 0.5f;
            colors.pressedColor = data.shipColor * 0.7f;
            btn.colors = colors;

            // Label
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
            img.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

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
    }
}
