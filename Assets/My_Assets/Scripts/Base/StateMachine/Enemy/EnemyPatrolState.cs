using UnityEngine;

public class EnemyPatrolState : State<Player>
{
    public EnemyPatrolState(Player owner) : base(owner) { }

    public override void OnEnterState()
    {
        owner.ChangeColor(Color.blue);
        Debug.Log("Estado: PATROL");
    }

    public override void OnStayState()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            owner.ChangeState(new EnemyChaseState(owner));
        }
    }
}
