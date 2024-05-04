using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Rewired;

[AddComponentMenu("")]
public class SimpleRemap : MonoBehaviour
{

    private const string category = "Default";
    private const string layout = "Default";
    private const string uiCategory = "UI";
    private int controllerId = 0;

    private InputMapper inputMapper = new InputMapper();

    public GameObject buttonPrefab;
    public GameObject textPrefab;
    public RectTransform fieldGroupTransform;
    public RectTransform actionGroupTransform;
    public GameObject statusUI;

    private ControllerType selectedControllerType = ControllerType.Keyboard;
    private int selectedControllerId = 0;
    private List<Row> rows = new List<Row>();
    [SerializeField] private List<string> protectedActions = new List<string> { "Vertical", "Horizontal" };
    [SerializeField] private string CancelAction = "Pause";

    private Player player { get { return ReInput.players.GetPlayer(0); } }
    private ControllerMap controllerMap
    {
        get
        {
            if (controller == null) return null;
            return player.controllers.maps.GetMap(controller.type, controller.id, category, layout);
        }
    }
    private Controller controller { get { return player.controllers.GetController(selectedControllerType, selectedControllerId); } }

    private void OnEnable()
    {
        if (!ReInput.isReady) return; // don't run if Rewired hasn't been initialized

        if (!CurrentController.isOnUI)
        {
            if (CurrentController.GetLastActiveController() != null && CurrentController.GetLastActiveController().type != ControllerType.Keyboard)
            {
                controllerId = 2;
            }
            else
            {
                controllerId = 0;
            }

            CurrentController.SetIsOnUI(true);
            CurrentController.UIandDefaultMaps("UI");
        }

        FindObjectOfType<CurrentController>().changed += Change;

        // Timeout after 5 seconds of listening
        inputMapper.options.timeout = 5f;

        // Ignore Mouse X and Y axes
        inputMapper.options.ignoreMouseXAxis = true;
        inputMapper.options.ignoreMouseYAxis = true;

        // Subscribe to events
        ReInput.ControllerConnectedEvent += OnControllerChanged;
        ReInput.ControllerDisconnectedEvent += OnControllerChanged;
        inputMapper.InputMappedEvent += OnInputMapped;
        inputMapper.ConflictFoundEvent += OnConflictFound;
        inputMapper.StoppedEvent += OnStopped;

        OnControllerSelected(controllerId);

        // Create UI elements
        InitializeUI();
    }

    private void Change()
    {
        if (CurrentController.GetLastActiveController() != null && CurrentController.GetLastActiveController().type != ControllerType.Keyboard)
        {
            controllerId = 2;
        }
        else
        {
            controllerId = 0;
        }

        OnControllerSelected(controllerId);

        InitializeUI();
    }

    private void OnDisable()
    {
        FindObjectOfType<CurrentController>().changed -= Change;

        // Make sure the input mapper is stopped first
        inputMapper.Stop();

        // Unsubscribe from events
        inputMapper.RemoveAllEventListeners();
        ReInput.ControllerConnectedEvent -= OnControllerChanged;
        ReInput.ControllerDisconnectedEvent -= OnControllerChanged;
        CurrentController.SetIsOnUI(false);

        foreach (Transform t in actionGroupTransform)
        {
            Object.Destroy(t.gameObject);
        }
        foreach (Transform t in fieldGroupTransform)
        {
            Object.Destroy(t.gameObject);
        }
    }

    private void RedrawUI()
    {
        if (controller == null)
        { // no controller is selected
            ClearUI();
            return;
        }

        // Update each button label with the currently mapped element identifier
        for (int i = 0; i < rows.Count; i++)
        {
            Row row = rows[i];
            InputAction action = rows[i].action;

            string name = string.Empty;
            int actionElementMapId = -1;

            // Find the first ActionElementMap that maps to this Action and is compatible with this field type
            foreach (var actionElementMap in controllerMap.ElementMapsWithAction(action.id))
            {
                if (actionElementMap.ShowInField(row.actionRange))
                {
                    name = actionElementMap.elementIdentifierName;
                    actionElementMapId = actionElementMap.id;
                    break;
                }
            }

            // Set the label in the field button
            row.text.text = name;

            // Set the field button callback
            row.button.onClick.RemoveAllListeners(); // clear the button event listeners first
            int index = i; // copy variable for closure
            row.button.onClick.AddListener(() => OnInputFieldClicked(index, actionElementMapId));
        }
    }

