using UnityEngine;

public class PlayerState_FireDefault : PlayerState_Base
{
    public PlayerState_FireDefault(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        player.HandleOnFireDefault();
    }

    public override void OnExit()
    {
    }
}