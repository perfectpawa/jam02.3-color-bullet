using System;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    protected float _maxHP;
    [SerializeField] protected float _currentHP;
    
    public Action<float> OnChangeCurrentHP;
    public Action<float> OnChangeMaxHP;
    
    public Action DeathAction;

    protected virtual void OnEnable()
    {
        _currentHP = _maxHP;
    }

    public void SetMaxHP(float maxHP)
    {
        _maxHP = maxHP;
        _currentHP = _maxHP;
        
        OnChangeMaxHP?.Invoke(_maxHP);
    }

    public virtual void TakeDamage(float dmg)
    {
        _currentHP -= dmg;

        if (_currentHP <= 0) _currentHP = 0;
        OnChangeCurrentHP?.Invoke(_currentHP);

        if(_currentHP == 0) HandleDeath();
    }

    private void HandleDeath()
    {
        DeathAction?.Invoke();
    }
}
