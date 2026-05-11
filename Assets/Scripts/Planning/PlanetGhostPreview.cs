using UnityEngine;

namespace DysonHarvest
{
    // Shows semi-transparent ghost spheres at future planet positions during Planning.
    // Attach to the same GameObject as PlanetOrbit (or add from SolarSystemSetup).
    public class PlanetGhostPreview : MonoBehaviour
    {
        public PlanetOrbit orbit;
        public GameConfigSO config;

        private GameObject[] _ghosts;
        private Material[] _ghostMats;
        private GameManager _gm;

        private void Start()
        {
            _gm = GameManager.Instance;
            _gm.OnPhaseChanged += OnPhaseChanged;

            BuildGhosts();

            // Show ghosts immediately since game starts in Planning
            SetGhostsActive(true);
            RefreshGhostPositions();
        }

        private void OnDestroy()
        {
            if (_gm != null)
                _gm.OnPhaseChanged -= OnPhaseChanged;

            if (_ghosts == null) return;
            foreach (var g in _ghosts)
                if (g != null) Destroy(g);
        }

        private void BuildGhosts()
        {
            int count = config.previewPulseCount;
            _ghosts = new GameObject[count];
            _ghostMats = new Material[count];

            Color baseColor = orbit.data.planetColor;

            for (int i = 0; i < count; i++)
            {
                var ghost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ghost.name = $"{orbit.data.planetName}_Ghost_{i + 1}";
                ghost.transform.localScale = Vector3.one * orbit.data.planetScale;
                Destroy(ghost.GetComponent<Collider>());

                float t = (float)i / Mathf.Max(count - 1, 1);
                float alpha = Mathf.Lerp(0.35f, 0.10f, t);

                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                // URP Lit transparent surface
                mat.SetFloat("_Surface", 1f);          // 1 = Transparent
                mat.SetFloat("_Blend", 0f);             // Alpha blend
                mat.renderQueue = 3000;
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                Color c = baseColor;
                c.a = alpha;
                mat.color = c;

                ghost.GetComponent<MeshRenderer>().material = mat;
                _ghosts[i] = ghost;
                _ghostMats[i] = mat;
            }
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            if (phase == GamePhase.Planning)
            {
                SetGhostsActive(true);
                RefreshGhostPositions();
            }
            else
            {
                SetGhostsActive(false);
            }
        }

        private void RefreshGhostPositions()
        {
            for (int i = 0; i < _ghosts.Length; i++)
                _ghosts[i].transform.position = orbit.GetPositionAtPulse(i + 1);
        }

        private void SetGhostsActive(bool active)
        {
            foreach (var g in _ghosts)
                g.SetActive(active);
        }
    }
}
