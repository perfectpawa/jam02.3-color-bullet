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

    private Vector2 _chargePosition;
    private Vector2 _chargeDirection;

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
    
    public void FireDefault(Vector2 position, Vector2 direction)
    {
        _canFire = false;
        _defaultFireRateTimer.Start();
        
        var bullet = poolDefault.Get();
        bullet.OnFire(position, direction);
    }
    
    public void FireShotgun(Vector2 position, Vector2 direction)
    {
        _canFire = false;
        _shotgunFireRateTimer.Start();
        
        var bullet = poolShotgun.Get();
        bullet.OnFire(position, direction);
    }

    public void ChargeSniper(Vector2 position, Vector2 direction)
    {
        _canFire = false;
        
        _chargePosition = position;
        _chargeDirection = direction;
    }
    
    public void FireSniper()
    {
        _sniperFireRateTimer.Start();
        
        var bullet = poolSniper.Get();
        bullet.OnFire(_chargePosition, _chargeDirection);
    }

    public void ChangeBulletType(BulletType type)
    {
        _currentBulletType = type;
    }
    
    public void ResetAllPool()
    {
        poolDefault.ResetPool();
        poolShotgun.ResetPool();
        poolSniper.ResetPool();
    }
}

public enum BulletType
{
    Default,
    Shotgun,
    Sniper,
}
