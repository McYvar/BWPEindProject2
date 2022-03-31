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

    LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

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
        hit.collider.GetComponent<IDamagable>()?.takeDamage(Player.currentDamage / 2 * currentSpell.itemLevel);
    }


    void SpellInformation()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        spellInformation.text = "Current Spell: ";
        if (currentSpell == null)
        {
            spellInformation.text += "None\nRange: 0\nNot available!";
            lineRenderer.SetPosition(1, transform.position);
            return;
        }
        else 
        {
            spellInformation.text += currentSpell.itemName + "\nRange: " + currentSpell.spellRange + "\n";
        }
        if (castCooldown <= 0)
        {
            spellInformation.text += "Available!";
            lineRenderer.SetPosition(1, transform.position + transform.forward * currentSpell.spellRange);
            lineRenderer.material.color = currentSpell.spellColor;
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
