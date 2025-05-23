using UnityEngine;

public class EnemyState_Attack : EnemyState_Base
{
    public EnemyState_Attack(EnemyController enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        enemy.StartCharge();
    }

    public override void Update()
    {
        enemy.FaceTarget();
        enemy.Attack();
    }
    
    public override void OnExit()
    {
        enemy.EndAttack();
    }
}
