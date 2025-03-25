using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class ColorPoolSpawner : PoolerBase<ColorPool>
{
    [SerializeField] private float _collisionRadius = 1f;
    [SerializeField] private float _existTime;
    [SerializeField] private List<PoolData> _poolDatas;

    public float CollisionRadius => _collisionRadius;
    public float ExistTime => _existTime;

    public AnimatorController GetColorAnimator(PlayerColor playerColor)
    {
        //get the animator from the list
        foreach (var data in _poolDatas.Where(data => data.playerColor == playerColor))
        {
            return data.animator;
        }

        Debug.LogError("Color not found in the list");

        return null;
    }

    public void SpawnColorPool(Vector3 position, PlayerColor playerColor)
    {
        var colorPool = Get();
        colorPool.transform.position = position;
        colorPool.SetupPool(playerColor, this);
    }

    public void SpawnRandomPool(Vector3 position)
    {
        var colorPool = Get();
        colorPool.transform.position = position;
        colorPool.SetupPool(_poolDatas[UnityEngine.Random.Range(0, _poolDatas.Count)].playerColor, this);
    }

    private void Despawn(ColorPool obj)
    {
        Return(obj);
    }

    protected override void GetSetup(ColorPool obj)
    {
        obj.DespawnAction += Despawn;
    }

    protected override void Initialize(ColorPool obj)
    {
    }
}

[Serializable]
public struct PoolData
{
    public PlayerColor playerColor;
    public AnimatorController animator;
}
