using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : PersistentSingleton<LevelManager>
{
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private ColorPoolSpawner _colorPoolSpawner;
    [SerializeField] private BulletManager _bulletManager;
    [SerializeField] private PlayerController _playerController;
    
    [SerializeField] private RectTransform _pausePanel;
    [SerializeField] private RectTransform _retryPanel;
    
    [FormerlySerializedAs("score")] [SerializeField] private int _score = 0;
    
    public Action<int> OnScoreChanged = delegate { };
    
    public void ResetLevel()
    {
        _enemySpawner.ResetPool();
        _colorPoolSpawner.ResetPool();
        _playerController.ResetPlayer();
        _bulletManager.ResetAllPool();
        
        Time.timeScale = 1;
        
        _pausePanel.gameObject.SetActive(false);
        _retryPanel.gameObject.SetActive(false);
        
        _score = 0;
    }
    
    public void PauseLevel()
    {
        Time.timeScale = 0;
        _pausePanel.gameObject.SetActive(true);
    }
    
    public void ResumeLevel()
    {
        Time.timeScale = 1;
        _pausePanel.gameObject.SetActive(false);
        _retryPanel.gameObject.SetActive(false);
    }

    public void LoseLevel()
    {
        _retryPanel.gameObject.SetActive(true);
    }
    
    public void AddScore(int add)
    {
        _score += add;
        var current = _enemySpawner.GetWaveSize();
        if (_score > current && _score % current == 0)
        {
            _enemySpawner.AddWaveSize(1);
        }
        
        OnScoreChanged?.Invoke(_score);
    }
}
