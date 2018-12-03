using UnityEngine;

namespace Rendering
{
    public class CameraController : MonoBehaviour
    {
        private Vector3 _startPos;
        private Vector3 _lastMousePosition;
        
        [Range(0f,0.1f)]
        [SerializeField]private float _sensitivity;

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _startPos = transform.position;
                _lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                var mouseDelta = _lastMousePosition - Input.mousePosition;
                transform.Translate(mouseDelta.x * _sensitivity, mouseDelta.y * _sensitivity, 0);
                _lastMousePosition = Input.mousePosition;
            }
        }
    }
}