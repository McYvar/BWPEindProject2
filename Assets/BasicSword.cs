using UnityEngine;

[CreateAssetMenu(menuName = "Items/Basic Sword")]
public class BasicSword : ItemObject
{
    public int damage;

    public override void DoPassive()
    {
        base.DoPassive();
    }

    public override void DoActive()
    {
        base.DoActive();
    }
}