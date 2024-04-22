using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemType itemType;

    [SerializeField] float value;

    public void CanInteract()
    {
        //Debug.Log("Can Interact");
        // show feedback
    }

    public void Interact()
    {
        //Debug.Log("Interacting with " + itemType);
        // pickup item
        // add item to inventory
        // remove item from scene
        // hide feedback
        // play sound

        switch (itemType)
        {
            case ItemType.Health:
                FindObjectOfType<Inventory>().AddItem(FindObjectOfType<PorpsManager>().GetItem(itemType));
                // shows the item in the UI
                Destroy(gameObject);
                break;
            case ItemType.Stamina:
                FindObjectOfType<Inventory>().AddItem(FindObjectOfType<PorpsManager>().GetItem(itemType));
                // shows the item in the UI
                Destroy(gameObject);
                break;
        }
    }

    public void StopInteracting()
    {
        //Debug.Log("Stop Interacting");
        // hide feedback
    }

    public float GetValue()
    {
        return value;
    }
}