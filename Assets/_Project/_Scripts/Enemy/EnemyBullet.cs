using UnityEngine;

public class EnemyBullet : Bullet
{
    [SerializeField] private EnemyBulletDmgDealer _dmgDealer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) return;
        
        if (!collision.TryGetComponent(out DamageReceiver receiver)) return;
        
        _dmgDealer.DealOneShotDamage(1, receiver, direction); //TODO: Change damage to a variable
        Despawn();
    }
    
}
