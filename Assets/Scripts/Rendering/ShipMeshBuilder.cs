using UnityEngine;

namespace DysonHarvest
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ShipMeshBuilder : MonoBehaviour
    {
        public ShipDataSO data;

        private void Awake()
        {
            if (data == null)
            {
                Debug.LogError($"[ShipMeshBuilder] '{gameObject.name}' necesita un ShipDataSO asignado en el Inspector.", this);
                return;
            }

            GetComponent<MeshFilter>().mesh = BuildArrowMesh(data.meshScale);

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = data.shipColor;
            GetComponent<MeshRenderer>().material = mat;
        }

        public static Mesh BuildArrowMesh(float scale)
        {
            // Isosceles triangle pointing in +Z, lying flat on XZ plane (Y=0)
            var mesh = new Mesh { name = "ShipArrow" };

            // Y=2 keeps the ship clearly above all planet surfaces (spheres reach Y≈0.9 max)
            var vertices = new Vector3[]
            {
                new Vector3(0f,            2f,  0.6f * scale),
                new Vector3(-0.3f * scale, 2f, -0.4f * scale),
                new Vector3( 0.3f * scale, 2f, -0.4f * scale),
            };

            // Double-sided: front face [0,1,2], back face [0,2,1]
            var triangles = new int[] { 0, 1, 2, 0, 2, 1 };

            var normals = new Vector3[]
            {
                Vector3.up, Vector3.up, Vector3.up
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
