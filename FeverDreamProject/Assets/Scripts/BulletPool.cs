using System.Collections.Generic;
using UnityEngine;

public class BulletPool : AbstractMonoBehaviourSingleton<BulletPool>
{
    [SerializeField]
    private BulletController bulletPrefab;
    private BulletController BulletPrefab => bulletPrefab;
    [SerializeField]
    private int poolSize;
    private int PoolSize => poolSize;

    private Queue<BulletController> Pool { get; set; }

    void Awake()
    {
        PopulatePool();
    }

    void PopulatePool()
    {
        Pool = new Queue<BulletController>();
        for (int i = 0; i < PoolSize; i++)
        {
            var bullet = Instantiate(BulletPrefab, transform);
            bullet.gameObject.SetActive(false);
            Pool.Enqueue(bullet);
        }
    }

    public BulletController GetBullet()
    {
        var bullet = Pool.Dequeue();
        if (Pool.Count == 0)
        {
            PopulatePool();
        }
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnToPool(BulletController bullet)
    {
        bullet.gameObject.SetActive(false);
        Pool.Enqueue(bullet);
    }
}