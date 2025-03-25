using UnityEngine;

public class EnemyState_KnockBack : EnemyState_Base
{
    public EnemyState_KnockBack(EnemyController enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        enemy.HandleOnStartKnockBack();
    }
    
    public override void FixedUpdate()
    {
        enemy.HandleKnockBack();
    }
}