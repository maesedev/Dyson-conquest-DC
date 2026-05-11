using System;
using System.Collections;
using UnityEngine;

namespace DysonHarvest
{
    public class PulseController : MonoBehaviour
    {
        public GameConfigSO config;

        public event Action OnPulse;

        private Coroutine _pulseCoroutine;
        public bool IsRunning { get; private set; }

        public void StartPulsing()
        {
            if (IsRunning) return;
            IsRunning = true;
            _pulseCoroutine = StartCoroutine(PulseLoop());
        }

        public void StopPulsing()
        {
            if (!IsRunning) return;
            IsRunning = false;
            if (_pulseCoroutine != null)
                StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = null;
        }

        private IEnumerator PulseLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(config.pulseIntervalSeconds);
                OnPulse?.Invoke();
            }
        }
    }
}
