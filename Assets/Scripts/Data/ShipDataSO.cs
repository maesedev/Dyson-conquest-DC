using UnityEngine;

namespace DysonHarvest
{
    [CreateAssetMenu(fileName = "ShipData", menuName = "Dyson Harvest/Ship Data")]
    public class ShipDataSO : ScriptableObject
    {
        public string shipTypeName = "Scout";
        public float flightEnergyPerPulse = 2f;
        public float speedUnitsPerPulse = 5f;
        public float anchorOrderCost = 1f;
        public float flightOrderCost = 1f;
        public bool canExtract = false;
        public bool isDysonModule = false;
        public Color shipColor = Color.cyan;
        [Range(0.2f, 3f)]
        public float meshScale = 0.6f;
    }
}
