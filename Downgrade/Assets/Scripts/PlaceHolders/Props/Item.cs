using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPickup
{
    public ItemType itemType;

    [SerializeField] private Sprite itemIcon;
    [SerializeField] private float value;
    [SerializeField] private float spawnForce;
    [SerializeField] private float pickUpCooldown = 0.5f;

    private bool canBePickedUp = false;

    private void Awake()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * spawnForce, ForceMode.Impulse);
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
                FindAnyObjectByType<PlayerUI>().SetItemIcon(itemIcon);
                Destroy(gameObject);
                break;
            case ItemType.Stamina:
                FindObjectOfType<PlayerInventory>().AddItem(FindObjectOfType<PropsManager>().GetItem(itemType));
                FindAnyObjectByType<PlayerUI>().SetItemIcon(itemIcon);
                Destroy(gameObject);
                break;
        }
    }

    private void PickUpCooldown()
    {
        canBePickedUp = true;

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}