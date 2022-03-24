using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public static ItemObject[] inventoryItems;

    private void Update()
    {
        foreach (ItemObject item in inventoryItems)
        {
            item.DoPassive();
        }
    }
}
