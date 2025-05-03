using System.Collections.Generic;

public class StateMachine<T>
{
    public State<T> CurrentState { get; private set; }

    public void ChangeState(State<T> newState)
    {
        if (CurrentState != null)
            CurrentState.OnExitState();

        CurrentState = newState;

        if (CurrentState != null)
            CurrentState.OnEnterState();
    }

    public void Update()
    {
        if (CurrentState != null)
            CurrentState.OnStayState();
    }
}
