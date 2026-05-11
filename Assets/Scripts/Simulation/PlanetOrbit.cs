using UnityEngine;

namespace DysonHarvest
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PlanetOrbit : MonoBehaviour
    {
        public PlanetDataSO data;
        public GameConfigSO config;

        [HideInInspector]
        public float currentAngleDeg;

        private PulseController _pulseController;

        private void Start()
        {
            _pulseController = FindFirstObjectByType<PulseController>();
            if (_pulseController != null)
                _pulseController.OnPulse += OnPulse;

            currentAngleDeg = data.startingAngleDeg;
            ApplyPosition();

            transform.localScale = Vector3.one * data.planetScale;
            GetComponent<MeshRenderer>().material.color = data.planetColor;
        }

        private void OnDestroy()
        {
            if (_pulseController != null)
                _pulseController.OnPulse -= OnPulse;
        }

        private void OnPulse()
        {
            currentAngleDeg += data.angularSpeedDeg;
            if (currentAngleDeg >= 360f)
                currentAngleDeg -= 360f;
            ApplyPosition();
        }

        private void ApplyPosition()
        {
            transform.position = AngleToPosition(currentAngleDeg, data.orbitRadius);
        }

        public Vector3 GetPositionAtPulse(int pulsesAhead)
        {
            float futureAngle = currentAngleDeg + data.angularSpeedDeg * pulsesAhead;
            return AngleToPosition(futureAngle, data.orbitRadius);
        }

        private static Vector3 AngleToPosition(float angleDeg, float radius)
        {
            float rad = angleDeg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius);
        }

        public float GetEffectiveGravityRadius()
        {
            return data.gravityRadius > 0f ? data.gravityRadius : config.defaultGravityRadius;
        }
    }
}
