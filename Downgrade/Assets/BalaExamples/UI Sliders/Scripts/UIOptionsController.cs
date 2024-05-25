using UnityEngine.UI;
using UnityEngine;

public class UIOptionsController : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private Image toggleImage;
    [SerializeField] private Sprite[] toggleStatusSprites;

    [SerializeField] private string option;
    [TextArea]
    [SerializeField] private string description;

    private bool isOn = true;

    // Evitar usar PlayerPrefs, usemos el SimpleSaveLoad para guardar y cargar
    // Toggle necesita llamarse en alguna parte para que se cargue correctamente el estado, no solo el sprite

    private void Start()
    {
        if (!slider)
        {
            if (PlayerPrefs.GetInt(option) == 0)
            {
                toggleImage.sprite = toggleStatusSprites[0];
            }
            else if (PlayerPrefs.GetInt(option) == 1)
            {
                toggleImage.sprite = toggleStatusSprites[1];
            }
        }
        else if (slider)
        {
            slider.value = PlayerPrefs.GetFloat(option);
        }
    }
    public void Toggle()
    {
        isOn = !isOn;
        if (isOn)
        {
            toggleImage.sprite = toggleStatusSprites[0];
            PlayerPrefs.SetInt(option, 0);
        }
        else
        {
            toggleImage.sprite = toggleStatusSprites[1];
            PlayerPrefs.SetInt(option, 1);
        }
    }

    public void IncreaseDecreaseSliderValue(int value)
    {
        if (value > 0 && slider.value + value > slider.maxValue || value < 0 && slider.value + value < slider.minValue) return;
        slider.value += value;
    }

    public string GetDescription()
    {
        return description;
    }
}
