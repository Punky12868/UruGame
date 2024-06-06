using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuMaps : MonoBehaviour
{
    [SerializeField] Button[] mapButtons;
    [SerializeField] GameObject[] maps;

    private Button currentSelectedButton;

    private void Update()
    {
        if (EventSystem.current == null) return;

        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;

        if (selectedGameObject != null && selectedGameObject.GetComponent<Button>() != null)
        {
            Button selectedButton = selectedGameObject.GetComponent<Button>();

            if (selectedButton != currentSelectedButton)
            {
                currentSelectedButton = selectedButton;
                UpdateMapSelection(selectedButton);
            }
        }
    }

    private void UpdateMapSelection(Button selectedButton)
    {
        for (int i = 0; i < mapButtons.Length; i++)
        {
            if (selectedButton == mapButtons[i])
            {
                maps[i].SetActive(true);
            }
            else
            {
                maps[i].SetActive(false);
            }
        }
    }
}
