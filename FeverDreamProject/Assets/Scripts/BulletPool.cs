using System.Collections.Generic;
using UnityEngine;

public class BulletPool : AbstractMonoBehaviourSingleton<BulletPool>
{
    [SerializeField]
    private GameObject bulletPrefab;
    private GameObject BulletPrefab => bulletPrefab;
    [SerializeField]
    private int poolSize;
    private int PoolSize => poolSize;

    private Queue<GameObject> Pool { get; set; }

    void Awake()
    {
        PopulatePool();
    }

    void PopulatePool()
    {
        Pool = new Queue<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            var bullet = Instantiate(BulletPrefab, transform);
            bullet.SetActive(false);
            Pool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        var bullet = Pool.Dequeue();
        if (Pool.Count == 0)
        {
            PopulatePool();
        }
        bullet.SetActive(true);
        return bullet;
    }
}