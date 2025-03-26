using System;
using UnityEngine;

public class EnemyBulletPool : BulletPool<EnemyBullet>
{
    private CountdownTimer _countdownTimer;

    private void Start()
    {
        _countdownTimer = new CountdownTimer(Info.fireRate);
    }
    
    private void Update()
    {
        _countdownTimer.Tick(Time.deltaTime);
    }

    public void Fire(Vector3 position, Vector3 direction)
    {
        if (!_countdownTimer.IsFinished) return;
        var bullet = Get();

        bullet.OnFire(position, direction);
            
        _countdownTimer.Start();
    }
}
