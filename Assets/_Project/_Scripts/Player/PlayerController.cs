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
        //call test knockBack after 1 second
        Invoke(nameof(TestKnockBack), 2f);
    }

    private void TestKnockBack()
    {
        Debug.Log("TestKnockBack");
        _knockBackDirection = -_lookDirection;
        _knockBackTimer.Start();
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
        var knockBackState = new PlayerState_KnockBack(this, _animator);
        var chargeState = new PlayerState_Charge(this, _animator);
        
        At(locomotionState, knockBackState, new FuncPredicate(() => _knockBackTimer.IsRunning));
        At(locomotionState, chargeState, new FuncPredicate(() => _chargeTimer.IsRunning));
        
        At(knockBackState, locomotionState, new FuncPredicate(() => !_knockBackTimer.IsRunning));
        At(chargeState, locomotionState, new FuncPredicate(() => !_chargeTimer.IsRunning));

        
        _stateMachine.SetState(locomotionState);
        return;

        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
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
    
    public void HandleKnockBack()
    {
        _rb.MovePosition(_rb.position + _knockBackDirection.normalized * (_knockBackSpeed * Time.fixedDeltaTime));
    }
    
    public void HandleCharge()
    {
    }

    public void HandleOnFireDefault()
    {
        
    }
}
