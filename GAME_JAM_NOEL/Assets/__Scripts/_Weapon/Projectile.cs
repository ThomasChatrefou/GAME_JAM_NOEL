using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private float speedProjectile;
    [SerializeField]
    private float rangeProjectile;
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

    public void LaunchProjectile()
    {
        rgbd.velocity = (dirProjectile * speedProjectile);
    }
    
    public Vector2 DirProjectile
    {
        get => dirProjectile;
        set => dirProjectile = value;
    }

}
