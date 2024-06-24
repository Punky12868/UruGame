using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnToggleSelected : MonoBehaviour
{
    [SerializeField] private Color normalColor = new Color32(163, 173, 85, 255);
    [SerializeField] private Color selectedColor = new Color(1, 1, 1, 1);

    [SerializeField] private Button currentSelectedButton;
    [SerializeField] private Image image;
    bool selected;

    private void Awake()
    {
        Show(false);
    }

    private void Update()
    {
        if (EventSystem.current == null) return;
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject == null) return;

        if (selectedGameObject == currentSelectedButton.gameObject && !selected)
        {
            Show(true);
            return;
        }
        else if (selectedGameObject != currentSelectedButton.gameObject && selected) Show(false);
    }

    private void Show(bool value)
    {
        if (value)
        {
            image.color = selectedColor;
            selected = true;
        }
        else
        {
            image.color = normalColor;
            selected = false;
        }
    }
}
