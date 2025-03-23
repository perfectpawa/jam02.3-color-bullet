using UnityEngine;
using StateMachineBehaviour;

public abstract class EnemyState_Base : IState {
    protected readonly EnemyController enemy;
    protected readonly Animator animator;
    
    protected EnemyState_Base(EnemyController enemy, Animator animator) {
        this.enemy = enemy;
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