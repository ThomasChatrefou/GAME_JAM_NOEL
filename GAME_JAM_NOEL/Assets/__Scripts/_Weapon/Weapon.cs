using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Sprite Management")] 
    private SpriteRenderer spriteRenderer;
    public Sprite weaponSpriteUI;
    public Sprite weaponSpriteGround;
    public Sprite weaponSpriteHands;

    [Header("Trigger Equip")] 
    public Collider2D colliderPlayerEquip2D;
    [SerializeField]
    private GameObject pressKeyUI;
    
    [Header("Ground")] 
    public bool onGround;
    [SerializeField]
    private PlayerController nearbyPlayer;
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

    private void OnEnable()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    //TODO RPC
    public void LaunchBaseProjectile(Vector2 direction, bool isFromEnemy = false)
    {
        if (!hasShoot)
        {
            Projectile proj = Instantiate(baseProjectilePrefab,firePos.position,
                Quaternion.Euler(0,0,Mathf.Atan2(firePos.position.x, firePos.position.y) * Mathf.Rad2Deg))
                .GetComponent<Projectile>();
            proj.DirProjectile = direction;
            proj.isFromEnemy = isFromEnemy;
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
    public void SpawnWeaponGroundServerRpc()
    {
        //Make the item disapears from the map and be equiped by the player
        spriteRenderer.sprite = weaponSpriteGround;
        colliderPlayerEquip2D.enabled = true;
        onGround = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnWeaponServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void MoveToParentServerRpc(ulong idGameObject)
    {
        transform.parent = FindObjectsOfType<NetworkObject>().First( o => o.OwnerClientId == idGameObject).transform;
    }

    public void SpawnWeaponGround(bool value)
    {
        //Make the item disapears from the map and be equiped by the player
        if (value)
        {
            spriteRenderer.sprite = weaponSpriteGround;    
        }
        else
        {
            spriteRenderer.sprite = weaponSpriteHands;
        }
        colliderPlayerEquip2D.enabled = value;
        onGround = value;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (onGround)
        {
            if (col.CompareTag("Player"))
            {
                Debug.Log("Detect Plater : " + OwnerClientId);
                Debug.Log("Is Owner : " + IsOwner);
                PlayerController player = col.gameObject.GetComponent<PlayerController>();
                if (player.IsOwner)
                {
                    if (!nearbyPlayer)
                    {
                        pressKeyUI.SetActive(true);
                    }
                    nearbyPlayer = player;
                    nearbyPlayer.NearbyWeapon = this;
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && nearbyPlayer)
        {
            if (other.gameObject.Equals(nearbyPlayer.gameObject))
            {
                if (nearbyPlayer)
                {
                    pressKeyUI.SetActive(false);
                }
                nearbyPlayer.NearbyWeapon = null;
                nearbyPlayer = null;
            }
        }
    }
}
