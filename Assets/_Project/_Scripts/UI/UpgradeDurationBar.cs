using UnityEngine;

public class UpgradeDurationBar : StatBar
{
    [SerializeField] private PlayerController _playerController;

    protected override void Start()
    {
        base.Start();
        SetMaxAmount(1);
        _playerController.BulletUpgradeTimer.OnTimerTick += OnChange;
    }

    private void OnEnable()
    {
    }

    public override void OnChange(float value)
    {
        value = 1 - value;
        base.OnChange(value);
    }

    private void OnDisable()
    {
        _playerController.BulletUpgradeTimer.OnTimerTick -= OnChange;
    }
}
