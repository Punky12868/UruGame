using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;

    private void OnEnable()
    {
        FindObjectOfType<MenuController>().DeactivateButtonsWithGameObject(menuPanel);
        FindObjectOfType<MenuController>().OnLoadHistory += NoOption;

        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            if (button.name == "Si")
            {
                button.onClick.AddListener(() => FindObjectOfType<GameManager>().NewGameErasedProgress());
            }
            else if (button.name == "No")
            {
                button.onClick.AddListener(() => NoOption());
                
            }
        }
    }

    private void NoOption()
    {
        if (!gameObject.activeInHierarchy) return;

        gameObject.SetActive(false);
        FindObjectOfType<MenuController>().ActivateButtonsWithGameObject(menuPanel);
        FindObjectOfType<MenuController>().LoadHistory();
    }
}
