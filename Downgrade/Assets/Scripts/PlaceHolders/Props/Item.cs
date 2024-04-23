using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPickup
{
    public ItemType itemType;

    [SerializeField] private float value;
    private float pickUpCooldown = 0.5f;

    private bool canBePickedUp = false;

    private void Awake()
    {
        Invoke("PickUpCooldown", pickUpCooldown);
    }

    public float GetValue()
    {
        return value;
    }

    public void CanPickup()
    {
        if (!canBePickedUp || GameManager.Instance.IsGamePaused())
            return;

        switch (itemType)
        {
            case ItemType.Health:
                FindObjectOfType<PlayerInventory>().AddItem(FindObjectOfType<PropsManager>().GetItem(itemType));
                // shows the item in the UI
                Destroy(gameObject);
                break;
            case ItemType.Stamina:
                FindObjectOfType<PlayerInventory>().AddItem(FindObjectOfType<PropsManager>().GetItem(itemType));
                // shows the item in the UI
                Destroy(gameObject);
                break;
        }
    }

    private void PickUpCooldown()
    {
        canBePickedUp = true;
    }
}