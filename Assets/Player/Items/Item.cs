using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [SerializeField] ItemObject item;
    [SerializeField] SpellsItemObject spell;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void AddSpell(SpellsItemObject item)
    {
        if (Spells.currentSpell != null)
        {
            if (item.itemLevel == Spells.currentSpell.itemLevel)
            {
                StartCoroutine(FindObjectOfType<MenuSystem>().DisplayThenRemoveChat("You already own this spell!"));
                return;
            }
            if (item.itemLevel < Spells.currentSpell.itemLevel)
            {
                StartCoroutine(FindObjectOfType<MenuSystem>().DisplayThenRemoveChat("You already own a stronger spell!"));
                return;
            }
        }
        Spells.switchSpell(item);
        Destroy(gameObject);
        return;
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
        if (other.CompareTag("Player"))
        {
            if (spell != null)
            {
                AddSpell(spell);
                return;
            }
            AddItem(item);
        }
    }
}
