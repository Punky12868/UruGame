using UnityEngine;
public class RotatingObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private bool activeOnAwake = false;
    [SerializeField] private bool rightDirection = true;
    private bool active = false;
    private void Awake() { active = activeOnAwake; }

    private void Update() 
    {  
        rotationSpeed = rightDirection ? Mathf.Abs(rotationSpeed) : -Mathf.Abs(rotationSpeed);
        if (active) transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void SetActiveRotation(bool value) { active = value; }
    public void SetRightDirection(bool value) { rightDirection = value; }
    public bool GetActiveRotation() { return active; }
    public void ResetRotation() { active = false; transform.eulerAngles = Vector3.zero; }
}
