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
            lr.startWidth = 0.15f;
            lr.endWidth = 0.15f;
            lr.useWorldSpace = true;

            Color c = data.planetColor;
            c.a = 0.25f;
            lr.startColor = c;
            lr.endColor = c;

            // Use URP unlit so the line renders correctly without lighting
            lr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            lr.material.color = c;

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
