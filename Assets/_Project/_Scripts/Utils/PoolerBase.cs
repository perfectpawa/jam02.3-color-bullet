using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PoolerBase<T> : MonoBehaviour where T : MonoBehaviour 
{
    [FormerlySerializedAs("_prefab")]
    [Header("Pooled Settings")]
    [SerializeField] protected T prefab;
    [SerializeField] protected GameObject holder;
    
    private List<T> _pool;
    private List<T> _activePool;
    
    protected int ActiveCount => _activePool.Count;

    private int _count = 0;

    private List<T> Pool {
        get {
            if (_pool == null) throw new InvalidOperationException("You need to call InitPool before using it.");
            return _pool;
        }
        set => _pool = value;
    }

    protected virtual void Awake()
    {
        if (prefab == null) throw new InvalidOperationException("Prefab is not set.");
        if (holder == null) throw new InvalidOperationException("Holder is not set.");

        InitPool(prefab, holder);
    }

    private void InitPool(T prefab, GameObject holder = null) {
        this.prefab = prefab;
        this.holder = holder;

        Pool = new List<T>();
        _activePool = new List<T>();
    }

    /// <summary>
    /// Get an object from the pool
    /// </summary>
    public virtual T Get()
    {
        if (Pool.Count == 0)
        {
            CreateNew();
        }

        var obj = Pool[0];
        Pool.RemoveAt(0);
        obj.gameObject.SetActive(true);
        
        _activePool.Add(obj);

        Initialize(obj);

        return obj;
    }

    /// <summary>
    /// Initialize the object when it is "retrieved" from the pool
    /// </summary>
    protected abstract void Initialize(T obj);

    /// <summary>
    ///  Set up the object when it is "created"
    /// </summary>
    protected abstract void GetSetup(T obj);

    private void CreateNew()
    {
        var obj = Instantiate(prefab, holder.transform);
        obj.gameObject.SetActive(false);

        obj.name = prefab.name + " - " + _count;
        _count += 1;

        GetSetup(obj);

        Pool.Add(obj);
    }

    /// <summary>
    ///  Return the object to the pool
    /// </summary>
    protected void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        Pool.Add(obj);
        _activePool.Remove(obj);
    }
}
