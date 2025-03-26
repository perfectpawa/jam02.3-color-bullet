using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : PoolerBase<EnemyController>
{
    [Header("Reference")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _farFromPlayer = 5f;

    [Header("Spawner Settings")]
    [SerializeField] private Transform topLeftLimit;
    [SerializeField] private Transform bottomDownLimit;
    [SerializeField] private float _waveCooldown = 1f;
    [SerializeField][UnityEngine.Range(1,100)] private int _maxWaveSize = 5;

    [Header("Visual Enemy")]
    [SerializeField] private List<AnimatorController> _animators;

    private CountdownTimer _waveTimer;

     [SerializeField]private float _currentMaxWave;
    private void Start()
    {
        _waveTimer = new CountdownTimer(_waveCooldown);
        _waveTimer.Stop();
        
        _currentMaxWave = _maxWaveSize;
    }

    private void Update()
    {
        _waveTimer.Tick(Time.deltaTime);

        if (_waveTimer.IsRunning) return;
        
        _waveTimer.Start();
        SpawnWave();
    }

    protected override void Initialize(EnemyController obj)
    {
    }

    protected override void GetSetup(EnemyController obj)
    {
        obj.DeathAction += () => Return(obj);
    }

    private void SpawnWave()
    {
        var waveSize = Mathf.Max(0, _currentMaxWave - ActiveCount);

        for (var i = 0; i < waveSize; i++)
        {
            var enemy = Get();
            if (!enemy) continue;

            enemy.transform.position = GetRandomPos();
            enemy.gameObject.SetActive(true);

            var playerColor = (PlayerColor)Random.Range(1, Enum.GetValues(typeof(PlayerColor)).Length);
            enemy.Initialize(playerColor, GetCloseRangeAnimator(playerColor));
        }
    }

    private Vector2 GetRandomPos()
    {
        Vector2 spawnRangeMin = bottomDownLimit.position;
        Vector2 spawnRangeMax = topLeftLimit.position;
        Vector2 randomPos;

        do
        {
            randomPos = new Vector2(
                Random.Range(spawnRangeMin.x, spawnRangeMax.x),
                Random.Range(spawnRangeMin.y, spawnRangeMax.y)
            );
        }
        while (Vector2.Distance(randomPos, _playerTransform.position) < _farFromPlayer);

        return randomPos;
    }

    private AnimatorController GetCloseRangeAnimator(PlayerColor color)
    {
        if (_animators == null || _animators.Count < 2) return null;

        return color switch
        {
            PlayerColor.Orange => _animators[0],
            PlayerColor.Purple => _animators[1],
            _ => null
        };
    }
    
    public void AddWaveSize(int amount)
    {
        _currentMaxWave += amount;
    }
    
    public int GetWaveSize()
    {
        return (int)_currentMaxWave;
    }
}
