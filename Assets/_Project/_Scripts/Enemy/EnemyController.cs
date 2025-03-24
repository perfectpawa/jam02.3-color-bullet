using System;
using KBCore.Refs;
using StateMachineBehaviour;
using UnityEngine;

public class EnemyController : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Rigidbody2D _rb;
    [SerializeField] private EnemyBulletPool _bulletPool;
    
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _chargeDuration = 0.5f;
    
    private Transform _target;

    private StateMachine _stateMachine;
    
    private Vector2 _moveDirection;
    
    private CountdownTimer _countdownTimer;
    
    #region unity callbacks
    private void Awake()
    {
        SetupStateMachine();
        SetupTarget();
        _countdownTimer = new CountdownTimer(_chargeDuration);
    }

    private void Update()
    {
        _stateMachine.Update();
        UpdateMoveDirection();
        
        _countdownTimer.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }
    #endregion

    #region setup
    private void SetupTarget()
    {
        _target = GameObject.FindWithTag("Player").transform;
    }
    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();
        
        var ChaseState = new EnemyState_Chase(this, GetComponent<Animator>());
        var AttackState = new EnemyState_Attack(this, GetComponent<Animator>());
        
        At(ChaseState, AttackState, new FuncPredicate(PredicateAttack));
        At(AttackState, ChaseState, new FuncPredicate(() => _countdownTimer.IsFinished));
        
        _stateMachine.SetState(ChaseState);

        return;
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
    }

    private bool PredicateAttack()
    {
        return Vector2.Distance(transform.position, _target.position) <= _attackRange;
    }
    #endregion
    
    private void UpdateMoveDirection()
    {
        _moveDirection = (_target.position - transform.position).normalized;
    }
    
    public void HandleLocomotion()
    {
        _rb.MovePosition(_rb.position + _moveDirection.normalized * (_moveSpeed * Time.fixedDeltaTime));
    }

    
    public void HandleCharge()
    {
        _countdownTimer.Start();
    }
    
    public void HandleLookAtTarget()
    {
        if (_moveDirection.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (_moveDirection.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    
    public void HandleAttack()
    {
        if (_countdownTimer.Process() < .9f) return;
        
        var position = transform.position;
        var direction = (_target.position - position).normalized;
        
        _bulletPool.Fire(position, direction);
    }
}
