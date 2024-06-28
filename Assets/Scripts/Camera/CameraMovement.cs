using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 0.01f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float zoomSpeed = 0.1f; // Уменьшенное значение скорости зумирования
    [SerializeField] private float minZoom = 5.0f;
    [SerializeField] private float maxZoom = 15.0f;
    [SerializeField] private float zoomLerpSpeed = 10.0f; // Скорость интерполяции

    private Vector3 dragOrigin;

    private MainControls controls;
    private Vector2 move;
    private float zoom;
    private float targetZoom; // Новый параметр для целевого зума

    private Camera cam;

    private void Awake()
    {
        controls = new MainControls();
        cam = Camera.main;
        targetZoom = cam.orthographicSize; // Устанавливаем начальное значение зума
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Default.CameraMove.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Default.CameraMove.canceled += ctx => move = Vector2.zero;
        controls.Default.Zoom.performed += ctx => AdjustZoom(ctx.ReadValue<float>());
        controls.Default.Zoom.canceled += ctx => zoom = 0;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        RightMouseDrag();
        WASDMouseDrag();
        SmoothZoom();
    }

    void RightMouseDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);
    }

    void AdjustZoom(float zoomValue)
    {
        if (cam.orthographic)
        {
            targetZoom -= zoomValue * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
        else
        {
            targetZoom -= zoomValue * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    void SmoothZoom()
    {
        if (cam.orthographic) // Если используешь ортографическую камеру
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
        }
        else // Если используешь перспективную камеру, настрой Field of View (FOV)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomLerpSpeed);
        }
    }

    void WASDMouseDrag()
    {
        Vector3 movement = new Vector3(move.x, move.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
}
