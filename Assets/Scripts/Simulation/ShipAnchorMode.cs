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
            transform.SetParent(planet.transform, worldPositionStays: true);
            Vector3 local = transform.localPosition;

            // If the ship is inside or too close to the planet center, push it to just outside the surface
            float surfaceDist = planet.data.planetScale * 0.5f + 1f;
            Vector2 flatOffset = new Vector2(local.x, local.z);
            if (flatOffset.magnitude < surfaceDist)
            {
                // Default anchor position: just in front of the planet along +Z
                local.x = 0f;
                local.z = surfaceDist;
            }

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
