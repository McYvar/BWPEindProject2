using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    // Finite state machine based on in lesson given instructions
    private Dictionary<System.Type, BaseState> stateDictionary = new Dictionary<System.Type, BaseState>();

    private BaseState currentState;
    public BaseState lastState;

    public FiniteStateMachine(System.Type startState, params BaseState[] states)
    {
        foreach(BaseState state in states)
        {
            state.Initialize(this);
            stateDictionary.Add(state.GetType(), state);
            state.OnAwake();
        }
        SwitchState(startState);
    }

    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }


    public void SwitchState(System.Type newStateStype)
    {
        currentState?.OnExit();
        lastState = currentState;
        currentState = stateDictionary[newStateStype];
        currentState?.OnEnter();
    }

}