using UnityEngine;

public class EnemyState_Chase : EnemyState_Base
{
    public EnemyState_Chase(EnemyController enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void FixedUpdate()
    {
        enemy.HandleLocomotion();
        enemy.HandleLookAtTarget();
    }
}
