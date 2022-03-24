using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour, IDamagable
{
    public int maxHealth;

    public int normalShield;
    public static int currentShield;

    public int normalDamage;
    public static int currentDamage;

    public int healt { get; set; }

    private FiniteStateMachine finiteStateMachine;

    public void Awake()
    {
        finiteStateMachine = new FiniteStateMachine(typeof(PlayerTurnState), GetComponents<BaseState>());
    }

    private void Start()
    {
        healt = maxHealth;
        currentDamage = normalDamage;
    }

    public void setHealth(int amount)
    {
        healt = amount;
    }


    public void takeDamage(int amount)
    {
        healt -= amount;
        Debug.Log("player took: " + amount + " damage!");
    }


    private void Update()
    {
        finiteStateMachine.OnUpdate();

        currentDamage = normalDamage;
        currentShield = normalShield;

        if (MenuSystem.inventoryItems == null) return;
        if (MenuSystem.inventoryItems.Length <= 0) return;
        foreach (ItemObject item in MenuSystem.inventoryItems)
        {
            if (item == null) continue;
            item.DoActive();
            item.DoPassive();
        }
        Debug.Log(currentDamage);
        Debug.Log(currentShield);
    }
}
