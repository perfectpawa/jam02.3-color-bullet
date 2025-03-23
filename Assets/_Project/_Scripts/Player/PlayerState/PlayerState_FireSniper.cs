using UnityEngine;

public class PlayerState_FireSniper : PlayerState_Base
{
    public PlayerState_FireSniper(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void FixedUpdate() {
        player.HandleCharge();
    }
}