using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell")]
public class SpellsItemObject : ItemObject
{
    public int spellRange;
    public int spellCooldown;
    public Color spellColor;
}
