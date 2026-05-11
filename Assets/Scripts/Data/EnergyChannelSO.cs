using System;
using UnityEngine;

namespace DysonHarvest
{
    [CreateAssetMenu(fileName = "EnergyChannel", menuName = "Dyson Harvest/Energy Channel")]
    public class EnergyChannelSO : ScriptableObject
    {
        public event Action<float> OnValueChanged;

        [NonSerialized]
        private float _runtimeValue;

        public float Value
        {
            get => _runtimeValue;
            set
            {
                _runtimeValue = Mathf.Max(0f, value);
                OnValueChanged?.Invoke(_runtimeValue);
            }
        }

        public void Initialize(float startingValue)
        {
            _runtimeValue = startingValue;
            OnValueChanged?.Invoke(_runtimeValue);
        }
    }
}
