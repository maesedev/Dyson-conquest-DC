using UnityEngine;

namespace DysonHarvest
{
    public class ShipAnchorMode : MonoBehaviour
    {
        private ShipController _ship;

        private void Awake()
        {
            _ship = GetComponent<ShipController>();
        }

        public void Activate(PlanetOrbit planet)
        {
            // Re-parent under the planet; Unity keeps world position intact
            transform.SetParent(planet.transform, worldPositionStays: true);
            // Snap onto the planet surface plane (Y=0 relative to planet)
            Vector3 local = transform.localPosition;
            local.y = 0f;
            transform.localPosition = local;
        }

        public void Deactivate()
        {
            // Unparent, preserving current world position
            transform.SetParent(null, worldPositionStays: true);
        }
    }
}
