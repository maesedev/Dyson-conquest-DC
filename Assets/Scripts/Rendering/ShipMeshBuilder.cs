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

            var vertices = new Vector3[]
            {
                new Vector3(0f,         0f,  0.6f * scale),  // tip  (front)
                new Vector3(-0.3f * scale, 0f, -0.4f * scale), // left base
                new Vector3( 0.3f * scale, 0f, -0.4f * scale), // right base
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
