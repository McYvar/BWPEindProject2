using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Health potion")]
public class HealthPotion : ItemObject
{
    public int howMuchHeal;

    public override void DoActive()
    {
        Player player = FindObjectOfType<Player>();
        player.takeDamage(-howMuchHeal);
    }
}
