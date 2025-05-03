using UnityEngine;

public class EnemyChaseState : State<Player>
{
    public EnemyChaseState(Player owner) : base(owner) { }

    public override void OnEnterState()
    {
        owner.ChangeColor(Color.red);
        Debug.Log("Estado: CHASE");

        owner.ReverseGravity();
    }

    public override void OnStayState()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            owner.ChangeState(new EnemyIdleState(owner));
        }
    }

    public override void OnExitState()
    {
        owner.ReverseGravity();
    }
}
