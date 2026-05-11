using System.Collections.Generic;
using UnityEngine;

namespace DysonHarvest
{
    public class ShipFlightMode : MonoBehaviour
    {
        private ShipController _ship;
        private EnergySystem _energySystem;
        private readonly Queue<Vector3> _waypoints = new();
        private bool _isActive;

        public bool HasExplicitOrder { get; private set; }
        public bool HasWaypoints => _waypoints.Count > 0;

        private void Awake()
        {
            _ship = GetComponent<ShipController>();
        }

        private void Start()
        {
            _energySystem = FindAnyObjectByType<EnergySystem>();
        }

        // Smooth movement every frame — only during Execution
        private void Update()
        {
            if (!_isActive) return;
            if (_waypoints.Count == 0) return;
            if (GameManager.Instance == null) return;
            if (GameManager.Instance.CurrentPhase != GamePhase.Execution) return;

            Vector3 target  = _waypoints.Peek();
            Vector3 current = transform.position;
            // speedUnitsPerPulse interpreted as units/second (pulse interval = 1s)
            float step = _ship.data.speedUnitsPerPulse * Time.deltaTime;

            Vector3 dir = (target - current).normalized;

            if (Vector3.Distance(current, target) <= step)
            {
                transform.position = target;
                _waypoints.Dequeue();
            }
            else
            {
                transform.position = current + dir * step;
            }

            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        // Called every pulse (1s): only energy cost, no movement
        public void OnPulse()
        {
            _energySystem.Consume(_ship.data.flightEnergyPerPulse);
        }

        public void Activate()
        {
            _isActive = true;
            HasExplicitOrder = true;
        }

        public void Deactivate()
        {
            _isActive = false;
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
    }
}
