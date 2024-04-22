using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] float throwForce;

    public void AddItem(GameObject addedItem)
    {
        if (item != null)
        {
            ReplaceItem(addedItem);
            return;
        }

        item = addedItem;
    }

    public void DropItem()
    {
        if (item == null)
            return;

        GameObject droppedItem = Instantiate(item, transform.position, Quaternion.identity);
        droppedItem.GetComponent<Rigidbody>().AddForce(Vector3.up + GetComponent<PlayerController>().GetLastDirection().normalized * throwForce, ForceMode.Impulse);
        item = null;
    }

    public void ReplaceItem(GameObject addedItem)
    {
        GameObject droppedItem = Instantiate(item, transform.position, Quaternion.identity);
        droppedItem.GetComponent<Rigidbody>().AddForce(Vector3.up + GetComponent<PlayerController>().GetLastDirection().normalized * throwForce, ForceMode.Impulse);
        item = null;

        item = addedItem;
    }

    public void UseItem()
    {
        if (item == null)
            return;

        switch (item.GetComponent<Item>().itemType)
        {
            case ItemType.Health:
                // use health item
                GetComponent<PlayerController>().GetHealth(item.GetComponent<Item>().GetValue());
                item = null;
                break;
            case ItemType.Stamina:
                // use stamina item
                GetComponent<PlayerController>().GetStamina(item.GetComponent<Item>().GetValue());
                item = null;
                break;
        }
    }
}
