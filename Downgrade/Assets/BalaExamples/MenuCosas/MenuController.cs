using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Rewired;

public class MenuControllerJ : MonoBehaviour
{
    Player player;
    ChangeCanvas changeCanvas;
    public Canvas mainCanvas;
    public Button mainButton;
    public Transform mainPos;
    public Transform inicialPos;

    private float afkTimer;
    [SerializeField]private float afkTimerLimit;

    [HideInInspector]public Canvas lastCanvas;
    [HideInInspector]public Button lastButton;
    private Canvas canvasPressAnyKey;
    private bool inicialActive;

    // Lista de canvas para historial => si se mete a un canvas ".Add" y si se sale "changeCanvas.GetNewCanvas(listaCanvas[anterior])" y ".RemoveAt(Count)"

    void Start()
    {
        player = ReInput.players.GetPlayer(0);
        ReInput.players.GetPlayer(0).controllers.maps.LoadMap(0, 0, "UI", "Default", true);
        //player.controllers.maps.SetMapsEnabled(true, "UI");

        changeCanvas = gameObject.GetComponent<ChangeCanvas>();
        canvasPressAnyKey = GameObject.FindGameObjectWithTag("AnyButtonCanvas").GetComponent<Canvas>();
    }

    void Update()
    {
        if (player.GetButtonDown("Cancel") && lastCanvas.gameObject.CompareTag("OpcionesCanvas"))
        {
            changeCanvas.GetNewCanvas(lastCanvas);
            changeCanvas.SelectButton(lastButton);
        }

        if (canvasPressAnyKey.gameObject.activeInHierarchy)
        {
            
            if (player.GetButtonDown("Submit"))
            {
                changeCanvas.SetAll(mainCanvas, mainButton, mainPos);
                canvasPressAnyKey.gameObject.SetActive(false);
            }
        }

        if (player.GetAnyButtonDown())
        {
            afkTimer = 0;
            inicialActive = false;
        }
        else if (afkTimer >= afkTimerLimit)
        {
            if (!inicialActive)
            {
                GoToInitialCanvas();
            }
        }
        else
        {
            afkTimer += Time.deltaTime;
        }



    }
    void GoToInitialCanvas()
    {
        canvasPressAnyKey.gameObject.SetActive(true);
        changeCanvas.GetNewCanvas(canvasPressAnyKey);
        changeCanvas.GetNewPos(inicialPos);
        inicialActive = true;
    }

}
