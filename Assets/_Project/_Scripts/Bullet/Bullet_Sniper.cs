using Unity.VisualScripting;
using UnityEngine;
public class Bullet_Sniper : PlayerBullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        
        if (!collision.TryGetComponent(out DamageReceiver receiver)) return;
        
        dmgDealer.DealOneShotDamage(1, receiver, direction); //TODO: Change damage to a variable
    }
}