using System;
using System.Collections.Generic;
using UnityEngine;

namespace DysonHarvest
{
    // Tracks DysonModule ships near the Sun and drives the victory condition.
    // Place on any GameObject in the scene and assign config.
    public class DysonAssemblyController : MonoBehaviour
    {
        public GameConfigSO config;

        [Header("Slots")]
        [Tooltip("Number of Dyson Module slots needed to win.")]
        public int totalSlots = 4;
        [Tooltip("How close a DysonModule must be to a slot center to lock in.")]
        public float captureRadius = 4f;

        public float Progress { get; private set; }

        public event Action<float> OnProgressChanged;
        public event Action OnVictory;

        private DysonSlot[] _slots;
        private PulseController _pulseController;
        private readonly List<ShipController> _moduleShips = new();

        private struct DysonSlot
        {
            public Vector3 position;
            public bool occupied;
            public LineRenderer ring;
        }

        private static readonly Color SlotEmpty = new Color(0.3f, 0.3f, 0.3f);
        private static readonly Color SlotFilled = new Color(1f, 0.8f, 0.1f);

        private void Start()
        {
            _pulseController = FindAnyObjectByType<PulseController>();
            if (_pulseController != null)
                _pulseController.OnPulse += OnPulse;

            BuildSlots();
        }

        private void OnDestroy()
        {
            if (_pulseController != null)
                _pulseController.OnPulse -= OnPulse;
        }

        private void BuildSlots()
        {
            _slots = new DysonSlot[totalSlots];
            for (int i = 0; i < totalSlots; i++)
            {
                float angle = (360f / totalSlots * i) * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(
                    Mathf.Cos(angle) * config.dysonModuleSlotRadius,
                    0f,
                    Mathf.Sin(angle) * config.dysonModuleSlotRadius);

                _slots[i].position = pos;
                _slots[i].occupied = false;
                _slots[i].ring = BuildSlotRing(pos, i);
            }
        }

        private LineRenderer BuildSlotRing(Vector3 center, int index)
        {
            var go = new GameObject($"DysonSlot_{index}");
            go.transform.position = center;
            var lr = go.AddComponent<LineRenderer>();
            lr.loop = true;
            lr.useWorldSpace = true;
            lr.startWidth = 0.3f;
            lr.endWidth = 0.3f;
            lr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

            int segs = 32;
            lr.positionCount = segs;
            float r = captureRadius * 0.8f;
            for (int j = 0; j < segs; j++)
            {
                float a = (360f / segs * j) * Mathf.Deg2Rad;
                lr.SetPosition(j, center + new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r));
            }

            SetRingColor(lr, SlotEmpty);
            return lr;
        }

        private void OnPulse()
        {
            // Refresh module ship list every pulse (ships can be spawned mid-game)
            _moduleShips.Clear();
            foreach (var ship in FindObjectsByType<ShipController>(FindObjectsSortMode.None))
            {
                if (ship.data != null && ship.data.isDysonModule)
                    _moduleShips.Add(ship);
            }

            bool changed = false;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].occupied) continue;

                foreach (var ship in _moduleShips)
                {
                    if (Vector3.Distance(ship.transform.position, _slots[i].position) <= captureRadius)
                    {
                        _slots[i].occupied = true;
                        SetRingColor(_slots[i].ring, SlotFilled);
                        changed = true;
                        break;
                    }
                }
            }

            if (changed) UpdateProgress();
        }

        private void UpdateProgress()
        {
            int filled = 0;
            foreach (var slot in _slots)
                if (slot.occupied) filled++;

            Progress = (float)filled / totalSlots;
            OnProgressChanged?.Invoke(Progress);

            if (Progress >= 1f)
            {
                OnVictory?.Invoke();
                GameManager.Instance.TriggerVictory();
            }
        }

        private static void SetRingColor(LineRenderer lr, Color c)
        {
            lr.startColor = c;
            lr.endColor = c;
            lr.material.color = c;
        }
    }
}
