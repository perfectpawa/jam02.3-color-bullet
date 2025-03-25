using UnityEngine;

public class EnemyBulletDmgDealer : DamageDealer
{
    public void DealOneShotDamage(float dmg, DamageReceiver receiver, Vector3 forceDirection)
    {
        _hasDealDamge.Clear();
        
        if (_hasDealDamge.Contains(receiver)) return;
        
        var player = receiver.GetComponent<PlayerController>();

        if (player == null) return;
        
        player.TakeKnockBack(forceDirection);
        
        receiver.TakeDamage(dmg);
        _hasDealDamge.Add(receiver);
    }
}
