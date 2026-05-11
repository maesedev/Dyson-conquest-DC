using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DysonHarvest
{
    // Handles ship selection and waypoint placement via mouse clicks.
    // Attach to an empty GameObject in the scene. Assign mainCamera.
    public class WaypointPlacer : MonoBehaviour
    {
        public Camera mainCamera;

        [Tooltip("Layer mask for the invisible world-plane collider (Y=0 quad) used for raycasting.")]
        public LayerMask worldPlaneLayer;

        [Tooltip("Layer mask for ship colliders.")]
        public LayerMask shipLayer;

        public ShipController SelectedShip { get; private set; }

        private GameManager _gm;
        private readonly List<GameObject> _waypointMarkers = new();
        private Material _markerMat;

        private void Start()
        {
            _gm = GameManager.Instance;
            _markerMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            _markerMat.color = Color.yellow;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Left-click: select ship
            if (mouse.leftButton.wasPressedThisFrame)
                TrySelectShip();

            // Right-click: order selected ship to fly to world position
            if (mouse.rightButton.wasPressedThisFrame && SelectedShip != null)
                TryOrderFlight();

            // Cancel selection
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                Deselect();
        }

        private void TrySelectShip()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, shipLayer))
            {
                var ship = hit.collider.GetComponentInParent<ShipController>();
                if (ship != null)
                {
                    SelectedShip = ship;
                    return;
                }
            }
            Deselect();
        }

        private void TryOrderFlight()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, worldPlaneLayer))
            {
                Vector3 destination = hit.point;
                destination.y = 0f;

                SelectedShip.OrderFlight(destination);
                PlaceWaypointMarker(destination);
            }
        }

        private void PlaceWaypointMarker(Vector3 worldPos)
        {
            ClearWaypointMarkers();

            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "WaypointMarker";
            marker.transform.position = worldPos;
            marker.transform.localScale = Vector3.one * 0.4f;
            Destroy(marker.GetComponent<Collider>());
            marker.GetComponent<MeshRenderer>().material = _markerMat;

            _waypointMarkers.Add(marker);
        }

        public void ClearWaypointMarkers()
        {
            foreach (var m in _waypointMarkers)
                if (m != null) Destroy(m);
            _waypointMarkers.Clear();
        }

        public void ForceSelect(ShipController ship)
        {
            SelectedShip = ship;
        }

        public void Deselect()
        {
            SelectedShip = null;
            ClearWaypointMarkers();
        }
    }
}
