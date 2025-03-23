using UnityEngine;
using StateMachineBehaviour;

public abstract class PlayerState_Base : IState {
    protected readonly PlayerController player;
    protected readonly Animator animator;
    
    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        
    protected const float crossFadeDuration = 0.1f;

    protected PlayerState_Base(PlayerController player, Animator animator) {
        this.player = player;
        this.animator = animator;
    }
        
    public virtual void OnEnter() {
        // noop
    }

    public virtual void Update() {
        // noop
    }

    public virtual void FixedUpdate() {
        // noop
    }

    public virtual void OnExit() {
        // noop
    }
}
