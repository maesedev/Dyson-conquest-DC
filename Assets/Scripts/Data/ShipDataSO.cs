using UnityEngine;

namespace DysonHarvest
{
    [CreateAssetMenu(fileName = "ShipData", menuName = "Dyson Harvest/Ship Data")]
    public class ShipDataSO : ScriptableObject
    {
        public string shipTypeName = "Scout";
        public float spawnCost = 5f;
        public float flightEnergyPerPulse = 2f;
        public float speedUnitsPerPulse = 5f;
        public float anchorOrderCost = 1f;
        public float flightOrderCost = 1f;
        public bool canExtract = false;
        public bool isDysonModule = false;
        public Color shipColor = Color.cyan;
        [Range(0.5f, 5f)]
        public float meshScale = 3f;
    }
}
