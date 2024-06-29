using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuListener : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    private void Awake() { mainMenuButton.onClick.AddListener(() => { GetComponent<CanvasGroup>().alpha = 1; }); }
}
