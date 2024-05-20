using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] Sprite nullIcon;
    [SerializeField] float throwForce;

    public void AddItem(GameObject addedItem) {item = addedItem;}

    public void DropItem()
    {
        if (item == null) return;

        Instantiate(item, transform.position, Quaternion.identity);
        GetComponent<PlayerInteraction>().SetPickUp();
        FindObjectOfType<PlayerUI>().SetItemIcon(nullIcon);
        item = null;
    }

    public void UseItem()
    {
        if (item == null) return;

        switch (item.GetComponent<Item>().itemType)
        {
            case ItemType.Health:
                // use health item
                UseHealthItem();
                break;
            case ItemType.Stamina:
                // use stamina item
                UseStaminaItem();
                break;
        }
    }

    private void UseHealthItem()
    {
        GetComponent<PlayerControllerOverhaul>().GainHealthProxy(item.GetComponent<Item>().GetValue());
        FindObjectOfType<PlayerUI>().SetItemIcon(nullIcon);
        item = null;
    }

    private void UseStaminaItem()
    {
        GetComponent<PlayerControllerOverhaul>().GainStaminaProxy(item.GetComponent<Item>().GetValue());
        FindObjectOfType<PlayerUI>().SetItemIcon(nullIcon);
        item = null;
    }

    public void DeleteItem()
    {
        FindObjectOfType<PlayerUI>().SetItemIcon(nullIcon);
        item = null;
    }

    public bool HasItem() {return item != null;}
}
