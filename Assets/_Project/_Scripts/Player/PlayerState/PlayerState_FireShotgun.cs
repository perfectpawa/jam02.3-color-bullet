using UnityEngine;

public class PlayerState_FireShotgun : PlayerState_Base
{
    public PlayerState_FireShotgun(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        player.HandleOnFireShotgun();
        player.HandleOnKnockBack();
    }

    public override void FixedUpdate() {
        player.HandleKnockBack();
    }
    
}