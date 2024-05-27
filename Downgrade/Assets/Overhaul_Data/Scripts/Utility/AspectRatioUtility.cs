using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioUtility : MonoBehaviour
{
    // El aspect ratio deseado (ejemplo: 16:9)
    public float targetAspectRatio = 16.0f / 9.0f;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        UpdateAspectRatio();
    }

    void Update()
    {
        // Actualiza el aspect ratio en cada frame (opcional)
        UpdateAspectRatio();
    }

    public void UpdateAspectRatio()
    {
        // El aspect ratio actual de la ventana
        float windowAspectRatio = (float)Screen.width / (float)Screen.height;

        // La escala de la ventana en relación con el aspect ratio objetivo
        float scaleHeight = windowAspectRatio / targetAspectRatio;

        // Si la ventana es más ancha que el aspect ratio objetivo
        if (scaleHeight < 1.0f)
        {
            Rect rect = _camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            _camera.rect = rect;
        }
        else // Si la ventana es más alta que el aspect ratio objetivo
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = _camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            _camera.rect = rect;
        }
    }
}