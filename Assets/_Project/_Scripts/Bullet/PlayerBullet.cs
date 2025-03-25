using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBullet : Bullet
{
    [SerializeField] protected PlayerBulletDmgDealer dmgDealer;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        
        if (!collision.TryGetComponent(out DamageReceiver receiver)) return;
        
        dmgDealer.DealOneShotDamage(Info.damage, receiver, direction);
        Despawn();
    }
    
    
}