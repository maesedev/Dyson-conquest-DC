using UnityEngine;

namespace DysonHarvest
{
    // Drag this onto an empty GameObject in the scene.
    // Assign GameConfig and the PlanetData assets; it will build the solar system at runtime.
    public class SolarSystemSetup : MonoBehaviour
    {
        public GameConfigSO config;
        public PlanetDataSO[] planets;

        [Header("Sun")]
        public Color sunColor = new Color(1f, 0.9f, 0.2f);
        public float sunScale = 3f;

        private void Awake()
        {
            BuildSun();
            foreach (var data in planets)
                BuildPlanet(data);
        }

        private void BuildSun()
        {
            var sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sun.name = "Sun";
            sun.transform.position = Vector3.zero;
            sun.transform.localScale = Vector3.one * sunScale;
            Destroy(sun.GetComponent<Collider>());

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = sunColor;
            // Make it emissive so it "glows" without any shader tricks
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", sunColor * 2f);
            sun.GetComponent<MeshRenderer>().material = mat;
        }

        private void BuildPlanet(PlanetDataSO data)
        {
            // --- Planet sphere ---
            var planetGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planetGO.name = data.planetName;
            Destroy(planetGO.GetComponent<Collider>());

            var orbit = planetGO.AddComponent<PlanetOrbit>();
            orbit.data = data;
            orbit.config = config;

            var orbitLine = new GameObject("OrbitLine").AddComponent<OrbitLineRenderer>();
            orbitLine.data = data;
            orbitLine.transform.SetParent(planetGO.transform, false);
            orbitLine.transform.localPosition = Vector3.zero;

            // --- Gravity zone child ---
            var gravityGO = new GameObject("GravityZone");
            gravityGO.transform.SetParent(planetGO.transform, false);
            gravityGO.transform.localPosition = Vector3.zero;
            gravityGO.layer = LayerMask.NameToLayer("Default");

            var col = gravityGO.AddComponent<SphereCollider>();
            col.isTrigger = true;

            var zone = gravityGO.AddComponent<PlanetGravityZone>();
            zone.Initialize(orbit);

            // --- Ghost preview (visible only during Planning phase) ---
            var ghost = planetGO.AddComponent<PlanetGhostPreview>();
            ghost.orbit = orbit;
            ghost.config = config;
        }
    }
}
