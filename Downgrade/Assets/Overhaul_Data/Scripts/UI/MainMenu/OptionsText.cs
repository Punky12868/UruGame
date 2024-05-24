using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class OptionsText : MonoBehaviour
{
    private TMP_Text _tmpText;
    private void Awake() { _tmpText = GetComponent<TMP_Text>(); }
    private void Update()
    {
        if (EventSystem.current == null) return;

        if (EventSystem.current.currentSelectedGameObject != null)
            if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out UIOptionsController uiOptionsController))
                if (_tmpText.text != uiOptionsController.GetDescription()) _tmpText.text = uiOptionsController.GetDescription();
    }
}
