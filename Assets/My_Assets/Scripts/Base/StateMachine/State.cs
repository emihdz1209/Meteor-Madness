public abstract class State<T>
{
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    public virtual void OnEnterState() { }
    public virtual void OnStayState() { }
    public virtual void OnExitState() { }
}
