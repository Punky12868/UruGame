using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float dropCooldown = 0.5f;
    bool canPickup = true;

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.IsGamePaused()) return;
        if (other.TryGetComponent(out IPickup pickup)) {if (!GetComponent<PlayerInventory>().HasItem() && canPickup) pickup.CanPickup();}
    }

    public void SetPickUp()
    {
        canPickup = false;
        Invoke("ResetPickUp", dropCooldown);
    }

    private void ResetPickUp() {canPickup = true;}
}
