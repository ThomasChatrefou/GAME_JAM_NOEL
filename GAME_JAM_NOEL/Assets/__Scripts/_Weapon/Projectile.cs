using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private float speedProjectile;
    [SerializeField] 
    private float lifeSpawn;
    [SerializeField]
    private float damageProjectile;
    
    [SerializeField]
    private Vector2 dirProjectile;
    
    public bool isFromEnemy;
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
        //transform.rotation = Quaternion.Euler(0,0, zAngle);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyProjectileServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();

        if (isFromEnemy)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("HitPlayer");
                character.TakeDamage(this);
            }
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("HitEnemy");
                character.TakeDamage(this);
            }
        }
    }   
    public Vector2 DirProjectile
    {
        get => dirProjectile;
        set => dirProjectile = value;
    }
    
    public float DamageProjectile => damageProjectile;

}
