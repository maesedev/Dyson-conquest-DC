using UnityEngine;
using UnityEngine.InputSystem;

namespace DysonHarvest
{
    // Handles keyboard shortcuts that control the game phase.
    // Mouse input is handled directly in CameraController and WaypointPlacer.
    public class DysonInputHandler : MonoBehaviour
    {
        private GameManager _gm;

        private void Start()
        {
            _gm = GameManager.Instance;
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // Space or Enter toggles between Planning and Execution
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                TogglePhase();
            }
        }

        private void TogglePhase()
        {
            if (_gm.CurrentPhase == GamePhase.Planning)
                _gm.StartExecution();
            else
                _gm.ReturnToPlanning();
        }
    }
}
