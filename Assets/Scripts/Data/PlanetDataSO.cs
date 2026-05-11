using UnityEngine;

namespace DysonHarvest
{
    [CreateAssetMenu(fileName = "PlanetData", menuName = "Dyson Harvest/Planet Data")]
    public class PlanetDataSO : ScriptableObject
    {
        public string planetName = "Planet";
        public float orbitRadius = 15f;
        public float angularSpeedDeg = 10f;
        public float startingAngleDeg = 0f;
        public float gravityRadius = 0f;         // 0 = use GameConfigSO.defaultGravityRadius
        public float extractionYieldPerPulse = 5f;
        public bool isHostile = false;
        public Color planetColor = Color.blue;
        [Range(0.5f, 5f)]
        public float planetScale = 1.5f;
    }
}
