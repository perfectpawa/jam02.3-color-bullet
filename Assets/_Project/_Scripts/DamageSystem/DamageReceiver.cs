using System;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    protected float _maxHP;
    [SerializeField] protected float _currentHP;
    
    public Action DeathAction;

    protected virtual void OnEnable()
    {
        _currentHP = _maxHP;
    }

    public void SetMaxHP(float maxHP)
    {
        _maxHP = maxHP;
        _currentHP = _maxHP;
    }

    public virtual void TakeDamage(float dmg)
    {
        _currentHP -= dmg;

        if (_currentHP > 0) return;
        
        _currentHP = 0;
        HandleDeath();
    }
    
    public float GetCurrentHP()
    {
        return _currentHP;
    }

    private void HandleDeath()
    {
        DeathAction?.Invoke();
    }
}
