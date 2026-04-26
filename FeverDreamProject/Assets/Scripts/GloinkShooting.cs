using System.Collections.Generic;
using UnityEngine;

public class GloinkShooting : MonoBehaviour
{
    [SerializeField]
    private float fireRate;
    private float FireRate => fireRate;
    [SerializeField]
    private float bulletDamage;
    private float BulletDamage => bulletDamage;
    [SerializeField]
    private float bulletSpeed;
    private float BulletSpeed => bulletSpeed;

    private List<BulletData> Bullets { get; set; }

    private bool IsReadyToFire { get; set; }
    private float ElapsedT { get; set; }

    private class BulletData
    {
        public Transform Transform { get; set; }
        public Vector3 Direction { get; set; }
    }

    void Awake()
    {
        IsReadyToFire = true;
    }

    void Update()
    {
        ElapsedT += Time.deltaTime;
        if (ElapsedT > FireRate)
        {
            ElapsedT = 0;
            IsReadyToFire = true;
        }

        UpdateBullets();
    }

    void UpdateBullets()
    {
        foreach (var bulletData in Bullets)
        {
            bulletData.Transform.position += bulletData.Direction * Time.deltaTime * BulletSpeed;
        }
    }

    public void Shoot(Vector3 dir)
    {
        if (!IsReadyToFire)
        {
            return;
        }

        ElapsedT = 0;
        IsReadyToFire = false;

        var bullet = BulletPool.Instance.GetBullet();
        Bullets.Add(new BulletData()
        {
            Transform = bullet.transform,
            Direction = dir
        });
    }
}