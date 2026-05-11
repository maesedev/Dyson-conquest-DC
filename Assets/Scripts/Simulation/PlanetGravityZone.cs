using UnityEngine;

namespace DysonHarvest
{
    // Attach to a child GameObject of the planet (with a SphereCollider trigger).
    // PlanetOrbit.Start() creates this child automatically — see SolarSystemSetup.
    [RequireComponent(typeof(SphereCollider))]
    public class PlanetGravityZone : MonoBehaviour
    {
        public PlanetOrbit ParentOrbit { get; private set; }

        public void Initialize(PlanetOrbit orbit)
        {
            ParentOrbit = orbit;
            var col = GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = orbit.GetEffectiveGravityRadius();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ShipController>(out var ship))
            {
                ship.insideGravityZone = true;
                ship.gravityZoneRef = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<ShipController>(out var ship))
            {
                ship.insideGravityZone = false;
                if (ship.gravityZoneRef == this)
                    ship.gravityZoneRef = null;
            }
        }
    }
}
