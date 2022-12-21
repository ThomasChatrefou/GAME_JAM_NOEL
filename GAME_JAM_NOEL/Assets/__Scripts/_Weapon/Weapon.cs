using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [Header("Projectiles")]
    [SerializeField]
    private float timerReload;
    [SerializeField] 
    private float timerReloadMax;
    [SerializeField]
    private GameObject baseProjectilePrefab;
    [SerializeField]
    private GameObject skillProjectilePrefab;
    [SerializeField] 
    private Transform firePos;

    private bool hasShoot;
    
    public Sprite weaponSpriteUI;
    public Sprite weaponSpriteGround;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasShoot)
        {
            timerReload -= Time.deltaTime;
            if (timerReload <= 0)
            {
                hasShoot = !hasShoot;
                timerReload = timerReloadMax;
            }
        }
    }

    //TODO RPC
    public void LaunchBaseProjectile(Vector2 direction, bool isFromEnemy = false)
    {
        if (!hasShoot)
        {
            Projectile proj = Instantiate(baseProjectilePrefab,firePos.position,Quaternion.identity).GetComponent<Projectile>();
            proj.DirProjectile = direction;
            proj.isFromEnemy = isFromEnemy;
            proj.transform.LookAt(direction);
            proj.GetComponent<NetworkObject>().Spawn(true);
        }
        hasShoot = true;
    }

    //TODO RPC
    [ServerRpc(RequireOwnership = false)]
    public void LaunchSkillProjectileServerRpc(Vector2 direction)
    {
        Projectile weapon = Instantiate(skillProjectilePrefab,transform.position,Quaternion.identity).GetComponent<Projectile>();
    }

    //TODO RPC
    [ServerRpc(RequireOwnership = false)]
    public void PickedUpServerRpc()
    {
        //Make the item disapears from the map and be equiped by the player
    }
}
