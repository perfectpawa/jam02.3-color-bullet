using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(CircleCollider2D))]
public class ColorPool : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private CircleCollider2D _circleCollider2D;
    [Header("Info")] 
    [SerializeField] private float _duration = 10f;
    [Header("Data")]
    [SerializeField] private PlayerColor _playerColor;
    [SerializeField] private float _collisionRadius;
    [SerializeField] private bool _isUsing;

    private CountdownTimer _existTimer;


    public Action<ColorPool> DespawnAction;

    private void Reset()
    {
        SetupDefault();
    }

    private void Awake()
    {
        SetupDefault();
        
        _existTimer = new CountdownTimer(_duration);
        _existTimer.Stop();
    }
    
    private void Update()
    {
        _existTimer.Tick(Time.deltaTime);

        if (_existTimer.IsFinished)
        {
            DespawnAction?.Invoke(this);
            _existTimer.Stop();
        }
    }

    private void SetupDefault()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public void Initialize()
    {
    }

    public void SetupPool(PlayerColor playerColor, ColorPoolSpawner spawner)
    {
        SetColor(playerColor);
        SetCollisionRadius(spawner.CollisionRadius);
        _animator.runtimeAnimatorController = spawner.GetColorAnimator(playerColor);

        _existTimer.Start();

        _isUsing = false;
        _animator.CrossFade("Explode", 0f);
    }

    private void SetColor(PlayerColor playerColor)
    {
        _playerColor = playerColor;
        transform.name = "Pool - " + _playerColor.ToString();
    }

    private void SetCollisionRadius(float collisionRadius)
    {
        _collisionRadius = collisionRadius;
        if (_circleCollider2D != null ) _circleCollider2D.radius = collisionRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isUsing) return;

        if (!collision.CompareTag("Player")) return;
        
        var player = collision.GetComponent<PlayerController>();
        player.ChangePlayerColor(_playerColor);
        _isUsing = true;
    }
}
