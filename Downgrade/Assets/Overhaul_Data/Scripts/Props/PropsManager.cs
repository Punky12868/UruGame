using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Skull,
    Flower,
    MBox,
    Mushroom,
    Chest,
    Statue,
}

public class PropsManager : MonoBehaviour
{
    [SerializeField] GameObject[] itemPrefabs;

    public GameObject GetItem(ItemType type)
    {
        if (type == ItemType.None)
            return null;

        foreach (var item in itemPrefabs)
        {
            if (item.GetComponent<Item>().itemType == type)
            {
                return item;
            }
        }

        return null;
    }
}
