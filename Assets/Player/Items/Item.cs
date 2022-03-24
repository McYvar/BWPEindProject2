using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [SerializeField] ItemObject item;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void AddItem(ItemObject item)
    {
        for (int i = 0; i < MenuSystem.inventoryItems.Length; i++)
        {
            if (MenuSystem.inventoryItems[i] == null)
            {
                MenuSystem.inventoryItems[i] = item;
                Destroy(gameObject);
                return;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) AddItem(item);
    }
}
