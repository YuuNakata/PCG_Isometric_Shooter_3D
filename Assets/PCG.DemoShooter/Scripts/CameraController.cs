using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Drag Settings")]
    public float moveSpeed = 1f;
    private Vector2 dragOrigin;
    private bool dragging = false;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 30f;


    void Update()
    {
        var mouse = Mouse.current;

        // Drag con click izquierdo
        if (mouse.leftButton.wasPressedThisFrame)
        {
            dragOrigin = mouse.position.ReadValue();
            dragging = true;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            dragging = false;
        }

        if (dragging)
        {
            Vector2 currentPos = mouse.position.ReadValue();
            Vector2 delta = currentPos - dragOrigin;

            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * moveSpeed * Time.deltaTime;

            Vector3 right = Camera.main.transform.right;
            Vector3 forward = Camera.main.transform.forward;
            right.y = 0; forward.y = 0;
            right.Normalize(); forward.Normalize();

            Vector3 worldMove = right * move.x + forward * move.z;

            if (!Physics.Raycast(transform.position, worldMove.normalized, 1f))
            {
                transform.Translate(worldMove, Space.World);
            }

            dragOrigin = currentPos;
        }

        // Zoom con la rueda del mouse
        // Zoom con la rueda del mouse (zoom real en cámara ortográfica)
        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0f)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }

    }
}
