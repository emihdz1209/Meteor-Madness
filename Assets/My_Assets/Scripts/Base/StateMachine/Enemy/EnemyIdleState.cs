using UnityEngine;

public class EnemyIdleState : State<Player>
{
    public EnemyIdleState(Player owner) : base(owner) { }

    public override void OnEnterState()
    {
        owner.ChangeColor(Color.gray);
        Debug.Log("Estado: IDLE");
    }

    public override void OnStayState()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            owner.ChangeState(new EnemyPatrolState(owner));
        }
    }
}
