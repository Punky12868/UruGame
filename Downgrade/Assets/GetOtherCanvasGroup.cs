using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOtherCanvasGroup : MonoBehaviour
{
    CanvasGroup thisCanvasGroup;
    [SerializeField] CanvasGroup target;
    private void Awake() {  thisCanvasGroup = GetComponent<CanvasGroup>(); }
    private void Update() { thisCanvasGroup.alpha = target.alpha; }
}
