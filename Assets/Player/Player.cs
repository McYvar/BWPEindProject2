using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    private FiniteStateMachine finiteStateMachine;

    public void Awake()
    {
        finiteStateMachine = new FiniteStateMachine(typeof(PlayerTurnState), GetComponents<BaseState>());
    }


    private void Update()
    {
        finiteStateMachine.OnUpdate();
    }
}
