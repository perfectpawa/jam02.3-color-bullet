using System;
using KBCore.Refs;
using StateMachineBehaviour;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [SerializeField, Anywhere] private InputReader _inputReader;
    [SerializeField, Self] private Rigidbody2D _rb;
    [SerializeField, Child] private Animator _animator;
    [SerializeField, Anywhere] private BulletManager _bulletManager;
    
    [SerializeField] private float _moveSpeed = 1.5f;
    
    [SerializeField] private float _knockBackDuration = 0.5f;
    [SerializeField] private float _knockBackSpeed = 1f;
    
    [SerializeField] private float _chargeDuration = 1.5f;
    

    private Camera _camera;
    
    private StateMachine _stateMachine;
    
    private Vector2 _moveDirection;
    private Vector2 _lookDirection;
    
    private CountdownTimer _knockBackTimer;
    private CountdownTimer _chargeTimer;
    
    private Vector2 _knockBackDirection;

    private bool _wantFire;
    
    #region Unity Callbacks
    private void OnEnable()
    {
        SubscribeInput();
    }

    private void OnDisable()
    {
        UnsubscribeInput();
    }

    private void Awake()
    {
        _camera = Camera.main;
        
        SetupInputReader();
        SetupTimer();
        SetupStateMachine();
    }

    private void Start()
    {
    }


    private void Update()
    {
        _stateMachine.Update();
        HandleUpdateInput();
        
        _knockBackTimer.Tick(Time.deltaTime);
        _chargeTimer.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }
    #endregion
    
    #region Setup
    private void SubscribeInput()
    {
        _inputReader.Fire += OnFire;
    }

    private void UnsubscribeInput()
    {
        _inputReader.Fire += OnFire;
    }
    
    private void OnFire(bool state)
    {
        _wantFire = state;
    }

    private void SetupTimer()
    {
        _knockBackTimer = new CountdownTimer(_knockBackDuration);
        _chargeTimer = new CountdownTimer(_chargeDuration);
    }
    
    private void SetupInputReader()
    {
        _inputReader.EnablePlayerAction();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();
        
        var locomotionState = new PlayerState_Locomotion(this, _animator);
        var fireDefaultState = new PlayerState_FireDefault(this, _animator);
        var fireShotgunState = new PlayerState_FireShotgun(this, _animator);
        var fireSniperState = new PlayerState_FireSniper(this, _animator);
        
        Any(fireDefaultState, new FuncPredicate(PredicateForDefault));
        Any(fireShotgunState, new FuncPredicate(PredicateForShotgun));
        Any(fireSniperState, new FuncPredicate(PredicateForSniper));
        
        At(fireDefaultState, locomotionState, new FuncPredicate(() => !_bulletManager.CanFire && !_wantFire));
        
        At(fireShotgunState, locomotionState, new FuncPredicate(() => _knockBackTimer.IsFinished));
        
        At(fireSniperState, locomotionState, new FuncPredicate(() => _chargeTimer.IsFinished));
        
        _stateMachine.SetState(locomotionState);
        return;

        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
    }

    private bool PredicateForDefault()
    {
        if (_bulletManager.CurrentBulletType != BulletType.Default) return false;
        return _bulletManager.CanFire && _wantFire;
    }

    private bool PredicateForShotgun()
    {
        if (_bulletManager.CurrentBulletType != BulletType.Shotgun) return false;
        return _bulletManager.CanFire && _wantFire;
    }
    
    private bool PredicateForSniper()
    {
        if (_bulletManager.CurrentBulletType != BulletType.Sniper) return false;
        return _bulletManager.CanFire && _wantFire;
    }
    
    
    #endregion
    
    private void HandleUpdateInput()
    {
        _moveDirection = _inputReader.Direction;
        var mousePosition = _camera.ScreenToWorldPoint(_inputReader.MousePosition);
        mousePosition.z = 0;
        _lookDirection = (mousePosition - transform.position).normalized;
    }
    
    public void HandleLocomotion()
    {
        _rb.MovePosition(_rb.position + _moveDirection.normalized * (_moveSpeed * Time.fixedDeltaTime));
    }
    
    public void HandleLookAtTarget()
    {
        switch (_lookDirection.x)
        {
            case > 0:
                transform.localScale = new Vector3(-1, 1, 1);
                transform.right = _lookDirection;
                break;
            case < 0:
                transform.localScale = new Vector3(1, 1, 1);
                transform.right = -_lookDirection;
                break;
        }
    }

    public void HandleOnKnockBack()
    {
        _knockBackTimer.Start();
    }
    
    public void HandleKnockBack()
    {
        _rb.MovePosition(_rb.position + _knockBackDirection.normalized * (_knockBackSpeed * Time.fixedDeltaTime));
    }
    
    public void HandleOnFireDefault()
    {
        _bulletManager.FireDefault(transform.position, _lookDirection);
    }
    
    public void HandleOnFireShotgun()
    {
        _bulletManager.FireShotgun(transform.position, _lookDirection);
        _knockBackDirection = -_lookDirection.normalized;
    }

    public void HandleOnChargeSniper()
    {
        _chargeTimer.Start();
        _bulletManager.ChargeSniper(transform.position, _lookDirection);
    }
    
    public void HandleOnFireSniper()
    {
        _bulletManager.FireSniper();
    }


    
}
