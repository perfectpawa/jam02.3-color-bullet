using UnityEngine;

public class PlayerBulletDmgDealer : DamageDealer
{
    public void DealOneShotDamage(float dmg, DamageReceiver receiver, Vector3 forceDirection)
    {
        _hasDealDamge.Clear();
        
        if (_hasDealDamge.Contains(receiver)) return;
        
        var enemy = receiver.GetComponent<EnemyController>();

        if (enemy == null) return;
        
        enemy.TakeKnockBack(forceDirection);
        
        receiver.TakeDamage(dmg);
        _hasDealDamge.Add(receiver);
    }
}