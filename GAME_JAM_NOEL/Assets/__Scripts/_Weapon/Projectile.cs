using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private float speedProjectile;
    [SerializeField] 
    private float lifeSpawn;
    [SerializeField]
    private float damageProjectile;
    [SerializeField]
    private float knockbackProjectile;
    
    [SerializeField]
    private Vector2 dirProjectile;
    [SerializeField]
    private bool isDestroyedOnFirstHit;
    
    public bool isFromEnemy;

    public UnityEvent onSpawnEvent;
    private Rigidbody2D rgbd;
    // Start is called before the first frame update
    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        LaunchProjectile();
    }

    private void Update()
    {
        lifeSpawn -= Time.deltaTime;
        if (lifeSpawn <= 0)
        {
            DestroyProjectileServerRpc();
        }
    }

    public void LaunchProjectile()
    {
        rgbd.velocity = (dirProjectile * speedProjectile);
        var zAngle = Mathf.Atan2(dirProjectile.x, dirProjectile.y) * Mathf.Rad2Deg;
        // Store the target rotation
        transform.rotation = Quaternion.Euler(0,0, zAngle);
        if (onSpawnEvent.GetPersistentEventCount() > 0)
        {
            onSpawnEvent.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyProjectileServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();

        if (isFromEnemy)
        {
            if (other.CompareTag("Player"))
            {
                character.TakeDamage(this);
                if (isDestroyedOnFirstHit)
                {
                    DestroyProjectileServerRpc();
                }
            }
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                character.TakeDamage(this);
                if (isDestroyedOnFirstHit)
                {
                    DestroyProjectileServerRpc();
                }
            }
        }
    }

    public void RotationTreeSkill()
    {
        transform.DORotate(new Vector3(0, 0, 360), lifeSpawn-0.15f,RotateMode.FastBeyond360).SetRelative(true);
    }
    

    public Vector2 DirProjectile
    {
        get => dirProjectile;
        set => dirProjectile = value;
    }
    
    public float DamageProjectile => damageProjectile;

}
