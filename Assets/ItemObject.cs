using UnityEngine;

public class ItemObject : ScriptableObject
{
    public string itemName;
    public Sprite spriteImage;
    public bool hasPassive;
    public bool hasActive;

    public virtual void DoPassive()
    {
        if (!hasPassive) return;
        // do passive
    }

    public virtual void DoActive()
    {
        if (!hasActive) return;
        // do active
    }
}
