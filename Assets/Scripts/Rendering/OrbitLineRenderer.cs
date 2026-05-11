using UnityEngine;

namespace DysonHarvest
{
    [RequireComponent(typeof(LineRenderer))]
    public class OrbitLineRenderer : MonoBehaviour
    {
        public PlanetDataSO data;
        [Range(32, 128)]
        public int segments = 64;

        private void Start()
        {
            var lr = GetComponent<LineRenderer>();
            lr.loop = true;
            lr.positionCount = segments;
            lr.startWidth = 0.5f;
            lr.endWidth = 0.5f;
            lr.useWorldSpace = true;

            // Use a dimmed opaque color (avoids transparency shader setup issues)
            Color c = data.planetColor * 0.35f;
            c.a = 1f;
            lr.startColor = c;
            lr.endColor = c;

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = c;
            lr.material = mat;

            for (int i = 0; i < segments; i++)
            {
                float angle = (360f / segments) * i * Mathf.Deg2Rad;
                lr.SetPosition(i, new Vector3(
                    Mathf.Cos(angle) * data.orbitRadius,
                    0f,
                    Mathf.Sin(angle) * data.orbitRadius));
            }
        }
    }
}
