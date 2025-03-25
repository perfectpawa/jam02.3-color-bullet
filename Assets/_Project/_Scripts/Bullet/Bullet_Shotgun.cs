using Unity.VisualScripting;
using UnityEngine;

public class Bullet_Shotgun : PlayerBullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        
        if (!collision.TryGetComponent(out DamageReceiver receiver)) return;
        
        var forceDirection = -(startPos - (Vector2)transform.position).normalized;
        
        dmgDealer.DealOneShotDamage(Info.damage, receiver, forceDirection);
    }
}
