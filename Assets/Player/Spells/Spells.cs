using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spells : MonoBehaviour
{
    public static SpellsItemObject currentSpell;
    public static int castCooldown;
    [SerializeField] SpellsItemObject firstSpell;
    [SerializeField] TMP_Text spellInformation;

    private void Start()
    {
        currentSpell = firstSpell;
        castCooldown = 0;
    }

    private void Update()
    {
        SpellInformation();

        if (PlayerInput.eastPressed && castCooldown <= 0)
        {
            if (currentSpell == null)
            {
                StartCoroutine(FindObjectOfType<MenuSystem>().DisplayThenRemoveChat("You have no spell yet!"));
                return;
            }
            castCooldown = currentSpell.spellCooldown;
            FireSpell();
        }

    }

    void FireSpell()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, currentSpell.spellRange);
        if (hit.collider == null) return;
        hit.collider.GetComponent<IDamagable>()?.takeDamage(Player.currentDamage / 2);
    }


    void SpellInformation()
    {
        spellInformation.text = "Current Spell: ";
        if (currentSpell == null)
        {
            spellInformation.text += "None\nRange: 0\nNot available!";
            return;
        }
        else 
        {
            spellInformation.text += currentSpell.itemName + "\nRange: " + currentSpell.spellRange + "\n";
        }
        if (castCooldown <= 0)
        {
            spellInformation.text += "Available!";
        }
        else
        {
            spellInformation.text += "On cooldown: " + castCooldown;
        }
    }

    
    public static void switchSpell(SpellsItemObject spell)
    {
        currentSpell = spell;
    }
}
