using UnityEngine;

namespace DysonHarvest
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Dyson Harvest/Game Config")]
    public class GameConfigSO : ScriptableObject
    {
        [Header("Pulse")]
        public float pulseIntervalSeconds = 1.5f;
        public int previewPulseCount = 3;

        [Header("Energy")]
        public float startingEnergy = 100f;
        public float portalEntropyPerPulse = 3f;
        public float emergencyOrderMultiplier = 2.5f;

        [Header("Gravity")]
        public float defaultGravityRadius = 8f;

        [Header("Victory")]
        public float dysonModuleSlotRadius = 5f;
    }
}
