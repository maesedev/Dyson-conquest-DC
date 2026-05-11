using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DysonHarvest
{
    public class DysonProgressDisplay : MonoBehaviour
    {
        public DysonAssemblyController assembly;
        public Image progressBar;
        public TMP_Text progressText;

        private void Start()
        {
            if (assembly != null)
                assembly.OnProgressChanged += Refresh;

            Refresh(0f);
        }

        private void OnDestroy()
        {
            if (assembly != null)
                assembly.OnProgressChanged -= Refresh;
        }

        private void Refresh(float progress)
        {
            if (progressBar != null)
                progressBar.fillAmount = progress;

            if (progressText != null)
                progressText.text = $"Esfera de Dyson: {progress * 100f:F0}%";
        }
    }
}
