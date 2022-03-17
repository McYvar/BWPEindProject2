using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    protected FiniteStateMachine stateManager;

    public void Initialize(FiniteStateMachine stateManager)
    {
        this.stateManager = stateManager;
    }


    public abstract void OnAwake();
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
}