    private void ClearUI()
    {
        // Clear button labels
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].text.text = string.Empty;
        }
    }

    private void InitializeUI()
    {

        // Delete placeholders
        foreach (Transform t in actionGroupTransform)
        {
            Object.Destroy(t.gameObject);
        }
        foreach (Transform t in fieldGroupTransform)
        {
            Object.Destroy(t.gameObject);
        }

        // Create Action fields and input field buttons
        foreach (var action in ReInput.mapping.ActionsInCategory(category))
        {
            if (action.type == InputActionType.Axis)
            {
                // Create a full range, one positive, and one negative field for Axis-type Actions
                //CreateUIRow(action, AxisRange.Full, action.descriptiveName);

                //CreateUIRow(action, AxisRange.Positive, !string.IsNullOrEmpty(action.positiveDescriptiveName) ? action.positiveDescriptiveName : action.descriptiveName + " +");
                //CreateUIRow(action, AxisRange.Negative, !string.IsNullOrEmpty(action.negativeDescriptiveName) ? action.negativeDescriptiveName : action.descriptiveName + " -");
            }
            else if (action.type == InputActionType.Button)
            {
                // Just create one positive field for Button-type Actions
                CreateUIRow(action, AxisRange.Positive, action.descriptiveName);
            }
        }

        RedrawUI();
    }

    private void CreateUIRow(InputAction action, AxisRange actionRange, string label, string axisLabel = null)
    {
        // Create the Action label
        GameObject labelGo = Object.Instantiate<GameObject>(textPrefab);
        labelGo.transform.SetParent(actionGroupTransform);
        labelGo.transform.SetAsLastSibling();
        labelGo.GetComponent<TMPro.TMP_Text>().text = label;

        if (labelGo.GetComponent<TMPro.TMP_Text>().text == "")
        {
            labelGo.GetComponent<TMPro.TMP_Text>().text = axisLabel;
        }

        // Create the input field button
        GameObject buttonGo = Object.Instantiate<GameObject>(buttonPrefab);
        buttonGo.transform.SetParent(fieldGroupTransform);
        buttonGo.transform.SetAsLastSibling();

        // Add the row to the rows list
        rows.Add(
            new Row()
            {
                action = action,
                actionRange = actionRange,
                button = buttonGo.GetComponent<Button>(),
                text = buttonGo.GetComponentInChildren<TMPro.TMP_Text>()
            }
        );
    }

    private void SetSelectedController(ControllerType controllerType)
    {
        bool changed = false;

        // Check if the controller type changed
        if (controllerType != selectedControllerType)
        { // controller type changed
            selectedControllerType = controllerType;
            changed = true;
        }

        // Check if the controller id changed
        int origId = selectedControllerId;
        if (selectedControllerType == ControllerType.Joystick)
        {
            if (player.controllers.joystickCount > 0) selectedControllerId = player.controllers.Joysticks[0].id;
            else selectedControllerId = -1;
        }
        else
        {
            selectedControllerId = 0;
        }
        if (selectedControllerId != origId) changed = true;

        // If the controller changed, stop the input mapper and update the UI
        if (changed)
        {
            inputMapper.Stop();
            RedrawUI();
        }
    }

    // Event Handlers

    // Called by the controller UI Buttons when pressed
    public void OnControllerSelected(int controllerType)
    {
        SetSelectedController((ControllerType)controllerType);
    }

    // Called by the input field UI Button when pressed
    private void OnInputFieldClicked(int index, int actionElementMapToReplaceId)
    {
        if (index < 0 || index >= rows.Count) return; // index out of range
        if (controller == null) return; // there is no Controller selected

        

        // Begin listening for input, but use a coroutine so it starts only after a short delay to prevent
        // the button bound to UI Submit from binding instantly when the input field is activated.
        StartCoroutine(StartListeningDelayed(index, actionElementMapToReplaceId));
    }

    private IEnumerator StartListeningDelayed(int index, int actionElementMapToReplaceId)
    {

        // Don't allow a binding for a short period of time after input field is activated
        // to prevent button bound to UI Submit from binding instantly when input field is activated.
        yield return new WaitForSeconds(0.1f);

        InputMapper.Context context = new InputMapper.Context()
        {
            actionId = rows[index].action.id,
            controllerMap = controllerMap,
            actionRange = rows[index].actionRange,
            actionElementMapToReplace = controllerMap.GetElementMap(actionElementMapToReplaceId)
        };

        inputMapper.Start(context);

        // Disable the UI Controller Maps while listening to prevent UI control and submissions.
        player.controllers.maps.SetMapsEnabled(false, uiCategory);

        // Update the UI text
        statusUI.SetActive(true);
    }

    private InputMapper.ConflictFoundEventData conflictData;

    void OnConflictFound(InputMapper.ConflictFoundEventData data)
    {
        conflictData = data; // store the event data for use in user response
        //Debug.Log(conflictData.conflicts[0].action.name);

        for (int i = 0; i < protectedActions.Count; i++)
        {
            if (conflictData.conflicts[0].action.name == protectedActions[i] || conflictData.conflicts[0].action.name == CancelAction)
            {
                conflictData.responseCallback(InputMapper.ConflictResponse.Cancel); // cancel polling
                Debug.Log("Can't remap the " + conflictData.assignment.action.name + " action due to " + conflictData.conflicts[0].action.name + " being a protected action");
                return;
            }
        }

        conflictData.responseCallback(InputMapper.ConflictResponse.Swap);
        Debug.Log("Swapped");
    }

    private void OnControllerChanged(ControllerStatusChangedEventArgs args)
    {
        SetSelectedController(selectedControllerType);
    }

    private void OnInputMapped(InputMapper.InputMappedEventData data)
    {
        RedrawUI();


    }

    private void OnStopped(InputMapper.StoppedEventData data)
    {
        statusUI.SetActive(false);
        Invoke("EnableUICategory", 0.1f);
    }

    private void EnableUICategory()
    {
        // Re-enable UI Controller Maps after listening is finished.
        player.controllers.maps.SetMapsEnabled(true, uiCategory);
    }

    // A small class to store information about the input field buttons
    private class Row
    {
        public InputAction action;
        public AxisRange actionRange;
        public Button button;
        public TMPro.TMP_Text text;
    }
}