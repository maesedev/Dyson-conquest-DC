using System;
using UnityEngine;

namespace DysonHarvest
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Config")]
        public GameConfigSO config;

        [Header("Channels")]
        public EnergyChannelSO energyChannel;

        [Header("Systems")]
        public PulseController pulseController;
        public EnergySystem energySystem;

        public GamePhase CurrentPhase { get; private set; } = GamePhase.Planning;
        public bool GameEnded { get; private set; }

        public event Action<GamePhase> OnPhaseChanged;
        public event Action OnVictory;
        public event Action OnGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            energyChannel.Initialize(config.startingEnergy);
            CurrentPhase = GamePhase.Planning;
        }

        public void StartExecution()
        {
            if (CurrentPhase == GamePhase.Execution) return;
            CurrentPhase = GamePhase.Execution;
            OnPhaseChanged?.Invoke(CurrentPhase);
            pulseController.StartPulsing();
        }

        public void ReturnToPlanning()
        {
            if (CurrentPhase == GamePhase.Planning) return;
            pulseController.StopPulsing();
            CurrentPhase = GamePhase.Planning;
            OnPhaseChanged?.Invoke(CurrentPhase);
        }

        public float GetOrderCost(float baseCost)
        {
            return CurrentPhase == GamePhase.Execution
                ? baseCost * config.emergencyOrderMultiplier
                : baseCost;
        }

        public void TriggerVictory()
        {
            if (GameEnded) return;
            GameEnded = true;
            pulseController.StopPulsing();
            OnVictory?.Invoke();
        }

        public void TriggerGameOver()
        {
            if (GameEnded) return;
            GameEnded = true;
            pulseController.StopPulsing();
            CurrentPhase = GamePhase.Planning;
            OnPhaseChanged?.Invoke(CurrentPhase);
            OnGameOver?.Invoke();
        }
    }
}
