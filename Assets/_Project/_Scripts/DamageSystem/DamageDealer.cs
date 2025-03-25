using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] protected List<LayerMask> _layerMasks;

    protected List<DamageReceiver> _hasDealDamge;

    protected bool _isDealingOverTimeDamage;

    private void Awake()
    {
        _hasDealDamge = new List<DamageReceiver>();
        _isDealingOverTimeDamage = false;
    }

    private void Start()
    {
        if (_layerMasks.Count == 0)
        {
            _layerMasks.Add(LayerMask.GetMask("Everything"));
        }
    }

    protected virtual void Update()
    {
        if (_isDealingOverTimeDamage) DealOverTimeDamage();
    }

    public virtual void DealOneShotDamage(float dmg)
    {
        _hasDealDamge.Clear();
    }

    public virtual void DealOneShotDamage(float dmg, DamageReceiver receiver)
    {
        _hasDealDamge.Clear();
    }

    protected virtual void DealOverTimeDamage()
    {

    }

    public void StartDealDamage()
    {
        _hasDealDamge.Clear();
        _isDealingOverTimeDamage = true;
    }

    public void StopDealDamage()
    {
        _isDealingOverTimeDamage = false;
    }
}
