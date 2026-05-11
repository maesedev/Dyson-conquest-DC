using UnityEngine;

namespace DysonHarvest
{
    public static class ShipFactory
    {
        public static ShipController Create(ShipDataSO data, Vector3 worldPosition)
        {
            var go = new GameObject(data.shipTypeName);
            // Disable before adding components so Awake() fires only after data is assigned
            go.SetActive(false);

            go.layer = LayerMask.NameToLayer("Ship");
            go.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.z);

            // Renderer components required by ShipMeshBuilder
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            var mesh = go.AddComponent<ShipMeshBuilder>();

            // Behavior components
            go.AddComponent<ShipAnchorMode>();
            go.AddComponent<ShipFlightMode>();
            var controller = go.AddComponent<ShipController>();

            // Assign data before Awake fires
            mesh.data = data;
            controller.data = data;

            // Collider for click selection and trigger detection
            var col = go.AddComponent<SphereCollider>();
            col.radius = 2.5f;
            col.isTrigger = false;

            // Rigidbody kinematic: required for OnTriggerEnter to fire on planet gravity zones
            var rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            // Activate now — Awake() runs for all components with data set
            go.SetActive(true);
            return controller;
        }
    }
}
