using UnityEngine.UI;
using UnityEngine;

public class UIOptionsController : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private Image toggleImage;
    [SerializeField] private Sprite[] toggleStatusSprites;
    [TextArea] [SerializeField] private string description;

    private bool isOn = true;

    public void Toggle()
    {
        isOn = !isOn;
        if (isOn) toggleImage.sprite = toggleStatusSprites[0];
        else toggleImage.sprite = toggleStatusSprites[1];
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
