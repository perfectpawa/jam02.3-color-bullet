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

    [SerializeField] private BulletType _currentBulletType = BulletType.Default;
    private CountdownTimer _defaultTimer;
    
    [SerializeField] private bool _canFire = true;

    public BulletType CurrentBulletType => _currentBulletType;
    public bool CanFire => _canFire;

    private void Start()
    {
        _defaultFireRateTimer = new CountdownTimer(poolDefault.Info.fireRate);
        _defaultFireRateTimer.OnTimerStop += () => _canFire = true;
        
        _shotgunFireRateTimer = new CountdownTimer(poolShotgun.Info.fireRate);
        _shotgunFireRateTimer.OnTimerStop += () => _canFire = true;
        
        _sniperFireRateTimer = new CountdownTimer(poolSniper.Info.fireRate);
        _sniperFireRateTimer.OnTimerStop += () => _canFire = true;
    }

    private void Update()
    {
        _defaultFireRateTimer.Tick(Time.deltaTime);
        _shotgunFireRateTimer.Tick(Time.deltaTime);
        _sniperFireRateTimer.Tick(Time.deltaTime);
    }

    public void Fire(Vector2 position, Vector2 direction, BulletType bulletType)
    {
        _canFire = false;
        switch (bulletType)
        {
            case BulletType.Default:
                FireDefault(position, direction);
                break;
            case BulletType.Shotgun:
                FireShotgun(position, direction);
                break;
            case BulletType.Sniper:
                FireSniper(position, direction);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bulletType), bulletType, null);
        }
    }
    
    private void FireDefault(Vector2 position, Vector2 direction)
    {
        _defaultFireRateTimer.Start();
        
        var bullet = poolDefault.Get();
        bullet.OnFire(position, direction);
    }
    
    private void FireShotgun(Vector2 position, Vector2 direction)
    {
        _shotgunFireRateTimer.Start();
        
        var bullet = poolShotgun.Get();
        bullet.OnFire(position, direction);
    }
    
    private void FireSniper(Vector2 position, Vector2 direction)
    {
        _sniperFireRateTimer.Start();
        
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
