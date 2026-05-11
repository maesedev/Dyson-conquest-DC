using System.Collections.Generic;
using UnityEngine;

namespace DysonHarvest
{
    public class ShipFlightMode : MonoBehaviour
    {
        private ShipController _ship;
        private EnergySystem _energySystem;
        private readonly Queue<Vector3> _waypoints = new();

        // Set when the player explicitly ordered Flight this pulse; prevents auto-anchor
        public bool HasExplicitOrder { get; private set; }

        private void Awake()
        {
            _ship = GetComponent<ShipController>();
        }

        private void Start()
        {
            _energySystem = FindFirstObjectByType<EnergySystem>();
        }

        public void Activate()
        {
            HasExplicitOrder = true;
        }

        public void Deactivate()
        {
            _waypoints.Clear();
            HasExplicitOrder = false;
        }

        public void SetDestination(Vector3 worldPos)
        {
            _waypoints.Clear();
            _waypoints.Enqueue(worldPos);
            HasExplicitOrder = true;
        }

        public void ClearExplicitOrderFlag()
        {
            HasExplicitOrder = false;
        }

        public void OnPulse()
        {
            float cost = _ship.data.flightEnergyPerPulse;
            _energySystem.Consume(cost);

            if (_waypoints.Count == 0) return;

            Vector3 target = _waypoints.Peek();
            float step = _ship.data.speedUnitsPerPulse;
            Vector3 current = transform.position;

            if (Vector3.Distance(current, target) <= step)
            {
                transform.position = target;
                _waypoints.Dequeue();
            }
            else
            {
                transform.position = current + (target - current).normalized * step;
            }

            // Orient ship toward movement direction
            if (_waypoints.Count > 0 || Vector3.Distance(current, target) > 0.01f)
            {
                Vector3 dir = (target - current).normalized;
                if (dir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }

        public bool HasWaypoints => _waypoints.Count > 0;
    }
}
