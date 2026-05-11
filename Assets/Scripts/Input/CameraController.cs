using UnityEngine;
using UnityEngine.InputSystem;

namespace DysonHarvest
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Zoom")]
        public float zoomMin = 5f;
        public float zoomMax = 80f;
        public float zoomSensitivity = 4f;

        [Header("Pan")]
        public float panSensitivity = 0.05f;

        private Camera _cam;
        private bool _isPanning;
        private Vector2 _panLastMousePos;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;
            // Ensure top-down orientation
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void Update()
        {
            HandleZoom();
            HandlePan();
        }

        private void HandleZoom()
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Approximately(scroll, 0f)) return;

            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - scroll * zoomSensitivity * Time.unscaledDeltaTime,
                zoomMin,
                zoomMax);
        }

        private void HandlePan()
        {
            var mouse = Mouse.current;

            if (mouse.middleButton.wasPressedThisFrame ||
                (mouse.rightButton.wasPressedThisFrame && Keyboard.current.altKey.isPressed))
            {
                _isPanning = true;
                _panLastMousePos = mouse.position.ReadValue();
            }

            if (mouse.middleButton.wasReleasedThisFrame ||
                (mouse.rightButton.wasReleasedThisFrame && _isPanning))
            {
                _isPanning = false;
            }

            if (!_isPanning) return;

            Vector2 currentPos = mouse.position.ReadValue();
            Vector2 delta = currentPos - _panLastMousePos;
            _panLastMousePos = currentPos;

            // Scale pan speed by orthographic size so it feels consistent at all zoom levels
            float scale = _cam.orthographicSize * panSensitivity;
            Vector3 move = new Vector3(-delta.x * scale, 0f, -delta.y * scale);
            transform.position += move;
        }
    }
}
