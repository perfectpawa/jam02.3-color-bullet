using System;
using KBCore.Refs;
using UnityEngine;

public abstract class Bullet : ValidatedMonoBehaviour
{
    [SerializeField, Child] private Animator _animator;
    [SerializeField] private BulletInfo info;

    [SerializeField] private float offset;
    
    public Action OnDespawn;
    public BulletInfo Info => info;

    private bool _isFlying;
    protected Vector2 startPos;
    protected Vector2 direction;
    private CountdownTimer _despawnTimer;

    protected virtual void Awake()
    {
        _despawnTimer = new CountdownTimer(info.duration);
        _despawnTimer.OnTimerStop += Despawn;
    }

    protected void Update()
    {
        _despawnTimer?.Tick(Time.deltaTime);
        if (_isFlying) Flying();
    }

    public virtual void OnFire(Vector2 position, Vector2 flyDirection)
    {
        _despawnTimer.Start();
        startPos = position;
        direction = flyDirection;
        _isFlying = true;
        
        var angle = Mathf.Atan2(flyDirection.y, flyDirection.x) * Mathf.Rad2Deg;
        _animator.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        transform.position = position + flyDirection.normalized * offset;
        
        _animator.CrossFade("Fly", 0);
    }

    protected virtual void Flying()
    {
        transform.Translate(direction * (info.flySpeed * Time.deltaTime));
    }
    
    protected void Despawn()
    {
        _isFlying = false;
        OnDespawn?.Invoke();
    }
}
