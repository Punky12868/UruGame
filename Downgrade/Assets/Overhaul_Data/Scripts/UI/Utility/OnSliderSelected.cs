using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnSliderSelected : MonoBehaviour
{
    [SerializeField] private Button[] currentSelectedButton;
    [SerializeField] private Color normalColor = new Color32(163, 173, 85, 255);
    [SerializeField] private Color selectedColor = new Color(1, 1, 1, 1);

    [SerializeField] private Image[] images;
    [SerializeField] private TMPro.TMP_Text[] texts;

    [SerializeField] private Button[] patchButtons;

    private void Update()
    {
        if (EventSystem.current == null) return;
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject == null) return;

        foreach (Button button in currentSelectedButton)
        {
            if (selectedGameObject == button.gameObject)
            {
                Show(true);
                return;
            }

            Show(false);
        }

        Patch();
    }

    private void Show(bool value)
    {
        if (value)
        {
            if (!NullOrCero.isArrayNullOrCero(images)) foreach (Image image in images) image.color = selectedColor;
            if (!NullOrCero.isArrayNullOrCero(texts)) foreach (TMPro.TMP_Text text in texts) text.color = selectedColor;
        }
        else
        {
            if (!NullOrCero.isArrayNullOrCero(images)) foreach (Image image in images) image.color = normalColor;
            if (!NullOrCero.isArrayNullOrCero(texts)) foreach (TMPro.TMP_Text text in texts) text.color = normalColor;
        }
    }

    private void Patch()
    {
        if (!GetComponent<Button>().interactable && FindObjectOfType<PauseMenu>().GetOnConfig() && !FindObjectOfType<PauseMenu>().GetOnSlider())
        {
            // if any of the patch buttons are not selected, set this button interactable
            if (EventSystem.current == null) return;
            GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (selectedGameObject == null) return;

            foreach (Button button in patchButtons) if (selectedGameObject == button.gameObject) return;

            GetComponent<Button>().interactable = true;
        }
    }
}
