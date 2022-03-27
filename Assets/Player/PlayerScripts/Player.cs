using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour, IDamagable
{
    public int startingHealth;

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
        healt = startingHealth;
        currentDamage = normalDamage;
    }


    private void Update()
    {
        InventoryBehaviour();

        if (MenuSystem.invIsOpen || MenuSystem.gameIsPaused) return;
        finiteStateMachine.OnUpdate();
    }

    public void setHealth(int amount)
    {
        healt = amount;
    }


    public void takeDamage(int amount)
    {
        healt -= amount;
        if (amount > 0)
        {
            StartCoroutine(FindObjectOfType<MenuSystem>().DisplayThenRemoveChat("You took: " + amount + " damage!"));
        }
        else if (amount == 0)
        {
            StartCoroutine(FindObjectOfType<MenuSystem>().DisplayThenRemoveChat("You took no damage!"));
        }
        else
        {
            StartCoroutine(FindObjectOfType<MenuSystem>().DisplayThenRemoveChat("You healed for: " + -amount + "!"));
        }
    }


    void InventoryBehaviour()
    {
        currentDamage = normalDamage;
        currentShield = normalShield;

        if (MenuSystem.inventoryItems == null) return;
        if (MenuSystem.inventoryItems.Length <= 0) return;
        foreach (ItemObject item in MenuSystem.inventoryItems)
        {
            if (item == null) continue;
            item.DoPassive();
        }
    }
}
