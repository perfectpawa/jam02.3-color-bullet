using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

public abstract class Button_Base : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    protected abstract void OnClick();
    
}
