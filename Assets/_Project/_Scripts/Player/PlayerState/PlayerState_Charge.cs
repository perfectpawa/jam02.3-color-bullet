using UnityEngine;

public class PlayerState_Charge : PlayerState_Base
{
    public PlayerState_Charge(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void FixedUpdate() {
        player.HandleCharge();
    }
}