using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSetScale : MonoBehaviour
{
    public float targetWidth = 1920;
    public float targetHeight = 1080;

    private CanvasScaler canvasScaler;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        UpdateUIScale();
    }

    void Update()
    {
        // Si necesitas actualizar la escala en cada frame, descomenta la siguiente l�nea
        // UpdateUIScale();
    }

    void UpdateUIScale()
    {
        float targetAspectRatio = targetWidth / targetHeight;
        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (currentAspectRatio >= targetAspectRatio)
        {
            // Si la resoluci�n actual es m�s ancha que la de referencia
            canvasScaler.matchWidthOrHeight = 1;
        }
        else
        {
            // Si la resoluci�n actual es m�s alta que la de referencia
            canvasScaler.matchWidthOrHeight = 0;
        }

        canvasScaler.referenceResolution = new Vector2(targetWidth, targetHeight);
    }
}
