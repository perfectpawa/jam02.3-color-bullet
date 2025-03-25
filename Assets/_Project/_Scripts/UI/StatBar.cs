using System;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _foreground;
    [Header("Sprites")]
    [SerializeField] private Sprite _backgroundSprite;
    [SerializeField] private Sprite _foregroundSprite;

    private float _maxAmount;
    
    protected virtual void Start()
    {
        _background.sprite = _backgroundSprite;
        _foreground.sprite = _foregroundSprite;
    }
    
    public void SetMaxAmount(float maxAmount) => _maxAmount = maxAmount;

    public void OnChange(float value)
    {
        if (_maxAmount == 0) return;
        _foreground.fillAmount = value/_maxAmount;
    }
}
