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

    public void LaunchBaseProjectile(Vector2 direction)
    {
        if (!hasShoot)
        {
            Projectile proj = Instantiate(baseProjectilePrefab,transform.position,Quaternion.identity).GetComponent<Projectile>();
            proj.DirProjectile = direction;
        }
        hasShoot = true;
    }

    public void LaunchSkillProjectile(Vector2 direction)
    {
        Projectile weapon = Instantiate(skillProjectilePrefab,transform.position,Quaternion.identity).GetComponent<Projectile>();
    }

    public void PickedUp()
    {
        //Make the item disapears from the map and be equiped by the player
    }
}
