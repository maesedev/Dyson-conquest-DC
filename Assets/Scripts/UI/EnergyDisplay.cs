using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DysonHarvest
{
    public class EnergyDisplay : MonoBehaviour
    {
        public EnergyChannelSO energyChannel;
        public GameConfigSO config;
        public TMP_Text energyText;
        public Image energyBar;

        private void OnEnable()
        {
            energyChannel.OnValueChanged += Refresh;
        }

        private void OnDisable()
        {
            energyChannel.OnValueChanged -= Refresh;
        }

        private void Start()
        {
            Refresh(energyChannel.Value);
        }

        private void Refresh(float value)
        {
            if (energyText != null)
                energyText.text = $"{value:F0} E";

            if (energyBar != null)
                energyBar.fillAmount = config.startingEnergy > 0f
                    ? Mathf.Clamp01(value / config.startingEnergy)
                    : 0f;
        }
    }
}
