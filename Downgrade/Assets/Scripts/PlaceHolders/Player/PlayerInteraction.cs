using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Item item;
    bool canInteract;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactable.CanInteract();
            item = other.GetComponent<Item>();
            canInteract = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactable.StopInteracting();
            item = null;
            canInteract = false;
        }
    }

    public void Interact()
    {
        if (canInteract)
        {
            item.Interact();
            item = null;
            canInteract = false;
        }
    }

    public bool GetInteractionStatus()
    {
        return canInteract;
    }
}
