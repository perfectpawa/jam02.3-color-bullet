using UnityEngine;

public class PlayerState_FireSniper : PlayerState_Base
{
    public PlayerState_FireSniper(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        player.HandleOnChargeSniper();
    }

    public override void OnExit()
    {
        player.HandleOnFireSniper();
    }
}