using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UISelector : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Ease easeType = Ease.InOutExpo;
    [SerializeField] private bool automaticSetPos;
    [SerializeField] private bool addButtonsManually;
    [SerializeField] private List<Button> manualButtons = new List<Button>();

    private Button currentButton;
    private List<Button> buttons = new List<Button>();

    private void Awake()
    {
        if (addButtonsManually) buttons = manualButtons;
        else
        {
            Button[] currentCanvasButtons = GetComponentInParent<Canvas>().gameObject.GetComponentsInChildren<Button>();
            foreach (Button button in currentCanvasButtons) if (!button.transform.GetComponentInParent<Slider>()) buttons.Add(button);
        }
        
    }

    private void Update()
    {
        if (EventSystem.current == null) return;

        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;

        if (selectedGameObject != null && selectedGameObject.GetComponent<Button>() != null)
        {
            if (selectedGameObject.GetComponent<Button>() != currentButton && buttons.Contains(selectedGameObject.GetComponent<Button>()))
            {
                currentButton = selectedGameObject.GetComponent<Button>();
                MoveSelector();
            }
        }
    }

    private void MoveSelector()
    {
        if (automaticSetPos)
        {
            transform.DOMove(currentButton.transform.position, duration).SetEase(easeType);
        }
        else
        {
            Vector3 newPos = new Vector3(transform.position.x, currentButton.transform.position.y, transform.position.z);
            transform.DOMove(newPos, duration).SetEase(easeType);
        }
        
    }

    public Button[] GetManualButtons()
    {
        return manualButtons.ToArray();
    }
}
