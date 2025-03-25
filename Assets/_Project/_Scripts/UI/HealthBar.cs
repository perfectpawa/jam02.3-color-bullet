using System;
using Unity.VisualScripting;
using UnityEngine;

public class HealthBar : StatBar
{
    [SerializeField] private PlayerDamageReceiver _playerDamageReceiver;

    private void OnEnable()
    {
        _playerDamageReceiver.OnChangeMaxHP += SetMaxAmount;
        _playerDamageReceiver.OnChangeCurrentHP += OnChange;    
    }

    private void OnDisable()
    {
        _playerDamageReceiver.OnChangeMaxHP -= SetMaxAmount;
        _playerDamageReceiver.OnChangeCurrentHP -= OnChange;
    }

}