using UnityEngine;

namespace DysonHarvest
{
    public class PhaseButtonController : MonoBehaviour
    {
        public void OnExecuteButtonPressed()
        {
            GameManager.Instance.StartExecution();
        }

        public void OnPlanningButtonPressed()
        {
            GameManager.Instance.ReturnToPlanning();
        }
    }
}
