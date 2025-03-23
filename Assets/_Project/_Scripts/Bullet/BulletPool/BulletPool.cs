using UnityEngine;

public abstract class BulletPool<T> : PoolerBase<T> where T : Bullet
{
    public BulletInfo Info { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Info = prefab.GetComponent<T>().Info;
        prefab.name = Info.bulletName;
    }
    
    protected override void Initialize(T obj)
    {
        obj.OnDespawn += () => Return(obj);
    }
    
    protected override void GetSetup(T obj)
    {
    }
}
