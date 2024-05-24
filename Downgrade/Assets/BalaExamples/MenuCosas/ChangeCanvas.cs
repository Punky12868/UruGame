using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeCanvas : MonoBehaviour
{
    [SerializeField] private Transform target; //CVCAM
    [SerializeField] public Canvas currentCanvas;

    private MenuControllerJ controller;
    [SerializeField] private float duration = 1;
    [SerializeField] private Ease easeType = Ease.InOutExpo;

    private Button SelectedButton;


    private void Start()
    {
        controller = gameObject.GetComponent<MenuControllerJ>();
    }
    public void GetNewCanvas(Canvas newCanvas)
    {
        Button[] oldCanvasButtons = currentCanvas.GetComponentsInChildren<Button>();
        Button[] newCanvasButtons = newCanvas.GetComponentsInChildren<Button>();

        foreach (Button button in oldCanvasButtons)
        {
            button.interactable = false;
        }
        if (currentCanvas.CompareTag("OpcionesButtons"))
        {
            currentCanvas.gameObject.SetActive(false);
            newCanvas.gameObject.SetActive(true);
        }
        foreach (Button button in newCanvasButtons)
        {
            button.interactable = true;
        }
        controller.lastCanvas = currentCanvas;
        currentCanvas = newCanvas;
        currentCanvas.gameObject.SetActive(true);
    }

    public void SelectButton(Button button)
    {
        controller.lastButton = SelectedButton;
        SelectedButton = button;
       
        if (currentCanvas.gameObject.CompareTag("OpcionesButtons"))
        {
            Invoke("SelectButtonInvoke", duration * 0.5f);
        }
        else
        {
            Invoke("SelectButtonInvoke", duration);
        }
    }

    public void GetNewPos(Transform pos)
    {
        target.DOMove(pos.position, duration).SetEase(easeType);
        target.DORotate(pos.rotation.eulerAngles, duration).SetEase(easeType);
    }

    public void SetAll(Canvas canvas, Button button, Transform pos)
    {
        GetNewCanvas(canvas);
        SelectButton(button);
        GetNewPos(pos);
    }

    private void SelectButtonInvoke()
    {
        SelectedButton.Select();
    }
}
