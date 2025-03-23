using UnityEngine;

public class EnemyState_Attack : EnemyState_Base
{
    public EnemyState_Attack(EnemyController enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        enemy.HandleCharge();
    }

    public override void Update()
    {
        enemy.HandleLookAtTarget();
    }

    public override void OnExit()
    {
        enemy.HandleAttack();
    }
}
