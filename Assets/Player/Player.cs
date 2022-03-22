using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour, IDamagable
{
    private FiniteStateMachine finiteStateMachine;
    public int dealsDamage;

    public int healt { get; set; }

    public void Awake()
    {
        finiteStateMachine = new FiniteStateMachine(typeof(PlayerTurnState), GetComponents<BaseState>());
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
    }
}
