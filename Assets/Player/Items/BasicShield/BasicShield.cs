using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Basic Shield")]
public class BasicShield : ItemObject
{
    public int damageReduction;

    public override void DoPassive()
    {
        base.DoPassive();
        Player.currentShield += damageReduction;
    }
}
