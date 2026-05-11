using UnityEngine;

namespace DysonHarvest
{
    public enum ShipMode { Anchor, Flight }

    [RequireComponent(typeof(ShipAnchorMode), typeof(ShipFlightMode), typeof(ShipMeshBuilder))]
    public class ShipController : MonoBehaviour
    {
        public ShipDataSO data;

        public ShipMode CurrentMode { get; private set; } = ShipMode.Anchor;
        public PlanetOrbit AnchoredPlanet { get; private set; }

        [HideInInspector]
        public bool insideGravityZone;
        [HideInInspector]
        public PlanetGravityZone gravityZoneRef;

        private ShipAnchorMode _anchorMode;
        private ShipFlightMode _flightMode;
        private EnergySystem _energySystem;
        private PulseController _pulseController;

        private void Awake()
        {
            _anchorMode = GetComponent<ShipAnchorMode>();
            _flightMode = GetComponent<ShipFlightMode>();
        }

        private void Start()
        {
            if (data == null)
            {
                Debug.LogError($"[ShipController] '{gameObject.name}' necesita un ShipDataSO asignado en el Inspector.", this);
                return;
            }

            _pulseController = FindAnyObjectByType<PulseController>();
            _energySystem = FindAnyObjectByType<EnergySystem>();

            if (_pulseController != null)
                _pulseController.OnPulse += OnPulse;

            if (data.canExtract)
                _energySystem?.RegisterExtractor(this);
        }

        private void OnDestroy()
        {
            if (_pulseController != null)
                _pulseController.OnPulse -= OnPulse;

            if (data != null && data.canExtract)
                _energySystem?.UnregisterExtractor(this);
        }

        private void Update()
        {
            if (CurrentMode == ShipMode.Flight && insideGravityZone
                && !_flightMode.HasExplicitOrder && gravityZoneRef != null)
            {
                SetMode(ShipMode.Anchor, gravityZoneRef.ParentOrbit);
            }
        }

        private void OnPulse()
        {
            if (CurrentMode == ShipMode.Flight)
            {
                _flightMode.OnPulse();
                _flightMode.ClearExplicitOrderFlag();
            }
        }

        // --- Public order API ---

        public void OrderFlight(Vector3 destination)
        {
            float cost = GameManager.Instance.GetOrderCost(data.flightOrderCost);
            if (!_energySystem.CanAfford(cost)) return;
            _energySystem.Consume(cost);

            if (CurrentMode == ShipMode.Anchor)
                SetMode(ShipMode.Flight, null);

            _flightMode.SetDestination(destination);
        }

        public void OrderAnchor(PlanetOrbit planet)
        {
            float cost = GameManager.Instance.GetOrderCost(data.anchorOrderCost);
            if (!_energySystem.CanAfford(cost)) return;
            _energySystem.Consume(cost);

            SetMode(ShipMode.Anchor, planet);
        }

        public void SetMode(ShipMode mode, PlanetOrbit planet)
        {
            if (CurrentMode == ShipMode.Anchor)
                _anchorMode.Deactivate();
            else
                _flightMode.Deactivate();

            CurrentMode = mode;
            AnchoredPlanet = null;

            if (mode == ShipMode.Anchor && planet != null)
            {
                AnchoredPlanet = planet;
                _anchorMode.Activate(planet);

                if (data.canExtract)
                    _energySystem?.RegisterExtractor(this);
            }
            else
            {
                _flightMode.Activate();

                if (data.canExtract)
                    _energySystem?.UnregisterExtractor(this);
            }
        }
    }
}
