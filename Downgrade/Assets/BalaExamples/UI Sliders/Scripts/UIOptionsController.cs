using UnityEngine.UI;
using UnityEngine;

public class UIOptionsController : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private Image toggleImage;
    [SerializeField] private string toggleKey;
    [SerializeField] private Sprite[] toggleStatusSprites;
    [TextArea] [SerializeField] private string description;

    private bool isOn = true;

    private void Awake()
    {
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
    }

    private void DelayedAwake()
    {
        if (toggleImage != null) LoadToggle();
    }

    public void Toggle()
    {
        isOn = !isOn;
        if (isOn) toggleImage.sprite = toggleStatusSprites[0];
        else toggleImage.sprite = toggleStatusSprites[1];

        OnToggle(isOn);

        if (GetComponent<ScreenMode>())
        {
            GetComponent<ScreenMode>().SetFullScreen(isOn);
        }

        if (FindObjectOfType<NarrationSystem>())
        {
            FindObjectOfType<NarrationSystem>().SetSubtitlesActivated(isOn);
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

    private void LoadToggle()
    {
        isOn = SimpleSaveLoad.Instance.LoadData<bool>(FileType.Config, toggleKey, true);
        if (isOn) toggleImage.sprite = toggleStatusSprites[0];
        else toggleImage.sprite = toggleStatusSprites[1];
    }

    public void OnToggle(bool value)
    {
        SimpleSaveLoad.Instance.SaveData(FileType.Config, toggleKey, value);
        Debug.Log("Toggle " + toggleKey + " is " + value);
    }
}
