using System;
using KBCore.Refs;
using StateMachineBehaviour;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : ValidatedMonoBehaviour
{
    #region Fields
    [Header("Reference")]
    [SerializeField, Self] private Rigidbody2D _rb;
    [SerializeField, Child] private Animator _animator;
    [SerializeField, Self] private EnemyDamageReceiver _damageReceiver;
    [SerializeField, Anywhere] private ColorPoolSpawner _colorPoolSpawner;
    [SerializeField, Anywhere] private EnemyBulletPool _bulletPool;
    
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 1.5f;
    
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _chargeDuration = 0.5f;
    [SerializeField] private float _aimOffsetAngle = 10f;

    [Header("Knock Back Settings")]
    [SerializeField] private float _knockBackDuration = 0.5f;
    [SerializeField] private float _knockBackSpeed = 1f;

    [Header("Reposition Settings")] 
    [SerializeField] private float _repositionRatio = 0.2f;
    [SerializeField] private float _repositionDuration = 0.5f;
    
    [Header("Info")]    
    [SerializeField] private float _maxHP = 3;
    [SerializeField] private PlayerColor _deathColor;
    
    public Action DeathAction = delegate { };
    
    private Transform _target;
    private StateMachine _stateMachine;
    
    private Vector2 _moveDirection;
    private Vector2 _repositionDirection;
    private Vector2 _knockBackDirection;
    
    private bool _isKnockBackActive;
    
    private CountdownTimer _chargeTimer;
    private CountdownTimer _knockBackTimer;
    private CountdownTimer _repositionTimer;
    
    private EnemyState_Chase _chaseState;
    #endregion
    
    #region Unity Callbacks
    private void Awake()
    {
        InitializeStateMachine();
        InitializeTarget();
        InitializeTimers();
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
        UpdateTimer();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = Vector2.zero;
        _stateMachine.FixedUpdate();
    }
    #endregion

    #region Initialization
    private void InitializeTarget()
    {
        _target = GameObject.FindWithTag("Player")?.transform;
    }

    private void InitializeTimers()
    {
        _chargeTimer = new CountdownTimer(_chargeDuration);
        
        _knockBackTimer = new CountdownTimer(_knockBackDuration);
        _knockBackTimer.OnTimerStop += () => _isKnockBackActive = false;
        
        _repositionTimer = new CountdownTimer(_repositionDuration);
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();

        _chaseState = new EnemyState_Chase(this, _animator);
        var attackState = new EnemyState_Attack(this, _animator);
        var knockBackState = new EnemyState_KnockBack(this, _animator);
        var repositionState = new EnemyState_Reposition(this, _animator);
        
        AddTransition(_chaseState, attackState, PredicateAttack);
        AddTransition(attackState, _chaseState, () => _chargeTimer.IsFinished && !PredicateAttack());
        
        AddAnyTransition(knockBackState, () => _isKnockBackActive);
        AddTransition(knockBackState, _chaseState, () => _knockBackTimer.IsFinished);
        
        AddTransition(attackState, repositionState, () => _chargeTimer.IsFinished && PredicateReposition());
        AddTransition(repositionState, _chaseState, () => _repositionTimer.IsFinished);

        _stateMachine.SetState(_chaseState);
    }

    private void AddTransition(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
    private void AddAnyTransition(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, new FuncPredicate(condition));

    private bool PredicateAttack() => Vector2.Distance(transform.position, _target.position) <= _attackRange;
    private bool PredicateReposition() => Random.Range(0f, 1f) <= _repositionRatio;
    #endregion
    
    #region Movement
    private void UpdateMoveDirection()
    {
        if (_target)
            _moveDirection = (_target.position - transform.position).normalized;
    }

    public void Move()
    {
        _rb.MovePosition(_rb.position + _moveDirection * (_moveSpeed * Time.fixedDeltaTime));
    }

    public void Reposition()
    {
        _rb.MovePosition(_rb.position + _repositionDirection * (_moveSpeed * Time.fixedDeltaTime));
    }

    public void StartReposition()
    {
        float angle;
    
        angle = Random.value < 0.7f ? Random.Range(0f, 135f) : 
            Random.Range(135f, 180f);

        angle *= (Random.Range(0, 2) == 0 ? -1 : 1);
        
        _repositionDirection = Quaternion.Euler(0, 0, angle) * _moveDirection;

        float duration;
        if (Mathf.Abs(angle) < 90f)
        {
            duration = Mathf.Lerp(_repositionDuration * 0.1f, _repositionDuration, Mathf.Abs(angle)/90f);
        }
        else
        {
            duration = Mathf.Lerp(_repositionDuration, _repositionDuration * 0.1f, Mathf.Abs(angle) / 180f);
        }
        
        Debug.DrawRay(transform.position, _repositionDirection, Color.red, 1f);
        
        _repositionTimer.Start(duration*duration);
    }
    #endregion

    #region Attack
    public void StartCharge() => _chargeTimer.Start();

    public void Attack()
    {
        if (_chargeTimer.Process() < 0.9f) return;
        
        var position = transform.position;
        var direction = (_target.position - position).normalized;
        direction = Quaternion.Euler(0, 0, Random.Range(-_aimOffsetAngle, _aimOffsetAngle)) * direction;
        
        _bulletPool.Fire(position, direction);
    }

    public void EndAttack() => _chargeTimer.Pause();
    #endregion

    #region KnockBack
    public void StartKnockBackTimer()
    {
        _knockBackTimer.Start();
    }

    public void TakeKnockBack(Vector2 direction)
    {
        _knockBackDirection = direction;
        _isKnockBackActive = true;
        _knockBackTimer.Start();
    }

    public void KnockBack()
    {
        _rb.MovePosition(_rb.position + _knockBackDirection * (_knockBackSpeed * Time.fixedDeltaTime));
    }
    #endregion

    #region Misc

    private void UpdateTimer()
    {
        _chargeTimer.Tick(Time.deltaTime);
        _knockBackTimer.Tick(Time.deltaTime);
        _repositionTimer.Tick(Time.deltaTime);
    }
    public void FaceTarget()
    {
        if (_moveDirection.x != 0)
            transform.localScale = new Vector3(_moveDirection.x > 0 ? -1 : 1, 1, 1);
    }

    public void HandleDeath()
    {
        _colorPoolSpawner.SpawnColorPool(transform.position, _deathColor);
        LevelManager.Instance.AddScore(1);
        DeathAction?.Invoke();
    }

    public void Initialize(PlayerColor color, RuntimeAnimatorController animator)
    {
        _deathColor = color;
        _animator.runtimeAnimatorController = animator;
        
        _damageReceiver.ResetHP();
        
        _stateMachine.SetState(_chaseState);
    }
    #endregion
}
