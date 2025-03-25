using UnityEngine;

public class PlayerState_KnockBack : PlayerState_Base
{
    public PlayerState_KnockBack(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        player.HandleOnKnockBack();
    }

    public override void FixedUpdate() {
        player.HandleKnockBack();
    }
}