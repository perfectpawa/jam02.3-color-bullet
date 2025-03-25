using System;
using KBCore.Refs;
using StateMachineBehaviour;
using UnityEngine;

public class EnemyController : ValidatedMonoBehaviour
{
    [Header("Reference")]
    [SerializeField,Anywhere] private ColorPoolSpawner _colorPoolSpawner;
    [SerializeField, Self] private Rigidbody2D _rb;
    [SerializeField] private EnemyBulletPool _bulletPool;
    [SerializeField, Self] private EnemyDamageReceiver _damageReceiver;
    [SerializeField, Child] private Animator _animator;
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 1.5f;
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _chargeDuration = 0.5f;
    [Header("Knock Back Settings")]
    [SerializeField] private float _knockBackDuration = 0.5f;
    [SerializeField] private float _knockBackSpeed = 1f;
    [Header("Info")]    
    [SerializeField] private float _maxHP = 3;

    [SerializeField] private PlayerColor _deathColor;
    
    public Action DeathAction;
    
    private Transform _target;

    private StateMachine _stateMachine;
    
    private Vector2 _moveDirection;
    
    private CountdownTimer _chargeTimer;
    
    private CountdownTimer _knockBackTimer;
    private Vector2 _knockBackDirection;

    private bool _getKnockBack = false;
    
    #region unity callbacks
    private void Awake()
    {
        SetupStateMachine();
        SetupTarget();
        _chargeTimer = new CountdownTimer(_chargeDuration);
        
        _knockBackTimer = new CountdownTimer(_knockBackDuration);
        _knockBackTimer.OnTimerStop += () => _getKnockBack = false;
    }

    private void Start()
    {
        _damageReceiver.SetMaxHP(_maxHP);
        _damageReceiver.DeathAction += HandleDeath;
    }

    private void Update()
    {
        _stateMachine.Update();
        UpdateMoveDirection();
        
        _chargeTimer.Tick(Time.deltaTime);
        _knockBackTimer.Tick(Time.deltaTime);
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
        
        var ChaseState = new EnemyState_Chase(this, _animator);
        var AttackState = new EnemyState_Attack(this, _animator);
        var KnockBackState = new EnemyState_KnockBack(this, _animator);
        
        At(ChaseState, AttackState, new FuncPredicate(PredicateAttack));
        At(AttackState, ChaseState, new FuncPredicate(() => _chargeTimer.IsFinished && !PredicateAttack()));
        
        Any(KnockBackState, new FuncPredicate(() => _getKnockBack));
        At(KnockBackState, ChaseState, new FuncPredicate(() => _knockBackTimer.IsFinished));
        
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
        _chargeTimer.Start();
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
        _rb.linearVelocity = Vector2.zero;

        if (_chargeTimer.Process() < .9f) return;
        
        var position = transform.position;
        var direction = (_target.position - position).normalized;
        
        _bulletPool.Fire(position, direction);
    }
    
    public void HandleEndAttack()
    {
        _chargeTimer.Pause();
    }

    public void TakeKnockBack(Vector2 direction)
    {
        _knockBackDirection = direction;
        _getKnockBack = true;
    }
    public void HandleKnockBack()
    {
        _rb.MovePosition(_rb.position + _knockBackDirection.normalized * (_knockBackSpeed * Time.fixedDeltaTime));
    }
    
    public void HandleOnStartKnockBack()
    {
        _knockBackTimer.Start();
    }

    public void HandleDeath()
    {
        _colorPoolSpawner.SpawnColorPool(transform.position, _deathColor);
        DeathAction?.Invoke();
        
    }
}
