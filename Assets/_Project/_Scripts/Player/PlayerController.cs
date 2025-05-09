using System;
using KBCore.Refs;
using StateMachineBehaviour;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Anywhere] private InputReader _inputReader;
    [SerializeField, Self] private Rigidbody2D _rb;
    [SerializeField, Child] private Animator _animator;
    [SerializeField, Anywhere] private BulletManager _bulletManager;
    [SerializeField, Child] private Transform _model;
    [SerializeField, Self] private PlayerDamageReceiver _damageReceiver;
    
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotateSpeed = 180f;
    [Header("Knock Back Settings")]
    [SerializeField] private float _knockBackDuration = 0.5f;
    [SerializeField] private float _knockBackSpeed = 1f;
    [Header("Charge Settings")]
    [SerializeField] private float _chargeDuration = 1.5f;
    [Header("Player Info")]
    [SerializeField] private PlayerColor _playerColor;
    [SerializeField] private float _playerMaxHP = 3f;
    [SerializeField] private float _bulletDuration = 10f;

    private Camera _camera;
    
    private StateMachine _stateMachine;
    
    private Vector2 _moveDirection;
    private Vector2 _lookDirection;
    
    private CountdownTimer _knockBackTimer;
    private CountdownTimer _chargeTimer;

    public CountdownTimer BulletUpgradeTimer { get; private set; }

    private Vector2 _knockBackDirection;

    private bool _wantFire;
    
    private bool _getKnockBack = false;
    
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
        _damageReceiver.DeathAction += HandleDeath;
        _damageReceiver.SetMaxHP(_playerMaxHP);
    }


    private void Update()
    {
        _stateMachine.Update();
        HandleUpdateInput();
        
        _knockBackTimer.Tick(Time.deltaTime);
        _chargeTimer.Tick(Time.deltaTime);
        BulletUpgradeTimer.Tick(Time.deltaTime);
        
        //if click "e" then change the color + 1
        if (Input.GetKeyDown(KeyCode.E))
        {
            int nextColor = (int)_playerColor + 1;
            if (nextColor > 2)
            {
                nextColor = 0;
            }
            
            ChangePlayerColor((PlayerColor)nextColor);
        }
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
        _inputReader.Fire -= OnFire;
    }
    
    private void OnFire(bool state)
    {
        _wantFire = state;
    }

    private void SetupTimer()
    {
        _knockBackTimer = new CountdownTimer(_knockBackDuration);
        _knockBackTimer.OnTimerStop += () => _getKnockBack = false;
        
        _chargeTimer = new CountdownTimer(_chargeDuration);
        
        BulletUpgradeTimer = new CountdownTimer(_bulletDuration);
        BulletUpgradeTimer.OnTimerStop += () => ChangePlayerColor(PlayerColor.White);
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
        var knockBackState = new PlayerState_KnockBack(this, _animator);
        
        Any(fireDefaultState, new FuncPredicate(PredicateForDefault));
        Any(fireShotgunState, new FuncPredicate(PredicateForShotgun));
        Any(fireSniperState, new FuncPredicate(PredicateForSniper));
        
        At(fireDefaultState, locomotionState, new FuncPredicate(() => !_bulletManager.CanFire || !_wantFire));
        
        At(fireShotgunState, locomotionState, new FuncPredicate(() => _knockBackTimer.IsFinished));
        
        At(fireSniperState, locomotionState, new FuncPredicate(() => _chargeTimer.IsFinished));
        
        Any(knockBackState, new FuncPredicate(() => _getKnockBack));
        At(knockBackState, locomotionState, new FuncPredicate(() => _knockBackTimer.IsFinished));
        
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
        var direction = _lookDirection.normalized;
        
        // Calculate the target rotation angle in degrees
        var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        var currentAngle = transform.eulerAngles.z;
        
        // Calculate the shortest rotation direction using DeltaAngle
        var shortestAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Smoothly interpolate to the target angle
        var newAngle = currentAngle + shortestAngle * _rotateSpeed * Time.deltaTime;
        
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
        
        var absAngle = Mathf.Abs(newAngle);
        if (newAngle is > 90f and < 270f)
        {
            transform.localScale = new Vector3(1, -1, 1); // Flip on Y-axis
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // Normal orientation
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
    
    public void ChangePlayerColor(PlayerColor playerColor)
    {
        if (_playerColor == playerColor)
        {
            if (_playerColor != PlayerColor.White) BulletUpgradeTimer.Start();
        }
        _playerColor = playerColor;
        _animator.CrossFade("Idle-" + _playerColor, 0f);

        switch (_playerColor)
        {
            case PlayerColor.White:
                _bulletManager.ChangeBulletType(BulletType.Default);
                break;
            case PlayerColor.Orange:
                _bulletManager.ChangeBulletType(BulletType.Shotgun);
                break;
            case PlayerColor.Purple:
                _bulletManager.ChangeBulletType(BulletType.Sniper);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (_playerColor != PlayerColor.White)
        {
            BulletUpgradeTimer.Start();
        }
    }
    
    public void TakeKnockBack(Vector3 direction)
    {
        _knockBackDirection = direction;
        _getKnockBack = true;
    }

    private void HandleDeath()
    {
        Debug.Log("Player is dead");
        Time.timeScale = 0;
        
        LevelManager.Instance.LoseLevel();
    }
    
    public void ResetPlayer()
    {
        _damageReceiver.ResetHP();
        _animator.CrossFade("Idle-White", 0f);
        ChangePlayerColor(PlayerColor.White);
        
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = Vector3.zero;
    }
}

public enum PlayerColor
{
    White,
    Orange,
    Purple,
}
