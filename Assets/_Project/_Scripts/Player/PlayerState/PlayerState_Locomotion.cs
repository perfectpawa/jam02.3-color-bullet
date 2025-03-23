using UnityEngine;

public class PlayerState_Locomotion : PlayerState_Base
{
    public PlayerState_Locomotion(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void Update()
    {
        player.HandleLookAtTarget();
    }

    public override void FixedUpdate() {
        player.HandleLocomotion();
    }
    
}
