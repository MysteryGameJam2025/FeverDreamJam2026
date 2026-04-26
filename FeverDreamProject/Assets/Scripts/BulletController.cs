using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float Damage { get; set; }
    public Action OnHit { get; set; }

    void OnCollisionEnter(Collision other)
    {
        OnHit?.Invoke();
        BulletPool.Instance.ReturnToPool(this);

        if (other.gameObject.layer == 7)
        {
            // do damage here
        }
    }
}
