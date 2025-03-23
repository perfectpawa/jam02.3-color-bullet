using System;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private BulletPool_Default poolDefault;
    [SerializeField] private BulletPool_Shotgun poolShotgun;
    [SerializeField] private BulletPool_Sniper poolSniper;
    
    private CountdownTimer _defaultFireRateTimer;
    private CountdownTimer _shotgunFireRateTimer;
    private CountdownTimer _sniperFireRateTimer;
    
    private bool _isFiring = false;

    public bool IsFiring => _isFiring;

    private void Start()
    {
        _defaultFireRateTimer = new CountdownTimer(poolDefault.Info.fireRate);
        _defaultFireRateTimer.OnTimerStart += () => _isFiring = true;
        
        _shotgunFireRateTimer = new CountdownTimer(poolShotgun.Info.fireRate);
        _shotgunFireRateTimer.OnTimerStart += () => _isFiring = true;
    }

    private void Update()
    {
        _defaultFireRateTimer.Tick(Time.deltaTime);
    }

    public void Fire(Vector2 position, Vector2 direction, BulletType type, PlayerController player)
    {
        
        switch (type)
        {
            case BulletType.Default:
                FireDefault(position, direction);
                break;
            case BulletType.Shotgun:
                FireShotgun(position, direction, player);
                break;
            case BulletType.Sniper:
                FireSniper(position, direction, player);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    private void FireDefault(Vector2 position, Vector2 direction)
    {
        if (_defaultFireRateTimer.IsRunning) return;
        _defaultFireRateTimer.Start();
        
        var bullet = poolDefault.Get();
        bullet.OnFire(position, direction);
    }
    
    private void FireShotgun(Vector2 position, Vector2 direction, PlayerController player)
    {
        if (_shotgunFireRateTimer.IsRunning) return;
        _shotgunFireRateTimer.Start();
        
        var bullet = poolShotgun.Get();
        bullet.OnFire(position, direction);
    }
    
    private void FireSniper(Vector2 position, Vector2 direction, PlayerController player)
    {
        var bullet = poolSniper.Get();
        bullet.OnFire(position, direction);
    }
}

public enum BulletType
{
    Default,
    Shotgun,
    Sniper,
}
