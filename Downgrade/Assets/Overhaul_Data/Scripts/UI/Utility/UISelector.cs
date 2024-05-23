using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UISelector : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Ease easeType = Ease.InOutExpo;

    private Button currentButton;
    private List<Button> buttons = new List<Button>();

    private void Awake()
    {
        Button[] currentCanvasButtons = GetComponentInParent<Canvas>().gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in currentCanvasButtons) if (!button.transform.GetComponentInParent<Slider>()) buttons.Add(button);
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
        Vector3 newPos = new Vector3(transform.position.x, currentButton.transform.position.y, transform.position.z);
        transform.DOMove(newPos, duration).SetEase(easeType);
    }
}
