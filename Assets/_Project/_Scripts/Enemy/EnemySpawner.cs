using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawner : PoolerBase<EnemyController>
{
    [Header("Reference")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _farFromPlayer = 5f;
    [Header("Spawner Settings")]
    [SerializeField] private Transform topleftLimit;
    [SerializeField] private Transform bottomdownLimit;

    [SerializeField] private float _waveCooldown = 1f;
    [FormerlySerializedAs("_waveSize")] [SerializeField][UnityEngine.Range(5,100)] private int _maxWaveSize = 5;
    [SerializeField] private int _waveCount = 0;
    [Header("Visual Enemy")]
    [SerializeField] private List<AnimatorController> _animators;

    CountdownTimer _waveTimer;

    int _waveIndex = 0;

    private void Start()
    {
        _waveTimer = new CountdownTimer(_waveCooldown);
        _waveTimer.Stop();
    }

    private void Update()
    {
        _waveTimer.Tick(Time.deltaTime);

        if (_waveTimer.IsFinished || !_waveTimer.IsRunning)
        {
            _waveTimer.Start();

            SpawnWave();
        }
    }

    protected override void GetSetup(EnemyController obj)
    {
        obj.DeathAction += () => Return(obj);
    }

    protected override void Initialize(EnemyController obj)
    {
    }

    private void Despawn(EnemyController obj)
    {
        Return(obj);
    }

    public void SpawnWave()
    {
        // if (_waveIndex >= _waveCount)
        // {
        //     return;
        // }
        //
        // _waveIndex++;
        
        var waveSize = _maxWaveSize - ActiveCount;

        for (var i = 0; i < waveSize; i++)
        {
            var enemy = Get();

     

            var spawnPos = GetRandomPos();

            var playerColor = (PlayerColor)Random.Range(1, Enum.GetValues(typeof(PlayerColor)).Length);
            
            enemy.transform.position = spawnPos;
            enemy.gameObject.SetActive(true);
            enemy.Initialize(playerColor, GetCloseRangeAnimator(playerColor));

        }
    }

    private Vector2 GetRandomPos()
    {
        Vector2 randomPos = default;
        var validPos = false;

        // Get the bounds from the corner points (top left and bottom down)
        Vector2 spawnRangeMin = bottomdownLimit.position;
        Vector2 spawnRangeMax = topleftLimit.position;

        // Try to find a valid position that meets the distance requirement
        while (!validPos)
        {
            // Generate a random position within the specified spawn range
            randomPos = new Vector2(
                Random.Range(spawnRangeMin.x, spawnRangeMax.x),
                Random.Range(spawnRangeMin.y, spawnRangeMax.y)
            );

            var distanceToPlayer = Vector2.Distance(randomPos, _playerTransform.position);
            // Check if the random position is at least 'farFromPlayer' units away from the player
            if (distanceToPlayer >= _farFromPlayer)
            {
                validPos = true; // Position is valid
            }
        }

        return randomPos;
    }
    
    private AnimatorController GetCloseRangeAnimator(PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.Orange:
                return _animators[0];
            case PlayerColor.Purple:
                return _animators[1];
            default:
                return null;
        }
        
    }
}
