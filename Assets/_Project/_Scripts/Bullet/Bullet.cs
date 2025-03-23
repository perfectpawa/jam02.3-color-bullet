using System;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] private BulletInfo info;
    
    public Action OnDespawn;
    public BulletInfo Info => info;

    private bool _isFlying;
    private Vector2 _direction;
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

    public virtual void OnFire(Vector2 position, Vector2 direction)
    {
        _despawnTimer.Start();
        _direction = direction;
        _isFlying = true;
        
        transform.position = position;
    }

    protected virtual void Flying()
    {
        transform.Translate(_direction * (info.flySpeed * Time.deltaTime));
    }
    
    private void Despawn()
    {
        _isFlying = false;
        OnDespawn?.Invoke();
    }
}
