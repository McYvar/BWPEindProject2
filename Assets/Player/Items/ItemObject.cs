using UnityEngine;

public abstract class ItemObject : ScriptableObject
{
    public string itemName;
    public int itemLevel;
    public Sprite spriteImage;
    public bool hasActive;
    public bool isSpell;

    public virtual void DoPassive()
    {
        // do passive
    }


    public virtual void DoActive()
    {
        // do active
    }
}
