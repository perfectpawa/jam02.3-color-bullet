using UnityEngine;

public class EnemyState_Reposition : EnemyState_Base
{
    public EnemyState_Reposition(EnemyController enemy, Animator animator) : base(enemy, animator)
    {
    }

    public override void OnEnter()
    {
        enemy.StartReposition();
    }

    public override void Update()
    {
        enemy.Reposition();
    }
}
