using System.Collections.Generic;
using UnityEngine;

namespace DysonHarvest
{
    public class EnergySystem : MonoBehaviour
    {
        public GameConfigSO config;
        public EnergyChannelSO energyChannel;

        private PulseController _pulseController;
        private readonly List<ShipController> _activeExtractors = new();

        private void Start()
        {
            _pulseController = FindAnyObjectByType<PulseController>();
            if (_pulseController != null)
                _pulseController.OnPulse += OnPulse;
        }

        private void OnDestroy()
        {
            if (_pulseController != null)
                _pulseController.OnPulse -= OnPulse;
        }

        private void OnPulse()
        {
            Consume(config.portalEntropyPerPulse);

            if (energyChannel.Value <= 0f)
            {
                GameManager.Instance.TriggerGameOver();
                return;
            }

            // Extraction income from ships anchored on extractable planets
            foreach (var ship in _activeExtractors)
            {
                if (ship == null || ship.AnchoredPlanet == null) continue;
                float yield = ship.AnchoredPlanet.data.isHostile
                    ? -ship.AnchoredPlanet.data.extractionYieldPerPulse
                    : ship.AnchoredPlanet.data.extractionYieldPerPulse;
                Produce(yield);
            }
        }

        public bool Consume(float amount)
        {
            if (energyChannel.Value < amount)
            {
                energyChannel.Value = 0f;
                return false;
            }
            energyChannel.Value -= amount;
            return true;
        }

        public void Produce(float amount)
        {
            energyChannel.Value += amount;
        }

        public bool CanAfford(float amount) => energyChannel.Value >= amount;

        public void RegisterExtractor(ShipController ship)
        {
            if (!_activeExtractors.Contains(ship))
                _activeExtractors.Add(ship);
        }

        public void UnregisterExtractor(ShipController ship)
        {
            _activeExtractors.Remove(ship);
        }
    }
}
