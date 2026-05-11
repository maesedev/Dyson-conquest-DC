using UnityEngine;

namespace DysonHarvest
{
    public class HUDController : MonoBehaviour
    {
        public GameObject planningPanel;
        public GameObject executionPanel;

        private GameManager _gm;

        private void Start()
        {
            _gm = GameManager.Instance;
            _gm.OnPhaseChanged += OnPhaseChanged;

            // Sync panels with initial state (always Planning at start)
            OnPhaseChanged(GamePhase.Planning);
        }

        private void OnDestroy()
        {
            if (_gm != null)
                _gm.OnPhaseChanged -= OnPhaseChanged;
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            if (planningPanel != null)
                planningPanel.SetActive(phase == GamePhase.Planning);

            if (executionPanel != null)
                executionPanel.SetActive(phase == GamePhase.Execution);
        }
    }
}
