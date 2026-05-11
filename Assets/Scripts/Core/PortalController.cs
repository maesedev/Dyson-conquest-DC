using UnityEngine;

namespace DysonHarvest
{
    // Place this on an empty GameObject in the scene.
    // The portal builds its own visual and handles ship spawning.
    public class PortalController : MonoBehaviour
    {
        [Header("Config")]
        public GameConfigSO config;
        public ShipDataSO[] launchableShips;

        [Header("References")]
        public Camera mainCamera;
        public WaypointPlacer waypointPlacer;
        public PortalLaunchPanel launchPanel;

        [Header("Position")]
        public float orbitRadius = 38f;
        public float angleDeg = 270f;

        private static readonly Color _portalColor = new Color(0.55f, 0.1f, 1f);
        private EnergySystem _energySystem;

        private void Awake()
        {
            float rad = angleDeg * Mathf.Deg2Rad;
            transform.position = new Vector3(
                Mathf.Cos(rad) * orbitRadius, 0f,
                Mathf.Sin(rad) * orbitRadius);

            BuildVisual();
        }

        private void Start()
        {
            _energySystem = FindAnyObjectByType<EnergySystem>();
            if (launchPanel != null)
                launchPanel.Initialize(this, launchableShips);
        }

        private void OnMouseDown()
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.Planning) return;
            launchPanel?.Show();
        }

        public void LaunchShip(ShipDataSO data)
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.Planning) return;
            if (_energySystem == null || !_energySystem.CanAfford(data.spawnCost))
            {
                Debug.Log($"[Portal] No hay suficiente energía para lanzar {data.shipTypeName} (costo: {data.spawnCost})");
                return;
            }

            _energySystem.Consume(data.spawnCost);
            var ship = ShipFactory.Create(data, transform.position);

            // Auto-select the new ship so the player can immediately give it a route
            waypointPlacer?.Deselect();
            if (waypointPlacer != null)
                waypointPlacer.ForceSelect(ship);

            launchPanel?.Hide();
        }

        private void BuildVisual()
        {
            // Ring of small spheres around the portal center
            int count = 10;
            float ringRadius = 2.5f;
            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count * i) * Mathf.Deg2Rad;
                var node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                node.name = "PortalNode";
                node.transform.SetParent(transform, false);
                node.transform.localPosition = new Vector3(
                    Mathf.Cos(angle) * ringRadius, 0f,
                    Mathf.Sin(angle) * ringRadius);
                node.transform.localScale = Vector3.one * 0.6f;
                Destroy(node.GetComponent<Collider>());

                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = _portalColor;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", _portalColor * 1.5f);
                node.GetComponent<MeshRenderer>().material = mat;
            }

            // Central collider for click detection
            var col = gameObject.AddComponent<SphereCollider>();
            col.radius = ringRadius + 0.5f;
            col.isTrigger = false;

            // Label
            var labelGO = new GameObject("PortalLabel");
            labelGO.transform.SetParent(transform, false);
            labelGO.transform.localPosition = new Vector3(0f, 0f, -4f);
        }
    }
}
